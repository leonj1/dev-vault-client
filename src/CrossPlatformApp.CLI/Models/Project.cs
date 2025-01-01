using System.Text.Json.Serialization;

namespace CrossPlatformApp.CLI.Models;

public class Project
{
    [JsonPropertyName("identifier")]
    public string Id { get; set; } = string.Empty;
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}
