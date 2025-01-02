﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿using System.CommandLine;
using System.CommandLine.Parsing;
using CrossPlatformApp.CLI.Commands;

namespace CrossPlatformApp.CLI;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("DevVault CLI tool");

        // Add setup command
        var setupCommand = new SetupCommand();
        setupCommand.SetHandler(async () => await setupCommand.HandleCommand());
        rootCommand.AddCommand(setupCommand);

        // Add run command
        var runCommand = new RunCommand();
        runCommand.SetHandler(async (string[] command) => await runCommand.HandleCommand(command), runCommand.CommandArgument);
        rootCommand.AddCommand(runCommand);

        // Add secrets command
        var secretsCommand = new SecretsCommand();
        rootCommand.AddCommand(secretsCommand);

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

        var exitCode = await rootCommand.InvokeAsync(argsToUse);
        Environment.Exit(exitCode);
        return exitCode;
    }
}
