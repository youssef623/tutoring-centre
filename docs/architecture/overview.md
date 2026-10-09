# Architecture overview

## Failures: results vs exceptions

**Rule.** Expected business outcomes are `Result` failures returned as values. Bugs and infrastructure faults are exceptions, handled once by a global handler.

| Situation | Mechanism | Example |
| --- | --- | --- |
| Invalid input | `Result` failure, kind `Validation` | slug `"Bad Slug"` → `centre.slug_invalid` |
| Something requested doesn't exist | `NotFound` | unknown centre (from Week 2) |
| Conflicts with existing data | `Conflict` | duplicate slug (Day 9) |
| A business rule says no | `Rule` | enrolling into a full group (Month 3) |
| The caller isn't allowed | `Forbidden` | non-system actor creating a centre (Week 2) |
| A bug | exception | reading `Value` of a failed `Result` |
| An infrastructure fault | exception | database unreachable, timeout |

**Kinds.** `Validation`, `NotFound`, `Conflict`, `Rule`, `Forbidden` — a closed set. The Api maps each kind to exactly one HTTP status (Day 11); adding a kind is an API-contract change.

**Codes.** `<feature>.<reason>`: lowercase feature, snake_case reason — `centre.slug_invalid`, `centre.name_too_long`. Codes are a public contract that the frontend translates; renaming one is a breaking change.

**Messages** are for developers and logs. Users see translated text chosen by the code.

**Safety.** Codes and messages never contain internals: no stack traces, SQL, file paths or secrets.

## Request pipeline (end of Week 3)

```
Every HTTP request:
  client
    → Api: CorrelationIdMiddleware → request logging → exception handler → status code pages
    → UseAuthentication (session cookie decrypted; OnValidatePrincipal revalidates against the
      database, 60 s cache — docs/architecture/authentication.md)
    → ActorMiddleware (claims → StaffActor; the one place claims become an actor)
    → UseAuthorization (authenticated by default; six routes opt out with .AllowAnonymous())
    → UseRateLimiter (the "login" policy only — POST /api/auth/login)
    → endpoint routing
    → AntiforgeryEndpointFilter (whole /api group; unsafe methods only — rejects before the
      handler runs, no side effects)

Command (HTTP POST/PUT/DELETE, CLI, jobs):
    → endpoint (bind only)
    → Dispatcher.SendAsync<TCommand, TResponse>
        1. validate (FluentValidation: shape only)            failure → Result.Failure(validation), nothing opened
        2. CheckTenantScope — only for an ITenantScoped command (tenancy.md, Layer 1):
             no actor            → Error.Unauthenticated("auth.not_authenticated")   401, nothing opened
             actor has no centre → Error.Forbidden("tenant.not_selected")            403, nothing opened
             [Day 28: permission check plugs in here too, still before BeginAsync]
        3. IUnitOfWork.BeginAsync(readOnly: false)             sets app.current_centre inside the transaction (tenancy.md, Layer 3)
        4. handler: authorize → Domain rules → repository tracks changes (no SaveChanges)
        5. failure result → Rollback;  success → SaveChanges once → Commit
           exception → Rollback (CancellationToken.None) → rethrow → global exception handler (generic 500, logged once)
           — this is also the failed-save path for a tenant violation that reaches SaveChanges: EF's
             own composite-key immutability (tenancy.md, Layer 4) or, failing that,
             TenantWriteGuardInterceptor throwing TenantViolationException (tenancy.md, Layer 2) —
             neither is expected to fire given step 2 and the query filter, but both are a last-resort
             exception, not a Result failure, because a request should never have reached them at all
    → Result → ResultHttpExtensions → JSON or Problem Details

Query (HTTP GET, reads):
    → Dispatcher.QueryAsync<TQuery, TResponse>
        1. validate
        2. CheckTenantScope — same rule and errors as the command pipeline's step 2
        3. IUnitOfWork.BeginAsync(readOnly: true)             PostgreSQL: SET TRANSACTION READ ONLY, then app.current_centre
        4. handler: read service → DTOs (no entities, no repositories) — rows outside the actor's
             centre are already invisible, filtered by EF's global query filter (tenancy.md, Layer 2)
             and, independently, by PostgreSQL row-level security (tenancy.md, Layer 3)
        5. Commit (never SaveChanges)
    → Result → ResultHttpExtensions
```

**Why validation runs before BEGIN.** Invalid input needs no database work, so no transaction (and no connection) is opened for it.

**Why the transaction starts before the handler, not just around SaveChanges.** Repositories will take row locks (`FOR UPDATE`) while the handler reads, and `UnitOfWork.BeginAsync` sets the PostgreSQL row-level-security tenant inside the transaction (tenancy.md, Layer 3); both must cover the whole handler, not just the save.

