using Orange.Models.DTO.Auth;

namespace Orange.Services.AuthAPI.Services.Interfaces
{
    public interface IAuthService
    {
        Task<UserDTO?> RegisterAsync(RegisterUserDTO request);
        Task<LoginUserResponseDTO> LoginAsync(LoginUserDTO request);
    }
}
