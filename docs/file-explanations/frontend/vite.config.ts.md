# `frontend/vite.config.ts`

## Purpose

Current configuration file. The source excerpt and structural map below expose its content without inferring behaviour from its filename.

## Where it fits

Category: **configuration**. [Open current file](../../../frontend/vite.config.ts). [Complete file index](../INDEX.md). [Current teaching overview](../../PROJECT_OVERVIEW3.md).

The reviewed file has 46 lines and 2,077 bytes. Source hash: `e67663cdb35d7011139e76aa0b69c70c8208d8fd3a41d253b46cf77f5dd5a68f`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { fileURLToPath } from "node:url";`
- Line 2: `import tailwindcss from "@tailwindcss/vite";`
- Line 3: `import { tanstackRouter } from "@tanstack/router-plugin/vite";`
- Line 4: `import react from "@vitejs/plugin-react";`
- Line 5: `import { configDefaults, defineConfig } from "vitest/config";`

## Walkthrough and declaration map

Read the data or directives in order; there are no recognized code declarations in this file.

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
import { fileURLToPath } from "node:url";
import tailwindcss from "@tailwindcss/vite";
import { tanstackRouter } from "@tanstack/router-plugin/vite";
import react from "@vitejs/plugin-react";
import { configDefaults, defineConfig } from "vitest/config";

// The API's fixed development URL (Task 3.1). The browser itself only talks to the Vite origin.
// HTTPS (Day 16): the session and antiforgery cookies are Secure + __Host-, which a real browser only
// stores from an HTTPS origin, so the dev API must run on its "https" launch profile.
// Overridable (Day 18): Playwright starts the API on its plain-http profile instead — localhost is a
// trustworthy origin even without TLS — and passes its URL here so the dev-server proxy matches it.
const apiTarget = process.env.API_TARGET ?? "https://localhost:7197";

export default defineConfig({
  plugins: [
    // Must come before the React plugin (TanStack Router docs).
    // Component tests live beside the routes they cover (e.g. routes/login.test.tsx); they are not routes.
    tanstackRouter({ target: "react", autoCodeSplitting: true, routeFileIgnorePattern: "\\.test\\.tsx$" }),
    react(),
    tailwindcss(),
  ],
  resolve: {
    alias: {
      "@": fileURLToPath(new URL("./src", import.meta.url)),
    },
  },
  build: {
    // 0, not the Vite default (4096): a small font file inlined as a data: URI is blocked by
    // `font-src 'self'` (Task 34.5/34.8) — every asset must be a real same-origin file instead.
    assetsInlineLimit: 0,
  },
  server: {
    proxy: {
      // secure: false — Node's TLS stack doesn't read the OS trust store, so it doesn't trust the
      // `dotnet dev-certs` self-signed certificate the way the OS (and browsers) do.
      "/api": { target: apiTarget, secure: false },
      "/health": { target: apiTarget, secure: false },
    },
  },
  test: {
    environment: "jsdom",
    setupFiles: ["./src/test/setup.ts"],
    // Playwright specs (frontend/e2e/) are not unit tests; Vitest must not try to run them.
    exclude: [...configDefaults.exclude, "e2e/**"],
  },
});
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
