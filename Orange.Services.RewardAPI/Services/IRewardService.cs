using Orange.Services.RewardAPI.Message;

namespace Orange.Services.RewardAPI.Services
{
    public interface IRewardService
    {
        Task UpdateRewardsAsync(RewardsMessage rewardsMessage);
    }
}
