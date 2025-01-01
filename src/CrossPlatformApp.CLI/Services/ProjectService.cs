using System.Text.Json;
using CrossPlatformApp.CLI.Models;

namespace CrossPlatformApp.CLI.Services;

public class ProjectService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "http://10.1.1.144:7601";

    public ProjectService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(BaseUrl)
        };
    }

    public async Task<List<Project>> GetProjectsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/projects");
            if (!response.IsSuccessStatusCode)
            {
                Console.Error.WriteLine($"HTTP Error: {response.StatusCode} - {response.ReasonPhrase}");
                return new List<Project>();
            }
            
            var content = await response.Content.ReadAsStringAsync();
            // Fix malformed JSON by adding missing commas between properties
            content = content.Replace("\"\"", "\",\"");
            
            
            using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));
            var projects = await JsonSerializer.DeserializeAsync(
                stream,
                JsonContext.Default.ListProject
            );
            
            if (projects == null || !projects.Any())
            {
                Console.WriteLine("No projects available");
                return new List<Project>();
            }
            
            return projects;
        }
        catch (HttpRequestException ex)
        {
            Console.Error.WriteLine($"HTTP Request Error: {ex.Message}");
            return new List<Project>();
        }
        catch (JsonException ex)
        {
            Console.Error.WriteLine($"JSON Parsing Error: {ex.Message}");
            return new List<Project>();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Unexpected Error: {ex.Message}");
            return new List<Project>();
        }
    }
}
