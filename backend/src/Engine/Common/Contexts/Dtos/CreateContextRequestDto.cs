

namespace DailyLifeMate.Engine.Common.Contexts.Dtos;

public record CreateContextRequestDto
{
    public required string Name { get; set; }
    public string Description { get; set; } = string.Empty;
}