**What happens when a query handler throws.** End the read-only transaction (roll back) and rethrow; the global handler returns 500.

**Who calls SaveChanges.** Only the command pipeline, exactly once, after a successful handler; queries never call it.

**Where tenant and permission checks go.** `Dispatcher.CheckTenantScope`, after validation and before `BeginAsync`, in both pipelines (Day 22 — see tenancy.md, Layer 1). It is a no-op for a request that does not implement `ITenantScoped`; Month 1's requests never do. The permission step (role-based authorization) is marked directly after it, in the same place, from Day 28. Idempotency for marked commands comes later (Month 6).

## Request path: `CreateCentreCommand` (CLI → table)

| Step | What happens | File |
| --- | --- | --- |
| 1 | `dotnet run --project src/TutoringCentre.Api -- seed` starts the host; `args is ["seed"]` | `src/TutoringCentre.Api/Program.cs` |
| 2 | Migrations are applied; per centre a scope is created and the actor is set to `SystemActor(null)` | `src/TutoringCentre.Api/Cli/SeedCommand.cs` |
| 3 | `Dispatcher.SendAsync<CreateCentreCommand, CreateCentreResult>` | `src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs` |
| 4 | Shape validation (no database work) | `.../Centres/Commands/CreateCentre/CreateCentreValidator.cs` |
| 5 | Read-write transaction begins | `src/TutoringCentre.Infrastructure/Persistence/UnitOfWork.cs` |
| 6 | Handler: actor must be `SystemActor`, else `centre.create_forbidden` | `.../CreateCentre/CreateCentreHandler.cs` |
| 7 | Handler: slug uniqueness via the repository (`centre.slug_taken`) | `src/TutoringCentre.Infrastructure/Repositories/CentreRepository.cs` |
| 8 | Domain rules and construction (UUIDv7 id) | `src/TutoringCentre.Domain/Centres/Centre.cs` |
| 9 | Repository `Add` tracks the entity — nothing written yet | `CentreRepository.cs` |
| 10 | Dispatcher saves once: `TimestampInterceptor` stamps `created_at`, `INSERT INTO platform.centres` | `.../Persistence/Interceptors/TimestampInterceptor.cs`, `.../Configurations/Centres/CentreConfiguration.cs` |
| 11 | Commit; `Result<CreateCentreResult>` returns to the CLI | `UnitOfWork.cs` |

There is no HTTP endpoint for this command — on purpose.

## Request path: `GET /api/system/info` (HTTP → table)

| Step | What happens | File |
| --- | --- | --- |
| 1 | Correlation ID resolved or generated; response header and log context set | `src/TutoringCentre.Api/Http/CorrelationIdMiddleware.cs` |
| 2 | Endpoint binds nothing, dispatches the query, maps the result | `src/TutoringCentre.Api/Endpoints/PlatformEndpoints.cs` |
| 3 | Query pipeline: validation, read-only transaction | `src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs`, `src/TutoringCentre.Infrastructure/Persistence/UnitOfWork.cs` |
| 4 | Handler reads version and calls the read service | `src/TutoringCentre.Application/Platform/Queries/GetSystemInfo/GetSystemInfoHandler.cs` |
| 5 | Read service asks EF for applied and pending migrations | `src/TutoringCentre.Infrastructure/ReadServices/SystemInfoReadService.cs` |
| 6 | `Result<SystemInfoDto>` becomes 200 JSON (or Problem Details) | `src/TutoringCentre.Api/Http/ResultHttpExtensions.cs` |
| 7 | Frontend: generated hook → `apiFetch` → `SystemInfoCard` | `frontend/src/api/generated/tutoring-centre.ts`, `frontend/src/api/apiFetch.ts`, `frontend/src/features/status/SystemInfoCard.tsx` |

## Decisions and conventions

- [ADR 0001 — Clean Architecture](../adr/0001-clean-architecture.md)
- [ADR 0002 — .NET and React](../adr/0002-dotnet-react.md)
- [ADR 0003 — Hand-written CQRS dispatcher](../adr/0003-custom-cqrs-dispatcher.md)
- [ADR 0004 — Unit-of-work port](../adr/0004-unit-of-work-port.md)
- [ADR 0005 — Encrypted cookie authentication over JWT](../adr/0005-cookie-authentication.md)
- [ADR 0006 — Tenant isolation as defense in depth (draft)](../adr/0006-tenant-isolation.md)
- [API conventions](api-conventions.md)
- [Authentication: login, sessions, CSRF, revocation](authentication.md)
- [Tenant isolation: the role model, and layers 1–4 as they land](tenancy.md)
