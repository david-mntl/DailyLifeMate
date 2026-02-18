using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DailyLifeMate.Engine.Features.Series.Dtos;

namespace DailyLifeMate.Engine.Features.Series.Services;

public interface IAnimeService
{
    // Returns the list of Animes
    Task<List<AnimeDto>> GetAllAnimesAsync();

    // Gets a single anime by ID
    Task<AnimeDto> GetAnimeByIdAsync(Guid id);

    // Creates an anime and links it to the "Anime Dashboard" context
    Task<AnimeDto> CreateAnimeAsync(CreateAnimeRequestDto request);

    // Updates Anime
    Task<AnimeDto> UpdateAnimeAsync(Guid id, UpdateAnimeRequestDto request);

    // Deletes the anime
    Task<bool> DeleteAnimeAsync(Guid id);
}
