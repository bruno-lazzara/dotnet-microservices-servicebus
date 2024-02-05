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

        [HttpPost]
        public async Task<IActionResult> Create(CouponDTO coupon)
        {
            if (!ModelState.IsValid)
            {
                return View(coupon);
            }

            var couponResponse = await _couponService.CreateCouponAsync(coupon);
            if (couponResponse == null)
            {
                return View(coupon);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _couponService.DeleteCouponAsync(id);
            return RedirectToAction("Index");
        }
    }
}
