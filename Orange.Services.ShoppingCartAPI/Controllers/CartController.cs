using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orange.Models.DTO;
using Orange.Services.ShoppingCartAPI.Data;
using Orange.Services.ShoppingCartAPI.Models.Entity;
using System.Reflection.PortableExecutable;

namespace Orange.Services.ShoppingCartAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly OrangeDbContext _context;
        public CartController(IMapper mapper, OrangeDbContext context)
        {
            _mapper = mapper;
            _context = context;
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
    }
}
