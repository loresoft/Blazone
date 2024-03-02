using Blazone.Authentication.Http;
using Blazone.Authentication.Options;
using Blazone.Authentication.Providers;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Blazone.Authentication;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddBlazoneClient(this IServiceCollection services, Action<AuthenticationEndpointOptions>? configure = null)
    {
        services.AddOptions<AuthenticationEndpointOptions>();
        if (configure != null)
            services.Configure(configure);

        services.AddAuthorizationCore();
        services.AddMemoryCache();

        services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();
        services.AddTransient<AuthenticationRequiredHandler>();

        return services;
    }

}
