using System.Text.Json.Serialization;

namespace CrossPlatformApp.CLI.Models;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(List<Project>))]
[JsonSerializable(typeof(Configuration))]
[JsonSerializable(typeof(Dictionary<string, string>))]
[JsonSerializable(typeof(List<Secret>))]
public partial class JsonContext : JsonSerializerContext
{
}
