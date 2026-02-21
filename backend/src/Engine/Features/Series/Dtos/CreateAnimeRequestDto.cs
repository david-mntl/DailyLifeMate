using System.Collections.Generic;

namespace DailyLifeMate.Engine.Features.Series.Dtos;

public record CreateAnimeRequestDto
{
    public required string Name { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<string>? ExternalLinks { get; set; }
}
