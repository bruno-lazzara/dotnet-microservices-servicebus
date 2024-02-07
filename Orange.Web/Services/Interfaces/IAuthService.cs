using Orange.Models.DTO.Auth;

namespace Orange.Web.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginUserResponseDTO?> LoginAsync(LoginUserDTO user);
        Task<UserDTO?> RegisterAsync(RegisterUserDTO user);
        Task<bool> AssignRoleAsync(RegisterUserDTO user);
    }
}
