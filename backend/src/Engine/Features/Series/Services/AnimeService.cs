using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DailyLifeMate.Domain.Core.Enums;
using DailyLifeMate.Domain.Core.Models;
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
    private readonly IRepository<Context> _contextRepository; // Need this to find the "Anime" folder
    private readonly ILogger<AnimeService> _logger;

    public AnimeService(
        IAnimeRepository animeRepository,
        IRepository<Context> contextRepository, // Generic repo for the Context entity
        ILogger<AnimeService> logger)
    {
        _animeRepository = animeRepository;
        _contextRepository = contextRepository;
        _logger = logger;
    }

    public async Task<List<AnimeDto>> GetAllAnimesAsync()
    {
        _logger.LogDebug("Fetching all items from the 'Animes' repository.");

        try
        {
            // Abstraction 1: The Repository handles the DB logic (Includes, AsNoTracking, etc.)
            var animes = await _animeRepository.GetAllAsync();

            _logger.LogInformation("Retrieved {Count} animes from database.", animes.Count());

            // Abstraction 2: Centralized Mapping
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
            // Business Logic: Find the correct "Context" (Folder) for this item
            // Using the Generic Repository's predicate FindAsync we implemented earlier
            var contexts = await _contextRepository.FindAsync(c => c.Name.Contains("Anime"));
            var animeContext = contexts.FirstOrDefault()
                ?? throw new Exception("The 'Anime Dashboard' context was not found. Did you run DbInitializer?");

            var anime = new Anime
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                ContextId = animeContext.Id,
                Description = request.Description ?? string.Empty,
                TotalEpisodes = request.TotalEpisodes,
                CurrentEpisodes = request.TotalEpisodes, // Default behavior
                Status = ActivityStatus.Active,
                ExternalLinks = []
            };

            await _animeRepository.AddAsync(anime);
            await _animeRepository.SaveChangesAsync(); // Commit the transaction

            _logger.LogInformation("Successfully created Anime {AnimeName}", anime.Name);

            // Return mapped object (Re-using the mapper)
            // Note: We pass the context manually here because the 'anime' object 
            // might not have the .Context navigation property loaded yet after a fresh add.
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
            // Abstraction 3: GetOrThrow helper to avoid repeating "if null throw"
            var anime = await GetAnimeOrThrowAsync(id);

            // Update Fields
            anime.Name = request.Name;
            anime.Description = request.Description ?? string.Empty;

            if (request.ExternalLinks != null)
            {
                anime.ExternalLinks = request.ExternalLinks;
            }

            // We explicitly call Update, then Save
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
            // Repositories usually handle "Delete by ID" efficiently
            // Check if exists first to return false if needed
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
    /// Change logic here, and it updates EVERY endpoint.
    /// </summary>
    private static AnimeDto MapToDto(Anime anime)
    {
        // Safe navigation check in case Context wasn't included
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
            CurrentEpisodes = anime.CurrentEpisodes,
            AiringStatus = anime.AiringStatus, // Ensure your Entity has this property
            Genres = anime.Genres,
            ExternalLinks = anime.ExternalLinks
        };
    }
}