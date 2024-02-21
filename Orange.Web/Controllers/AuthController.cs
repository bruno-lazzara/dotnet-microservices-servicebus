using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Orange.Models.DTO.Auth;
using Orange.Web.Services.Interfaces;
using Orange.Web.Utils;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Orange.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ITokenProvider _tokenProvider;
        public AuthController(IAuthService authService, ITokenProvider tokenProvider)
        {
            _authService = authService;
            _tokenProvider = tokenProvider;
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginUserDTO user)
        {
            LoginUserResponseDTO? userLogin = await _authService.LoginAsync(user);
            if (userLogin == null || userLogin.User == null)
            {
                ModelState.AddModelError("CustomError", "Username or password is incorrect");
                return View(user);
            }

            await SignInUserAsync(userLogin.User);
            _tokenProvider.SetToken(userLogin.Token);

            return RedirectToAction("Index", "Home");
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
            await HttpContext.SignOutAsync();
            _tokenProvider.ClearToken();
            return RedirectToAction("Index", "Home");
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

        private async Task SignInUserAsync(UserDTO userDto)
        {
            ClaimsIdentity identity = new(CookieAuthenticationDefaults.AuthenticationScheme);

            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email, userDto.Email));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, userDto.Id));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name, userDto.Name));
            identity.AddClaim(new Claim(ClaimTypes.Name, userDto.Email));

            foreach (var role in userDto.Roles)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, role));
            }

            ClaimsPrincipal principal = new(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}
