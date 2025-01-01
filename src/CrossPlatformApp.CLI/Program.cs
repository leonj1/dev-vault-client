﻿﻿using System.CommandLine;
using CrossPlatformApp.CLI.Commands;

namespace CrossPlatformApp.CLI;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("DevVault CLI tool");
        var setupCommand = new SetupCommand();
        
        setupCommand.SetHandler(async () => await setupCommand.HandleCommand());
        rootCommand.AddCommand(setupCommand);

        return await rootCommand.InvokeAsync(args);
    }
}
