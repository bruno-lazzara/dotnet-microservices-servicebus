using Orange.Models.DTO;
using Orange.Services.EmailAPI.Message;

namespace Orange.Services.EmailAPI.Services
{
    public interface IEmailService
    {
        Task EmailCartAndLogAsync(CartDTO cartDTO);
        Task RegisterUserEmailAndLogAsync(string email);
        Task LogOrderPlacedAsync(RewardsMessage rewardsMessage);
    }
}
