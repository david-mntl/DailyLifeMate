using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

using DailyLifeMate.Domain.Interfaces;
using DailyLifeMate.Engine.Features.Series.Models;

using Microsoft.Extensions.Logging;

namespace DailyLifeMate.Infrastructure.ExternalApis.Jikan;

public class JikanAnimeMetadataProvider : IAnimeMetadataProvider
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<JikanAnimeMetadataProvider> _logger;

    public JikanAnimeMetadataProvider(HttpClient httpClient, ILogger<JikanAnimeMetadataProvider> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<AnimeMetadata?> GetAnimeInfoAsync(string name)
    {
        _logger.LogInformation("Searching Jikan API for anime: {Name}", name);

        try
        {
            // Jikan V4 Search Endpoint. Limit to 1 to get the most relevant result.
            var url = $"anime?q={System.Uri.EscapeDataString(name)}&limit=1";
            var response = await _httpClient.GetFromJsonAsync<JikanSearchResponse>(url);

            _logger.LogDebug("Response from Jikan API for {Name}: {Response}", name, response?.Data);
            var topResult = response?.Data.FirstOrDefault();

            if (topResult == null)
            {
                _logger.LogWarning("No results found in Jikan API for: {Name}", name);
                return null;
            }

            return new AnimeMetadata
            {
                Title = topResult.Title,
                Synopsis = topResult.Synopsis ?? string.Empty,
                TotalEpisodes = topResult.Episodes,
                Status = topResult.Status,
                ReleasedOn = topResult.Aired?.From,
                ImageUrl = topResult.Images?.Jpg?.LargeImageUrl,
                Genres = topResult.Genres.Select(g => g.Name).ToList()
            };
        }
        catch (HttpRequestException ex)
        {
            // Network failures, 502 Bad Gateway, 429 Too Many Requests, etc.
            _logger.LogError(ex, "HTTP request failed while calling Jikan API for {Name}.", name);
            return null; // If no success - Don't crash
        }
        catch (System.Exception ex)
        {
            // JSON parsing errors or other unexpected exceptions
            _logger.LogError(ex, "Unexpected error parsing Jikan API response for {Name}.", name);
            return null; // If no success - Don't crash
        }
    }
}
