﻿﻿﻿﻿using System.CommandLine;
using CrossPlatformApp.CLI.Commands;

namespace CrossPlatformApp.CLI;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        var fileOption = new Option<FileInfo?>(
            name: "--file",
            description: "The file to process")
        {
            IsRequired = true
        };

        var rootCommand = new RootCommand("DevVault CLI tool");
        rootCommand.AddOption(fileOption);
        
        // Add file processing handler
        rootCommand.SetHandler((FileInfo? file) =>
        {
            return ProcessFileAsync(file);
        }, fileOption);

        // Add setup command
        var setupCommand = new SetupCommand();
        setupCommand.SetHandler(async () => await setupCommand.HandleCommand());
        rootCommand.AddCommand(setupCommand);

        return await rootCommand.InvokeAsync(args);
    }

    private static async Task<int> ProcessFileAsync(FileInfo? file)
    {
        try
        {
            if (file == null)
            {
                Console.Error.WriteLine("File is required");
                return 1;
            }

            if (!file.Exists)
            {
                Console.Error.WriteLine($"File not found: {file.FullName}");
                return 1;
            }

            // Use platform-agnostic path operations
            var fileName = Path.GetFileName(file.FullName);
            var directory = Path.GetDirectoryName(file.FullName);

            Console.WriteLine($"Processing file: {fileName}");
            Console.WriteLine($"Located in: {directory}");

            // Example of platform-specific line endings handling
            var content = await File.ReadAllTextAsync(file.FullName);
            var lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            
            Console.WriteLine($"File contains {lines.Length} lines");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error processing file: {ex.Message}");
            return 1;
        }
    }
}
