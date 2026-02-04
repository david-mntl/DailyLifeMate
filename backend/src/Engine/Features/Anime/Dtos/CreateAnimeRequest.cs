using System.Collections.Generic;
using DailyLifeMate.Domain.Core.Models;

namespace DailyLifeMate.Engine.Features.Anime.Dtos;

public class CreateAnimeRequest
{
    public required string Title { get; set; }
    public string Description { get; set; } = string.Empty;
    public int TotalEpisodes { get; set; }

    // Optional: User can send links immediately
    public List<ExternalLink>? ExternalLinks { get; set; }
}