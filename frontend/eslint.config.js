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
