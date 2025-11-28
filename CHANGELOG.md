# Changelog

<!-- There is always Unreleased section on the top. Subsections (Add, Changed, Fix, Removed) should be Add as needed. -->
## Unreleased

## 12.0.0 - 2025-11-28
- [**BC**] Move repository

## 11.1.0 - 2025-03-17
- Update dependencies

## 11.0.0 - 2025-03-13
- [**BC**] Use net9.0

## 10.0.0 - 2024-01-09
- [**BC**] Use net8.0
- Fix package metadata

## 9.0.0 - 2023-09-10
- [**BC**] Use `Alma` namespace

## 8.0.0 - 2023-08-10
- [**BC**] Use net7.0

## 7.0.0 - 2022-01-05
- [**BC**] Use net6.0

## 6.0.1 - 2021-10-25
- Fix .net version

## 6.0.0 - 2021-10-25
- [**BC**] Rename `getEnvs` to `getAll`
- [**BC**] Move all functions to Module `Envs`
- Update dependencies
- Add `Envs.set` function
- Add `Envs.tryGetRaw` function
- Add `Envs.tryResolve` function
- Add `Envs.clear` function
- [**BC**] Rename function `loadFromFile` to `Envs.loadRawFromFile`
- Add `Envs.loadResolvedFromFile` function

## 5.0.0 - 2020-11-23
- [**BC**] Use .netcore 5.0

## 4.0.0 - 2020-11-23
- Fix parsing environment variables from file, to allow value with `=`
- Update dependencies
- [**BC**] Use .netcore 3.1
- Add `AssemblyInfo`
- [**BC**] Change namespace to `Lmc.Environment`

## 3.1.0 - 2019-06-26
- Add lint

## 3.0.0 - 2019-06-10
- Add `loadFromFile` function to explicitly load variables from file
- [**BC**] Remove `getEnvs` function to return a `Map` of all currently loaded Environment variables (Key/Value)
- [**BC**] Change `tryGetEnv` function (_use `Map.tryFind` instead_)
- [**BC**] Change `getEnv` function (_use `Map.find` instead_)

## 2.1.0 - 2019-04-18
- Add functions
    - `merge` - to add/ignore more env variables to current env variables
    - `update` - to add/update more env variables to current env variables

## 2.0.0 - 2019-02-07
- Use `envFile` path as is

## 1.0.0 - 2019-01-31
- Initial implementation
