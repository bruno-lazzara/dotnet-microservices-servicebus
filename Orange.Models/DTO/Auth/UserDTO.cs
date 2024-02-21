namespace Orange.Models.DTO.Auth
{
    public class UserDTO : BaseUserDTO
    {
        public string Id { get; set; }
        public List<string> Roles { get; set; } = [];
    }
}
