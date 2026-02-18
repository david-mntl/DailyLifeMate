

using DailyLifeMate.Domain.Persistence;

namespace DailyLifeMate.Infrastructure.Repositories;

public interface IAnimeRepository : IRepository<Engine.Features.Series.Models.Anime>
{
}