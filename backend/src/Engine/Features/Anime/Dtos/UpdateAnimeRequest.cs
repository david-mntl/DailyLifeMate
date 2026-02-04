using System.Collections.Generic;
using DailyLifeMate.Domain.Core.Models;

namespace DailyLifeMate.Engine.Features.Anime.Dtos;

public class UpdateAnimeRequest
{
    public required string Title { get; set; }
    public int CurrentEpisodes { get; set; }
    public string AiringStatus { get; set; } = "Planned";
    public List<ExternalLink>? ExternalLinks { get; set; }
}