# frontend/src/features/language/LanguageSwitcher.tsx

## Purpose

A button that toggles the UI language between Arabic and English.

## Where It Fits

frontend/src/features/language. Used on the login page, centre picker, status page and inside the user menu of `AppShell`. Persistence and `<html dir>` updates happen in `@/i18n`, not here.

## Walkthrough

`useTranslation("common")` gives `i18n` and `t`. `isArabic = i18n.language === "ar"` (7). `toggle` (9-11): `void i18n.changeLanguage(isArabic ? "en" : "ar")`. Renders a ghost small `Button` labelled with the *other* language (`t("language.english")` when Arabic is active, otherwise `t("language.arabic")`); both labels are the same in both languages (`العربية` / `English`) so a user can always find their language.

## Concepts Used

### Internationalisation (i18next) and RTL layout

#### What it means

i18n moves all user-visible text into per-language resource files looked up by key. Arabic is right-to-left, so direction is set on the document and layout uses logical CSS properties (`start`/`end`) that flip automatically.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#634-internationalisation-and-right-to-left-layout](../../../../../PROJECT_OVERVIEW2.md#634-internationalisation-and-right-to-left-layout).)

#### Where it appears in this file

Language toggle.

#### How it works here

Lines 5-17.

#### Why it matters here

Changing the language triggers i18next's `languageChanged` event, which `i18n/index.ts` listens to in order to set `<html lang dir>` and store the choice.

### React components, props, state and re-rendering

#### What it means

A component is a function returning UI from props and state. When state a component depends on changes, React calls the function again (a re-render) and updates only the DOM that differs.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing](../../../../../PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

Re-render on language change.

#### How it works here

`useTranslation` hook.

#### Why it matters here

The hook subscribes the component to i18next, so labels re-render when the language changes.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

Persisted preference lives in `localStorage` key `tcm.lang` (written by `i18n/index.ts`).

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`frontend/src/i18n/index.ts`](../../i18n/index.ts.md)
- [`frontend/src/features/language/LanguageSwitcher.test.tsx`](LanguageSwitcher.test.tsx.md)
- [`frontend/src/features/shell/AppShell.tsx`](../shell/AppShell.tsx.md)
- [`frontend/src/components/ui/button.tsx`](../../components/ui/button.tsx.md)
