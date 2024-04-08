using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orange.Models.DTO;
using Orange.Services.OrderAPI.Data;
using Orange.Services.OrderAPI.Models.Entity;
using Orange.Services.OrderAPI.Services.Interfaces;
using Orange.Services.OrderAPI.Utility;
using Stripe;
using Stripe.Checkout;
using System.Net;
using System.Security.Claims;

namespace Orange.Services.OrderAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly OrangeDbContext _context;
        private readonly IProductService _productService;
        private readonly MessageBus.IMessageBus _messageBus;
        private readonly IConfiguration _configuration;
        public OrderController(IMapper mapper, OrangeDbContext context, IProductService productService, MessageBus.IMessageBus messageBus, IConfiguration configuration)
        {
            _mapper = mapper;
            _context = context;
            _productService = productService;
            _messageBus = messageBus;
            _configuration = configuration;
        }

        [Authorize]
        [HttpGet("GetOrders")]
        public async Task<IActionResult> Get(string? userId = "")
        {
            try
            {
                IEnumerable<OrderHeader> orders;
                if (User.IsInRole(Constants.ROLE_ADMIN))
                {
                    orders = await _context.OrderHeaders.Include(oh => oh.OrderDetails).OrderByDescending(oh => oh.OrderHeaderId).ToListAsync();
                }
                else
                {
                    orders = await _context.OrderHeaders.Include(oh => oh.OrderDetails).Where(oh => oh.UserId == userId).OrderByDescending(oh => oh.OrderHeaderId).ToListAsync();
                }

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [Authorize]
        [HttpGet("GetOrder/{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                string? userId = User.Claims.Where(u => u.Type == ClaimTypes.NameIdentifier).FirstOrDefault()?.Value;

                var order = await _context.OrderHeaders.Include(oh => oh.OrderDetails).FirstOrDefaultAsync(oh => oh.OrderHeaderId == id);
                if (order == null
                    || (!User.IsInRole(Constants.ROLE_ADMIN) && order.UserId != userId))
                {
                    return NotFound("Order not found.");
                }

                var orderDto = _mapper.Map<OrderHeaderDTO>(order);
                return Ok(orderDto);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost("CreateOrder")]
        [Authorize]
        public async Task<IActionResult> CreateOrder([FromBody] CartDTO cart)
        {
            try
            {
                OrderHeaderDTO orderHeaderDTO = _mapper.Map<OrderHeaderDTO>(cart.CartHeader);
                orderHeaderDTO.OrderTime = DateTime.Now;
                orderHeaderDTO.Status = Utility.Constants.STATUS_PENDING;
                orderHeaderDTO.OrderDetails = _mapper.Map<IEnumerable<OrderDetailsDTO>>(cart.CartDetails);

                OrderHeader orderCreated = _mapper.Map<OrderHeader>(orderHeaderDTO);
                await _context.OrderHeaders.AddAsync(orderCreated);
                await _context.SaveChangesAsync();

                orderHeaderDTO.OrderHeaderId = orderCreated.OrderHeaderId;

                return StatusCode((int)HttpStatusCode.Created, orderHeaderDTO);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [Authorize]
        [HttpPost("CreateStripeSession")]
        public async Task<IActionResult> CreateStripeSession([FromBody] StripeRequestDTO stripeRequestDTO)
        {
            try
            {
                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    SuccessUrl = stripeRequestDTO.ApprovedUrl,
                    CancelUrl = stripeRequestDTO.CancelUrl,
                    LineItems = [],
                    Mode = "payment"
                };

                if (stripeRequestDTO.OrderHeader.Discount > 0)
                {
                    options.Discounts =
                    [
                        new SessionDiscountOptions
                        {
                            Coupon = stripeRequestDTO.OrderHeader.CouponCode
                        }
                    ];
                }

                foreach (var item in stripeRequestDTO.OrderHeader.OrderDetails)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.ProductPrice * 100),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.ProductName
                            }
                        },
                        Quantity = item.Count
                    };

                    options.LineItems.Add(sessionLineItem);
                }

                var service = new SessionService();
                Session stripeSession = service.Create(options);

                stripeRequestDTO.StripeSessionUrl = stripeSession.Url;

                OrderHeader orderHeader = _context.OrderHeaders.First(h => h.OrderHeaderId == stripeRequestDTO.OrderHeader.OrderHeaderId);
                orderHeader.StripeSessionId = stripeSession.Id;
                await _context.SaveChangesAsync();

                return StatusCode((int)HttpStatusCode.Created, stripeRequestDTO);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [Authorize]
        [HttpPost("ValidateStripeSession")]
        public async Task<IActionResult> ValidateStripeSession([FromBody] int orderHeaderId)
        {
            try
            {
                OrderHeader orderHeader = _context.OrderHeaders.First(h => h.OrderHeaderId == orderHeaderId);

                var service = new SessionService();
                Session stripeSession = await service.GetAsync(orderHeader.StripeSessionId);

                var paymentIntentService = new PaymentIntentService();
                PaymentIntent paymentIntent = await paymentIntentService.GetAsync(stripeSession.PaymentIntentId);

                if (paymentIntent.Status == "succeeded")
                {
                    orderHeader.PaymentIntentId = paymentIntent.Id;
                    orderHeader.Status = Utility.Constants.STATUS_APPROVED;
                    await _context.SaveChangesAsync();

                    RewardsDTO rewardsDTO = new()
                    {
                        OrderId = orderHeader.OrderHeaderId,
                        RewardsActivity = Convert.ToInt32(orderHeader.OrderTotal),
                        UserId = orderHeader.UserId
                    };

                    string topicName = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");

                    await _messageBus.PublishMessageAsync(rewardsDTO, topicName);
                }

                return Ok(_mapper.Map<OrderHeaderDTO>(orderHeader));
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [Authorize]
        [HttpPost("UpdateOrderStatus/{orderId:int}")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] string newStatus)
        {
            try
            {
                OrderHeader? order = await _context.OrderHeaders.FirstOrDefaultAsync(oh => oh.OrderHeaderId == orderId);
                if (order == null)
                {
                    return NotFound("Order not found.");
                }

                if (newStatus == Constants.STATUS_CANCELED)
                {
                    var refundOptions = new RefundCreateOptions
                    {
                        Reason = RefundReasons.RequestedByCustomer,
                        PaymentIntent = order.PaymentIntentId,
                    };

                    var refundService = new RefundService();
                    Refund refund = await refundService.CreateAsync(refundOptions);   
                }

                order.Status = newStatus;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }
}
