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

            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<Project>()
                    .Title("Select a project")
                    .PageSize(10)
                    .UseConverter(p => p.Name)
                    .AddChoices(projects)
            );

            var selectedProject = selection;

            await File.WriteAllTextAsync(
                ConfigFile, 
                $"PROJECT_ID={selectedProject.Id}\nPROJECT_NAME={selectedProject.Name}"
            );

            AnsiConsole.MarkupLine("[green]Configuration saved successfully![/]");
            return 0;
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error during setup: {ex.Message}[/]");
            return 1;
        }
    }
}
