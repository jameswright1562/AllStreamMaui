using System;
using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using MoviesApp.Shared.Models;

namespace MoviesApp.Shared.Services;

public static class ServiceBuilder
{
    public static IServiceCollection AddSharedServices(this IServiceCollection services, Func<IServiceProvider, IFormFactor> formFactorFactory, Settings settings)
    {
        services.AddSingleton<IFormFactor>(formFactorFactory);
        services.AddHttpClient<IMovieService, ImdbApiDevMovieService>(c =>
        {
            c.BaseAddress = new Uri("https://api.themoviedb.org/3/");
            if (!string.IsNullOrWhiteSpace(settings.TmdbApiKey))
            {
                c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", settings.TmdbApiKey);
            }
            c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        });
        services.AddHttpClient<IEpicSportsService, EpicSportsService>(c =>
        {
            c.BaseAddress = new Uri("https://epicsports.djsofficial.com/");
            c.Timeout = TimeSpan.FromSeconds(15);
            c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
            c.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0 Safari/537.36");
            c.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", "en-US,en;q=0.9");
            c.DefaultRequestHeaders.TryAddWithoutValidation("Connection", "keep-alive");
            c.DefaultRequestHeaders.TryAddWithoutValidation("Referer", "https://epicsports.djsofficial.com/");
        })
        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate,
            AllowAutoRedirect = true,
        });
        return services;
    }

    public static IServiceCollection AddSharedServices(this IServiceCollection services, Func<IServiceProvider, IFormFactor> formFactorFactory, string? tmdbApiKey)
    {
        services.AddSingleton<IFormFactor>(formFactorFactory);
        services.AddHttpClient<IMovieService, ImdbApiDevMovieService>(c =>
        {
            c.BaseAddress = new Uri("https://api.themoviedb.org/3/");
            if (!string.IsNullOrWhiteSpace(tmdbApiKey))
            {
                c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tmdbApiKey);
            }
            c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        });
        services.AddHttpClient<IEpicSportsService, EpicSportsService>(c =>
        {
            c.BaseAddress = new Uri("https://epicsports.djsofficial.com/");
            c.Timeout = TimeSpan.FromSeconds(15);
            c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
            c.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0 Safari/537.36");
            c.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", "en-US,en;q=0.9");
            c.DefaultRequestHeaders.TryAddWithoutValidation("Connection", "keep-alive");
            c.DefaultRequestHeaders.TryAddWithoutValidation("Referer", "https://epicsports.djsofficial.com/");
        })
        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate,
            AllowAutoRedirect = true,
        });
        return services;
    }
}
