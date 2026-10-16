using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace AsopropesmarApp.Helpers
{
    public class HttpCustomHandler : DelegatingHandler
    {
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public HttpCustomHandler(AuthenticationStateProvider authenticationStateProvider)
        {
            _authenticationStateProvider = authenticationStateProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user?.Identity?.IsAuthenticated == true)
            {
                var accessTokenClaim = user.FindFirst("access_token");
                if (accessTokenClaim != null && !string.IsNullOrEmpty(accessTokenClaim.Value))
                {
                    request.Headers.Authorization =
                        new AuthenticationHeaderValue("Bearer", accessTokenClaim.Value);
                }
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
