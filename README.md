# Purpose

This is a command line tool that allows you to manage secrets for your development projects.

```
# select the project that has the secrets you want to use locally
jose@plex:~/src/dev-vault-client$ ./publish/devvault setup
1. ProjectA

Enter project number or 'q' to quit:
1
Configuration saved successfully!

# show the secrets for the project
jose@plex:~/src/dev-vault-client$ ./publish/devvault secrets
┌───────────────┬────────────┬──────┐
│ NAME          │ VALUE      │ NOTE │
├───────────────┼────────────┼──────┤
│ SECRETA       │ somevallue │      │
│ ANOTHERSECRET │ foobar     │      │
└───────────────┴────────────┴──────┘

# example script that shows the environment variables are not set
jose@plex:~/src/dev-vault-client$ ./test-script.sh 
This is a test script
Secret: 
Secret: 

# example script that shows the environment variables are set thanks to devvault
jose@plex:~/src/dev-vault-client$ ./publish/devvault run -- ./test-script.sh 
This is a test script
Secret: somevallue
Secret: foobar
```

## Build

```
make publish
```

### Static Binary

Build a standalone static binary for your current platform:

```bash
make publish
# binary gets built in the publish directory
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


## Testing

The project uses xUnit for testing and Docker for consistent test environments.

Run tests:
```bash
make test
```
