using Microsoft.AspNetCore.Authentication;

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
            if (httpContext != null)
            {
                string _token = await httpContext.GetTokenAsync("access_token") ?? string.Empty;

                if (!string.IsNullOrEmpty(_token))
                    request.Headers.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
