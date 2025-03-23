using System.Text.Json.Serialization;

namespace Pixeldrain.Models;

public record UserInfo
{
    [JsonPropertyName("username")] public required string Username { get; init; }

    [JsonPropertyName("email")] public string? Email { get; init; }

    [JsonPropertyName("subscription")] public required SubscriptionInfo Subscription { get; init; }

    [JsonPropertyName("storage_space_used")]
    public long StorageSpaceUsed { get; init; }

    [JsonPropertyName("filesystem_storage_used")]
    public long FileSystemStorageUsed { get; init; }

    [JsonPropertyName("balance_micro_eur")]
    public int BalanceMicroEur { get; init; }

    [JsonPropertyName("is_admin")] public bool IsAdmin { get; init; }

    [JsonPropertyName("hotlinking_enabled")]
    public bool HotlinkingEnabled { get; init; }

    [JsonPropertyName("monthly_transfer_cap")]
    public long MonthlyTransferCap { get; init; }

    [JsonPropertyName("monthly_transfer_used")]
    public long MonthlyTransferUsed { get; init; }

    [JsonPropertyName("file_viewer_branding")]
    public Dictionary<string, string>? FileViewerBranding { get; init; }

    [JsonPropertyName("file_embed_domains")]
    public string FileEmbedDomains { get; init; } = string.Empty;

    [JsonPropertyName("skip_file_viewer")] public bool SkipFileViewer { get; init; }

    [JsonPropertyName("affiliate_user_name")]
    public string AffiliateUserName { get; init; } = string.Empty;
}