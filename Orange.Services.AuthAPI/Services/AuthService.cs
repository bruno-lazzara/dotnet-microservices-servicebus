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
        public AuthService(OrangeDbContext dbContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
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

                UserDTO userDTO = new()
                {
                    Email = user.Email,
                    Id = user.Id,
                    Name = user.Name,
                    PhoneNumber = user.PhoneNumber
                };

                responseDTO.User = userDTO;
                //responseDTO.Token = generate token
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