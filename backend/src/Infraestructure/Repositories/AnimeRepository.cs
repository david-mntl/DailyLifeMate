using System;
using System.Threading.Tasks;

using DailyLifeMate.Infrastructure.Database;

using Microsoft.EntityFrameworkCore;

namespace DailyLifeMate.Infrastructure.Repositories;

public class AnimeRepository : Repository<Engine.Features.Series.Models.Anime>, IAnimeRepository
{
    public AnimeRepository(DailyLifeMateDbContext context) : base(context)
    {
    }

    public override async Task<Engine.Features.Series.Models.Anime?> GetByIdAsync(Guid id)
    {
        return await _context.Animes
            .Include(a => a.Context)
            .FirstOrDefaultAsync(a => a.Id == id);
    }


}