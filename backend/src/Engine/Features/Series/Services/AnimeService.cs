using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DailyLifeMate.Domain.Core.Enums;
using DailyLifeMate.Domain.Core.Models;
using DailyLifeMate.Domain.Interfaces;
using DailyLifeMate.Domain.Persistence;
using DailyLifeMate.Engine.Features.Series.Dtos;
using DailyLifeMate.Engine.Features.Series.Exceptions;
using DailyLifeMate.Engine.Features.Series.Models;
using DailyLifeMate.Infrastructure.Repositories;

using Microsoft.Extensions.Logging;

namespace DailyLifeMate.Engine.Features.Series.Services;

public class AnimeService : IAnimeService
{
    private readonly IAnimeRepository _animeRepository;
    private readonly IRepository<Context> _contextRepository;
    private readonly IAnimeMetadataProvider _metadataProvider;
    private readonly ILogger<AnimeService> _logger;

    public AnimeService(
        IAnimeRepository animeRepository,
        IRepository<Context> contextRepository, // Generic repo for the Context entity
        IAnimeMetadataProvider animeMetadataProvider, // For fetching metadata from external APIs
        ILogger<AnimeService> logger)
    {
        _animeRepository = animeRepository;
        _contextRepository = contextRepository;
        _metadataProvider = animeMetadataProvider;
        _logger = logger;
    }

    public async Task<List<AnimeDto>> GetAllAnimesAsync()
    {
        _logger.LogDebug("Fetching all items from the 'Animes' repository.");

        try
        {
            var animes = await _animeRepository.GetAllAsync();
            _logger.LogInformation("Retrieved {Count} animes from database.", animes.Count());

            return animes.Select(MapToDto).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch animes.");
            throw;
        }
    }

    public async Task<AnimeDto> CreateAnimeAsync(CreateAnimeRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Name cannot be empty.");

        _logger.LogInformation("Request to create anime: {Name}", request.Name);

        try
        {
            var contexts = await _contextRepository.FindAsync(c => c.Name.Contains("Anime"));
            var animeContext = contexts.FirstOrDefault()
                ?? throw new Exception("The 'Anime Dashboard' context was not found");

            var anime = await AnimeCreationHandlerAsync(request, animeContext.Id);

            await _animeRepository.AddAsync(anime);
            await _animeRepository.SaveChangesAsync(); // Commit the transaction

            _logger.LogInformation("Successfully created Anime {AnimeName}", anime.Name);
            return MapToDto(anime, animeContext.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating anime {Name}", request.Name);
            throw;
        }
    }

    public async Task<AnimeDto> UpdateAnimeAsync(Guid id, UpdateAnimeRequestDto request)
    {
        try
        {
            var anime = await GetAnimeOrThrowAsync(id);

            anime.Name = request.Name;
            anime.Description = request.Description ?? string.Empty;

            if (request.ExternalLinks != null)
            {
                anime.ExternalLinks = request.ExternalLinks;
            }
            await _animeRepository.UpdateAsync(anime);
            await _animeRepository.SaveChangesAsync();

            _logger.LogInformation("Updated Anime {Id}. Name: {Name}", id, anime.Name);

            return MapToDto(anime);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating anime {Id}", id);
            throw;
        }
    }

    public async Task<bool> DeleteAnimeAsync(Guid id)
    {
        try
        {
            if (!await _animeRepository.ExistsAsync(id))
            {
                _logger.LogWarning("Delete failed: Anime {Id} not found.", id);
                return false;
            }

            await _animeRepository.DeleteAsync(id);
            await _animeRepository.SaveChangesAsync();

            _logger.LogInformation("Deleted Anime {Id}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting anime {Id}", id);
            throw;
        }
    }

    public async Task<AnimeDto> GetAnimeByIdAsync(Guid id)
    {
        try
        {
            var anime = await GetAnimeOrThrowAsync(id);
            return MapToDto(anime);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving anime {Id}", id);
            throw;
        }
    }

    private async Task<Anime> AnimeCreationHandlerAsync(CreateAnimeRequestDto request, Guid contextId)
    {
        var metadata = await _metadataProvider.GetAnimeInfoAsync(request.Name);

        if (metadata != null)
        {
            _logger.LogInformation("Successfully retrieved Jikan metadata for Anime: {Title}", request.Name);
        }
        else
        {
            _logger.LogWarning("No anime metadata found for {Title}. Falling back to manual creation.", request.Name);
        }

        return new Anime
        {
            Id = Guid.NewGuid(),
            ContextId = contextId,

            // Prioritize API data, fallback to user request
            Name = metadata?.Title ?? request.Name,

            Description = request.Description,

            Synopsis = metadata?.Synopsis ?? string.Empty,
            ImageUrl = metadata?.ImageUrl ?? string.Empty,

            TotalEpisodes = metadata?.TotalEpisodes ?? 0,
            // Right now falling to the same amount of total episodes until we have a better way to track the current available episodes
            CurrentAvailableEpisodes = metadata?.TotalEpisodes ?? 0,

            ReleasedOn = metadata?.ReleasedOn != null ? DateTime.SpecifyKind(metadata.ReleasedOn.Value, DateTimeKind.Utc) : null,
            Genres = metadata?.Genres ?? [],
            AiringStatus = metadata?.Status ?? string.Empty, // E.g., "Finished Airing"

            Status = ActivityStatus.Active, // Active item in the dashboard
            ExternalLinks = [] // Later links will be added to watch the anime.
        };
    }

    /// <summary>
    /// Centralizes the logic for "If not found, log and throw 404".
    /// </summary>
    private async Task<Anime> GetAnimeOrThrowAsync(Guid id)
    {
        var anime = await _animeRepository.GetByIdAsync(id);
        if (anime == null)
        {
            _logger.LogWarning("Anime with ID {Id} was not found.", id);
            throw new AnimeNotFoundException(id);
        }
        return anime;
    }

    /// <summary>
    /// Centralizes the Entity -> DTO mapping.
    /// </summary>
    private static AnimeDto MapToDto(Anime anime)
    {
        // Safe navigation check in case Context wasn't included or it is not relevant for the case. Ex: Testing
        var contextName = anime.Context?.Name ?? "Unknown";
        return MapToDto(anime, contextName);
    }

    private static AnimeDto MapToDto(Anime anime, string contextName)
    {
        return new AnimeDto
        {
            Id = anime.Id,
            Name = anime.Name,
            Description = anime.Description ?? string.Empty,
            ContextName = contextName,
            TotalEpisodes = anime.TotalEpisodes,
            CurrentAvailableEpisodes = anime.CurrentAvailableEpisodes,
            AiringStatus = anime.AiringStatus,
            Genres = anime.Genres,
            ExternalLinks = anime.ExternalLinks,
            ImageUrl = anime.ImageUrl,
            Synopsis = anime.Synopsis,
            ReleasedOn = anime.ReleasedOn,
            NextAirDateUtc = anime.NextAirDateUtc
        };
    }
}