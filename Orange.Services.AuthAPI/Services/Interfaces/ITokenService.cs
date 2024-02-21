using Orange.Services.AuthAPI.Models;

namespace Orange.Services.AuthAPI.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(ApplicationUser user, IEnumerable<string> roles);
    }
}
