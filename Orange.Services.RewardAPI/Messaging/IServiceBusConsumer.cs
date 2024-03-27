namespace Orange.Services.RewardAPI.Messaging
{
    public interface IServiceBusConsumer
    {
        Task Start();
        Task Stop();
    }
}
