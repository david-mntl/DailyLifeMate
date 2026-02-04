using System;
using System.Collections.Generic;
using DailyLifeMate.Domain.Core.Models;

namespace DailyLifeMate.Engine.Features.Anime.Dtos;

public class AnimeDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // New Fields
    public int TotalEpisodes { get; set; }
    public int CurrentEpisodes { get; set; }
    public string AiringStatus { get; set; } = string.Empty;
    public List<string> Genres { get; set; } = new();

    // The rich link objects
    public List<ExternalLink> ExternalLinks { get; set; } = new();

    public string ContextName { get; set; } = string.Empty;
}