using System.Threading.Tasks;

using DailyLifeMate.Engine.Features.Series.Models;

namespace DailyLifeMate.Domain.Interfaces;

public interface IAnimeMetadataProvider
{
    /// <summary>
    /// Searches for an anime by name and returns the closest match's metadata.
    /// Returns null if no match is found.
    /// </summary>
    Task<AnimeMetadata?> GetAnimeInfoAsync(string name);
}
