# `Dockerfile`

## Purpose

Current Docker/infrastructure file. The source excerpt and structural map below expose its content without inferring behaviour from its filename.

## Where it fits

Category: **Docker/infrastructure**. [Open current file](../../Dockerfile). [Complete file index](INDEX.md). [Current teaching overview](../PROJECT_OVERVIEW3.md).

The reviewed file has 62 lines and 3,071 bytes. Source hash: `4a09a36c9b9d4ef7ded4ce773394592283d93d993bc4e90878417adf30daf55c`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

- Line 1: syntax=docker/dockerfile:1
- Line 3: Builds the frontend's production bundle and the Api's publish output in separate stages, so the final image
- Line 4: never contains the SDK, node_modules, source, or anything else beyond what is needed to run (Task 34.2).
- Line 6: ---- Frontend build ------------------------------------------------------------------------------------------
- Line 10: Dependencies before source: this layer is reused across builds unless package*.json itself changed.
- Line 17: ---- Backend build ---------------------------------------------------------------------------------------
- Line 21: Carries the Git commit into every project's AssemblyInformationalVersionAttribute (the SDK's own
- Line 22: "+<SourceRevisionId>" convention — Task 34.2), so /api/system/info reports exactly what is deployed.
- Line 25: Solution-wide build settings and the central package versions, then every project file, before any source:
- Line 26: `dotnet restore` is cached across builds unless a project reference or a package version actually changed.
- Line 36: No database and no proxy configured reaches this step: build-time OpenAPI generation
- Line 37: (Microsoft.Extensions.ApiDescription.Server) runs as part of publish, and Program.cs's own
- Line 38: isDocumentGeneration guard keeps that side-effect-free (Month 1), satisfying fail-fast options validation
- Line 39: with in-memory placeholder configuration instead.
- Line 46: ---- Final stage: runtime only ---------------------------------------------------------------------------
- Line 56: The base image's own built-in non-root user (Microsoft's documented convention since .NET 8): no manual
- Line 57: user/group creation needed, and no process in this image ever runs as root.
- Line 60: CMD, not ENTRYPOINT: `docker run tutoring-centre:local id` (the non-root verification check, Task 34.2)
- Line 61: needs a plain `docker run <image> <command>` to replace the default command entirely, not append to it.

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```text
# syntax=docker/dockerfile:1

# Builds the frontend's production bundle and the Api's publish output in separate stages, so the final image
# never contains the SDK, node_modules, source, or anything else beyond what is needed to run (Task 34.2).

# ---- Frontend build ------------------------------------------------------------------------------------------
FROM node:22.12-alpine AS frontend-build
WORKDIR /frontend

# Dependencies before source: this layer is reused across builds unless package*.json itself changed.
COPY frontend/package.json frontend/package-lock.json ./
RUN npm ci

COPY frontend/ ./
RUN npm run build

# ---- Backend build ---------------------------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:10.0.401 AS backend-build
WORKDIR /src

# Carries the Git commit into every project's AssemblyInformationalVersionAttribute (the SDK's own
# "+<SourceRevisionId>" convention — Task 34.2), so /api/system/info reports exactly what is deployed.
ARG COMMIT_SHA=unknown

# Solution-wide build settings and the central package versions, then every project file, before any source:
# `dotnet restore` is cached across builds unless a project reference or a package version actually changed.
COPY Directory.Build.props Directory.Packages.props global.json .editorconfig ./
COPY src/TutoringCentre.Domain/TutoringCentre.Domain.csproj src/TutoringCentre.Domain/
COPY src/TutoringCentre.Application/TutoringCentre.Application.csproj src/TutoringCentre.Application/
COPY src/TutoringCentre.Infrastructure/TutoringCentre.Infrastructure.csproj src/TutoringCentre.Infrastructure/
COPY src/TutoringCentre.Api/TutoringCentre.Api.csproj src/TutoringCentre.Api/
RUN dotnet restore src/TutoringCentre.Api/TutoringCentre.Api.csproj

COPY src/ src/

# No database and no proxy configured reaches this step: build-time OpenAPI generation
# (Microsoft.Extensions.ApiDescription.Server) runs as part of publish, and Program.cs's own
# isDocumentGeneration guard keeps that side-effect-free (Month 1), satisfying fail-fast options validation
# with in-memory placeholder configuration instead.
RUN dotnet publish src/TutoringCentre.Api/TutoringCentre.Api.csproj \
    -c Release \
    -o /app/publish \
    -p:SourceRevisionId=${COMMIT_SHA} \
    --no-self-contained

# ---- Final stage: runtime only ---------------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

COPY --from=backend-build /app/publish .
COPY --from=frontend-build /frontend/dist ./wwwroot

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

# The base image's own built-in non-root user (Microsoft's documented convention since .NET 8): no manual
# user/group creation needed, and no process in this image ever runs as root.
USER $APP_UID

# CMD, not ENTRYPOINT: `docker run tutoring-centre:local id` (the non-root verification check, Task 34.2)
# needs a plain `docker run <image> <command>` to replace the default command entirely, not append to it.
CMD ["dotnet", "TutoringCentre.Api.dll"]
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
