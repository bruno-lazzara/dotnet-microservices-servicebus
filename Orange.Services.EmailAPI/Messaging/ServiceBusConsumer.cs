using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using Orange.MessageBus;
using Orange.Models.DTO;
using Orange.Services.EmailAPI.Message;
using Orange.Services.EmailAPI.Services;
using System.Text;

namespace Orange.Services.EmailAPI.Messaging
{
    public class ServiceBusConsumer : IServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string emailCartQueue;
        private readonly string emailNewUserQueue;
        private readonly string orderCreatedTopic;
        private readonly string orderCreatedEmailSubscription;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;
        private readonly ServiceBusProcessor _emailCartProcessor;
        private readonly ServiceBusProcessor _emailNewUserProcessor;
        private readonly ServiceBusProcessor _emailOrderPlacedProcessor;
        public ServiceBusConsumer(IConfiguration configuration, EmailService emailService)
        {
            _emailService = emailService;
            _configuration = configuration;

            serviceBusConnectionString = Constants.ConnectionString;

            emailCartQueue = _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue");
            emailNewUserQueue = _configuration.GetValue<string>("TopicAndQueueNames:EmailRegisteredUserQueue");

            orderCreatedTopic = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
            orderCreatedEmailSubscription = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedEmailSubscription");

            var client = new ServiceBusClient(serviceBusConnectionString);
            _emailCartProcessor = client.CreateProcessor(emailCartQueue);
            _emailNewUserProcessor = client.CreateProcessor(emailNewUserQueue);
            _emailOrderPlacedProcessor = client.CreateProcessor(orderCreatedTopic, orderCreatedEmailSubscription);
        }

        public async Task Start()
        {
            _emailCartProcessor.ProcessMessageAsync += OnEmailCartRequestReceivedAsync;
            _emailCartProcessor.ProcessErrorAsync += ErrorHandlerAsync;

            _emailNewUserProcessor.ProcessMessageAsync += OnEmailNewUserReceivedAsync;
            _emailNewUserProcessor.ProcessErrorAsync += ErrorHandlerAsync;

            _emailOrderPlacedProcessor.ProcessMessageAsync += OnOrderPlacedRequestReceivedAsync;
            _emailOrderPlacedProcessor.ProcessErrorAsync += ErrorHandlerAsync;

            await _emailCartProcessor.StartProcessingAsync();
            await _emailNewUserProcessor.StartProcessingAsync();
            await _emailOrderPlacedProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await _emailCartProcessor.StopProcessingAsync();
            await _emailCartProcessor.DisposeAsync();

            await _emailNewUserProcessor.StopProcessingAsync();
            await _emailNewUserProcessor.DisposeAsync();

            await _emailOrderPlacedProcessor.StopProcessingAsync();
            await _emailOrderPlacedProcessor.DisposeAsync();
        }

        private async Task OnEmailCartRequestReceivedAsync(ProcessMessageEventArgs args)
        {
            try
            {
                var message = args.Message;
                var body = Encoding.UTF8.GetString(message.Body);

                CartDTO objMessage = JsonConvert.DeserializeObject<CartDTO>(body);

                await _emailService.EmailCartAndLogAsync(objMessage);

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

                await _emailService.RegisterUserEmailAndLogAsync(email);

                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task OnOrderPlacedRequestReceivedAsync(ProcessMessageEventArgs args)
        {
            try
            {
                var message = args.Message;
                var body = Encoding.UTF8.GetString(message.Body);

                RewardsMessage rewardsMessage = JsonConvert.DeserializeObject<RewardsMessage>(body);

                await _emailService.LogOrderPlacedAsync(rewardsMessage);

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
