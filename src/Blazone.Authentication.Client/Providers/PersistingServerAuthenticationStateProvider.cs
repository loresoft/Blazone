using System.Diagnostics;
using System.Security.Claims;
using System.Security.Principal;

using Blazone.Authentication.Models;
using Blazone.Authentication.Options;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Blazone.Authentication.Providers;

/// <summary>
///     AuthenticationStateProvider that uses PersistentComponentState to flow the authentication
///     state to the client which is then fixed for the lifetime of the WebAssembly application.
/// </summary>
/// <remarks>
///     This is a server-side AuthenticationStateProvider that uses PersistentComponentState to flow
///     the authentication state to the client which is then fixed for the lifetime of the
///     WebAssembly application.
/// </remarks>
/// <seealso cref="Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider" />
/// <seealso cref="Microsoft.AspNetCore.Components.Authorization.IHostEnvironmentAuthenticationStateProvider" />
public sealed class PersistingServerAuthenticationStateProvider
    : ServerAuthenticationStateProvider, IDisposable
{
    private readonly PersistentComponentState _componentState;
    private readonly PersistingComponentStateSubscription _componentSubscription;

    private Task<AuthenticationState>? _authenticationStateTask;

    public PersistingServerAuthenticationStateProvider(PersistentComponentState persistentComponentState)
    {
        _componentState = persistentComponentState;

        AuthenticationStateChanged += OnAuthenticationStateChanged;
        _componentSubscription = _componentState.RegisterOnPersisting(OnPersistingAsync, RenderMode.InteractiveWebAssembly);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _componentSubscription.Dispose();
        AuthenticationStateChanged -= OnAuthenticationStateChanged;
    }

    private void OnAuthenticationStateChanged(Task<AuthenticationState> task)
    {
        _authenticationStateTask = task;
    }

    private async Task OnPersistingAsync()
    {
        if (_authenticationStateTask is null)
            throw new UnreachableException($"Authentication state not set in {nameof(OnPersistingAsync)}().");

        var authenticationState = await _authenticationStateTask;
        var principal = authenticationState.User;
        var identity = principal.Identity as ClaimsIdentity;

        if (identity?.IsAuthenticated != true)
            return;

        var claims = identity.Claims
            .Select(c => new ClaimValue(c.Type, c.Value))
            .ToList();

        var userAuthentication = new UserClaim(
            identity.IsAuthenticated,
            identity.NameClaimType,
            identity.RoleClaimType,
            claims);

        _componentState.PersistAsJson(nameof(UserClaim), userAuthentication);
    }
}
