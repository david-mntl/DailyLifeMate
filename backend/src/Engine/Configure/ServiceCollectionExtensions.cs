using DailyLifeMate.Domain.Core.Models;
using DailyLifeMate.Domain.Persistence;
using DailyLifeMate.Engine.Features.Series.Services;
using DailyLifeMate.Infrastructure.Repositories;

using Microsoft.Extensions.DependencyInjection;

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
}
