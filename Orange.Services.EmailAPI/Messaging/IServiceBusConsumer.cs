namespace Orange.Services.EmailAPI.Messaging
{
    public interface IServiceBusConsumer
    {
        Task Start();
        Task Stop();
    }
}
