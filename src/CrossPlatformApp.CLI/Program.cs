﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿using System.CommandLine;
using System.CommandLine.Parsing;
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

        // Add run command
        var runCommand = new RunCommand();
        runCommand.SetHandler(async (string[] command) => await runCommand.HandleCommand(command), runCommand.CommandArgument);
        rootCommand.AddCommand(runCommand);

        // Find the index of double dashes
        var dashIndex = Array.IndexOf(args, "--");
        var argsToUse = args;

        if (dashIndex >= 0 && args[0] == "run")
        {
            // Remove the double dashes and pass everything after as command arguments
            argsToUse = args.Take(dashIndex)
                           .Concat(args.Skip(dashIndex + 1))
                           .ToArray();
        }

        try
        {
            var exitCode = await rootCommand.InvokeAsync(argsToUse);
            Environment.ExitCode = exitCode;
            return exitCode;
        }
        catch (Exception)
        {
            Environment.ExitCode = 1;
            return 1;
        }
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
