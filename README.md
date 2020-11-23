F-Environment
=============

Library for resolving Environment variables.

Priority of Environment variables are (_from the most important_):
- environment variable (_exported directly in script, or globally available_)
- in the `.env` file

## Install

Add following into `paket.dependencies`
```
git ssh://git@bitbucket.lmc.cz:7999/archi/nuget-server.git master Packages: /nuget/
# LMC Nuget dependencies:
nuget Lmc.Environment
```

Add following into `paket.references`
```
Lmc.Environment
```

## Use
```fs
open Lmc.Environment

result {
    do! loadFromFile "/file/path/.env"  // load variables from file (or return error if file is not found)
}
|> ignore

let envs = getEnvs()    // get all loaded variables

// create functions to find in environment keys
let tryGetEnv key = envs |> Map.tryFind key
let getEnv key = envs |> Map.find key

let optional =          // string option
    "OPTIONAL_ENV_VAR"
    |> tryGetEnv

let optionalOrDefault = // string
    "OPTIONAL_ENV_VAR"
    |> tryGetEnv
    |> function
        | Some value -> value
        | _ -> "default"

let mandatory =         // string
    "MANDATORY_ENV_VAR"
    |> getEnv           // or exception
```

## Release
1. Increment version in `Environment.fsproj`
2. Update `CHANGELOG.md`
3. Commit new version and tag it
4. Run `$ fake build target release`
5. Go to `nuget-server` repo, run `faket build target copyAll` and push new versions

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
