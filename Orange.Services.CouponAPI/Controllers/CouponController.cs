using Microsoft.AspNetCore.Mvc;
using Orange.Services.CouponAPI.Data;

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
                var list = _context.Coupons.ToList();
                return Ok(list);
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
                    return NotFound();
                }
                return Ok(coupon);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }
}
