F-Environment
=============

Library for resolving Environment variables.

Priority of Environment variables are (_from the most important_):
- environment variable (_exported directly in script, or globally available_)
- in the `.env` file

## Release
1. Increment version in `src/Environment.fsproj`
2. Run `$ fake build target release`
3. Move Environment package (`Environment.VERSION.nupkg`) from `./release` dir to the NugetServer packages dir
4. Update `CHANGELOG.md`

## Development
### Requirements
- [dotnet core](https://dotnet.microsoft.com/learn/dotnet/hello-world-tutorial)
- [FAKE](https://fake.build/fake-gettingstarted.html)

### Build
```bash
fake build
```

### Watch
```bash
fake build target watch
```
