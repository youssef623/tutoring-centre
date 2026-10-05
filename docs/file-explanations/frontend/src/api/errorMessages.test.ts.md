# frontend/src/api/errorMessages.test.ts

## Purpose

Unit tests of `messageFor` fallback behaviour (English resources).

## Where It Fits

frontend/src/api; run by Vitest. `src/test/setup.ts` imports `@/i18n`, so i18next is initialised in English before these tests run.

## Walkthrough

Three tests with Arrange/Act/Assert comments: known code `centre.slug_invalid` -> 'The web address may only contain lowercase letters, digits and single hyphens.'; unknown code `centre.something_new` with kind `conflict` -> 'This conflicts with existing data.'; unknown code and unknown kind (`"bogus" as ErrorKind`) -> 'Something went wrong. Please try again.' Not tested: Arabic output, the `unexpected` and `unauthenticated` kinds.

## Concepts Used

### Internationalisation (i18next) and RTL layout

#### What it means

i18n moves all user-visible text into per-language resource files looked up by key. Arabic is right-to-left, so direction is set on the document and layout uses logical CSS properties (`start`/`end`) that flip automatically.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#634-internationalisation-and-right-to-left-layout](../../../../PROJECT_OVERVIEW2.md#634-internationalisation-and-right-to-left-layout).)

#### Where it appears in this file

Real resources in tests.

#### How it works here

Lines 19-21, 37, 53.

#### Why it matters here

The tests assert on the actual English strings from `errors.json`, so changing the JSON wording updates the expectation.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`frontend/src/api/errorMessages.ts`](errorMessages.ts.md)
- [`frontend/src/api/errors.ts`](errors.ts.md)
- [`frontend/src/test/setup.ts`](../test/setup.ts.md)
