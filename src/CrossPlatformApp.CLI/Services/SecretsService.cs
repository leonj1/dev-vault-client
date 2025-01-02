using System.Text.Json;
using CrossPlatformApp.CLI.Models;

namespace CrossPlatformApp.CLI.Services;

public class SecretsService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "http://10.1.1.144:7601";

    public SecretsService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(BaseUrl)
        };
    }

    public async Task<List<Secret>> GetSecretsAsync(string projectId, bool verbose)
    {
        try
        {
            var url = $"/projects/{projectId}/secrets";
            if (verbose)
            {
                Console.WriteLine($"HTTP GET {BaseUrl}{url}");
            }

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                if (verbose)
                {
                    Console.Error.WriteLine($"HTTP Error: {response.StatusCode} - {response.ReasonPhrase}");
                }
                return new List<Secret>();
            }

            var content = await response.Content.ReadAsStringAsync();
            
            // Fix malformed JSON by adding missing commas
            content = content.Replace("\"\"", "\",\"")
                           .Replace("}{", "},{");
            
            if (verbose)
            {
                Console.WriteLine($"Response (after fixing): {content}");
            }

            using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));
            var secrets = await JsonSerializer.DeserializeAsync(
                stream,
                JsonContext.Default.ListSecret
            );

            return secrets ?? new List<Secret>();
        }
        catch (HttpRequestException ex)
        {
            if (verbose)
            {
                Console.Error.WriteLine($"HTTP Request Error: {ex.Message}");
            }
                return new List<Secret>();
        }
        catch (JsonException ex)
        {
            if (verbose)
            {
                Console.Error.WriteLine($"JSON Parsing Error: {ex.Message}");
            }
                return new List<Secret>();
        }
        catch (Exception ex)
        {
            if (verbose)
            {
                Console.Error.WriteLine($"Unexpected Error: {ex.Message}");
            }
                return new List<Secret>();
        }
    }
}
