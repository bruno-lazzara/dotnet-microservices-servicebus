using Newtonsoft.Json;
using Orange.Models.DTO;
using Orange.Web.Services.Interfaces;
using System.Text;

namespace Orange.Web.Services
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenProvider _tokenProvider;
        public BaseService(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider)
        {
            _httpClientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;
        }

        public async Task<HttpResponseMessage?> SendAsync(RequestDTO request, bool withBearer = true)
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

                if (withBearer)
                {
                    var token = _tokenProvider.GetToken();
                    message.Headers.Add("Authorization", $"Bearer {token}");
                }

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
