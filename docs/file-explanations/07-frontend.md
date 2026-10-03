# `frontend/`

Part of the [file index](INDEX.md). Vite + React 19 + TypeScript SPA. **Verified by running here:** `npm ci` OK; `vitest --run` 3 files / 9 tests pass; `npm run typecheck` clean; `npm run lint` 0 errors, 2 warnings. Concepts: overview §6.17 and §12.

## Tooling/config files
| File | Details |
| --- | --- |
| `package.json` | `"type": "module"` (ESM). Scripts: `dev` (vite), `build` (`tsc -b && vite build` — type-check then bundle), `lint`, `format`, `typecheck` (`tsc -b --noEmit`), `preview`, `test` (watch), `test:ci` (`vitest --run`). Unused-by-code dependencies worth knowing: `shadcn` (CLI, listed as a runtime dependency), `next-themes` (only `useTheme` in `sonner.tsx`). |
| `package-lock.json` | lockfile v3; `npm ci` fails if it disagrees with `package.json`. Excluded from Prettier. |
| `vite.config.ts` | `/// <reference types="vitest/config" />` adds the `test` key to Vite's config type. Plugins in order: `tanstackRouter({ target: "react", autoCodeSplitting: true })` (must precede React plugin), `react()`, `tailwindcss()`. Alias `@` → `./src` (mirrored in `tsconfig*.json` `paths` and `components.json`). `server.proxy` for `/api` and `/health` → `http://localhost:5080`. `test`: jsdom environment, `setupFiles ./src/test/setup.ts`. |
| `tsconfig.json` / `.app.json` / `.node.json` | Project references. App config: target ES2023, bundler module resolution, `verbatimModuleSyntax` (type imports must use `import type`), `jsx: react-jsx`, `strict`, `noUncheckedIndexedAccess` (indexing returns `T \| undefined` — see `byCode[lang][error.code] ?? …`), `erasableSyntaxOnly` (no enums/namespaces/parameter properties), no unused locals/params. Node config only covers `vite.config.ts`. |
| `eslint.config.js` | Flat config. `strictTypeChecked` (type-aware, needs `projectService`), `eslint-plugin-react-hooks` flat recommended, `react-refresh` (vite preset). Ignores `dist`, `coverage`, `src/routeTree.gen.ts`. Override for `src/routes/**` allows `Route` as an extra export — yet lint still reports the 2 warnings I saw. |
| `.prettierrc`, `.prettierignore` | width 100, double quotes, semicolons, trailing commas; ignores generated tree and lockfile. Note: `components/ui/*.tsx` (shadcn output) use no semicolons — i.e., not Prettier-formatted. |
| `components.json` | shadcn CLI settings: style `base-nova`, `tailwind.css: src/index.css`, `tailwind.config: ""` (v4), aliases `@/components`, `@/lib/utils`, `@/components/ui`, `@/hooks`; `iconLibrary: lucide`; `rtl: false`. |
| `.gitignore` | Vite template. |
| `index.html` | `<html lang="en">` (static, even though Arabic is planned), favicon link, `<div id="root">`, module script. `<title>frontend</title>` is the template default. |
| `public/favicon.svg`, `public/icons.svg` | Served as-is at `/favicon.svg` and `/icons.svg`. `icons.svg` is an SVG sprite (symbols such as `bluesky-icon`); I found no reference to it in `src` or `index.html`. |
| `README.md` | Conventions + stale/planned content (see overview §12). |

## `src/` files
- **`main.tsx`** — described in overview §6.17.
- **`index.css`** — `@import "tailwindcss"`, `tw-animate-css`, `shadcn/tailwind.css`, `@fontsource-variable/geist`; `@custom-variant dark (&:is(.dark *))`; `@theme inline` maps CSS variables to Tailwind color/radius tokens; `:root` and `.dark` define oklch palettes; `@layer base` applies border/background/text defaults and the Geist font. Nothing toggles `.dark` yet.
- **`app/queryClient.ts`** — exports the singleton `queryClient`: `retry: 1`, `staleTime: 30_000` (comments explain both).
- **`api/errors.ts`** — `ErrorKind` (`"validation"|"notFound"|"conflict"|"rule"|"forbidden"|"unexpected"`; first five mirror the C# enum in camelCase, `unexpected` is frontend-only) and `ApiError` (`kind`, `code`, `message`, `status`, optional `fieldErrors`, `correlationId`).
- **`api/problemDetails.ts`** — `ProblemDetails` interface (RFC 9457 + `code`, `traceId`, `correlationId`, `errors`) and the `isProblemDetails(value: unknown): value is ProblemDetails` **type guard** (`typeof object`, non-null, `"title" in value` + string, `"status" in value` + number). Type guards let TypeScript narrow `unknown` after a runtime check.
- **`api/errorMessages.ts`** — `Lang = "en" | "ar"`, dictionaries `byCode`, `byKind`, `generic`, and `messageFor(error, lang)` = `byCode[lang][error.code] ?? byKind[lang][error.kind] ?? generic[lang]` (nullish coalescing chain: most specific wins). Codes listed match backend codes: `validation.failed`, `centre.name_required`, `centre.name_too_long`, `centre.slug_invalid`, `centre.time_zone_invalid`, `centre.slug_taken`, `centre.create_forbidden`. Comment: Arabic strings are placeholders until i18n files (Day 17). **No component calls `messageFor`.**
- **`api/fixtures/problem-400.json`, `problem-404.json`** — sample backend bodies; not imported anywhere I found (probably intended for a future contract test).
- **`features/status/api.ts`, `useReadiness.ts`, `StatusCard.tsx`** — overview §6.17.
- **`routes/__root.tsx`, `routes/index.tsx`, `routeTree.gen.ts`** — overview §6.17. The generated file starts with `/* eslint-disable */`, `// @ts-nocheck` and a "do not edit" banner.
- **`components/ui/*`** — shadcn "base-nova" style over `@base-ui/react` (`button.tsx` wraps `ButtonPrimitive`). Each element gets a `data-slot` attribute used by Tailwind selectors (e.g. `has-data-[slot=card-footer]`). `button-variants.ts` is separate from `button.tsx` (keeps `react-refresh` happy: component files should export only components). Variants via `cva`: `default|outline|secondary|ghost|destructive|link` × sizes `default|xs|sm|lg|icon*`. `sonner.tsx` reads `useTheme()` from `next-themes` (default `"system"`) and styles toasts from CSS variables.
- **`lib/utils.ts`** — `export { cn } from "cn"`. The UI components themselves import `cn` straight from `"cn"`, so this file is unused.
- **Test helpers** (`test/setup.ts`, `test/render.tsx`, `test/msw/handlers.ts`, `test/msw/server.ts`) — overview §6.17. Default handlers: healthy `/health/ready` and a sample `/api/system/info` (migration id there is a mock value, not the real one).
- **Tests:** `errorMessages.test.ts` (3: code hit, kind fallback, generic fallback via `"bogus" as ErrorKind`), `problemDetails.test.ts` (3), `StatusCard.test.tsx` (3: healthy; 503 shows "API is running but the database is unavailable", a Retry button, and *not* "Cannot reach the API"; Retry refetch flips to healthy).

## Not present although documented/planned
`i18next` + `src/i18n/`; generated OpenAPI client (`src/api/generated/`); `src/hooks/`; any feature other than `status`; any use of `/api/system/info`; theme toggle; RTL handling beyond the README convention.
