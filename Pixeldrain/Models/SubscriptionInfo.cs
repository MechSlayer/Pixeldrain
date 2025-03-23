using System.Text.Json.Serialization;

namespace Pixeldrain.Models;

public record SubscriptionInfo
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }
    
    [JsonPropertyName("name")]
    public required string Name { get; init; }
    
    [JsonPropertyName("type")]
    public required string Type { get; init; }
    
    [JsonPropertyName("file_size_limit")]
    public long FileSizeLimit { get; init; }
    
    [JsonPropertyName("file_expiry_days")]
    public int FileExpiryDays { get; init; }
    
    [JsonPropertyName("storage_space")]
    public long StorageSpace { get; init; }
    
    [JsonPropertyName("price_per_tb_storage")]
    public int PricePerTbStorage { get; init; }
    
    [JsonPropertyName("price_per_tb_bandwidth")]
    public int PricePerTbBandwidth { get; init; }
    
    [JsonPropertyName("monthly_transfer_cap")]
    public long MonthlyTransferCap { get; init; }
    
    [JsonPropertyName("file_viewer_branding")]
    public bool FileViewerBranding { get; init; }
    
    [JsonPropertyName("filesystem_access")]
    public bool FileSystemAccess { get; init; }
    
    [JsonPropertyName("filesystem_storage_limit")]
    public long FileSystemStorageLimit { get; init; }
}