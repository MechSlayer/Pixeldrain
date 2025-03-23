using System.Text.Json.Serialization;

namespace Pixeldrain.Models;

public record FileInfo
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }
    
    [JsonPropertyName("name")]
    public required string Name { get; init; }
    
    [JsonPropertyName("size")]
    public long Size { get; init; }
    
    [JsonPropertyName("views")]
    public int Views { get; init; }
    
    [JsonPropertyName("bandwidth_used")]
    public long BandwidthUsed { get; init; }
    
    [JsonPropertyName("bandwidth_used_paid")]
    public  long BandwidthUsedPaid { get; init; }
    
    [JsonPropertyName("downloads")]
    public int Downloads { get; init; }
    
    [JsonPropertyName("date_upload")]
    public DateTime DateUploadUtc { get; init; }
    
    [JsonPropertyName("date_last_view")]
    public DateTime DateLastViewUtc { get; init; }
    
    [JsonPropertyName("mime_type")]
    public string? MimeType { get; init; }
    
    [JsonPropertyName("thumbnail_href")]
    public string? ThumbnailHref { get; init; }
    
    [JsonPropertyName("hash_sha256")]
    public required byte[] HashSha256 { get; init; }
    
    [JsonPropertyName("can_edit")]
    public bool CanEdit { get; init; }
}