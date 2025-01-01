# System Patterns

## Architecture
- Clean Architecture principles
- Platform-agnostic core logic
- Strategy pattern for platform-specific operations (if needed)

## Key Technical Decisions
1. .NET 8 for cross-platform support
2. Command-line interface using System.CommandLine for better argument parsing
3. Platform abstraction for any OS-specific operations

## Design Patterns
- Strategy Pattern for platform-specific implementations
- Factory Pattern for creating platform-specific instances
- Command Pattern for CLI operations

## Code Organization
- src/ - Source code directory
  - Core/ - Core business logic
  - Infrastructure/ - Platform-specific implementations
  - CLI/ - Command-line interface implementation
- tests/ - Test projects
