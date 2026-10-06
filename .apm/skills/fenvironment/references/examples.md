# Examples — Alma.Environment

This file is the single source of truth for all example code in this skill.
Examples are ordered by increasing complexity and are self-contained.

A `.env` file used by several examples below:

```ini
# Service configuration
SERVICE_HOST=localhost
SERVICE_PORT=8080
API_URL="http://${SERVICE_HOST}:${SERVICE_PORT}/v1"
WORKER_NAME="worker-$SERVICE_HOST"
```

## Basic Loading

Load and resolve a `.env` file inside the `result { }` workflow. A missing file
yields an `Error`.

```fsharp
open Alma.Environment
open Feather.ErrorHandling

let load () =
    result {
        do! Envs.loadResolvedFromFile "/etc/example/.env"
    }
```

## Reading Values

`tryGetRaw` returns the stored value (no interpolation); `tryResolve` returns the
interpolated value; `getAll` returns everything as stored.

```fsharp
open Alma.Environment

// Stored verbatim, e.g. "http://${SERVICE_HOST}:${SERVICE_PORT}/v1"
let rawUrl : string option = Envs.tryGetRaw "API_URL"

// Interpolated, e.g. "http://localhost:8080/v1"
let resolvedUrl : string option = Envs.tryResolve "API_URL"

let all : Map<string, string> = Envs.getAll ()
let host = all |> Map.tryFind "SERVICE_HOST"
```

## Force Override

`forceLoadResolvedFromFile` replaces variables that are already exported, unlike
the non-force loaders.

```fsharp
open Alma.Environment
open Feather.ErrorHandling

Envs.set ("SERVICE_HOST", "exported-host")

result {
    // Overrides SERVICE_HOST with the value from the file
    do! Envs.forceLoadResolvedFromFile "/etc/example/.env"
}
|> ignore

let host = Envs.tryGetRaw "SERVICE_HOST" // value from the file, not "exported-host"
```

## Order-Independent Resolution

When references may appear before their targets, load raw and resolve on demand.

```fsharp
open Alma.Environment
open Feather.ErrorHandling

result {
    do! Envs.loadRawFromFile "/etc/example/.env"
}
|> ignore

// Resolves against the final state, regardless of line order in the file
let url = Envs.tryResolve "API_URL" |> Option.defaultValue ""

// Unknown references resolve to an empty string
Envs.set ("GREETING", "hello $MISSING")
let greeting = Envs.tryResolve "GREETING" // Some "hello "
```

## Combining Maps

`merge` keeps the current value on conflict; `update` takes the new value. Both
preserve non-conflicting keys.

```fsharp
open Alma.Environment

let current = Map [ "SERVICE_HOST", "localhost"; "SERVICE_PORT", "8080" ]
let incoming = Map [ "SERVICE_PORT", "9090"; "WORKER_NAME", "worker-a" ]

// SERVICE_PORT stays "8080"; WORKER_NAME added
let kept = Envs.merge current incoming

// SERVICE_PORT becomes "9090"; WORKER_NAME added
let replaced = Envs.update current incoming

// Envs.set takes a single tuple; pipe a list of pairs through List.iter
[ "SERVICE_HOST", "localhost"; "SERVICE_PORT", "8080" ]
|> List.iter Envs.set
```

## Test Setup

Clear keys before asserting and run env-touching tests sequentially. `Result.orFail`
fails the test fast if a load returns an `Error`.

```fsharp
open Expecto
open Feather.ErrorHandling
open Alma.Environment

[<Tests>]
let tests =
    testList "config" [
        testCase "resolves API_URL from file" <| fun _ ->
            [ "SERVICE_HOST"; "SERVICE_PORT"; "API_URL" ]
            |> List.iter Envs.clear

            Envs.loadResolvedFromFile "./Fixtures/.env" |> Result.orFail

            Expect.equal
                (Envs.tryResolve "API_URL")
                (Some "http://localhost:8080/v1")
                "API_URL should be resolved from the .env file"
    ]

[<EntryPoint>]
let main argv =
    Tests.runTestsInAssemblyWithCLIArgs [ Sequenced ] argv
```

## Full Workflow

Load configuration with file values yielding to exported variables, then read a
mandatory and an optional value.

```fsharp
open Alma.Environment
open Feather.ErrorHandling

let configure () =
    result {
        do! Envs.loadResolvedFromFile "/etc/example/.env"

        let envs = Envs.getAll ()

        let getEnv key = envs |> Map.find key          // throws if absent
        let tryGetEnv key = envs |> Map.tryFind key     // string option

        let apiUrl = getEnv "API_URL"
        let workerName =
            tryGetEnv "WORKER_NAME"
            |> Option.defaultValue "worker-default"

        return apiUrl, workerName
    }
```
