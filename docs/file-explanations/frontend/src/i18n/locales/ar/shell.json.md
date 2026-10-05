# frontend/src/i18n/locales/ar/shell.json

## Purpose

Arabic strings for the authenticated app shell and dashboard (namespace `shell`).

## Where It Fits

frontend/src/i18n/locales/ar. Bundled by `i18n/index.ts` as namespace `shell`; the `en` twin must have the same keys (checked by `scripts/check-i18n-keys.mjs`).

## Walkthrough

Same keys as the English file; for example `userMenu.logout` is `تسجيل الخروج` and `dashboard.welcome` is `مرحبًا، {name}` (the same `{name}` placeholder).

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

No bugs or surprises found while reading this file.

## Related Files

- [`frontend/src/i18n/index.ts`](../../index.ts.md)
- [`frontend/src/i18n/locales/en/shell.json`](../en/shell.json.md)
- [`frontend/src/features/shell/AppShell.tsx`](../../../features/shell/AppShell.tsx.md)
- [`frontend/src/routes/_authenticated/index.tsx`](../../../routes/_authenticated/index.tsx.md)
