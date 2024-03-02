using System.Net.Http.Json;
using System.Security.Claims;

using Blazone.Authentication.Models;
using Blazone.Authentication.Options;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Blazone.Authentication.Providers;

public class ServerAuthenticationStateProvider : AuthenticationStateProvider
{
    private static readonly ClaimsPrincipal _notAuthorized = new(new ClaimsIdentity());
    private const string _cacheKey = "Blazone:User:Current";

    private readonly HttpClient _httpClient;
    private readonly ILogger<ServerAuthenticationStateProvider> _logger;
    private readonly AuthenticationEndpointOptions _authenticationOptions;
    private readonly IMemoryCache _memoryCache;

    public ServerAuthenticationStateProvider(
        HttpClient httpClient,
        ILogger<ServerAuthenticationStateProvider> logger,
        IOptions<AuthenticationEndpointOptions> authenticationOptions,
        IMemoryCache memoryCache)
    {
        _httpClient = httpClient;
        _logger = logger;
        _authenticationOptions = authenticationOptions.Value;
        _memoryCache = memoryCache;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var claimsPrincipal = await _memoryCache.GetOrCreateAsync(_cacheKey, LoadUser);
            return new AuthenticationState(claimsPrincipal ?? _notAuthorized);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user authentication: {message}", ex.Message);
            return new AuthenticationState(_notAuthorized);
        }
    }

    private async Task<ClaimsPrincipal> LoadUser(ICacheEntry cacheEntry)
    {
        var userUrl = _authenticationOptions.UserUrl();
        var userData = await _httpClient.GetFromJsonAsync<UserClaim>(userUrl);
        if (userData == null || !userData.IsAuthenticated)
            return _notAuthorized;

        var claims = userData.Claims?.Select(c => new Claim(c.Type, c.Value));

        var identity = new ClaimsIdentity(
            claims: claims,
            authenticationType: nameof(ServerAuthenticationStateProvider),
            nameType: userData.NameClaimType,
            roleType: userData.RoleClaimType);

        _logger.LogInformation("User {userName} authenticated.", identity.Name);

        // set cache time
        cacheEntry.SlidingExpiration = _authenticationOptions.UserCacheTime;

        return new ClaimsPrincipal(identity);
    }
}
