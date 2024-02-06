namespace Orange.Models.DTO.Auth
{
    public class RegisterUserDTO : BaseUserDTO
    {
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
