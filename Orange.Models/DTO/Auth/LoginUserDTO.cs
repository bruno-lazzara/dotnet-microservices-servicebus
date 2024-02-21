using System.ComponentModel.DataAnnotations;

namespace Orange.Models.DTO.Auth
{
    public class LoginUserDTO
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
