using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DailyLifeMate.Engine.Features.Anime.Dtos;

namespace DailyLifeMate.Domain.Features.Services.Anime;

public interface IAnimeService
{
    // Returns the list of Animes
    Task<List<AnimeDto>> GetAllAnimesAsync();

    // Gets a single anime by ID
    Task<AnimeDto> GetAnimeByIdAsync(Guid id);

    // Creates an anime and links it to the "Anime Dashboard" context
    Task<AnimeDto> CreateAnimeAsync(CreateAnimeRequest request);

    // Updates Anime
    Task<AnimeDto> UpdateAnimeAsync(Guid id, UpdateAnimeRequest request);

    // Deletes the anime
    Task<bool> DeleteAnimeAsync(Guid id);
}