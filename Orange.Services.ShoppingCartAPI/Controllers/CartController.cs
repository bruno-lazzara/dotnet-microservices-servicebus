using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Orange.Models.DTO;
using Orange.Services.ShoppingCartAPI.Data;

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

        }
    }
}
