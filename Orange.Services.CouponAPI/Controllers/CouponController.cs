using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orange.Models.DTO;
using Orange.Services.CouponAPI.Data;
using Orange.Services.CouponAPI.Models.Entity;

namespace Orange.Services.CouponAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CouponController : ControllerBase
    {
        private readonly OrangeDbContext _context;
        private readonly IMapper _mapper;
        public CouponController(OrangeDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult Get()
        {
            try
            {
                List<Coupon> list = _context.Coupons.ToList();
                if (list.Count == 0)
                {
                    return NotFound("No coupons found.");
                }

                List<CouponDTO> listDTO = _mapper.Map<List<CouponDTO>>(list);

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
                var coupon = _context.Coupons.FirstOrDefault(c => c.CouponId == id);
                if (coupon == null)
                {
                    return NotFound("Coupon not found.");
                }

                var couponDTO = _mapper.Map<CouponDTO>(coupon);

                return Ok(couponDTO);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet("code/{code}")]
        public ActionResult Get(string code)
        {
            try
            {
                var coupon = _context.Coupons.FirstOrDefault(c => c.CouponCode.ToLower() == code.ToLower());
                if (coupon == null)
                {
                    return NotFound("Coupon not found.");
                }

                var couponDTO = _mapper.Map<CouponDTO>(coupon);

                return Ok(couponDTO);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public ActionResult Post([FromBody] CouponDTO couponDTO)
        {
            try
            {
                Coupon coupon = _mapper.Map<Coupon>(couponDTO);
                _context.Coupons.Add(coupon);
                _context.SaveChanges();

                var couponDTOResponse = _mapper.Map<CouponDTO>(coupon);

                return StatusCode(201, couponDTOResponse);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPut]
        public ActionResult Put([FromBody] CouponDTO couponDTO)
        {
            try
            {
                Coupon coupon = _mapper.Map<Coupon>(couponDTO);
                _context.Coupons.Update(coupon);
                _context.SaveChanges();

                var couponDTOResponse = _mapper.Map<CouponDTO>(coupon);

                return Ok(couponDTOResponse);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            try
            {
                Coupon? coupon = _context.Coupons.FirstOrDefault(c => c.CouponId == id);
                if (coupon == null)
                {
                    return NotFound("Coupon not found.");
                }

                _context.Coupons.Remove(coupon);
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
