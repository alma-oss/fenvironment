open Expecto

[<EntryPoint>]
let main argv =
    argv
    |> Tests.runTestsInAssembly {
        defaultConfig with
            runInParallel = false
    }
