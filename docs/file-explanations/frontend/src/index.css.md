# frontend/src/index.css

## Purpose

Global styles: Tailwind v4 imports, the Hessa brand theme tokens (light and dark), fonts and base layer rules.

## Where It Fits

frontend/src. Imported by `main.tsx`; configured by `components.json`. Defines the CSS variables that shadcn components and the brand pages use.

## Walkthrough

`@import` of `tailwindcss`, `tw-animate-css`, `shadcn/tailwind.css`, `@fontsource-variable/geist` and (new) `@fontsource/ibm-plex-sans-arabic`. `@custom-variant dark (&:is(.dark *))`. `@theme inline { ... }`: `--font-sans: 'Geist Variable', 'IBM Plex Sans Arabic', sans-serif` (comment: Geist has no Arabic glyphs, so the browser falls through per character to IBM Plex Sans Arabic and one stack serves both languages); new colour tokens `--color-brand*`, `--color-success*`, `--color-warning*` mapping to variables; existing sidebar/chart/radius tokens. `:root` (light): comment 'Hessa - an education teal is the one brand colour, used sparingly (primary actions, active nav, focus); amber is a secondary highlight reserved for brand surfaces'. Values are `oklch(...)`: `--primary: oklch(0.6 0.104 184.7)` (teal), `--ring` the same teal, `--brand`, `--brand-accent: oklch(0.666 0.157 58.3)` (amber), `--success`, `--warning`, `--radius: 0.75rem`, sidebar tokens tinted teal, foreground `oklch(0.21 0.02 210)`. `.dark`: a lighter teal `oklch(0.785 0.133 181.9)` as primary, dark teal-grey backgrounds. `@layer base` (unchanged structure): `border-border outline-ring/50` on everything, `body` background/foreground, `html` `font-sans`.

## Concepts Used

### Tailwind, shadcn/ui and variants

#### What it means

Tailwind composes styles from small utility classes. shadcn/ui copies component source into your repo; `cva` (class-variance-authority) maps variant props like `variant="outline"` to class strings.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing](../../../PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

Design tokens as CSS variables.

#### How it works here

`:root` and `.dark` blocks.

#### Why it matters here

Components use semantic classes (`bg-primary`, `text-muted-foreground`); re-theming means changing variables, not components.

### Internationalisation (i18next) and RTL layout

#### What it means

i18n moves all user-visible text into per-language resource files looked up by key. Arabic is right-to-left, so direction is set on the document and layout uses logical CSS properties (`start`/`end`) that flip automatically.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#634-internationalisation-and-right-to-left-layout](../../../PROJECT_OVERVIEW2.md#634-internationalisation-and-right-to-left-layout).)

#### Where it appears in this file

One font stack for two scripts.

#### How it works here

`--font-sans` line.

#### Why it matters here

Per-character font fallback means Arabic text renders in IBM Plex Sans Arabic without language-specific CSS.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

Dark theme tokens exist but nothing toggles the `.dark` class yet; `next-themes` has no provider in the app. Brand-panel gradients on the login page are hard-coded hex values, not these tokens.

## Related Files

- [`frontend/components.json`](../components.json.md)
- [`frontend/src/main.tsx`](main.tsx.md)
- [`frontend/src/components/ui/card.tsx`](components/ui/card.tsx.md)
- [`frontend/src/components/brand/HessaLogo.tsx`](components/brand/HessaLogo.tsx.md)
