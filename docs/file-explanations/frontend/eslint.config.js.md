# `frontend/eslint.config.js`

## Purpose

Source comment (intent): "]), { files: ["*

## Where it fits

Category: **configuration**. [Open current file](../../../frontend/eslint.config.js). [Complete file index](../INDEX.md). [Current teaching overview](../../PROJECT_OVERVIEW3.md).

The reviewed file has 36 lines and 1,284 bytes. Source hash: `9c78d7d14fff9836bd89d18025ee5676bf5b08dfda9b413694020ba3b78aa05c`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import js from "@eslint/js";`
- Line 2: `import globals from "globals";`
- Line 3: `import reactHooks from "eslint-plugin-react-hooks";`
- Line 4: `import reactRefresh from "eslint-plugin-react-refresh";`
- Line 5: `import tseslint from "typescript-eslint";`
- Line 6: `import { defineConfig, globalIgnores } from "eslint/config";`

## Walkthrough and declaration map

Read the data or directives in order; there are no recognized code declarations in this file.

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```text
import js from "@eslint/js";
import globals from "globals";
import reactHooks from "eslint-plugin-react-hooks";
import reactRefresh from "eslint-plugin-react-refresh";
import tseslint from "typescript-eslint";
import { defineConfig, globalIgnores } from "eslint/config";

export default defineConfig([
  // Build output, coverage and the router's generated file are not linted.
  globalIgnores(["dist", "coverage", "src/routeTree.gen.ts", "src/api/generated/**"]),
  {
    files: ["**/*.{ts,tsx}"],
    extends: [
      js.configs.recommended,
      tseslint.configs.strictTypeChecked,
      reactHooks.configs.flat.recommended,
      reactRefresh.configs.vite,
    ],
    languageOptions: {
      globals: globals.browser,
      parserOptions: {
        // Type-aware rules need the TypeScript program; the project service finds the right tsconfig per file.
        projectService: true,
        tsconfigRootDir: import.meta.dirname,
      },
    },
  },
  {
    // TanStack Router's file-based routes always export `Route` alongside the page
    // component; this is the framework's own documented pattern, not a disable.
    files: ["src/routes/**/*.{ts,tsx}"],
    rules: {
      "react-refresh/only-export-components": ["warn", { allowExportNames: ["Route"] }],
    },
  },
]);
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
