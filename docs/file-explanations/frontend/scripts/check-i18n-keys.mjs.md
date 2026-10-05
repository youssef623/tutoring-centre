# frontend/scripts/check-i18n-keys.mjs

## Purpose

Node script that fails when the English and Arabic translation files do not have exactly the same namespaces and keys.

## Where It Fits

frontend/scripts. Run by `npm run i18n:check` and by a CI step. Reads `src/i18n/locales/{en,ar}/*.json`. No dependencies beyond Node.

## Walkthrough

Top: shebang, imports (`readFileSync`, `readdirSync`, `fileURLToPath`, `path`), `localesDir = <script dir>/../src/i18n/locales`, `Languages = ["en","ar"]` (11). `flattenKeys(value, prefix)` (13-21) turns nested JSON into dotted key lists (`login.title`). `loadNamespace(language, namespace)` (23-27) reads and parses a file; `namespacesFor(language)` (29-31) lists `*.json`. Main (33-59): builds the set of namespaces over both languages; for each (sorted) namespace: if a language lacks the file entirely -> error `missing entirely in ...` and `continue`; otherwise compares key sets in both directions and reports `present in en but missing in ar: keys` (and the reverse). Any error sets `hasError`. After the loop `process.exit(1)` on error, else prints `i18n:check passed - N namespace(s), keys match in both languages.` (current run: 5 namespaces).

## Concepts Used

### Internationalisation (i18next) and RTL layout

#### What it means

i18n moves all user-visible text into per-language resource files looked up by key. Arabic is right-to-left, so direction is set on the document and layout uses logical CSS properties (`start`/`end`) that flip automatically.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#634-internationalisation-and-right-to-left-layout](../../../PROJECT_OVERVIEW2.md#634-internationalisation-and-right-to-left-layout).)

#### Where it appears in this file

Key parity check.

#### How it works here

Lines 33-59.

#### Why it matters here

Because both languages are bundled and looked up by key, a missing Arabic key would silently fall back to English (`fallbackLng: "en"` in `i18n/index.ts`); this script makes that a CI failure instead.

### Continuous Integration

#### What it means

CI runs automated build, test and analysis on every change in a clean machine so that regressions are caught before merge. Workflows are YAML files describing jobs (parallel units) made of steps.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#11-configuration--environment--deployment](../../../PROJECT_OVERVIEW2.md#11-configuration--environment--deployment).)

#### Where it appears in this file

CI step.

#### How it works here

Run by `npm run i18n:check`.

#### Why it matters here

A cheap static check in the same pipeline as lint and tests.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

Reads only the locale folder; no environment variables.

## Gotchas and Issues

Checks key *names* only; it does not check that values are non-empty, that ICU placeholders (e.g. `{name}`) match between languages, or that keys are used in code.

## Related Files

- [`frontend/src/i18n/index.ts`](../src/i18n/index.ts.md)
- [`frontend/package.json`](../package.json.md)
- [`.github/workflows/ci.yml`](../../.github/workflows/ci.yml.md)
