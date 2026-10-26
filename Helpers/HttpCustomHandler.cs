using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;

namespace AsopropesmarApp.Helpers;

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
        var _token = authState.User.FindFirst("access_token")?.Value ?? string.Empty;

        if (!string.IsNullOrEmpty(_token))
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", _token);

        return await base.SendAsync(request, cancellationToken);
    }
}
