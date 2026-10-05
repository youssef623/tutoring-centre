# frontend/src/components/brand/HessaLogo.tsx

## Purpose

The Hessa brand mark and wordmark as React components (inline SVG).

## Where It Fits

frontend/src/components/brand. `HessaWordmark` is used on the login page header; `HessaMark` on the login page's brand panel.

## Walkthrough

`HessaMark({ className })` (8-36): `useId()` generates a unique gradient id so several marks on a page do not collide; an `svg` 48x48 with `role="img"` and `aria-hidden="true"` (decorative); gradient stops use CSS variables with fallbacks (`var(--brand-grad-from, #14b8a6)`, `--brand-grad-to`, `--brand-grad-accent`); a rounded rectangle tile (`rx=13`), white mortarboard paths and a tassel with an amber dot. `HessaWordmark({ className, showArabic = true })` (42-62): inline-flex with the mark (`size-9`), the text `Hessa` in the heading font and, by default, the Arabic word حصة in a `<span dir="rtl">` so it reads correctly in both LTR and RTL pages.

## Concepts Used

### React components, props, state and re-rendering

#### What it means

A component is a function returning UI from props and state. When state a component depends on changes, React calls the function again (a re-render) and updates only the DOM that differs.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing](../../../../../PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

Components with props and a hook.

#### How it works here

`useId` (9), props with defaults (44-47).

#### Why it matters here

`useId` is the React-provided way to get an id that is stable across renders and unique per component instance.

### Internationalisation (i18next) and RTL layout

#### What it means

i18n moves all user-visible text into per-language resource files looked up by key. Arabic is right-to-left, so direction is set on the document and layout uses logical CSS properties (`start`/`end`) that flip automatically.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#634-internationalisation-and-right-to-left-layout](../../../../../PROJECT_OVERVIEW2.md#634-internationalisation-and-right-to-left-layout).)

#### Where it appears in this file

Bilingual brand text.

#### How it works here

`dir="rtl"` span at line 55.

#### Why it matters here

A direction attribute on the Arabic span keeps it right-to-left inside a left-to-right layout.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

The brand text `Hessa` and `حصة` are hard-coded (not in the locale files); `import { cn } from "cn"` is the `cn` npm package, not `lib/utils.ts`.

## Related Files

- [`frontend/src/routes/login.tsx`](../../routes/login.tsx.md)
- [`frontend/src/index.css`](../../index.css.md)
