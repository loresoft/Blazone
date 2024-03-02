using System.Net.Http.Headers;
using System.Net.Mime;

using Blazone.Authentication;
using Blazone.Authentication.Http;

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using Sample.WebAssembly.Client.Services;

namespace Sample.WebAssembly.Client;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.Services.AddBlazoneClient();
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

        var app = builder.Build();

        await app.RunAsync();
    }
}
