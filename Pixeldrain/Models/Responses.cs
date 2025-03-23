using System.Text.Json.Serialization;

namespace Pixeldrain.Models;


public record ItemCreationResponse([property: JsonPropertyName("id")] string Id);

public record GetUserFilesResponse(
    [property: JsonPropertyName("files")] List<FileInfo> Files
);

public record GetUserListsResponse(
    [property: JsonPropertyName("lists")] List<ListInfoMetadata> Lists
);

public record ErrorResponse(
    [property: JsonPropertyName("value")] string Value,
    [property: JsonPropertyName("message")]
    string? Message
);


public record LoginResponse
{
    [JsonPropertyName("auth_key")]
    public required string AuthKey { get; init; }

    [JsonPropertyName("creation_ip_address")]
    public required string CreationIpAddress { get; init; }

    [JsonPropertyName("user_agent")]
    public string CreationUserAgent { get; init; } = string.Empty;
    
    [JsonPropertyName("creation_time")]
    public required DateTime CreationTime { get; init; }
    
    [JsonPropertyName("last_used_time")]
    public required DateTime LastUsedTime { get; init; }
}