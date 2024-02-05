namespace Orange.Models.DTO.Auth
{
    public class LoginUserResponseDTO
    {
        public UserDTO? User { get; set; }
        public string Token { get; set; } = string.Empty;
    }
}
