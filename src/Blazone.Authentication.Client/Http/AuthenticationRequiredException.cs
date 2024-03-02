using Blazone.Authentication.Options;

using Microsoft.AspNetCore.Components;

namespace Blazone.Authentication.Http;

#pragma warning disable RCS1194 // Implement exception constructors
public class AuthenticationRequiredException : Exception
#pragma warning restore RCS1194 // Implement exception constructors
{
    private readonly NavigationManager _navigationManager;
    private readonly AuthenticationEndpointOptions _authenticationOptions;

    public AuthenticationRequiredException(NavigationManager navigationManager, AuthenticationEndpointOptions authenticationOptions)
        : base("The requested endpoint required authentication")
    {
        _navigationManager = navigationManager;
        _authenticationOptions = authenticationOptions;
    }

    public void Redirect()
    {
        var signInUrl = _authenticationOptions.SignInUrl(_navigationManager.Uri);
        var absoluteUri = _navigationManager.ToAbsoluteUri(signInUrl);

        _navigationManager.NavigateTo(absoluteUri.ToString(), true);
    }
}
