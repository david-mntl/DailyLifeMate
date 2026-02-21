using System;
using System.Collections.Generic;

using DailyLifeMate.Domain.Core.Models;

namespace DailyLifeMate.Engine.Features.Series.Dtos;

public record AnimeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string Synopsis { get; set; } = string.Empty;
    public DateTime? ReleasedOn { get; set; }
    public DateTime? NextAirDateUtc { get; set; }
    public int? TotalEpisodes { get; set; }
    public int? CurrentAvailableEpisodes { get; set; } // Not going to be implemented yet.
    public string AiringStatus { get; set; } = string.Empty;
    public List<string> Genres { get; set; } = new();
    public List<ExternalLink> ExternalLinks { get; set; } = new();
    public string ContextName { get; set; } = string.Empty;
}
