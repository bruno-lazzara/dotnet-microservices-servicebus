namespace Orange.Models.DTO
{
    public class ResponseDTO<TResult>
    {
        public TResult? Result { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
