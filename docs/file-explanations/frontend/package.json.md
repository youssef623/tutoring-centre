# frontend/package.json

## Purpose
The npm manifest for the SPA: the scripts (dev, build, lint, format, typecheck, preview, test) and the runtime and dev dependencies.

## Where it fits
Frontend root. CI runs `npm ci` and the scripts `lint`, `typecheck`, `test:ci` and `build` (`ci.yml:55-68`). Dependabot updates it weekly.

## Walkthrough
- **Line 5:** `"type": "module"`, so `.js` config files are ES modules.
- **Lines 6–15, scripts:**

| Script | Command | Notes |
| --- | --- | --- |
| `dev` | `vite` | dev server on 5173 with proxy |
| `build` | `tsc -b && vite build` | type-checks the project references, then bundles to `dist/` |
| `lint` | `eslint .` | 0 errors, 2 warnings today |
| `format` | `prettier --write .` | not run in CI |
| `typecheck` | `tsc -b --noEmit` | |
| `preview` | `vite preview` | serves `dist/` |
| `test` | `vitest` | watch mode |
| `test:ci` | `vitest --run` | single run, 9 tests pass |

- **Lines 16–32, dependencies:**
  - React 19;
  - TanStack Router and Query;
  - Tailwind 4 (and its Vite plugin);
  - shadcn (for `shadcn/tailwind.css`), `@base-ui/react`, CVA, `cn` (shadcn's clsx + tailwind-merge replacement), `lucide-react`, `sonner`, `next-themes`, `tw-animate-css`, the Geist font.
- **Lines 33–54, devDependencies:**
  - ESLint 10 + typescript-eslint + the react-hooks and react-refresh plugins;
  - Vite 8 and the React plugin;
  - Vitest 5, jsdom, Testing Library, MSW;
  - Prettier;
  - TypeScript ~6.0.2;
  - the TanStack router plugin.

## Concepts used
- **Caret (`^`) vs tilde (`~`) ranges.** TypeScript is pinned to the 6.0.x patch line. The lockfile fixes exact versions.

## Data and control flow
Not applicable.

## Configuration and environment
None.

## Gotchas and issues
- **Build tooling in runtime deps.** `@tailwindcss/vite`, `tailwindcss` and `shadcn` are in `dependencies`, though they're build-time tools. That's harmless for an SPA, which ships no node_modules.
- **No `engines` field.** Nothing enforces the README's Node ≥ 22.12.
- **`name` and `version`** are still the template's `frontend` / `0.0.0`.

## Related files
- [vite.config.ts](vite.config.ts.md)
- [eslint.config.js](eslint.config.js.md)
- [ci.yml](../.github/workflows/ci.yml.md)
