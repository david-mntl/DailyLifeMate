using DailyLifeMate.Engine.Services;

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