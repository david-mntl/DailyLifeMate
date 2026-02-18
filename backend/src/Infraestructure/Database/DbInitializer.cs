using System.Linq;

using DailyLifeMate.Domain.Core.Models;

namespace DailyLifeMate.Infrastructure.Database;

public static class DbInitializer
{
    public const string SEED_CONTEXT_NAME = "Anime Dashboard";
    public static void Initialize(DailyLifeMateDbContext context)
    {
        context.Database.EnsureCreated();

        if (context.Contexts.Any()) return;

        // We create the "Empty Folder" for your Anime feature
        var animeDashboard = new Context
        {
            Id = System.Guid.NewGuid(),
            Name = SEED_CONTEXT_NAME,
            Description = "Personal current anime watch list."
        };

        context.Contexts.Add(animeDashboard);
        context.SaveChanges();
    }
}
