using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orange.MessageBus;
using Orange.Models.DTO;
using Orange.Services.OrderAPI.Data;
using Orange.Services.OrderAPI.Models.Entity;
using Orange.Services.OrderAPI.Services.Interfaces;
using Stripe;
using Stripe.Checkout;
using System.Net;

namespace Orange.Services.OrderAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly OrangeDbContext _context;
        private readonly IProductService _productService;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;
        public OrderController(IMapper mapper, OrangeDbContext context, IProductService productService, IMessageBus messageBus, IConfiguration configuration)
        {
            _mapper = mapper;
            _context = context;
            _productService = productService;
            _messageBus = messageBus;
            _configuration = configuration;
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
    }
}
