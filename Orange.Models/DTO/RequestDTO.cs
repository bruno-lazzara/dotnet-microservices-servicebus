namespace Orange.Models.DTO
{
    public class RequestDTO<TRequest>
    {
        public HttpMethod HttpMethod { get; set; } = HttpMethod.Get;
        public string Url { get; set; }
        public TRequest Data { get; set; }
        public string AccessToken { get; set; }
    }
}
