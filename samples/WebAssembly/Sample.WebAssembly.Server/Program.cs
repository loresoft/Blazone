using System.Security.Claims;

using Blazone.Authentication;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Identity.Web;

using Sample.WebAssembly.Server.Services;

namespace Sample.WebAssembly.Server;

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
            .AddAuthorization()
            .AddBlazoneServer()
            .AddAntiforgery();

        builder.Services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen();

        builder.Services
            .AddProblemDetails();

        builder.Services
            .TryAddSingleton<WeatherForecastService>();

        builder.Services.AddTransient<IClaimsTransformation, ClaimsTransformation>();

        builder.Services.Configure<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme, options =>
        {
            options.Cookie.Name = ".Sample.Authentication";
        });
    }

    private static void ConfigureMiddleware(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
            app.UseWebAssemblyDebugging();
        else
            app.UseHsts();

        app.UseExceptionHandler();
        app.UseStatusCodePages();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseBlazorFrameworkFiles();
        app.UseHttpsRedirection();

        app.UseStaticFiles();
        app.UseAntiforgery();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapBlazoneEndpoints();

        app.MapGet("/api/weather", (WeatherForecastService weatherService, ClaimsPrincipal principal) => weatherService.Get(principal))
            .WithName("GetWeatherForecast")
            .WithOpenApi()
            .RequireAuthorization();

        app.MapFallbackToFile("index.html");
    }
}
