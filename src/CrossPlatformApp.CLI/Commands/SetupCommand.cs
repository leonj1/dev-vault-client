using System.CommandLine;
using CrossPlatformApp.CLI.Models;
using CrossPlatformApp.CLI.Services;
using Spectre.Console;

namespace CrossPlatformApp.CLI.Commands;

public class SetupCommand : Command
{
    private readonly ProjectService _projectService;
    private static readonly string ConfigFile = Path.Combine(Directory.GetCurrentDirectory(), "devvault.config");

    public SetupCommand() : base(name: "setup", description: "Setup project configuration")
    {
        _projectService = new ProjectService();
    }

    public async Task<int> HandleCommand()
    {
        try
        {
            var projects = await _projectService.GetProjectsAsync();
            
            if (!projects.Any())
            {
                AnsiConsole.MarkupLine("No projects");
                return 0;
            }

            // Display numbered list of projects
            for (int i = 0; i < projects.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {projects[i].Name}");
            }
            Console.WriteLine("\nEnter project number or 'q' to quit:");

            while (true)
            {
                var input = Console.ReadLine()?.Trim().ToLower();
                
                if (input == "q")
                {
                    return 0;
                }

                if (int.TryParse(input, out int selection) && 
                    selection > 0 && 
                    selection <= projects.Count)
                {
                    var selectedProject = projects[selection - 1];
                    
                    await File.WriteAllTextAsync(
                        ConfigFile, 
                        $"PROJECT_ID={selectedProject.Id}\nPROJECT_NAME={selectedProject.Name}"
                    );

                    AnsiConsole.MarkupLine("[green]Configuration saved successfully![/]");
                    return 0;
                }

                Console.WriteLine("Invalid selection. Please enter a valid number or 'q' to quit.");
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error during setup: {ex.Message}[/]");
            return 1;
        }
    }
}
