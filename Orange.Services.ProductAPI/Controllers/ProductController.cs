using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orange.Models.DTO;
using Orange.Services.ProductAPI.Data;
using Orange.Services.ProductAPI.Models.Entity;
using Utils;

namespace Orange.Services.ProductAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly OrangeDbContext _context;
        private readonly IMapper _mapper;
        public ProductController(OrangeDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult Get()
        {
            try
            {
                List<Product> list = _context.Products.ToList();
                if (list.Count == 0)
                {
                    return NotFound("No products found.");
                }

                List<ProductDTO> listDTO = _mapper.Map<List<ProductDTO>>(list);

                return Ok(listDTO);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet("{id:int}")]
        public ActionResult Get(int id)
        {
            try
            {
                var product = _context.Products.FirstOrDefault(p => p.ProductId == id);
                if (product == null)
                {
                    return NotFound("Product not found.");
                }

                var productDTO = _mapper.Map<ProductDTO>(product);

                return Ok(productDTO);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Authorize(Roles = Constants.ROLE_ADMIN)]
        public ActionResult Post([FromBody] ProductDTO productDTO)
        {
            try
            {
                Product product = _mapper.Map<Product>(productDTO);
                _context.Products.Add(product);
                _context.SaveChanges();

                var productDTOResponse = _mapper.Map<ProductDTO>(product);

                return StatusCode(201, productDTOResponse);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPut]
        [Authorize(Roles = Constants.ROLE_ADMIN)]
        public ActionResult Put([FromBody] ProductDTO productDTO)
        {
            try
            {
                Product product = _mapper.Map<Product>(productDTO);
                _context.Products.Update(product);
                _context.SaveChanges();

                var productDTOResponse = _mapper.Map<ProductDTO>(product);

                return Ok(productDTOResponse);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = Constants.ROLE_ADMIN)]
        public ActionResult Delete(int id)
        {
            try
            {
                Product? product = _context.Products.FirstOrDefault(p => p.ProductId == id);
                if (product == null)
                {
                    return NotFound("Product not found.");
                }

                _context.Products.Remove(product);
                _context.SaveChanges();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }
}
