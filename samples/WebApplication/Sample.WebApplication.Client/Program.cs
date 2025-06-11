using System.Net.Http.Headers;
using System.Net.Mime;

using Blazone.Authentication;
using Blazone.Authentication.Http;
using Blazone.Authentication.Providers;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using Sample.WebApplication.Client.Services;

namespace Sample.WebApplication.Client;

internal class Program
{
    static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);

        builder.Services.AddBlazoneClient<PersistentAuthenticationStateProvider>();
        builder.Services.AddCascadingAuthenticationState();


        builder.Services
            .AddHttpClient<GatewayClient>(client =>
            {
                client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
                client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
            })
            .AddHttpMessageHandler<AuthenticationRequiredHandler>();

        builder.Services
            .AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

        await builder.Build().RunAsync();
    }
}
