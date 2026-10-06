---
name: fenvironment
description: >-
  Use whenever generating or reviewing F# code that loads environment variables
  from `.env` files or resolves `$VAR` / `${VAR}` references via the
  `Alma.Environment` library. Trigger on `Envs.loadResolvedFromFile`,
  `Envs.forceLoadResolvedFromFile`, `Envs.loadRawFromFile`, `Envs.tryResolve`,
  `Envs.tryGetRaw`, `Envs.getAll`, `Envs.set`, `Envs.clear`, `Envs.merge`,
  `Envs.update`, and on mentions of dotenv parsing, env-var priority chains,
  exported-vs-file precedence, or recursive variable interpolation in F#.
---

# F-Environment

Library: [alma-oss/fenvironment](https://github.com/alma-oss/fenvironment)
NuGet: `Alma.Environment`

## Purpose

`Alma.Environment` is an F# library that loads environment variables from `.env`
files and resolves references between them (`$NAME` or `${NAME}`). Values that
are already exported in the process environment take priority over values defined
in a `.env` file, unless a force loader is used.

## When to Use

- Loading configuration from a `.env` file into the process environment in F#.
- Resolving variables that interpolate other variables (e.g. a URL built from a
  host and a port variable).
- Reading a single variable with or without reference resolution.
- Merging two sets of variables with explicit conflict-resolution semantics.

## When NOT to Use

- When you need a typed configuration binder or schema validation — this library
  returns plain `string` values only.
- When values must override exported process variables and you are not using the
  force loader (it will silently keep the existing value otherwise).
- For non-`.env` formats (JSON, YAML, INI) — only the simple `KEY=VALUE` dotenv
  format is parsed.

## Main Concepts

- **`Envs`** — the single `[<RequireQualifiedAccess>]` module exposing the whole API.
- **Raw load** — load values verbatim without resolving references.
- **Resolved load** — resolve references at load time, line by line.
- **Force load** — like resolved load but overrides already-exported variables.
- **Priority chain** — exported/global env var wins over `.env` value (non-force loaders).
- **Reference syntax** — `$NAME` or `${NAME}`; resolution is recursive; an
  unknown reference resolves to an empty string.
- **`tryGetRaw` vs `tryResolve`** — read a value as stored vs read it with
  references resolved on demand.
- **`merge` vs `update`** — combine two `Map<string,string>` keeping the current
  value vs taking the new value on key conflict.

## Related Libraries

- **`Feather.ErrorHandling`** — provides the `result { }` computation expression
  and helpers (e.g. `Result.orFail`) used to consume the `Result`-returning loaders.

## Keywords for Search

dotenv, .env, environment variable, env var, `Envs`, `loadResolvedFromFile`,
`forceLoadResolvedFromFile`, `loadRawFromFile`, `tryResolve`, `tryGetRaw`,
`getAll`, variable interpolation, `${VAR}`, priority chain, override env var, F#.

## Reference Files

- For composition principles, recommended API usage, error handling, integration,
  naming, and testing guidance, read `references/preferred-patterns.md`.
- For known pitfalls, incorrect assumptions, and mistake → fix guidance, read
  `references/anti-patterns.md`.
- For worked, self-contained code examples (basic through full workflow), read
  `references/examples.md`.
