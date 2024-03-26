using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orange.Models.DTO;
using Orange.Services.OrderAPI.Data;
using Orange.Services.OrderAPI.Models.Entity;
using Orange.Services.OrderAPI.Services.Interfaces;
using Orange.Services.OrderAPI.Utility;
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
        public OrderController(IMapper mapper, OrangeDbContext context, IProductService productService)
        {
            _mapper = mapper;
            _context = context;
            _productService = productService;
        }

        [HttpPost("CreateOrder")]
        [Authorize]
        public async Task<IActionResult> CreateOrder([FromBody] CartDTO cart)
        {
            try
            {
                OrderHeaderDTO orderHeaderDTO = _mapper.Map<OrderHeaderDTO>(cart.CartHeader);
                orderHeaderDTO.OrderTime = DateTime.Now;
                orderHeaderDTO.Status = Constants.STATUS_PENDING;
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
    }
}
