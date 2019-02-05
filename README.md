F-Environment
=============

Library for resolving Environment variables.

Priority of Environment variables are (_from the most important_):
- environment variable (_exported directly in script, or globally available_)
- in the `.env` file

## Install
```
dotnet add package -s $NUGET_SERVER_PATH Lmc.Kafka
```
Where `$NUGET_SERVER_PATH` is the URL of nuget server
- it should be http://development-nugetserver-common-stable.service.devel1-services.consul:31794 (_make sure you have a correct port, since it changes with deployment_)
- see http://consul-1.infra.pprod/ui/devel1-services/services/development-nugetServer-common-stable for detailed information (and port)

## Use
```fs
open Environment

let errorLog = printfn "Error: %s"

let envs =
    ".env"  // .env file name
    |> getEnvs errorLog
let getEnv = getEnv envs            // getEnv function with "baked in" parsed env variables
let tryGetEnv = tryGetEnv envs      // tryGetEnv -||-

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
1. Increment version in `src/Environment.fsproj`
2. Update `CHANGELOG.md`
3. Commit new version and tag it
4. Run `$ fake build target release`

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
