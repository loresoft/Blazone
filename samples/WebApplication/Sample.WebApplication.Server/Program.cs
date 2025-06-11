using Blazone.Authentication;
using Blazone.Authentication.Providers;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Identity.Web;

using Sample.Shared;
using Sample.WebApplication.Server.Components;

namespace Sample.WebApplication.Server;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

        ConfigureServices(builder);

        var app = builder.Build();

        ConfigureMiddleware(app);

        app.Run();
    }

    private static void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Services
            .AddMicrosoftIdentityWebAppAuthentication(builder.Configuration);

        builder.Services
            .AddAuthorization()
            .AddBlazoneServer()
            .AddBlazoneClient<PersistingServerAuthenticationStateProvider>()
            .AddAntiforgery();

        builder.Services
            .AddRazorComponents()
            .AddInteractiveWebAssemblyComponents();

        builder.Services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen();

        builder.Services
            .TryAddSingleton<WeatherForecastService>();

        builder.Services.Configure<CookieAuthenticationOptions>(
            name: CookieAuthenticationDefaults.AuthenticationScheme,
            configureOptions: options => options.Cookie.Name = ".Sample.Authentication"
        );

    }

    private static void ConfigureMiddleware(Microsoft.AspNetCore.Builder.WebApplication app)
    {
        if (app.Environment.IsDevelopment())
            app.UseWebAssemblyDebugging();
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseHttpsRedirection();

        app.UseStaticFiles();
        app.UseAntiforgery();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapBlazoneEndpoints();

        app.MapRazorComponents<App>()
            .AddInteractiveWebAssemblyRenderMode()
            .AddAdditionalAssemblies(typeof(Client._Imports).Assembly);
    }
}
