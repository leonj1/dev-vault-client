# Technical Context

## Technologies
- .NET 8 SDK
- C# 12
- System.CommandLine NuGet package for CLI argument parsing

## Development Setup
### Prerequisites
- .NET 8 SDK installed on both Windows and Linux environments
- Visual Studio Code with C# extensions (recommended)
- Git for version control

### Build Commands
- `dotnet build` - Build the application
- `dotnet run` - Run the application
- `dotnet test` - Run unit tests
- `dotnet publish -c Release -r win-x64 --self-contained` - Publish for Windows
- `dotnet publish -c Release -r linux-x64 --self-contained` - Publish for Linux

## Technical Constraints
- Must run on both Windows DOS and Linux bash
- Must handle platform-specific path separators and line endings
- Must use platform-agnostic file system operations
- Must handle platform-specific console output differences
