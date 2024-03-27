using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orange.MessageBus;
using Orange.Models.DTO;
using Orange.Services.ShoppingCartAPI.Data;
using Orange.Services.ShoppingCartAPI.Models.Entity;
using Orange.Services.ShoppingCartAPI.Services.Interfaces;

namespace Orange.Services.ShoppingCartAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly OrangeDbContext _context;
        private readonly IProductService _productService;
        private readonly ICouponService _couponService;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;
        public CartController(IMapper mapper, OrangeDbContext context, IProductService productService, ICouponService couponService, IMessageBus messageBus, IConfiguration configuration)
        {
            _mapper = mapper;
            _context = context;
            _productService = productService;
            _couponService = couponService;
            _messageBus = messageBus;
            _configuration = configuration;
        }

        [HttpGet("GetCart/{userId}")]
        public async Task<IActionResult> GetCart(string userId)
        {
            try
            {
                CartDTO cart = new();

                CartHeader? cartHeader = _context.CartHeaders.FirstOrDefault(ch => ch.UserId == userId);
                if (cartHeader == null)
                {
                    return NotFound("No cart found for this user");
                }

                List<CartDetails> cartDetails = _context.CartDetails.Where(cd => cd.CartHeaderId == cartHeader.CartHeaderId).ToList();

                var products = await _productService.GetProducts();

                foreach (var item in cartDetails)
                {
                    item.Product = products.FirstOrDefault(p => p.ProductId == item.ProductId);

                    if (item.Product != null)
                    {
                        cartHeader.CartTotal += item.Count * item.Product.Price;
                    }
                }

                //Apply coupon if any
                if (!string.IsNullOrWhiteSpace(cartHeader.CouponCode))
                {
                    var coupon = await _couponService.GetCouponByCode(cartHeader.CouponCode);
                    if (coupon != null && cartHeader.CartTotal >= coupon.MinAmount)
                    {
                        cartHeader.CartTotal -= coupon.DiscountAmount;
                        cartHeader.Discount = coupon.DiscountAmount;
                    }
                }

                cart.CartHeader = _mapper.Map<CartHeaderDTO>(cartHeader);
                cart.CartDetails = _mapper.Map<List<CartDetailsDTO>>(cartDetails);

                return Ok(cart);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost("ApplyCoupon")]
        public async Task<IActionResult> ApplyCoupon([FromBody] CartDTO cart)
        {
            try
            {
                var cartFromDb = await _context.CartHeaders.FirstAsync(ch => ch.UserId == cart.CartHeader.UserId);
                cartFromDb.CouponCode = cart.CartHeader.CouponCode;
                _context.CartHeaders.Update(cartFromDb);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost("CartUpsert")]
        public async Task<IActionResult> CartUpsert(CartDTO cartDTO)
        {
            try
            {
                var cartHeaderFromDb = await _context.CartHeaders
                    .AsNoTracking()
                    .FirstOrDefaultAsync(ch => ch.UserId == cartDTO.CartHeader.UserId);

                if (cartHeaderFromDb == null)
                {
                    //Create header and details
                    CartHeader cartHeader = _mapper.Map<CartHeader>(cartDTO.CartHeader);
                    await _context.CartHeaders.AddAsync(cartHeader);
                    await _context.SaveChangesAsync();

                    cartDTO.CartDetails.First().CartHeaderId = cartHeader.CartHeaderId;
                    CartDetails cartDetails = _mapper.Map<CartDetails>(cartDTO.CartDetails.First());
                    await _context.CartDetails.AddAsync(cartDetails);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    //Check if details has the same product
                    var cartDetailsFromDb = await _context.CartDetails.AsNoTracking().FirstOrDefaultAsync(cd =>
                        cd.ProductId == cartDTO.CartDetails.First().ProductId &&
                        cd.CartHeaderId == cartHeaderFromDb.CartHeaderId);

                    if (cartDetailsFromDb == null)
                    {
                        //Create card details
                        cartDTO.CartDetails.First().CartHeaderId = cartHeaderFromDb.CartHeaderId;
                        CartDetails cartDetails = _mapper.Map<CartDetails>(cartDTO.CartDetails.First());
                        await _context.CartDetails.AddAsync(cartDetails);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        //update count in cart details
                        cartDTO.CartDetails.First().Count += cartDetailsFromDb.Count;
                        cartDTO.CartDetails.First().CartHeaderId = cartDetailsFromDb.CartHeaderId;
                        cartDTO.CartDetails.First().CartDetailsId = cartDetailsFromDb.CartDetailsId;

                        CartDetails cartDetails = _mapper.Map<CartDetails>(cartDTO.CartDetails.First());
                        _context.CartDetails.Update(cartDetails);
                        await _context.SaveChangesAsync();
                    }
                }

                return Ok(cartDTO);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost("RemoveCartDetails")]
        public async Task<IActionResult> RemoveCartDetails([FromBody] int cartDetailsId)
        {
            try
            {
                var cartDetails = await _context.CartDetails
                    .FirstAsync(cd => cd.CartDetailsId == cartDetailsId);

                int totalCountOfCartItems = _context.CartDetails.Where(cd => cd.CartHeaderId == cartDetails.CartHeaderId).Count();
                _context.CartDetails.Remove(cartDetails);

                if (totalCountOfCartItems == 1)
                {
                    var cartHeaderToRemove = await _context.CartHeaders
                        .FirstAsync(ch => ch.CartHeaderId == cartDetails.CartHeaderId);

                    _context.CartHeaders.Remove(cartHeaderToRemove);
                }

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost("EmailCartRequest")]
        public async Task<IActionResult> EmailCartRequest([FromBody] CartDTO cart)
        {
            try
            {
                await _messageBus.PublishMessageAsync(cart, _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue"));

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }
}
