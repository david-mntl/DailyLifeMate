using System.Collections.Generic;

using DailyLifeMate.Domain.Core.Models;

namespace DailyLifeMate.Engine.Features.Series.Dtos;

public record UpdateAnimeRequestDto
{
    public required string Name { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<ExternalLink>? ExternalLinks { get; set; }
}
