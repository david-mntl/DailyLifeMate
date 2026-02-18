using System;
using System.Collections.Generic;

using DailyLifeMate.Domain.Core.Models;

namespace DailyLifeMate.Engine.Features.Series.Dtos;

public record AnimeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public uint TotalEpisodes { get; set; }
    public uint CurrentEpisodes { get; set; }
    public string AiringStatus { get; set; } = string.Empty;
    public List<string> Genres { get; set; } = new();
    public List<ExternalLink> ExternalLinks { get; set; } = new();
    public string ContextName { get; set; } = string.Empty;
}
