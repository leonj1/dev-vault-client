using System.Text.Json.Serialization;

namespace CrossPlatformApp.CLI.Models;

public class Configuration
{
    [JsonPropertyName("projectId")]
    public string ProjectId { get; set; } = string.Empty;
}
