using System.CommandLine;
using System.Diagnostics;

namespace CrossPlatformApp.CLI.Commands;

public class RunCommand : Command
{
    public readonly Argument<string[]> CommandArgument;

    public RunCommand() : base(name: "run", description: "Run a command after double dashes (--)")
    {
        CommandArgument = new Argument<string[]>(
            name: "command",
            description: "The command to run (specified after --)",
            getDefaultValue: () => Array.Empty<string>())
        {
            Arity = ArgumentArity.OneOrMore
        };
        AddArgument(CommandArgument);
    }

    public async Task<int> HandleCommand(string[] commandArgs)
    {
        if (commandArgs.Length == 0)
        {
            Console.Error.WriteLine("No command specified");
            return 1;
        }

        var startInfo = new ProcessStartInfo
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = Directory.GetCurrentDirectory(),
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        // Execute the command directly
        startInfo.FileName = commandArgs[0];
        
        // Add remaining arguments directly to ArgumentList
        foreach (var arg in commandArgs.Skip(1))
        {
            startInfo.ArgumentList.Add(arg);
        }

        using var process = new Process { StartInfo = startInfo };

        process.OutputDataReceived += (sender, e) =>
        {
            if (e.Data != null)
            {
                Console.WriteLine(e.Data);
            }
        };

        process.ErrorDataReceived += (sender, e) =>
        {
            if (e.Data != null)
            {
                Console.Error.WriteLine(e.Data);
            }
        };

        try
        {
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            await process.WaitForExitAsync();
            
            // Ensure all output is flushed and get the exit code
            process.WaitForExit();
            var exitCode = process.ExitCode;

            // Set the environment exit code to match
            Environment.ExitCode = exitCode;
            return exitCode;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error executing command: {ex.Message}");
            return 1;
        }
    }
}
