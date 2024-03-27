using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System.Text;

namespace Orange.MessageBus
{
    public class MessageBus : IMessageBus
    {
        public async Task PublishMessageAsync(object message, string topicQueueName)
        {
            await using var client = new ServiceBusClient(Constants.ConnectionString);

            ServiceBusSender sender = client.CreateSender(topicQueueName);

            var json = JsonConvert.SerializeObject(message);
            ServiceBusMessage finalMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(json))
            {
                CorrelationId = Guid.NewGuid().ToString()
            };

            await sender.SendMessageAsync(finalMessage);
            await client.DisposeAsync();
        }
    }
}
