using System.Net;

using Blazone.Authentication.Endpoints;
using Blazone.Authentication.Options;

using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Blazone.Authentication;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddBlazoneServer(this IServiceCollection services, Action<AuthenticationEndpointOptions>? configure = null)
    {
        // endpoint builder
        services.TryAddSingleton<AuthenticationEndpointBuilder>();

        // override redirects for endpoints
        services.Configure<OpenIdConnectOptions>(
                OpenIdConnectDefaults.AuthenticationScheme,
                options => options.Events.OnRedirectToIdentityProvider = HandleRedirect
            );

        services.AddOptions<AuthenticationEndpointOptions>();
        if (configure != null)
            services.Configure(configure);

        // include AuthenticationJsonContext in JsonOptions by default
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<JsonOptions>, AuthenticationJsonOptionsSetup>());

        return services;
    }


    public static IEndpointRouteBuilder MapBlazoneEndpoints(this IEndpointRouteBuilder builder)
    {
        if (builder is null)
            throw new ArgumentNullException(nameof(builder));

        var authenticationEndpoint = builder.ServiceProvider.GetRequiredService<AuthenticationEndpointBuilder>();
        authenticationEndpoint?.AddRoutes(builder);

        return builder;
    }

    private static Task HandleRedirect(RedirectContext context)
    {
        var options = context.HttpContext.RequestServices.GetService<IOptions<AuthenticationEndpointOptions>>();
        var preventRoutes = options?.Value.PreventRedirectRoutes ?? ["/api"];
        var path = context.Request.Path;

        // don't redirect for ajax/json calls
        if (preventRoutes.Any(route => path.StartsWithSegments(route)))
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            context.HandleResponse();
        }

        return Task.CompletedTask;
    }
}
