/**
 * Per-route layout data read by the shell (Day 53). A route that needs more width than the reading-width column
 * every other page uses — the timetable's seven day columns — sets `staticData: { layout: "wide" }`.
 */
declare module "@tanstack/react-router" {
  interface StaticDataRouteOption {
    layout?: "wide";
  }
}

export {};
