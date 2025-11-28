module Alma.Environment.ResolvingDotEnvs

open System
open System.IO
open Expecto
open Feather.ErrorHandling
open Alma.Environment

let private tee f a =
    f a
    a

let expectNotSetYet envs =
    let currentEnvs = Envs.getAll()
    envs
    |> List.iter (fun (key, _) ->
        Expect.isNone (currentEnvs |> Map.tryFind key) $"Environment variable ${key} should not be set before .env file is loaded"
    )

let defineExpected envs =
    envs
    |> List.map (tee (fst >> Envs.clear))
    |> tee expectNotSetYet

[<Tests>]
let shouldResolveEnvironmentVariables =
    testList "Environment - resolve envs var" [
        testCase "Raw from file" <| fun _ ->
            let expected = defineExpected [
                "DOMAIN", "domain"
                "CONTEXT", "context"
                "PURPOSE", "purpose"
                "VERSION", "123"
                "SPOT", "all,common"
                "INSTANCE", "$DOMAIN.$CONTEXT.${PURPOSE}.${VERSION}"
                "BOX", "$SPOT@$INSTANCE"
            ]

            Envs.loadRawFromFile "./Fixtures/.env" |> Result.orFail
            let envs = Envs.getAll()

            expected
            |> List.iter (fun (key, value) ->
                let found = envs |> Map.tryFind key

                Expect.equal (Some value) found $"Environment variable ${key} should be set from .env file"
            )

        testCase "Resolved from file" <| fun _ ->
            let expected = defineExpected [
                "DOMAIN", "domain"
                "CONTEXT", "context"
                "PURPOSE", "purpose"
                "VERSION", "123"
                "SPOT", "all,common"
                "INSTANCE", "domain.context.purpose.123"
                "BOX", "all,common@domain.context.purpose.123"
            ]

            Envs.loadResolvedFromFile "./Fixtures/.env" |> Result.orFail
            let envs = Envs.getAll()

            expected
            |> List.iter (fun (key, expectedValue) ->
                let found = envs |> Map.tryFind key

                Expect.equal found (Some expectedValue) $"Environment variable ${key} should be set and resolved from .env file"
            )

        testCase "Resolve from file and global envs" <| fun _ ->
            defineExpected [
                "DOMAIN", "domain"
                "CONTEXT", "context"
                "PURPOSE", "purpose"
                "VERSION", "123"
                "SPOT", "all,common"
                "INSTANCE", "domain.context.purpose.123"
                "BOX", "all,common@domain.context.purpose.123"
            ]
            |> ignore

            Envs.loadRawFromFile "./Fixtures/.env" |> Result.orFail

            let expectedResolved = defineExpected [
                "NEW_BOX", "old: all,common@domain.context.purpose.123"
                "FOO", "bar: "
                "BAR", ""
            ]

            [
                "NEW_BOX", "old: $BOX"
                "FOO", "bar: $BAR"
            ]
            |> List.iter Envs.set

            expectedResolved
            |> List.iter (fun (key, expectedValue) ->
                let resolved = Envs.tryResolve key |> Option.defaultValue ""

                Expect.equal resolved expectedValue $"Environment value ${key} should be resolved"
            )

        testCase "Resolve from file with wrong order" <| fun _ ->
            let expected = defineExpected [
                "DOMAIN", "domain"
                "CONTEXT", "context"
                "PURPOSE", "purpose"
                "VERSION", "123"
                "SPOT", "all,common"
                "INSTANCE", "..."
                "BOX", "@"
            ]

            Envs.loadResolvedFromFile "./Fixtures/.env-wrong-order" |> Result.orFail
            let envs = Envs.getAll()

            expected
            |> List.iter (fun (key, expectedValue) ->
                let found = envs |> Map.tryFind key

                Expect.equal found (Some expectedValue) $"Environment variable ${key} should be set and resolved from .env file"
            )

        testCase "Resolve from file and global envs with wrong order" <| fun _ ->
            defineExpected [
                "DOMAIN", "domain"
                "CONTEXT", "context"
                "PURPOSE", "purpose"
                "VERSION", "123"
                "SPOT", "all,common"
                "INSTANCE", "..."
                "BOX", "@"
            ]
            |> ignore

            Envs.loadResolvedFromFile "./Fixtures/.env-wrong-order" |> Result.orFail

            let expectedResolved = defineExpected [
                "NEW_BOX", "old: @"
                "FOO", "bar: "
                "BAR", ""
            ]

            [
                "NEW_BOX", "old: $BOX"
                "FOO", "bar: $BAR"
            ]
            |> List.iter Envs.set

            expectedResolved
            |> List.iter (fun (key, expectedValue) ->
                let resolved = Envs.tryResolve key |> Option.defaultValue ""

                Expect.equal resolved expectedValue $"Environment value ${key} should be resolved"
            )

        testCase "Raw from file and global envs with wrong order resolved later" <| fun _ ->
            defineExpected [
                "DOMAIN", "domain"
                "CONTEXT", "context"
                "PURPOSE", "purpose"
                "VERSION", "123"
                "SPOT", "all,common"
                "INSTANCE", "domain.context.purpose.123"
                "BOX", "all,common@domain.context.purpose.123"
            ]
            |> ignore

            Envs.loadRawFromFile "./Fixtures/.env-wrong-order" |> Result.orFail

            let expectedResolved = defineExpected [
                "NEW_BOX", "old: all,common@domain.context.purpose.123"
                "FOO", "bar: "
                "BAR", ""
            ]

            [
                "NEW_BOX", "old: $BOX"
                "FOO", "bar: $BAR"
            ]
            |> List.iter Envs.set

            expectedResolved
            |> List.iter (fun (key, expectedValue) ->
                let resolved = Envs.tryResolve key |> Option.defaultValue ""

                Expect.equal resolved expectedValue $"Environment value ${key} should be resolved"
            )
    ]
