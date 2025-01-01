using System.CommandLine;
using Xunit;
using CrossPlatformApp.CLI;

namespace CrossPlatformApp.Tests;

public class FileProcessingTests : IDisposable
{
    private readonly string _testFilePath;

    public FileProcessingTests()
    {
        // Create test file in the test project directory
        _testFilePath = Path.Combine(Directory.GetCurrentDirectory(), "test.txt");
        File.WriteAllLines(_testFilePath, new[]
        {
            "Line 1",
            "Line 2",
            "Line 3"
        });
    }

    [Fact]
    public async Task ProcessFile_ShouldHandleValidFile()
    {
        // Arrange
        var args = new[] { "--file", _testFilePath };

        // Act
        var result = await Program.Main(args);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task ProcessFile_ShouldHandleNonExistentFile()
    {
        // Arrange
        var nonExistentFile = Path.Combine(Directory.GetCurrentDirectory(), "nonexistent.txt");
        var args = new[] { "--file", nonExistentFile };

        // Act
        var result = await Program.Main(args);

        // Assert
        Assert.NotEqual(0, result);
    }

    public void Dispose()
    {
        // Cleanup test file
        if (File.Exists(_testFilePath))
        {
            File.Delete(_testFilePath);
        }
    }
}
