using System.Text.Json;
using System.Text.Json.Serialization;

namespace Entities.ErrorModels;

public class ErrorDetails
{
    [JsonPropertyName("statusCode")]
    public int StatusCode { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = "";

    public override string ToString() => JsonSerializer.Serialize(this);
}
