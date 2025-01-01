using System.CommandLine;

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

        var rootCommand = new RootCommand("Cross-platform console application example");
        rootCommand.AddOption(fileOption);

        rootCommand.SetHandler(async (file) =>
        {
            try
            {
                if (file == null)
                {
                    Console.Error.WriteLine("File is required");
                    return;
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
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error processing file: {ex.Message}");
            }
        }, fileOption);

        return await rootCommand.InvokeAsync(args);
    }
}
