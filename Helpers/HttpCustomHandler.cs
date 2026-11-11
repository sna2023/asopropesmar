using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;

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
            var token = user.FindFirst("access_token")?.Value;

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
