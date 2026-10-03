# frontend/index.html

## Purpose

The single HTML page of the SPA: mounts point for React and the module entry script.

## Where It Fits

Frontend root. Served by Vite dev server and used as the build entry; loads `src/main.tsx`.

## Walkthrough

`<html lang="en">` (static; language switching is planned, not implemented), `<meta charset>`, favicon link `/favicon.svg`, viewport meta, `<title>frontend</title>` (template default - not a product title), `<div id="root"></div>`, `<script type="module" src="/src/main.tsx">`. `main.tsx` throws if `#root` is missing.

## Concepts Used

### Vite dev server, bundling and proxy

#### What it means

Vite serves source modules during development and bundles for production. Its dev proxy forwards chosen paths (`/api`, `/health`) to another server so the browser sees one origin and no CORS configuration is needed.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#617-frontend-concepts-react-server-state-and-routing](../../PROJECT_OVERVIEW.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

HTML entry.

#### How it works here

Script tag.

#### Why it matters here

Vite treats `index.html` as the build entry.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

Page title is the placeholder `frontend`.

## Related Files

- [`frontend/src/main.tsx`](src/main.tsx.md)
- [`frontend/vite.config.ts`](vite.config.ts.md)
