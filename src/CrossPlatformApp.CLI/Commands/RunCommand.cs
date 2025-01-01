using System.CommandLine;
using System.Diagnostics;

namespace CrossPlatformApp.CLI.Commands;

public class RunCommand : Command
{
    public readonly Argument<string[]> CommandArgument;

    public RunCommand() : base(name: "run", description: "Run a command")
    {
        CommandArgument = new Argument<string[]>(
            name: "command",
            description: "The command to run",
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
            FileName = "bash",
            Arguments = $"-e -c \"{string.Join(" ", commandArgs.Select(arg => $"\"{arg.Replace("\"", "\\\"")}\""))}\"",
            WorkingDirectory = Directory.GetCurrentDirectory(),
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

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
            
            // Ensure all output is flushed
            process.WaitForExit();
            return process.ExitCode;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error executing command: {ex.Message}");
            return 1;
        }
    }
}
