# src/TutoringCentre.Api/Properties/launchSettings.json

## Purpose
Local launch profiles used by `dotnet run` and IDEs. It defines URLs and sets `ASPNETCORE_ENVIRONMENT=Development`. It is not used in deployment.

## Where it fits
Api dev tooling. README step 4 runs `--launch-profile http`. The frontend's Vite proxy targets `http://localhost:5080` (`frontend/vite.config.ts:9`), and the comment there calls this "the API's fixed development URL (Task 3.1)".

## Walkthrough
- **Lines 4–12, profile `http`:** `applicationUrl: http://localhost:5080`, `launchBrowser: true`, environment Development.
- **Lines 13–21, profile `https`:** `https://localhost:7197;http://localhost:5245`, Development.

## Concepts used
- **Launch profiles.**

## Data and control flow
Not applicable.

## Configuration and environment
`ASPNETCORE_ENVIRONMENT=Development`. This turns on user-secrets loading, which the connection string depends on.

## Gotchas and issues
- The `https` profile's ports (7197/5245) don't match the Vite proxy (5080). Running it breaks the frontend's dev proxy.
- `launchBrowser: true` opens `/`, which returns 404 because no root endpoint exists.

## Related files
- [Program.cs](../Program.cs.md)
- [vite.config.ts](../../../frontend/vite.config.ts.md)
