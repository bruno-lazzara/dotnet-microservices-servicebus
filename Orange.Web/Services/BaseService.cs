using Newtonsoft.Json;
using Orange.Models.DTO;
using Orange.Web.Services.Interfaces;
using System.Text;

namespace Orange.Web.Services
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public BaseService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<HttpResponseMessage?> SendAsync<TRequest>(RequestDTO<TRequest> request)
        {
            HttpResponseMessage? response = null;
            try
            {
                HttpClient client = _httpClientFactory.CreateClient("OrangeAPI");
                HttpRequestMessage message = new()
                {
                    RequestUri = new Uri(request.Url)
                };
                message.Headers.Add("Accept", "application/json");
                if (request.Data != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(request.Data), Encoding.UTF8, "application/json");
                }
                message.Method = request.HttpMethod;

                response = await client.SendAsync(message);
            }
            catch (Exception ex)
            {

            }

            return response;
        }
    }
}
