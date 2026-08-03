using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;

namespace AsopropesmarApp.Helpers
{
    public class HttpCustomHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpCustomHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            string _token = string.Empty;

            if (httpContext != null)
            {
                _token = await httpContext.GetTokenAsync("access_token") ?? string.Empty;
            }

            if (!string.IsNullOrEmpty(_token))
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", _token);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}