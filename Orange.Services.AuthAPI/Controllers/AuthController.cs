using Microsoft.AspNetCore.Mvc;
using Orange.Models.DTO.Auth;
using Orange.Services.AuthAPI.Services.Interfaces;

namespace Orange.Services.AuthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDTO registerUserDTO)
        {
            var response = await _authService.RegisterAsync(registerUserDTO);
            if (response == null)
            {
                return BadRequest();
            }
            return StatusCode(201, response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login()
        {
            return Ok();
        }
    }
}
