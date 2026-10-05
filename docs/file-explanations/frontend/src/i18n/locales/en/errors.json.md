# frontend/src/i18n/locales/en/errors.json

## Purpose

English error messages keyed by backend error `code` and `kind` (namespace `errors`).

## Where It Fits

frontend/src/i18n/locales/en. Bundled by `i18n/index.ts` as namespace `errors`; the `ar` twin must have the same keys (checked by `scripts/check-i18n-keys.mjs`).

## Walkthrough

`byCode`: `validation.failed` and `centre.name_required`, `name_too_long`, `slug_invalid`, `time_zone_invalid`, `slug_taken`, `create_forbidden`. `byKind`: `validation`, `notFound`, `conflict`, `rule`, `forbidden`, `unauthenticated` (`Please sign in to continue.`), `unexpected`. `generic`: `Something went wrong. Please try again.` Consumed by `messageFor` (`api/errorMessages.ts`). The nesting follows the dotted backend codes (`centre.slug_invalid` is `byCode.centre.slug_invalid`).

## Concepts Used

### Internationalisation (i18next) and RTL layout

#### What it means

i18n moves all user-visible text into per-language resource files looked up by key. Arabic is right-to-left, so direction is set on the document and layout uses logical CSS properties (`start`/`end`) that flip automatically.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#634-internationalisation-and-right-to-left-layout](../../../../../../PROJECT_OVERVIEW2.md#634-internationalisation-and-right-to-left-layout).)

#### Where it appears in this file

Translation resource.

#### How it works here

Nested keys looked up by `t("key.path")`.

#### Why it matters here

Keeping text in JSON lets the same component render in either language.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No `byCode` entries for `auth.*` or `tenant.*` codes; those fall back to the `byKind` text.

## Related Files

- [`frontend/src/i18n/index.ts`](../../index.ts.md)
- [`frontend/src/i18n/locales/ar/errors.json`](../ar/errors.json.md)
- [`frontend/src/api/errorMessages.ts`](../../../api/errorMessages.ts.md)
