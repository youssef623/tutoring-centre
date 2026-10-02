# Frontend

Not scaffolded yet (planned for Day 3). This file records the decisions made ahead of time, so the scaffolding tool fills in a shape that's already chosen rather than deciding it for us.

## Stack

- **React + TypeScript**, `strict` mode on.
- **Vite** for the dev server and build.
- **TanStack Router** for routing, **TanStack Query** for server state / data fetching.
- **Tailwind CSS** + **shadcn/ui** for styling and base components.
- **Generated API client** from the backend's OpenAPI spec — no hand-written fetch wrappers for endpoints.
- **i18next** with Arabic and English locales.
- **Logical CSS properties** (`margin-inline-start`, `padding-inline-end`, etc.) instead of physical ones (`margin-left`, `padding-right`), so layout flips correctly for Arabic RTL instead of needing a separate RTL stylesheet.

## Planned folder structure

```
frontend/
  app/            # app shell, routing setup, providers
  api/            # generated API client + query hooks
  features/       # one folder per feature (feature-based, not type-based)
  components/ui/  # shadcn/ui components and shared primitives
  i18n/           # i18next config and locale files (ar, en)
```

## API contract used by the status page

| Request | Response | Meaning |
| --- | --- | --- |
| `GET /health/ready` | `200` with plain-text body `Healthy` | API and database reachable |
| `GET /health/ready` | `503` with plain-text body `Unhealthy` | API running, database unavailable |
| (no response / network error) | — | API unreachable |
