# `frontend/package.json`

## Purpose

JSON data with top-level keys `name`, `private`, `version`, `type`, `scripts`, `dependencies`, `devDependencies`. Its consumers determine whether this is build configuration, a contract or a language resource.

## Where it fits

Category: **configuration**. [Open current file](../../../frontend/package.json). [Complete file index](../INDEX.md). [Current teaching overview](../../PROJECT_OVERVIEW3.md).

The reviewed file has 68 lines and 1,998 bytes. Source hash: `00db5d6061abc2e9ff28bc419d843c666d235b01f24a6a238d778a14ecd09d5e`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Read the data or directives in order; there are no recognized code declarations in this file.

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```json
{
  "name": "frontend",
  "private": true,
  "version": "0.0.0",
  "type": "module",
  "scripts": {
    "dev": "vite",
    "build": "tsc -b && vite build",
    "lint": "eslint .",
    "format": "prettier --write .",
    "typecheck": "tsc -b --noEmit",
    "preview": "vite preview",
    "test": "vitest",
    "test:ci": "vitest --run",
    "generate:api": "orval --config ./orval.config.ts",
    "i18n:check": "node scripts/check-i18n-keys.mjs",
    "test:e2e": "playwright test"
  },
  "dependencies": {
    "@base-ui/react": "^1.8.0",
    "@fontsource-variable/geist": "^5.3.0",
    "@fontsource/ibm-plex-sans-arabic": "^5.3.0",
    "@hookform/resolvers": "^5.9.1",
    "@tailwindcss/vite": "^4.3.3",
    "@tanstack/react-query": "^5.104.1",
    "@tanstack/react-router": "^1.170.41",
    "class-variance-authority": "^0.7.1",
    "cn": "^0.4.0",
    "i18next": "^26.4.2",
    "i18next-icu": "^2.4.4",
    "intl-messageformat": "^11.2.15",
    "lucide-react": "^1.49.0",
    "next-themes": "^0.4.6",
    "react": "^19.2.8",
    "react-dom": "^19.2.8",
    "react-hook-form": "^7.89.0",
    "react-i18next": "^17.0.15",
    "shadcn": "^4.21.1",
    "sonner": "^2.0.8",
    "tailwindcss": "^4.3.3",
    "tw-animate-css": "^1.4.0",
    "zod": "^4.6.5"
  },
  "devDependencies": {
    "@eslint/js": "^10.0.1",
    "@playwright/test": "^1.63.0",
    "@tanstack/router-plugin": "^1.168.42",
    "@testing-library/dom": "^10.4.2",
    "@testing-library/jest-dom": "^7.0.1",
    "@testing-library/react": "^16.3.3",
    "@types/node": "^24.13.3",
    "@types/react": "^19.2.18",
    "@types/react-dom": "^19.2.7",
    "@vitejs/plugin-react": "^6.1.1",
    "eslint": "^10.10.0",
    "eslint-plugin-react-hooks": "^7.1.1",
    "eslint-plugin-react-refresh": "^0.5.6",
    "globals": "^17.12.0",
    "jsdom": "^30.1.1",
    "msw": "^2.15.0",
    "orval": "^8.39.0",
    "prettier": "^3.9.9",
    "typescript": "~6.0.2",
    "typescript-eslint": "^8.69.0",
    "vite": "^8.3.0",
    "vitest": "^5.0.3"
  }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
