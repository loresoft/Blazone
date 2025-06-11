using Blazone.Authentication.Http;
using Blazone.Authentication.Options;
using Blazone.Authentication.Providers;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Blazone.Authentication;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddBlazoneClient(this IServiceCollection services, Action<AuthenticationEndpointOptions>? configure = null)
    {
        return AddBlazoneClient<EndpointAuthenticationStateProvider>(services, configure);
    }

    public static IServiceCollection AddBlazoneClient<TProvider>(this IServiceCollection services, Action<AuthenticationEndpointOptions>? configure = null)
        where TProvider : AuthenticationStateProvider
    {
        services.AddOptions<AuthenticationEndpointOptions>();
        if (configure != null)
            services.Configure(configure);

        services.AddAuthorizationCore();
        services.AddMemoryCache();

        services.TryAddTransient<AuthenticationStateProvider, TProvider>();
        services.TryAddTransient<AuthenticationRequiredHandler>();

        return services;
    }
}
