using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DailyLifeMate.Infrastructure.ExternalApis.Jikan;

// REMARKS:
// - Only map the fields that are actually interesting for the case. Jikan returns a ton of data.
// - Marked internal even if it is a monolith project: To explicit express "Do not use this outside of Infrastructure!"
internal class JikanSearchResponse
{
    [JsonPropertyName("data")]
    public List<JikanAnimeData> Data { get; set; } = [];
}

internal class JikanAnimeData
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("synopsis")]
    public string? Synopsis { get; set; }

    [JsonPropertyName("episodes")]
    public int? Episodes { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("aired")]
    public JikanAired? Aired { get; set; }

    [JsonPropertyName("images")]
    public JikanImages? Images { get; set; }

    [JsonPropertyName("genres")]
    public List<JikanGenre> Genres { get; set; } = [];
}

internal class JikanAired
{
    [JsonPropertyName("from")]
    public System.DateTime? From { get; set; }
}

internal class JikanImages
{
    [JsonPropertyName("jpg")]
    public JikanJpg? Jpg { get; set; }
}

internal class JikanJpg
{
    [JsonPropertyName("large_image_url")]
    public string? LargeImageUrl { get; set; }
}

internal class JikanGenre
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}
