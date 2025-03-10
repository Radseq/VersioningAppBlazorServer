using VersioningAppBlazorServer.Components;
using VersioningAppBlazorServer.Services.Versioning;
using VersioningAppBlazorServer.Services;
using Radzen;

namespace VersioningAppBlazorServer;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var services = builder.Services;

        var configuration = builder.Configuration;

        // Add services to the container.
        services.AddRazorComponents()
            .AddInteractiveServerComponents();

        services.AddSingleton<IVersioningService, VersioningService>();

        services.AddMemoryCache();
        services.AddSingleton<ICacheService, CacheService>();

        services.AddRadzenComponents();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();
        app.UseAntiforgery();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}
