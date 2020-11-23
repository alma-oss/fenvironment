# Changelog

<!-- There is always Unreleased section on the top. Subsections (Add, Changed, Fix, Removed) should be Add as needed. -->
## Unreleased
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
