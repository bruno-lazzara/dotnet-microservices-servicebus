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
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var userCart = await LoadUserCartAsync();
            return View(userCart);
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
