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
