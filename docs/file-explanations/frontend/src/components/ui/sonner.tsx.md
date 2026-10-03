# frontend/src/components/ui/sonner.tsx

## Purpose

Toast container wrapper (Sonner) themed from `next-themes` and CSS variables.

## Where It Fits

frontend/src/components/ui. Rendered once in `routes/__root.tsx`. No code currently triggers a toast.

## Walkthrough

`Toaster` calls `useTheme()` from `next-themes` with default `"system"` (no `ThemeProvider` exists in the app, so the default applies), passes `theme`, per-type icons from `lucide-react`, CSS-variable styles (`--normal-bg` etc.) and `toastOptions.classNames.toast = "cn-toast"`.

## Concepts Used

### React components, props, state and re-rendering

#### What it means

A component is a function returning UI from props and state. When state a component depends on changes, React calls the function again (a re-render) and updates only the DOM that differs.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#617-frontend-concepts-react-server-state-and-routing](../../../../../PROJECT_OVERVIEW.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

Hook inside a wrapper component.

#### How it works here

`useTheme()`.

#### Why it matters here

Theme-aware third-party component.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

Dead feature until toasts are used; `next-themes` without provider.

## Related Files

- [`frontend/src/routes/__root.tsx`](../../routes/__root.tsx.md)
- [`frontend/src/index.css`](../../index.css.md)
