﻿using Microsoft.AspNetCore.Identity;
using Orange.Models.DTO.Auth;
using Orange.Services.AuthAPI.Data;
using Orange.Services.AuthAPI.Models;
using Orange.Services.AuthAPI.Services.Interfaces;

namespace Orange.Services.AuthAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly OrangeDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenService _tokenService;
        public AuthService(OrangeDbContext dbContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ITokenService tokenService)
        {
            _context = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
        }

        public async Task<bool> AssignRoleAsync(string email, string roleName)
        {
            try
            {
                var user = _context.ApplicationUsers.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
                if (user == null)
                {
                    return false;
                }

                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }

                await _userManager.AddToRoleAsync(user, roleName);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<LoginUserResponseDTO> LoginAsync(LoginUserDTO request)
        {
            var responseDTO = new LoginUserResponseDTO();
            try
            {
                var user = _context.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == request.UserName.ToLower());
                bool isValid = await _userManager.CheckPasswordAsync(user, request.Password);

                if (user == null || !isValid)
                {
                    return responseDTO;
                }

                var token = _tokenService.GenerateToken(user);
                if (string.IsNullOrWhiteSpace(token))
                {
                    return responseDTO;
                }

                UserDTO userDTO = new()
                {
                    Email = user.Email,
                    Id = user.Id,
                    Name = user.Name,
                    PhoneNumber = user.PhoneNumber
                };

                responseDTO.User = userDTO;
                responseDTO.Token = token;
            }
            catch (Exception ex)
            {

            }

            return responseDTO;
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

                var userResponse = _context.ApplicationUsers.First(u => u.UserName == request.Email);

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
