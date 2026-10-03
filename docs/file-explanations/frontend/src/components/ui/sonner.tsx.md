# frontend/src/components/ui/sonner.tsx

## Purpose
The shadcn wrapper around **Sonner**'s `<Toaster>`. It adds a theme from `next-themes`, lucide icons for each toast type, and CSS variables so toasts use the app's popover colours and radius.

## Where it fits
UI primitives. Mounted once in `src/routes/__root.tsx:15` (`<Toaster richColors />`). The comment there says Day 12's error utilities will show API failures through it. Nothing calls `toast()` yet.

## Walkthrough
- **Line 6:** `const { theme = "system" } = useTheme()`.
- **Lines 9–43:** `<Sonner theme={theme as ToasterProps["theme"]} ...>`:
  - `icons`: `CircleCheckIcon`, `InfoIcon`, `TriangleAlertIcon`, `OctagonXIcon`, and a spinning `Loader2Icon`;
  - `style` sets `--normal-bg`, `--normal-text`, `--normal-border` and `--border-radius` from theme tokens;
  - `toastOptions.classNames.toast = "cn-toast"`;
  - then `{...props}`.

## Concepts used
- **Toast notifications.**
- **Theming via CSS variables.**
- **Context-based theme** (`next-themes`).

## Data and control flow
Future `toast(...)` call → Sonner store → `<Toaster>` renders.

## Configuration and environment
None.

## Gotchas and issues
- **The theme is always "system".** No `ThemeProvider` from `next-themes` is mounted, so `useTheme()` returns no theme and the fallback applies. Dark mode would also need the `.dark` class (defined in `index.css`), which nothing sets.
- **`theme as ...` is a type assertion.** The README forbids `any` and `!`; this is neither, but it is an unchecked cast.
- **Not Prettier-formatted.**

## Related files
- [routes/__root.tsx](../../routes/__root.tsx.md)
- [index.css](../../index.css.md)
