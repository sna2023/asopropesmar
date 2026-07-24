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

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (_httpContextAccessor.HttpContext != null)
            {
                string token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token") ?? string.Empty;

                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
