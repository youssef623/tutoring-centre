# frontend/src/index.css

## Purpose
The global stylesheet. It imports Tailwind v4, the animation utilities, shadcn's base CSS and the Geist font. It defines the design tokens (colours in OKLCH, radii, fonts) for light and dark themes, maps them into Tailwind's theme, and sets base element styles.

## Where it fits
Imported once by `main.tsx:7`. The Tailwind Vite plugin processes it. `components.json` points the shadcn CLI here. Every UI component consumes its tokens (e.g. `bg-card`, `text-muted-foreground`).

## Walkthrough
- **Lines 1–4, imports:** `tailwindcss`, `tw-animate-css`, `shadcn/tailwind.css`, `@fontsource-variable/geist`.
- **Line 6:** `@custom-variant dark (&:is(.dark *));`. The `dark:` variant activates under a `.dark` ancestor (class strategy).
- **Lines 8–49, `@theme inline`:** maps the CSS variables to Tailwind tokens:
  - fonts (`--font-sans: 'Geist Variable'`, `--font-heading` = sans);
  - colours (`--color-primary: var(--primary)` …, including sidebar and chart colours);
  - radii derived from `--radius` (sm 0.6× … 4xl 2.6×).
- **Lines 51–84, `:root`:** light-theme values in OKLCH. These are neutral greys, a red `--destructive`, and `--radius: 0.625rem`.
- **Lines 86–118, `.dark`:** dark-theme values.
- **Lines 120–130, `@layer base`:** every element gets `border-border` and `outline-ring/50`, `body` gets the background and foreground colours, and `html` gets `font-sans`.

## Concepts used
- **Tailwind v4 CSS-first configuration** (`@theme`, `@custom-variant`), so there is no `tailwind.config.js`.
- **Design tokens as CSS custom properties.**
- **OKLCH colour space:** perceptually uniform.

## Data and control flow
Not applicable.

## Configuration and environment
None.

## Gotchas and issues
- **The `.dark` theme is never activated.** No code adds the class, and there's no `ThemeProvider`.
- **Many tokens are unused today** (sidebar, chart).
- **Not Prettier-formatted:** it mixes 4-space and 2-space indentation, and the closing braces at lines 123–129 are misaligned.

## Related files
- [main.tsx](main.tsx.md)
- [components.json](../components.json.md)
- [sonner.tsx](components/ui/sonner.tsx.md)
