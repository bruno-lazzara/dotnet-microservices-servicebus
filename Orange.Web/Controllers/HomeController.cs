using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orange.Models.DTO;
using Orange.Web.Models;
using Orange.Web.Services.Interfaces;
using System.Diagnostics;

namespace Orange.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        public HomeController(IProductService productService, ICartService cartService)
        {
            _productService = productService;
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            List<ProductDTO> products = await _productService.GetAllAsync();
            return View(products);
        }

        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                TempData["error"] = "Product not found";
                return RedirectToAction("Index");
            }
            return View(product);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Details(ProductDTO product)
        {
            CartDTO cart = new()
            {
                CartHeader = new CartHeaderDTO
                {
                    UserId = User.Claims.Where(u => u.Type == JwtClaimTypes.Subject)?.FirstOrDefault()?.Value
                },
                CartDetails =
                [
                    new CartDetailsDTO {
                        Count = product.Count,
                        ProductId = product.ProductId
                    }
                ]
            };

            var cartResponse = await _cartService.UpsertCartAsync(cart);
            if (cartResponse == null)
            {
                TempData["error"] = "Error to add product to the cart";
                return RedirectToAction("Details", new { id = product.ProductId });
            }

            TempData["success"] = "Item added to the cart";

            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
