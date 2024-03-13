using Blazone.Authentication;

using Microsoft.Identity.Web;

using Sample.ServerRender.Web.Components;
using Sample.ServerRender.Web.Services;

namespace Sample.ServerRender.Web;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

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
            .AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services
            .AddAuthorization()
            .AddBlazoneServer()
            .AddAntiforgery();

        builder.Services
            .AddSingleton<WeatherForecastService>();
    }

    private static void ConfigureMiddleware(WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();
        app.UseAntiforgery();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapBlazoneEndpoints();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();
    }
}
