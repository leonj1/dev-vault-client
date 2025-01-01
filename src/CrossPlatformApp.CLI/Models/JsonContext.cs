using System.Text.Json.Serialization;

namespace CrossPlatformApp.CLI.Models;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(List<Project>))]
public partial class JsonContext : JsonSerializerContext
{
}
