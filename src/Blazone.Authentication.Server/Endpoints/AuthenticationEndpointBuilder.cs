using System.Security.Claims;

using Blazone.Authentication.Models;
using Blazone.Authentication.Options;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

namespace Blazone.Authentication.Endpoints;

public class AuthenticationEndpointBuilder
{
    private readonly AuthenticationEndpointOptions _options;

    public AuthenticationEndpointBuilder(IOptions<AuthenticationEndpointOptions> options)
    {
        _options = options.Value;
    }

    public virtual void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup(_options.RoutePrefix);

        group
            .MapGet(_options.SignInRoute, SignIn)
            .WithTags("Authentication")
            .WithSummary("Initiate authentication challenge")
            .WithDescription("Initiate authentication challenge")
            .AllowAnonymous();

        group
            .MapGet(_options.SignOutRoute, SignOut)
            .WithTags("Authentication")
            .WithSummary("Initiate sign-out operation")
            .WithDescription("Initiate sign-out operation")
            .AllowAnonymous();

        group
            .MapGet(_options.UserRoute, GetUser)
            .Produces<UserClaim>()
            .WithTags("Authentication")
            .WithSummary("Get current user claims")
            .WithDescription("Get current user claims")
            .AllowAnonymous();
    }

    private Ok<UserClaim> GetUser(ClaimsPrincipal principal)
    {
        var identity = principal.Identity as ClaimsIdentity;

        if (identity == null)
            return TypedResults.Ok(new UserClaim(false));

        var claims = identity.Claims
            .Select(c => new ClaimValue(c.Type, c.Value))
            .ToList();

        var userAuthentication = new UserClaim(
            identity.IsAuthenticated,
            identity.NameClaimType,
            identity.RoleClaimType,
            claims);

        return TypedResults.Ok(userAuthentication);
    }

    private ChallengeHttpResult SignIn([FromQuery] string? returnUrl)
    {
        var properties = GetAuthProperties(returnUrl);
        return TypedResults.Challenge(properties, [OpenIdConnectDefaults.AuthenticationScheme]);
    }

    private SignOutHttpResult SignOut([FromQuery] string? returnUrl)
    {
        var properties = GetAuthProperties(returnUrl);
        return TypedResults.SignOut(properties, [OpenIdConnectDefaults.AuthenticationScheme, CookieAuthenticationDefaults.AuthenticationScheme]);
    }

    private AuthenticationProperties GetAuthProperties(string? returnUrl)
    {
        const string pathBase = "/";

        if (string.IsNullOrEmpty(returnUrl))
            returnUrl = pathBase;
        else if (!Uri.IsWellFormedUriString(returnUrl, UriKind.Relative))
            returnUrl = new Uri(returnUrl, UriKind.Absolute).PathAndQuery;
        else if (returnUrl[0] != '/')
            returnUrl = $"{pathBase}{returnUrl}";

        return new AuthenticationProperties { RedirectUri = returnUrl,  };
    }

}
