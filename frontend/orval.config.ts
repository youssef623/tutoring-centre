import { defineConfig } from "orval";

export default defineConfig({
  tutoringCentre: {
    input: { target: "./openapi/TutoringCentre.Api.json" },
    output: {
      client: "react-query",
      httpClient: "fetch",
      mode: "single",
      target: "./src/api/generated/tutoring-centre.ts",
      clean: true,
      override: {
        // Every generated request goes through apiFetch — the single place errors become ApiError.
        mutator: { path: "./src/api/apiFetch.ts", name: "apiFetch" },
        // apiFetch returns the parsed body (not { data, status, headers }), so generated types must describe the body only.
        fetch: { includeHttpResponseReturnType: false },
      },
    },
  },
});
