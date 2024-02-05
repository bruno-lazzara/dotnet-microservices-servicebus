using Microsoft.AspNetCore.Identity;
using Orange.Models.DTO.Auth;
using Orange.Services.AuthAPI.Data;
using Orange.Services.AuthAPI.Models;
using Orange.Services.AuthAPI.Services.Interfaces;

namespace Orange.Services.AuthAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly OrangeDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AuthService(OrangeDbContext dbContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public Task<LoginUserResponseDTO> LoginAsync(LoginUserDTO request)
        {
            throw new NotImplementedException();
        }

        public async Task<UserDTO?> RegisterAsync(RegisterUserDTO request)
        {
            try
            {
                ApplicationUser user = new()
                {
                    UserName = request.Email,
                    Email = request.Email,
                    NormalizedEmail = request.Email.ToUpper(),
                    Name = request.Name,
                    PhoneNumber = request.PhoneNumber
                };

                var result = await _userManager.CreateAsync(user, request.Password);
                if (!result.Succeeded)
                {
                    return null;
                }

                var userResponse = _dbContext.ApplicationUsers.First(u => u.UserName == request.Email);

                UserDTO userDTO = new()
                {
                    Email = user.Email,
                    Id = user.Id,
                    Name = user.Name,
                    PhoneNumber = user.PhoneNumber,
                };

                return userDTO;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
