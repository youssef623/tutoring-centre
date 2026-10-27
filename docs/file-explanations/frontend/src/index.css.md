# `frontend/src/index.css`

## Purpose

Current source file. The source excerpt and structural map below expose its content without inferring behaviour from its filename.

## Where it fits

Category: **source**. [Open current file](../../../../frontend/src/index.css). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 220 lines and 7,923 bytes. Source hash: `5d4d818019040c241c6c20b46e5406205e7065b45f86aac4b0b0655e5725d60d`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Read the data or directives in order; there are no recognized code declarations in this file.

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```text
@import "tailwindcss";
@import "tw-animate-css";
@import "shadcn/tailwind.css";
@import "@fontsource-variable/geist";
@import "@fontsource/ibm-plex-sans-arabic";

@custom-variant dark (&:is(.dark *));

@theme inline {
    --font-heading: var(--font-sans);
    /* Geist has no Arabic glyphs; the browser falls through per-character to IBM Plex Sans Arabic, so one stack serves both languages. */
    --font-sans: 'Geist Variable', 'IBM Plex Sans Arabic', sans-serif;
    --color-brand: var(--brand);
    --color-brand-foreground: var(--brand-foreground);
    --color-brand-accent: var(--brand-accent);
    --color-brand-accent-foreground: var(--brand-accent-foreground);
    --color-sidebar-ring: var(--sidebar-ring);
    --color-sidebar-border: var(--sidebar-border);
    --color-sidebar-accent-foreground: var(--sidebar-accent-foreground);
    --color-sidebar-accent: var(--sidebar-accent);
    --color-sidebar-primary-foreground: var(--sidebar-primary-foreground);
    --color-sidebar-primary: var(--sidebar-primary);
    --color-sidebar-foreground: var(--sidebar-foreground);
    --color-sidebar: var(--sidebar);
    --color-chart-5: var(--chart-5);
    --color-chart-4: var(--chart-4);
    --color-chart-3: var(--chart-3);
    --color-chart-2: var(--chart-2);
    --color-chart-1: var(--chart-1);
    --color-ring: var(--ring);
    --color-input: var(--input);
    --color-border: var(--border);
    --color-destructive: var(--destructive);
    --color-accent-foreground: var(--accent-foreground);
    --color-accent: var(--accent);
    --color-muted-foreground: var(--muted-foreground);
    --color-muted: var(--muted);
    --color-secondary-foreground: var(--secondary-foreground);
    --color-secondary: var(--secondary);
    --color-primary-foreground: var(--primary-foreground);
    --color-primary: var(--primary);
    --color-popover-foreground: var(--popover-foreground);
    --color-popover: var(--popover);
    --color-card-foreground: var(--card-foreground);
    --color-card: var(--card);
    --color-foreground: var(--foreground);
    --color-background: var(--background);
    --color-success-foreground: var(--success-foreground);
    --color-success: var(--success);
    --color-warning-foreground: var(--warning-foreground);
    --color-warning: var(--warning);
    --radius-sm: calc(var(--radius) * 0.6);
    --radius-md: calc(var(--radius) * 0.8);
    --radius-lg: var(--radius);
    --radius-xl: calc(var(--radius) * 1.4);
    --radius-2xl: calc(var(--radius) * 1.8);
    --radius-3xl: calc(var(--radius) * 2.2);
    --radius-4xl: calc(var(--radius) * 2.6);
}

:root {
    /* Hessa (حصة) — an education teal is the one brand colour, used sparingly (primary actions, active nav, focus).
       Amber is a secondary highlight reserved for brand surfaces (the login panel), never for app chrome. */
    --background: oklch(1 0 0);
    --foreground: oklch(0.21 0.02 210);
    --card: oklch(1 0 0);
    --card-foreground: oklch(0.21 0.02 210);
    --popover: oklch(1 0 0);
    --popover-foreground: oklch(0.21 0.02 210);
    --primary: oklch(0.6 0.104 184.7);
    --primary-foreground: oklch(0.985 0.005 180);
    --secondary: oklch(0.97 0.01 200);
    --secondary-foreground: oklch(0.3 0.04 195);
    --muted: oklch(0.968 0.008 210);
    --muted-foreground: oklch(0.5 0.03 215);
    --accent: oklch(0.955 0.03 185);
    --accent-foreground: oklch(0.3 0.06 190);
    --destructive: oklch(0.577 0.245 27.325);
    --success: oklch(0.6 0.14 152);
    --success-foreground: oklch(0.985 0 0);
    --warning: oklch(0.78 0.15 80);
    --warning-foreground: oklch(0.32 0.05 80);
    --border: oklch(0.91 0.012 205);
    --input: oklch(0.91 0.012 205);
    --ring: oklch(0.6 0.104 184.7);
    --brand: oklch(0.6 0.104 184.7);
    --brand-foreground: oklch(0.985 0.005 180);
    --brand-accent: oklch(0.666 0.157 58.3);
    --brand-accent-foreground: oklch(0.21 0.02 60);
    --chart-1: oklch(0.6 0.104 184.7);
```

The excerpt stops at line 90; use the original file for the remaining 130 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
