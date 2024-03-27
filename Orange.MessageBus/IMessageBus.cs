namespace Orange.MessageBus
{
    public interface IMessageBus
    {
        Task PublishMessageAsync(object message, string topicQueueName);
    }
}
