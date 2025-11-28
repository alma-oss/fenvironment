F-Environment
=============

[![NuGet](https://img.shields.io/nuget/v/Alma.Environment.svg)](https://www.nuget.org/packages/Alma.Environment)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Alma.Environment.svg)](https://www.nuget.org/packages/Alma.Environment)
[![Tests](https://github.com/alma-oss/fenvironment/actions/workflows/tests.yaml/badge.svg)](https://github.com/alma-oss/fenvironment/actions/workflows/tests.yaml)

Library for resolving Environment variables.

Priority of Environment variables are (_from the most important_):
- environment variable (_exported directly in script, or globally available_)
- in the `.env` file

## Install

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
