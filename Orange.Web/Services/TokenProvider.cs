using Orange.Web.Services.Interfaces;
using Orange.Web.Utils;

namespace Orange.Web.Services
{
    public class TokenProvider : ITokenProvider
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public TokenProvider(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public void ClearToken()
        {
            _contextAccessor.HttpContext?.Response.Cookies.Delete(Constants.TOKEN_KEY);
        }

        public string? GetToken()
        {
            string? token = null;
            _contextAccessor.HttpContext?.Request.Cookies.TryGetValue(Constants.TOKEN_KEY, out token);
            return token;
        }

        public void SetToken(string token)
        {
            _contextAccessor.HttpContext?.Response.Cookies.Append(Constants.TOKEN_KEY, token);
        }
    }
}
