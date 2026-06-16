# Preferred Patterns — Alma.Environment

## Core Principles

- **Priority chain.** With the non-force loaders, a variable that already exists
  in the process environment is never replaced by a `.env` value; the file only
  fills in variables that are not yet set.
- **Resolve at load vs resolve on demand.** A resolved load (`loadResolvedFromFile`)
  resolves references *line by line* as the file is read, so a reference is only
  filled if its target is already known at that point. A raw load
  (`loadRawFromFile`) stores values verbatim and lets you resolve later with
  `tryResolve`, which always sees the final state of all variables.
- **Unknown references become empty.** Resolving a reference to a variable that
  is not set yields an empty string, never an error or the literal `$NAME`.

## Recommended API Usage

- Choose the loader by intent: raw (no resolution), resolved (resolution at load
  time, file yields to existing vars), or force-resolved (resolution at load time,
  overrides existing vars). See `examples.md` → Basic Loading and
  `examples.md` → Force Override.
- To read configuration after loading, prefer `getAll` once and look up keys in
  the returned `Map`, rather than calling a getter per key. See `examples.md` →
  Reading Values.
- When a value may interpolate other variables and you want the resolved result
  regardless of file order, load raw and read with `tryResolve`. See
  `examples.md` → Order-Independent Resolution.

## Error Handling

- The loaders return `Result<unit, string>`; a missing file produces an `Error`
  with a message. Consume them inside the `result { }` computation expression
  from `Feather.ErrorHandling` and bind with `do!`. See `examples.md` →
  Basic Loading.
- `getAll`, `tryGetRaw`, `tryResolve`, `set`, `clear`, `merge`, and `update` do
  not return `Result`; the `try*` readers return `string option` for absent keys.

## Composition

- Use `merge` to keep the current value on a key conflict (existing config wins).
- Use `update` to take the incoming value on a key conflict (incoming config wins).
- Both operate on `Map<string,string>` and leave non-conflicting keys from both
  sides intact. See `examples.md` → Combining Maps.

## Integration with Other Libraries

- Pair with `Feather.ErrorHandling` for the `result { }` workflow and helpers such
  as `Result.orFail` (handy in tests to fail fast on a load error).

## Naming Conventions

- Always call through the qualified `Envs.` prefix (the module is
  `[<RequireQualifiedAccess>]`).
- `Envs.set` takes a single tuple argument `(key, value)`, which composes well
  with `List.iter Envs.set` over a list of pairs.

## Testing Recommendations

- The process environment is global mutable state shared across tests. Clear the
  keys under test with `Envs.clear` before asserting, and run env-touching tests
  sequentially (Expecto `Sequenced`) to avoid cross-test interference. See
  `examples.md` → Test Setup.
