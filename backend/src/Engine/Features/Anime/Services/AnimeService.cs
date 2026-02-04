using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DailyLifeMate.Domain.Core.Enums;
using DailyLifeMate.Domain.Core.Models;
using DailyLifeMate.Domain.Exceptions;
using DailyLifeMate.Domain.Features.Services.Anime;
using DailyLifeMate.Engine.Features.Anime.Dtos;
using DailyLifeMate.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DailyLifeMate.Domain.Features.Anime;

public class AnimeService : IAnimeService
{
    private readonly DailyLifeMateDbContext _db;
    private readonly ILogger<AnimeService> _logger;

    public AnimeService(DailyLifeMateDbContext db, ILogger<AnimeService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<List<AnimeDto>> GetAllAnimesAsync()
    {
        _logger.LogDebug("Fetching all items from the 'Animes' table.");

        try
        {
            var animes = await _db.Animes
                .Include(a => a.Context)
                .AsNoTracking()
                .ToListAsync();

            _logger.LogInformation("Retrieved {Count} animes from database.", animes.Count);

            return animes.Select(a => new AnimeDto
            {
                Id = a.Id,
                Title = a.Title,
                // Inherited properties
                ContextName = a.Context.Name,
                // Specialized properties
                TotalEpisodes = a.TotalEpisodes,
                CurrentEpisodes = a.CurrentEpisodes,
                AiringStatus = a.AiringStatus,
                Genres = a.Genres,           // JSONB handled automatically!
                ExternalLinks = a.ExternalLinks // JSONB handled automatically!
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch animes.");
            throw;
        }
    }

    public async Task<AnimeDto> CreateAnimeAsync(CreateAnimeRequest request)
    {
        _logger.LogInformation("Request to create anime: {Title}", request.Title);

        if (string.IsNullOrWhiteSpace(request.Title))
            throw new ArgumentException("Title cannot be empty.");

        try
        {
            var animeContext = await _db.Contexts
                .FirstOrDefaultAsync(c => c.Name.Contains("Anime"));

            if (animeContext == null)
            {
                throw new Exception("The 'Anime Dashboard' context was not found. Did you run DbInitializer?");
            }

            var anime = new Anime
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                ContextId = animeContext.Id, // Linking to the folder
                Description = request.Description,

                // Defaults
                TotalEpisodes = request.TotalEpisodes,
                CurrentEpisodes = 0,
                Status = ActivityStatus.Active,

                // If the user sent links, use them; otherwise empty list
                ExternalLinks = request.ExternalLinks ?? new List<ExternalLink>()
            };

            _db.Animes.Add(anime);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Successfully created Anime {Id} in Context {ContextName}", anime.Id, animeContext.Name);

            return new AnimeDto
            {
                Id = anime.Id,
                Title = anime.Title,
                ContextName = animeContext.Name,
                ExternalLinks = anime.ExternalLinks
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating anime {Title}", request.Title);
            throw;
        }
    }

    public async Task<AnimeDto> UpdateAnimeAsync(Guid id, UpdateAnimeRequest request)
    {
        try
        {
            var anime = await _db.Animes.FirstOrDefaultAsync(a => a.Id == id);

            if (anime == null)
            {
                _logger.LogWarning("Update failed: Anime {Id} with name {Name} not found.", id, request.Title);
                throw new AnimeNotFoundException(id);
            }

            // Update Fields
            anime.Title = request.Title;
            anime.CurrentEpisodes = request.CurrentEpisodes;
            anime.AiringStatus = request.AiringStatus;

            if (request.ExternalLinks != null)
            {
                anime.ExternalLinks = request.ExternalLinks;
            }

            await _db.SaveChangesAsync();
            _logger.LogInformation("Updated Anime {Id}. Name: {Name}", id, anime.Title);

            return new AnimeDto
            {
                Id = anime.Id,
                Title = anime.Title,
                CurrentEpisodes = anime.CurrentEpisodes,
                ExternalLinks = anime.ExternalLinks
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating anime {Id}Name: {Name}", id, request.Title);
            throw;
        }
    }

    public async Task<bool> DeleteAnimeAsync(Guid id)
    {
        try
        {
            var anime = await _db.Animes.FindAsync(id);
            if (anime == null)
            {
                return false;
            }

            _db.Animes.Remove(anime);
            await _db.SaveChangesAsync();

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
            _logger.LogDebug("Retrieving Anime with id {Id}", id);
            var anime = await _db.Animes
                .Include(a => a.Context)
                .AsNoTracking()          // Optimization: fast read, no tracking overhead
                .FirstOrDefaultAsync(a => a.Id == id);

            if (anime == null)
            {
                _logger.LogWarning("Anime with ID {Id} was not found.", id);
                throw new AnimeNotFoundException(id);
            }

            return new AnimeDto
            {
                Id = anime.Id,
                Title = anime.Title,
                Description = anime.Description,
                ContextName = anime.Context.Name,

                // Specialized Anime fields
                TotalEpisodes = anime.TotalEpisodes,
                CurrentEpisodes = anime.CurrentEpisodes,
                AiringStatus = anime.AiringStatus,

                Genres = anime.Genres,
                ExternalLinks = anime.ExternalLinks
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving anime {Id}", id);
            throw;
        }
    }
}