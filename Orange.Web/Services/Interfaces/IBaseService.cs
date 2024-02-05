using Orange.Models.DTO;

namespace Orange.Web.Services.Interfaces
{
    public interface IBaseService
    {
        Task<ResponseDTO<TResponse>?> SendAsync<TRequest, TResponse>(RequestDTO<TRequest> request);
    }
}
