using Microsoft.AspNetCore.Components;

namespace Blazone.Authentication.Options;

public class AuthenticationEndpointOptions
{
    private const char separator = '/';

    public string RoutePrefix { get; set; } = "/blazone";

    public string SignInRoute { get; set; } = "/signin";

    public string SignOutRoute { get; set; } = "/signout";

    public string UserRoute { get; set; } = "/user";

    public string ReturnDefault { get; set; } = "/";

    public string[] PreventRedirectRoutes { get; set; } = ["/api"];

    public TimeSpan UserCacheTime { get; set; } = TimeSpan.FromMinutes(20);

    public string SignInUrl(string? returnUrl = null)
        => BuildReturn(RoutePrefix, SignInRoute, returnUrl ?? "/");

    public string SignOutUrl(string? returnUrl = null)
        => BuildReturn(RoutePrefix, SignOutRoute, returnUrl ?? "/");

    public string UserUrl()
        => BuildPath(RoutePrefix, UserRoute);

    private static string BuildPath(string prefix, string route)
    {
        var prefixSpan = prefix.AsSpan().Trim(separator);
        var routeSpan = route.AsSpan().Trim(separator);

        return $"/{prefixSpan}/{routeSpan}";
    }

    private static string BuildReturn(string prefix, string route, string returnUrl)
    {
        var prefixSpan = prefix.AsSpan().Trim(separator);
        var routeSpan = route.AsSpan().Trim(separator);
        var returnSpan = Uri.EscapeDataString(returnUrl);

        return $"/{prefixSpan}/{routeSpan}?returnUrl={returnSpan}";
    }
}
