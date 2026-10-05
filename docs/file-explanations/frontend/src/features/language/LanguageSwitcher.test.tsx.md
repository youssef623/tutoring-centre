# frontend/src/features/language/LanguageSwitcher.test.tsx

## Purpose

Tests that switching language sets the document direction, `lang` attribute and the stored preference.

## Where It Fits

frontend/src/features/language. Uses the real i18next instance (`@/i18n`), no mocks.

## Walkthrough

`afterEach` resets to English so other tests are unaffected. Test 1: click the only button, wait for a button named `English`, expect `document.documentElement.dir === "rtl"`, `lang === "ar"` and `localStorage.getItem("tcm.lang") === "ar"`. Test 2: click twice (waiting for `English` then `العربية`) and expect `dir === "ltr"`, `lang === "en"`. 2 tests.

## Concepts Used

### Internationalisation (i18next) and RTL layout

#### What it means

i18n moves all user-visible text into per-language resource files looked up by key. Arabic is right-to-left, so direction is set on the document and layout uses logical CSS properties (`start`/`end`) that flip automatically.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#634-internationalisation-and-right-to-left-layout](../../../../../PROJECT_OVERVIEW2.md#634-internationalisation-and-right-to-left-layout).)

#### Where it appears in this file

RTL as a document property.

#### How it works here

Lines 21-23, 37-38.

#### Why it matters here

Direction is set on `<html>`, so every logical CSS property flips together.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`frontend/src/features/language/LanguageSwitcher.tsx`](LanguageSwitcher.tsx.md)
- [`frontend/src/i18n/index.ts`](../../i18n/index.ts.md)
