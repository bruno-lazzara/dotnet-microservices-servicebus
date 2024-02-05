using Microsoft.AspNetCore.Mvc;
using Orange.Models.DTO;
using Orange.Web.Services.Interfaces;

namespace Orange.Web.Controllers
{
    public class CouponController : Controller
    {
        private readonly ICouponService _couponService;
        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        public async Task<IActionResult> Index()
        {
            List<CouponDTO> coupons = await _couponService.GetAllAsync();
            return View(coupons);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }
    }
}
