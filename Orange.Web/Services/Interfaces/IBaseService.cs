using Orange.Models.DTO;

namespace Orange.Web.Services.Interfaces
{
    public interface IBaseService
    {
        Task<HttpResponseMessage?> SendAsync<TRequest>(RequestDTO<TRequest> request);
    }
}
