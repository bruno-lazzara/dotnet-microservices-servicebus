using Microsoft.EntityFrameworkCore;
using Orange.Services.RewardAPI.Data;
using Orange.Services.RewardAPI.Message;
using Orange.Services.RewardAPI.Models.Entity;

namespace Orange.Services.RewardAPI.Services
{
    public class RewardService : IRewardService
    {
        private DbContextOptions<OrangeDbContext> _dbOptions;

        public RewardService(DbContextOptions<OrangeDbContext> dbOptions)
        {
            _dbOptions = dbOptions;
        }

        public async Task UpdateRewardsAsync(RewardsMessage rewardsMessage)
        {
            try
            {
                //TODO - send the e-mail using a service like SendGrid

                Rewards rewards = new()
                {
                    OrderId = rewardsMessage.OrderId,
                    RewardsActivity = rewardsMessage.RewardsActivity,
                    UserId = rewardsMessage.UserId,
                    RewardsDate = DateTime.Now
                };

                await using var _context = new OrangeDbContext(_dbOptions);

                await _context.Rewards.AddAsync(rewards);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
            }
        }
    }
}
