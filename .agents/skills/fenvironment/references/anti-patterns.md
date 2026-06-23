# Anti-Patterns — Alma.Environment

Each entry is **mistake → why → fix**.

## Relying on file order with a resolved load

- **Mistake:** Using `Envs.loadResolvedFromFile` on a `.env` where a variable
  references another that is defined *later* in the file.
- **Why:** Resolution happens line by line during load, so a forward reference is
  resolved against a not-yet-set target and collapses to an empty string.
- **Fix:** Either order the file so referenced variables come first, or use
  `Envs.loadRawFromFile` and read with `Envs.tryResolve`, which resolves against
  the final state regardless of order. See `examples.md` → Order-Independent
  Resolution.

## Expecting `.env` to override an exported variable

- **Mistake:** Assuming a value in the `.env` file replaces a variable already
  exported in the environment.
- **Why:** The non-force loaders honor the priority chain — existing process
  variables win and the file value is skipped.
- **Fix:** Use `Envs.forceLoadResolvedFromFile` when the file must take
  precedence. See `examples.md` → Force Override.

## Reading a raw value and expecting references resolved

- **Mistake:** Calling `Envs.tryGetRaw` on a value that contains `$NAME` /
  `${NAME}` and using it as a final value.
- **Why:** `tryGetRaw` returns the stored value verbatim and performs no
  interpolation.
- **Fix:** Use `Envs.tryResolve` to get the interpolated result. See
  `examples.md` → Reading Values.

## Assuming `getAll` resolves references

- **Mistake:** Treating the `Map` returned by `Envs.getAll` as fully resolved.
- **Why:** `getAll` returns variables exactly as stored; entries loaded raw still
  contain unresolved `$NAME` placeholders.
- **Fix:** Resolve per key with `Envs.tryResolve`, or load with a resolved loader
  in correct order. See `examples.md` → Reading Values.

## Calling `Envs.set` with curried arguments

- **Mistake:** Writing `Envs.set key value`.
- **Why:** `Envs.set` takes a single tuple argument `(key, value)`, not two
  curried arguments.
- **Fix:** Call `Envs.set (key, value)`; this also lets you pipe a list of pairs
  through `List.iter Envs.set`. See `examples.md` → Combining Maps.

## Expecting an exception or literal on an unknown reference

- **Mistake:** Designing logic around an unresolved `${NAME}` staying literal or
  throwing.
- **Why:** An unknown reference resolves to an empty string by design.
- **Fix:** Ensure the referenced variable is set before resolving, or treat an
  empty result as "not provided". See `examples.md` → Order-Independent
  Resolution.
