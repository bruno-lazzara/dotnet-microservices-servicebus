
namespace Orange.Models.DTO
{
    public class RequestDTO
    {
        public HttpMethod HttpMethod { get; set; } = HttpMethod.Get;
        public string Url { get; set; }
        public object? Data { get; set; }
        public string AccessToken { get; set; }
        public ContentType ContentType { get; set; } = ContentType.Json;
    }
}
