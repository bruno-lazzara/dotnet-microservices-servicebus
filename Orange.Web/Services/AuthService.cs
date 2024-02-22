using Newtonsoft.Json;
using Orange.Models.DTO;
using Orange.Models.DTO.Auth;
using Orange.Web.Services.Interfaces;
using Orange.Web.Utils;

namespace Orange.Web.Services
{
    public class AuthService : IAuthService
    {
        private readonly IBaseService _baseService;
        public AuthService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<bool> AssignRoleAsync(RegisterUserDTO user)
        {
            bool roleAssigned = false;
            try
            {
                var response = await _baseService.SendAsync(new RequestDTO
                {
                    HttpMethod = HttpMethod.Post,
                    Url = Routes.AuthAPI + $"/api/auth/assignrole",
                    Data = user
                });

                if (response != null && response.IsSuccessStatusCode)
                {
                    roleAssigned = true;
                }
            }
            catch (Exception ex)
            {

            }
            return roleAssigned;
        }

        public async Task<LoginUserResponseDTO?> LoginAsync(LoginUserDTO user)
        {
            LoginUserResponseDTO? userResponseDTO = null;
            try
            {
                var response = await _baseService.SendAsync(new RequestDTO
                {
                    HttpMethod = HttpMethod.Post,
                    Url = Routes.AuthAPI + $"/api/auth/login",
                    Data = user
                }, false);

                if (response != null && response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    userResponseDTO = JsonConvert.DeserializeObject<LoginUserResponseDTO>(content);
                }
            }
            catch (Exception ex)
            {

            }
            return userResponseDTO;
        }

        public async Task<UserDTO?> RegisterAsync(RegisterUserDTO user)
        {
            UserDTO? userDTO = null;
            try
            {
                var response = await _baseService.SendAsync(new RequestDTO
                {
                    HttpMethod = HttpMethod.Post,
                    Url = Routes.AuthAPI + $"/api/auth/register",
                    Data = user
                }, false);

                if (response != null && response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    userDTO = JsonConvert.DeserializeObject<UserDTO>(content);
                }
            }
            catch (Exception ex)
            {

            }
            return userDTO;
        }
    }
}
