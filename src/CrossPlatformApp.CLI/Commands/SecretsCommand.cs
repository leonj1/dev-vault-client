using System.CommandLine;
using CrossPlatformApp.CLI.Services;

namespace CrossPlatformApp.CLI.Commands;

public class SecretsCommand : Command
{
    private readonly ProjectService _projectService;
    private readonly SecretsService _secretsService;

    public SecretsCommand() : base("secrets", "Manage secrets for the application")
    {
        _projectService = new ProjectService();
        _secretsService = new SecretsService();

        var fileOption = new Option<FileInfo?>(
            name: "--file",
            description: "Optional file containing secrets")
        {
            IsRequired = false
        };

        var verboseOption = new Option<bool>(
            name: "--verbose",
            description: "Show HTTP calls and detailed operation information")
        {
            IsRequired = false
        };

        AddOption(fileOption);
        AddOption(verboseOption);

        this.SetHandler(async (FileInfo? file, bool verbose) => 
            await HandleCommand(file, verbose), fileOption, verboseOption);
    }

    public async Task<int> HandleCommand(FileInfo? file, bool verbose)
    {
        var projectId = await _projectService.GetCurrentProjectIdAsync(verbose);
        if (string.IsNullOrEmpty(projectId))
        {
            Console.Error.WriteLine("No project configured. Run 'devvault setup' first.");
            return 1;
        }

        if (file != null)
        {
            if (!file.Exists)
            {
                Console.Error.WriteLine($"File not found: {file.FullName}");
                return 1;
            }

            // TODO: Implement file processing logic
            if (verbose)
            {
                Console.WriteLine($"Processing secrets from file: {file.FullName}");
            }
            return 0;
        }

        var secrets = await _secretsService.GetSecretsAsync(projectId, verbose);
        if (!secrets.Any())
        {
            Console.Error.WriteLine("No secrets found for the current project.");
            return 1;
        }

        // Display secrets
        foreach (var secret in secrets)
        {
            Console.WriteLine($"{secret.Name}={secret.Value}");
            if (verbose)
            {
                Console.WriteLine($"  Source: {secret.Source}");
                Console.WriteLine($"  ID: {secret.Identifier}");
            }
        }

        return 0;
    }
}