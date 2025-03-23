using System.Text.Json.Serialization;

namespace Pixeldrain.Models;

public record ListInfoMetadata
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }
    
    [JsonPropertyName("title")]
    public required string Title { get; init; }
    
    [JsonPropertyName("date_created")]
    public required DateTime DateCreatedUtc { get; init; }
    
    [JsonPropertyName("file_count")]
    public required int FileCount { get; init; }
    
    [JsonPropertyName("can_edit")]
    public required bool CanEdit { get; init; }
    
}

public record ListInfo : ListInfoMetadata
{
    [JsonPropertyName("files")]
    public required List<FileInfo> Files { get; init; }
}