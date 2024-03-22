using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orange.Models.DTO;
using Orange.Services.OrderAPI.Data;
using Orange.Services.OrderAPI.Models.Entity;
using Orange.Services.OrderAPI.Services.Interfaces;
using Orange.Services.OrderAPI.Utility;
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
    }
}
