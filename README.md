# Cross-Platform .NET Console Application

A cross-platform console application built with .NET 8 that processes files consistently across Windows and Linux environments.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started) (for running tests)
- Make (available by default on Linux, install via [Make for Windows](https://gnuwin32.sourceforge.net/packages/make.htm) on Windows)

## Project Structure

```
.
├── src/
│   ├── CrossPlatformApp.CLI/     # Main console application
│   └── CrossPlatformApp.Tests/   # Unit tests
├── .githooks/                    # Git hooks for CI
├── Dockerfile.test              # Docker configuration for testing
└── Makefile                    # Build and test automation
```

## Getting Started

1. Clone the repository:
   ```bash
   git clone <repository-url>
   cd <repository-name>
   ```

2. Initialize the project (installs git hooks and restores packages):
   ```bash
   make init
   ```

## Building the Project

### Development Build

Build the solution:
```bash
cd src
dotnet build
```

### Static Binary

Build a standalone static binary for your current platform:

```bash
make publish
```

The binary will be available in the `publish` directory as:
- Linux: `publish/CrossPlatformApp`
- Windows: `publish/CrossPlatformApp.exe`
- macOS: `publish/CrossPlatformApp`

The generated binary is:
- Fully statically linked (no external dependencies)
- Native AOT compiled for maximum performance
- Optimized for minimal size
- Self-contained (no .NET runtime required)
- Stripped of debug symbols
- Platform-specific and optimized for the target OS

To clean up:
```bash
make clean-publish
```

Note: The binary must be built on the target platform (e.g., build on Linux for Linux, Windows for Windows, etc.).

## Running the Application

Run the application with a file path:
```bash
cd src/CrossPlatformApp.CLI
dotnet run -- --file <path-to-file>
```

Example:
```bash
dotnet run -- --file test.txt
```

## Development Workflow

1. Make your changes
2. Run tests locally:
   ```bash
   make test
   ```
3. Push your changes (tests will run automatically before push)

## Testing

The project uses xUnit for testing and Docker for consistent test environments.

Run tests:
```bash
make test
```

Clean up test artifacts:
```bash
make clean-test
```

Test results are available in the `./testresults` directory after running tests.

## Git Hooks

The project uses git hooks to ensure code quality:

- Install hooks: `make install-hooks`
- Uninstall hooks: `make uninstall-hooks`

The pre-push hook automatically runs tests before pushing changes.

## Cross-Platform Compatibility

The application is designed to work consistently across platforms:
- Uses platform-agnostic file operations
- Handles different line endings (CRLF/LF)
- Provides consistent exit codes
- Works on both Windows DOS and Linux bash

## Make Commands

- `make init` - Initialize project (install hooks and restore packages)
- `make test` - Run tests in Docker
- `make clean-test` - Clean up test artifacts
- `make install-hooks` - Install git hooks
- `make uninstall-hooks` - Remove git hooks
- `make publish` - Build static binary for current platform
- `make clean-publish` - Clean up published binaries
