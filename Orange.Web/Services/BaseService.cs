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

        public async Task<ResponseDTO<TResponse>?> SendAsync<TRequest, TResponse>(RequestDTO<TRequest> request)
        {
            ResponseDTO<TResponse>? responseDTO = new();
            try
            {
                HttpClient client = _httpClientFactory.CreateClient("OrangeAPI");
                HttpRequestMessage message = new();
                message.Headers.Add("Accept", "application/json");
                message.RequestUri = new Uri(request.Url);
                if (request.Data != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(request.Data), Encoding.UTF8, "application/json");
                }
                message.Method = request.HttpMethod;

                HttpResponseMessage? response = null;

                response = await client.SendAsync(message);

                if (!response.IsSuccessStatusCode)
                {
                    responseDTO.Message = $"Error - returned status code {response.StatusCode}.";
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    responseDTO = JsonConvert.DeserializeObject<ResponseDTO<TResponse>>(content);
                }
            }
            catch (Exception ex)
            {
                responseDTO = new()
                {
                    Message = $"Error"
                };
            }

            return responseDTO;
        }
    }
}
