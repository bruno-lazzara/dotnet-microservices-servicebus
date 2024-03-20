namespace Orange.MessageBus
{
    public interface IMessageBus
    {
        Task PublishMessage(object message, string topicQueueName);
    }
}
