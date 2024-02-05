using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Orange.Models.DTO;
using Orange.Services.CouponAPI.Data;
using Orange.Services.CouponAPI.Models.Entity;

namespace Orange.Services.CouponAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
                var response = new ResponseDTO<List<CouponDTO>>();

                var list = _context.Coupons.ToList();
                response.Result = _mapper.Map<List<CouponDTO>>(list);

                if (response.Result.Count == 0)
                {
                    response.Message = "No coupons found.";
                }

                return Ok(response);
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
                var response = new ResponseDTO<CouponDTO>();

                var coupon = _context.Coupons.FirstOrDefault(c => c.CouponId == id);
                response.Result = _mapper.Map<CouponDTO>(coupon);

                if (response.Result == null)
                {
                    response.Message = "Coupon not found.";
                }

                return Ok(response);
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
                var response = new ResponseDTO<CouponDTO>();

                var coupon = _context.Coupons.FirstOrDefault(c => c.CouponCode.ToLower() == code.ToLower());
                response.Result = _mapper.Map<CouponDTO>(coupon);

                if (response.Result == null)
                {
                    response.Message = "Coupon not found.";
                }

                return Ok(response);
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
                var response = new ResponseDTO<CouponDTO>();

                Coupon coupon = _mapper.Map<Coupon>(couponDTO);
                _context.Coupons.Add(coupon);
                _context.SaveChanges();

                response.Result = _mapper.Map<CouponDTO>(coupon);

                return StatusCode(201, response);
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
                var response = new ResponseDTO<CouponDTO>();

                Coupon coupon = _mapper.Map<Coupon>(couponDTO);
                _context.Coupons.Update(coupon);
                _context.SaveChanges();

                response.Result = _mapper.Map<CouponDTO>(coupon);

                return Ok(response);
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
                var response = new ResponseDTO<bool>();

                Coupon? coupon = _context.Coupons.FirstOrDefault(c => c.CouponId == id);
                if (coupon == null)
                {
                    response.Message = "Coupon not found.";
                    response.Result = false;
                    return Ok(response);
                }

                _context.Coupons.Remove(coupon);
                _context.SaveChanges();

                response.Message = "Coupon deleted.";
                response.Result = true;
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }
}
