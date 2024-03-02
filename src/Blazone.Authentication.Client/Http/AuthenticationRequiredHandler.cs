using System.Net;

using Blazone.Authentication.Options;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;

namespace Blazone.Authentication.Http;

public class AuthenticationRequiredHandler : DelegatingHandler
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly NavigationManager _navigationManager;
    private readonly AuthenticationEndpointOptions _authenticationOptions;

    public AuthenticationRequiredHandler(AuthenticationStateProvider authenticationStateProvider, NavigationManager navigationManager, IOptions<AuthenticationEndpointOptions> authenticationOptions)
    {
        _authenticationStateProvider = authenticationStateProvider;
        _navigationManager = navigationManager;
        _authenticationOptions = authenticationOptions.Value;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var state = await _authenticationStateProvider.GetAuthenticationStateAsync();

        var responseMessage = state.User.Identity?.IsAuthenticated == true
            ? await base.SendAsync(request, cancellationToken)
            : new HttpResponseMessage(HttpStatusCode.Unauthorized);

        if (responseMessage.StatusCode == HttpStatusCode.Unauthorized)
            throw new AuthenticationRequiredException(_navigationManager, _authenticationOptions);

        return responseMessage;
    }
}
