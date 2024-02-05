using Microsoft.AspNetCore.Identity;

namespace Orange.Services.AuthAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
    }
}
