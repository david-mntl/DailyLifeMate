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
        try
        {
            _logger.LogDebug("Fetching all animes from the database context.");
            return await _db.Animes
                .AsNoTracking() // Optimization: doesn't track changes for read-only
                .Select(a => new AnimeDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Description = a.Description
                })
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching all animes.");
            throw new Exception("A problem was found wwhile fetching all animes");
        }
    }

    public async Task<AnimeDto> CreateAnimeAsync(CreateAnimeRequest request)
    {
        _logger.LogDebug("Creating Anime");
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ArgumentException("Anime title cannot be empty.");
        }

        try
        {
            var anime = new Anime
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description
            };

            _db.Animes.Add(anime);
            await _db.SaveChangesAsync();

            return new AnimeDto
            {
                Id = anime.Id,
                Title = anime.Title,
                Description = anime.Description
            };
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database update error while creating anime: {Title}", request.Title);
            throw new Exception("Could not save the anime. It might already exist or the data is invalid.");
        }
    }

    public async Task<AnimeDto> GetAnimeByIdAsync(Guid id)
    {
        try 
        {
            _logger.LogDebug("Retrieving Anime with id {id}", id);
            var anime = await _db.Animes.FindAsync(id);
            if (anime == null)
            {
                throw new AnimeNotFoundException(id);
            }

            return new AnimeDto 
            { 
                Id = anime.Id, 
                Title = anime.Title 
            };
        }
        catch (AnimeNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            var errorMsg = $"An unexpected error occurred while fetching the anime with ID: {id}.";
            _logger.LogError(ex, errorMsg);
            throw new Exception(errorMsg);
        }
    }

    public async Task<AnimeDto> UpdateAnimeAsync(Guid id, UpdateAnimeRequest request)
    {
        try
        {
            _logger.LogDebug("Updating Anime with id {id}", id);
            var anime = await _db.Animes.FindAsync(id);

            if (anime == null)
            {
                throw new KeyNotFoundException($"Anime with ID {id} not found.");
            }

            // Mapping fields from the Update request
            anime.Title = request.Title;
            anime.Description = request.Description;
            anime.Rating = request.Rating; // Now we can update the rating!

            await _db.SaveChangesAsync();

            return new AnimeDto
            {
                Id = anime.Id,
                Title = anime.Title,
                Description = anime.Description,
                Rating = anime.Rating
            };
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating anime {Id}", id);
            throw new Exception("The engine encountered an error while updating the record.");
        }
    }

    public async Task<bool> DeleteAnimeAsync(Guid id)
    {
        try
        {
            _logger.LogDebug("Deleting Anime with id {id}", id);
            var anime = await _db.Animes.FindAsync(id);

            if (anime == null)
            {
                return false;
            }

            _db.Animes.Remove(anime);
            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting anime {Id}", id);
            throw new Exception("Could not delete the record. It may be linked to other data.");
        }
    }
}




// // Inside your AnimeService
// public void UpdateAirDate(Item animeItem, DateTime externalDate)
// {
//     // 1. Ensure the date is treated as UTC
//     // If externalDate comes from a JSON API with a TZ offset, 
//     // .ToUniversalTime() handles the math for you.
//     DateTime utcDate = externalDate.ToUniversalTime();

//     // 2. Find or create the metadata record
//     var metadata = animeItem.Metadata
//         .FirstOrDefault(m => m.Key == AnimeMetadataKeys.NextAirDate.ToString());

//     if (metadata == null)
//     {
//         metadata = new ItemMetadata { 
//             Id = Guid.NewGuid(), 
//             Key = AnimeMetadataKeys.NextAirDate.ToString() 
//         };
//         animeItem.Metadata.Add(metadata);
//     }

//     // 3. Save as "o" (ISO 8601) - The Roundtrip Format
//     // This results in: "2026-01-27T20:00:00.0000000Z"
//     metadata.Value = utcDate.ToString("o"); 
// }