using Microsoft.AspNetCore.Mvc;
using Orange.Services.CouponAPI.Data;
using Orange.Services.CouponAPI.Models.DTO;
using Orange.Services.CouponAPI.Models.Entity;

namespace Orange.Services.CouponAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponController : ControllerBase
    {
        private readonly OrangeDbContext _context;
        public CouponController(OrangeDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult Get()
        {
            try
            {
                var response = new ResponseDTO<List<Coupon>>();

                response.Result = _context.Coupons.ToList();
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
                var response = new ResponseDTO<Coupon>();

                response.Result = _context.Coupons.FirstOrDefault(c => c.CouponId == id);
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
    }
}
