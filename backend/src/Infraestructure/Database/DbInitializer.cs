using System.Linq;
using DailyLifeMate.Domain.Core;

namespace DailyLifeMate.Infrastructure.Database;

public static class DbInitializer
{
    public static void Initialize(DailyLifeMateDbContext context)
    {
        context.Database.EnsureCreated();

        if (context.Contexts.Any()) return;

        // We create the "Empty Folder" for your Anime feature
        var animeDashboard = new Context
        {
            Id = System.Guid.NewGuid(),
            Name = "Anime Dashboard",
            Description = "Personal current anime watch list."
        };

        context.Contexts.Add(animeDashboard);
        context.SaveChanges();
    }
}