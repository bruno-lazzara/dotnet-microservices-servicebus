using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Orange.Models.DTO.Auth;
using Orange.Web.Services.Interfaces;

namespace Orange.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            var roleList = new List<SelectListItem>()
            {
                new() { Text = "ADMIN", Value = "ADMIN" },
                new() { Text = "CUSTOMER", Value = "CUSTOMER" }
            };

            ViewBag.RoleList = roleList;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            return View();
        }
    }
}
