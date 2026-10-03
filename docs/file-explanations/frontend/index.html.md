# frontend/index.html

## Purpose
The single HTML page of the SPA and Vite's entry point. It declares the favicon and viewport, provides the `#root` mount node, and loads `/src/main.tsx` as an ES module.

## Where it fits
Frontend entry. Vite serves it in dev and rewrites it during `vite build` (into `dist/index.html`). `main.tsx` mounts React into `#root`.

## Walkthrough
- **Line 2:** `<html lang="en">`.
- **Line 5:** favicon `/favicon.svg` (from `public/`).
- **Line 6:** responsive viewport.
- **Line 7:** `<title>frontend</title>`.
- **Line 10:** `<div id="root"></div>`.
- **Line 11:** `<script type="module" src="/src/main.tsx">`.

## Concepts used
- **Vite HTML entry:** module scripts are resolved and bundled from here.

## Data and control flow
Browser → `index.html` → `main.tsx` → React tree.

## Configuration and environment
None.

## Gotchas and issues
- **Template leftovers.** The title is still the template's `frontend`, and the favicon is the Vite logo.
- **No RTL support yet.** There's no `dir` attribute, and `lang` is fixed to `en`. Arabic will need `lang="ar" dir="rtl"` set dynamically (planned i18n, Day 17).

## Related files
- [main.tsx](src/main.tsx.md)
- [vite.config.ts](vite.config.ts.md)
