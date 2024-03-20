using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using Orange.MessageBus;
using Orange.Models.DTO;
using System.Text;

namespace Orange.Services.EmailAPI.Messaging
{
    public class ServiceBusConsumer : IServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string emailCartQueue;
        private readonly IConfiguration _configuration;
        private readonly ServiceBusProcessor _emailCartProcessor;
        public ServiceBusConsumer(IConfiguration configuration)
        {
            _configuration = configuration;

            serviceBusConnectionString = Constants.ConnectionString;
            emailCartQueue = _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue");

            var client = new ServiceBusClient(serviceBusConnectionString);
            _emailCartProcessor = client.CreateProcessor(emailCartQueue);
        }

        public async Task Start()
        {
            _emailCartProcessor.ProcessMessageAsync += OnEmailCartRequestReceivedAsync;
            _emailCartProcessor.ProcessErrorAsync += ErrorHandlerAsync;
            await _emailCartProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await _emailCartProcessor.StopProcessingAsync();
            await _emailCartProcessor.DisposeAsync();
        }

        private async Task OnEmailCartRequestReceivedAsync(ProcessMessageEventArgs args)
        {
            try
            {
                var message = args.Message;
                var body = Encoding.UTF8.GetString(message.Body);

                CartDTO objMessage = JsonConvert.DeserializeObject<CartDTO>(body);

                //TODO - log email

                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private Task ErrorHandlerAsync(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
    }
}
