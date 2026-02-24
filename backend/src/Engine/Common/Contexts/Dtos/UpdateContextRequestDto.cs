public record UpdateContextRequestDto
{
    public required string Name { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsArchived { get; set; }
}