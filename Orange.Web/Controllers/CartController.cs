using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orange.Models.DTO;
using Orange.Web.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;

namespace Orange.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;
        public CartController(ICartService cartService, IOrderService orderService)
        {
            _cartService = cartService;
            _orderService = orderService;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var userCart = await LoadUserCartAsync();
            return View(userCart);
        }

        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            var userCart = await LoadUserCartAsync();
            return View(userCart);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Checkout(CartDTO cartDTO)
        {
            CartDTO userCart = await LoadUserCartAsync();
            userCart.CartHeader.Phone = cartDTO.CartHeader.Phone;
            userCart.CartHeader.Email = cartDTO.CartHeader.Email;
            userCart.CartHeader.Name = cartDTO.CartHeader.Name;

            var order = await _orderService.CreateOrderAsync(userCart);
            if (order == null)
            {
                TempData["error"] = "Error to place order";
            }
            else
            {
                //TODO - get stripe session and redirect to stripe to place the order
            }

            return View();
        }

        [Authorize]
        public async Task<IActionResult> Remove(int cartDetailsId)
        {
            bool itemRemoved = await _cartService.RemoveFromCartAsync(cartDetailsId);
            if (itemRemoved)
            {
                TempData["success"] = "Item removed from cart";
            }
            else
            {
                TempData["error"] = "Error to remove item from cart";
            }

            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(CartDTO cart)
        {
            bool couponApplied = await _cartService.ApplyCouponAsync(cart);
            if (couponApplied)
            {
                TempData["success"] = "Coupon applied";
            }
            else
            {
                TempData["error"] = "Coupon not found";
            }

            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EmailCart(CartDTO cart)
        {
            var fullCart = await LoadUserCartAsync();
            fullCart.CartHeader.Email = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Email)?.FirstOrDefault()?.Value;
            bool emailSent = await _cartService.EmailCartAsync(fullCart);
            if (emailSent)
            {
                TempData["success"] = "E-mail will be processed and sent shortly";
            }
            else
            {
                TempData["error"] = "Service not available";
            }

            return RedirectToAction("Index");
        }

        private async Task<CartDTO> LoadUserCartAsync()
        {
            string? userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return new();
            }
            var response = await _cartService.GetCartByUserIdAsync(userId);
            return response ?? new();
        }
    }
}
