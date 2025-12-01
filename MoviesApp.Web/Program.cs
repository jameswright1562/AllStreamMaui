using MoviesApp.Web.Components;
using MoviesApp.Shared.Services;
using MoviesApp.Web.Services;
using MoviesApp.Shared.Models;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
var settings = builder.Configuration.Get<Settings>() ?? new Settings();
builder.Services.AddSingleton(settings);
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add device-specific services used by the MoviesApp.Shared project
MoviesApp.Shared.Services.ServiceBuilder.AddSharedServices(builder.Services, sp => new FormFactor(), settings.TmdbApiKey);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();



app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(
        typeof(MoviesApp.Shared._Imports).Assembly);

app.Run();
