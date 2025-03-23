using System.Text.Json.Serialization;

namespace Pixeldrain.Models;

public record CreateListRequest(
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("anonymous"), JsonIgnore]
    bool Anonymous,
    [property: JsonPropertyName("files")] IEnumerable<string> Files
);