/// <reference types="vitest/config" />
import { fileURLToPath } from "node:url";
import tailwindcss from "@tailwindcss/vite";
import { tanstackRouter } from "@tanstack/router-plugin/vite";
import react from "@vitejs/plugin-react";
import { defineConfig } from "vite";

// The API's fixed development URL (Task 3.1). The browser itself only talks to the Vite origin.
// HTTPS (Day 16): the session and antiforgery cookies are Secure + __Host-, which a real browser only
// stores from an HTTPS origin, so the dev API must run on its "https" launch profile.
const apiTarget = "https://localhost:7197";

export default defineConfig({
  plugins: [
    // Must come before the React plugin (TanStack Router docs).
    tanstackRouter({ target: "react", autoCodeSplitting: true }),
    react(),
    tailwindcss(),
  ],
  resolve: {
    alias: {
      "@": fileURLToPath(new URL("./src", import.meta.url)),
    },
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
  },
});
