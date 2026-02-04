
using DailyLifeMate.Domain.Features.Anime;
using DailyLifeMate.Domain.Features.Services.Anime;
using Microsoft.Extensions.DependencyInjection;

namespace DailyLifeMate.Engine;

public static class ConfigureServices
{
    public static IServiceCollection AddEngineServices(this IServiceCollection services)
    {
        // Adding services Services
        services.AddScoped<IAnimeService, AnimeService>();
        return services;
    }
}