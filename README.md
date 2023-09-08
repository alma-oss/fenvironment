F-Environment
=============

Library for resolving Environment variables.

Priority of Environment variables are (_from the most important_):
- environment variable (_exported directly in script, or globally available_)
- in the `.env` file

## Install

Add following into `paket.dependencies`
```
source https://nuget.pkg.github.com/almacareer/index.json username: "%PRIVATE_FEED_USER%" password: "%PRIVATE_FEED_PASS%"
# LMC Nuget dependencies:
nuget Alma.Environment
```

NOTE: For local development, you have to create ENV variables with your github personal access token.
```sh
export PRIVATE_FEED_USER='{GITHUB USERNANME}'
export PRIVATE_FEED_PASS='{TOKEN}'	# with permissions: read:packages
```

Add following into `paket.references`
```
Alma.Environment
```

## Use
```fs
open Alma.Environment

result {
    do! Envs.loadFromFile "/file/path/.env"  // load variables from file (or return error if file is not found)
}
|> ignore

let envs = Envs.getAll()    // get all loaded variables

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

## Development
### Requirements
- [dotnet core](https://dotnet.microsoft.com/learn/dotnet/hello-world-tutorial)

### Build
```bash
./build.sh build
```

### Tests
```bash
./build.sh -t tests
```
