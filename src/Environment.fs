namespace Alma.Environment

[<RequireQualifiedAccess>]
module Envs =
    open System
    open System.Text.RegularExpressions
    open Alma.ErrorHandling

    [<AutoOpen>]
    module internal LowLevel =
        type Key = Key of string
        type RawValue = RawValue of string

        type ResolvedValue = ResolvedValue of RawValue

        [<RequireQualifiedAccess>]
        module Key =
            let value (Key value) = value

        [<RequireQualifiedAccess>]
        module RawValue =
            let value (RawValue value) = value
            let empty = RawValue ""
            let replace reference (RawValue value) (ResolvedValue (RawValue resolved)) =
                RawValue (value.Replace(reference, resolved))

        let set (Key key) (RawValue value) =
            Environment.SetEnvironmentVariable(key, value)

        /// <see>https://stackoverflow.com/questions/12014179/f-get-environment-variables-as-a-generic-collection</see>
        let getAll () =
            Environment.GetEnvironmentVariables()
            |> Seq.cast<Collections.DictionaryEntry>
            |> Seq.map (fun d ->
                d.Key :?> string |> Key,
                d.Value :?> string |> RawValue
            )
            |> Map

        let tryGet (Key key) =
            match Environment.GetEnvironmentVariable(key) with
            | empty when String.IsNullOrWhiteSpace(empty) -> None
            | value -> Some (RawValue value)

        let rec resolveValue = function
            | RawValue (Regex @"\$\{([a-zA-Z0-9_]+)\}" [ reference ]) as value ->
                let referencePlaceholder = sprintf "${%s}" reference

                Key reference
                |> tryGet
                |> Option.defaultValue RawValue.empty
                |> ResolvedValue
                |> RawValue.replace referencePlaceholder value
                |> resolveValue

            | RawValue (Regex @"\$([a-zA-Z0-9_]+)" [ reference ]) as value ->
                let referencePlaceholder = sprintf "$%s" reference

                Key reference
                |> tryGet
                |> Option.defaultValue RawValue.empty
                |> ResolvedValue
                |> RawValue.replace referencePlaceholder value
                |> resolveValue

            | value -> value

        let clear key =
            set key (RawValue null)

    /// Inspired by http://www.codesuji.com/2018/02/28/F-and-DotEnv/
    let private loadFromFileAndResolve resolve filePath =
        result {
            if IO.File.Exists(filePath) |> not then
                return! Error (sprintf "File %s does not exists." filePath)

            IO.File.ReadAllLines(filePath)
            |> Seq.iter (fun line ->
                let lineWithoutComments = Regex.Replace(line, "#.*", "")
                let parts =
                    lineWithoutComments.Split('=', 2)
                    |> Array.map (fun x -> x.Trim())

                match parts with
                | [| key; value |] when tryGet (Key key) = None ->
                    value.Trim([|'"'; '''|])
                    |> RawValue
                    |> resolve
                    |> set (Key key)
                | _ -> ()
            )
        }

    /// <summary>
    /// It will load all values from a file.
    /// </summary>
    let loadRawFromFile = loadFromFileAndResolve id

    /// <summary>
    /// It will load all values from a file and resolve.
    /// </summary>
    let loadResolvedFromFile = loadFromFileAndResolve resolveValue

    let set (key, value) = set (Key key) (RawValue value)

    let getAll () =
        getAll ()
        |> Seq.map (fun kv -> kv.Key |> Key.value, kv.Value |> RawValue.value)
        |> Map.ofSeq

    /// <summary>
    /// <para>It will return the environment variable as it was set.</para>
    /// <para><b>NOTE:</b> If it references other variable, it won't be resolved!</para>
    /// </summary>
    let tryGetRaw key =
        Key key
        |> tryGet
        |> Option.map RawValue.value

    /// <summary>
    /// <para>It will return the environment variable with all references resolved.</para>
    /// <para>Allowed references are: <code>$REFERENCE</code> or <code>${REFERENCE}</code></para>
    /// </summary>
    let tryResolve key =
        Key key
        |> tryGet
        |> Option.map (resolveValue >> RawValue.value)

    let clear key =
        Key key
        |> clear

    let private combine onSame (currentEnvs: Map<string, string>) (newEnvs: Map<string, string>): Map<string, string> =
        newEnvs
        |> Map.fold (fun acc key newValue ->
            match acc.TryFind key with
            | Some currentValue -> acc.Add(key, onSame currentValue newValue)
            | _ -> acc.Add(key, newValue)
        ) currentEnvs

    let merge currentEnvs newEnvs =
        combine (fun currentValue _newValue -> currentValue) currentEnvs newEnvs

    let update currentEnvs newEnvs =
        combine (fun _currentValue newValue -> newValue) currentEnvs newEnvs
