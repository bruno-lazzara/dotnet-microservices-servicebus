using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using Orange.MessageBus;
using Orange.Models.DTO;
using Orange.Services.EmailAPI.Services;
using System.Text;

namespace Orange.Services.EmailAPI.Messaging
{
    public class ServiceBusConsumer : IServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string emailCartQueue;
        private readonly string emailNewUserQueue;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;
        private readonly ServiceBusProcessor _emailCartProcessor;
        private readonly ServiceBusProcessor _emailNewUserProcessor;
        public ServiceBusConsumer(IConfiguration configuration, EmailService emailService)
        {
            _emailService = emailService;
            _configuration = configuration;

            serviceBusConnectionString = Constants.ConnectionString;
            emailCartQueue = _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue");
            emailNewUserQueue = _configuration.GetValue<string>("TopicAndQueueNames:EmailRegisteredUserQueue");

            var client = new ServiceBusClient(serviceBusConnectionString);
            _emailCartProcessor = client.CreateProcessor(emailCartQueue);
            _emailNewUserProcessor = client.CreateProcessor(emailNewUserQueue);
        }

        public async Task Start()
        {
            _emailCartProcessor.ProcessMessageAsync += OnEmailCartRequestReceivedAsync;
            _emailCartProcessor.ProcessErrorAsync += ErrorHandlerAsync;

            _emailNewUserProcessor.ProcessMessageAsync += OnEmailNewUserReceivedAsync;
            _emailNewUserProcessor.ProcessErrorAsync += ErrorHandlerAsync;

            await _emailCartProcessor.StartProcessingAsync();
            await _emailNewUserProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await _emailCartProcessor.StopProcessingAsync();
            await _emailCartProcessor.DisposeAsync();

            await _emailNewUserProcessor.StopProcessingAsync();
            await _emailNewUserProcessor.DisposeAsync();
        }

        private async Task OnEmailCartRequestReceivedAsync(ProcessMessageEventArgs args)
        {
            try
            {
                var message = args.Message;
                var body = Encoding.UTF8.GetString(message.Body);

                CartDTO objMessage = JsonConvert.DeserializeObject<CartDTO>(body);

                await _emailService.EmailCartAndLog(objMessage);

                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task OnEmailNewUserReceivedAsync(ProcessMessageEventArgs args)
        {
            try
            {
                var message = args.Message;
                var body = Encoding.UTF8.GetString(message.Body);

                string email = JsonConvert.DeserializeObject<string>(body);

                await _emailService.RegisterUserEmailAndLog(email);

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
