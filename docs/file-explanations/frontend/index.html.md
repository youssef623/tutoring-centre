# frontend/index.html

## Purpose

The single HTML page of the SPA: mount point for React and the module entry script.

## Where It Fits

Frontend root. Served by the Vite dev server and used as the build entry; loads `src/main.tsx`.

## Walkthrough

`<html lang="en">` (static default; once the app starts `src/i18n/index.ts` sets `document.documentElement.lang` and `dir` from the detected language), `<meta charset>`, favicon link `/favicon.svg` (the brand icon), viewport meta, `<title>Hessa</title>` (changed from the template default `frontend`), `<div id="root"></div>`, `<script type="module" src="/src/main.tsx">`. `main.tsx` throws if `#root` is missing.

## Concepts Used

### Vite dev server, bundling and proxy

#### What it means

Vite serves source modules during development and bundles for production. Its dev proxy forwards chosen paths (`/api`, `/health`) to another server so the browser sees one origin and no CORS configuration is needed.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing](../../PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

HTML entry for Vite.

#### How it works here

Lines 1-13.

#### Why it matters here

Vite treats `index.html` as the entry of the build and rewrites the script reference.

### Internationalisation (i18next) and RTL layout

#### What it means

i18n moves all user-visible text into per-language resource files looked up by key. Arabic is right-to-left, so direction is set on the document and layout uses logical CSS properties (`start`/`end`) that flip automatically.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#634-internationalisation-and-right-to-left-layout](../../PROJECT_OVERVIEW2.md#634-internationalisation-and-right-to-left-layout).)

#### Where it appears in this file

Language/direction attributes.

#### How it works here

`lang="en"` on `<html>`.

#### Why it matters here

Static first paint is English/LTR; the i18n module corrects it synchronously before React renders.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

The static `lang="en"` is briefly wrong for an Arabic user between HTML parse and the module executing.

## Related Files

- [`frontend/src/main.tsx`](src/main.tsx.md)
- [`frontend/vite.config.ts`](vite.config.ts.md)
- [`frontend/src/i18n/index.ts`](src/i18n/index.ts.md)
