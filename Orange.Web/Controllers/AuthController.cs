using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Orange.Models.DTO.Auth;
using Orange.Web.Services.Interfaces;
using Orange.Web.Utils;

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
            PopulateViewBags();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterUserDTO user)
        {
            UserDTO? userRegistered = await _authService.RegisterAsync(user);
            if (userRegistered == null)
            {
                TempData["error"] = "Error to register user";
                PopulateViewBags();
                return View(user);
            }

            if (string.IsNullOrEmpty(user.Role))
            {
                user.Role = Constants.ROLE_CUSTOMER;
            }

            bool roleAssigned = await _authService.AssignRoleAsync(user);
            if (!roleAssigned)
            {
                TempData["error"] = "User registered. Error to assign user roles";
            }
            else
            {
                TempData["success"] = "User registered successfully";
            }
            
            return RedirectToAction("Login");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            return View();
        }

        private void PopulateViewBags()
        {
            var roleList = new List<SelectListItem>()
            {
                new() { Text = Constants.ROLE_ADMIN, Value = Constants.ROLE_ADMIN },
                new() { Text = Constants.ROLE_CUSTOMER, Value = Constants.ROLE_CUSTOMER }
            };

            ViewBag.RoleList = roleList;
        }
    }
}
