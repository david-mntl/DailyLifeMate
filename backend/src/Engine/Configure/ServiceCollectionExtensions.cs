using System;

using DailyLifeMate.Domain.Core.Models;
using DailyLifeMate.Domain.Interfaces;
using DailyLifeMate.Domain.Persistence;
using DailyLifeMate.Engine.Features.Series.Services;
using DailyLifeMate.Infrastructure.ExternalApis.Jikan;
using DailyLifeMate.Infrastructure.ExternalApis.Jikan.Models;
using DailyLifeMate.Infrastructure.Repositories;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DailyLifeMate.Engine.Configure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEngineServices(this IServiceCollection services)
    {
        services.AddScoped<IAnimeService, AnimeService>();
        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IRepository<Context>, Repository<Context>>();
        services.AddScoped<IAnimeRepository, AnimeRepository>();
        return services;
    }

    public static IServiceCollection AddProviders(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure options
        services.Configure<JikanApiSettings>(configuration.GetSection(JikanApiSettings.SectionName));

        // Register HTTP Client for Jikan
        services.AddHttpClient<IAnimeMetadataProvider, JikanAnimeMetadataProvider>((serviceProvider, client) =>
        {
            var settings = serviceProvider.GetRequiredService<IOptions<JikanApiSettings>>().Value;
            if (string.IsNullOrWhiteSpace(settings.BaseUrl))
            {
                throw new ArgumentException("Jikan BaseUrl is missing from appsettings.json");
            }

            client.BaseAddress = new Uri(settings.BaseUrl);

            // Jikan requires a user-agent, it's polite and prevents getting blocked
            client.DefaultRequestHeaders.Add("User-Agent", settings.UserAgent);
        });

        return services;
    }
}
