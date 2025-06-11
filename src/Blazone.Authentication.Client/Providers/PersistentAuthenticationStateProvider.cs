using System.Security.Claims;

using Blazone.Authentication.Models;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Blazone.Authentication.Providers;

/// <summary>
///     AuthenticationStateProvider that determines the user's authentication state by
///     looking for data persisted in the page when it was rendered on the server.
/// </summary>
/// <remarks>
/// <para>
///     This is a client-side AuthenticationStateProvider that determines the user's authentication state by
///     looking for data persisted in the page when it was rendered on the server. This authentication state will
///     be fixed for the lifetime of the WebAssembly application. So, if the user needs to log in or out, a full
///     page reload is required.
/// </para>
/// <para>
///     This only provides a user name and email for display purposes. It does not actually include any tokens
///     that authenticate to the server when making subsequent requests. That works separately using a
///     cookie that will be included on HttpClient requests to the server.
/// </para>
/// </remarks>
/// <seealso cref="Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider" />
public class PersistentAuthenticationStateProvider : AuthenticationStateProvider
{
    private static readonly Task<AuthenticationState> _unauthenticatedTask =
        Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));

    private readonly Task<AuthenticationState> _authenticationStateTask = _unauthenticatedTask;

    public PersistentAuthenticationStateProvider(PersistentComponentState componentState)
    {
        if (!componentState.TryTakeFromJson<AuthenticationStateData?>(nameof(AuthenticationStateData), out var userData) || userData is null)
            return;

        var claims = userData.Claims?.Select(c => new Claim(c.Type, c.Value));

        var identity = new ClaimsIdentity(
            claims: claims,
            authenticationType: nameof(PersistentAuthenticationStateProvider),
            nameType: userData.NameClaimType,
            roleType: userData.RoleClaimType);

        var principal = new ClaimsPrincipal(identity);
        var authenticationState = new AuthenticationState(principal);

        _authenticationStateTask = Task.FromResult(authenticationState);
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync() => _authenticationStateTask;
}
