# Reading order for the file explanations

A path through the 118 explanation files, ordered so each step only relies on things you have already read. Every link opens that file's explanation. Open the real source file side by side while you read.

**Total time:** roughly 6–8 hours, in 9 stages. Each stage ends with checkpoint questions. If you can answer them, move on.

**You can skip on a first pass:** the five test `.csproj` files, `.gitattributes`, `.gitignore`, `frontend/.gitignore`, and the four `*.semantic.md` files. They are covered in stage 9.

---

## Stage 0 — The big picture (≈45 min)

Start with what the project is and why it is shaped this way. Read nothing else until these make sense.

1. [README.md](README.md.md): what the product is, how to run it.
2. [../PROJECT_OVERVIEW.md](../PROJECT_OVERVIEW.md), §1–§3 only: current state and architecture diagram.
3. [docs/adr/0001-clean-architecture.md](docs/adr/0001-clean-architecture.md.md): why four projects and inward-only references.
4. [docs/adr/0002-dotnet-react.md](docs/adr/0002-dotnet-react.md.md): why .NET plus React, and the same-origin plan.

**Checkpoint:**
- What are the "three front doors", and why do they force this architecture?
- Which project may reference which?
- Why may the Api reference Infrastructure, and only for what?

---

## Stage 1 — Build setup: how the solution is put together (≈30 min)

1. [TutoringCentre.slnx](TutoringCentre.slnx.md): the 9 projects.
2. [global.json](global.json.md): the pinned SDK.
3. [Directory.Build.props](Directory.Build.props.md): settings every project inherits. Note `TreatWarningsAsErrors`; it explains every `[SuppressMessage]` you'll see later.
4. [Directory.Packages.props](Directory.Packages.props.md): every NuGet version in one place.
5. [.editorconfig](.editorconfig.md): style rules, and why tests may use underscores.
6. The four src project files, inner to outer:
   [Domain](src/TutoringCentre.Domain/TutoringCentre.Domain.csproj.md) →
   [Application](src/TutoringCentre.Application/TutoringCentre.Application.csproj.md) →
   [Infrastructure](src/TutoringCentre.Infrastructure/TutoringCentre.Infrastructure.csproj.md) →
   [Api](src/TutoringCentre.Api/TutoringCentre.Api.csproj.md)
7. [AssemblyMarker.cs (Domain)](src/TutoringCentre.Domain/AssemblyMarker.cs.md). Read one; the other three are identical.

**Checkpoint:**
- Why does no `.csproj` set `TargetFramework` or package versions?
- What packages does Application have, and why only *abstractions*?

---

## Stage 2 — Domain: the business core (≈1 h)

Read the failure model first, then the building blocks, then the first real entity.

1. [docs/architecture/overview.md](docs/architecture/overview.md.md), top half ("Failures"): the rules the next files implement.
2. [Common/ErrorKind.cs](src/TutoringCentre.Domain/Common/ErrorKind.cs.md)
3. [Common/Error.cs](src/TutoringCentre.Domain/Common/Error.cs.md)
4. [Common/Result.cs](src/TutoringCentre.Domain/Common/Result.cs.md)
5. [Common/Entity.cs](src/TutoringCentre.Domain/Common/Entity.cs.md)
6. [Centres/SupportedLocale.cs](src/TutoringCentre.Domain/Centres/SupportedLocale.cs.md)
7. [Centres/Centre.cs](src/TutoringCentre.Domain/Centres/Centre.cs.md). This is the most important file in this stage, because it uses everything above.
8. [Common/ITenantOwned.cs](src/TutoringCentre.Domain/Common/ITenantOwned.cs.md): a placeholder for later.

Then read the tests, which double as executable examples:

9. [ResultTests.cs](tests/TutoringCentre.Domain.Tests/Common/ResultTests.cs.md) → [ErrorTests.cs](tests/TutoringCentre.Domain.Tests/Common/ErrorTests.cs.md) → [EntityTests.cs](tests/TutoringCentre.Domain.Tests/Common/EntityTests.cs.md) → [CentreTests.cs](tests/TutoringCentre.Domain.Tests/Centres/CentreTests.cs.md)

**Checkpoint:**
- When is a failure a `Result` and when is it an exception?
- What happens if you read `.Value` of a failed result?
- List the four checks in `Centre.Create`, in order.
- Why `\z` instead of `$` in the slug regex?

---

## Stage 3 — Application: the use-case pipeline (≈1.5 h)

This is the heart of the backend. Contracts first, then ports, then the dispatcher that ties them together.

1. CQRS contracts:
   - [ICommand.cs](src/TutoringCentre.Application/Common/Cqrs/ICommand.cs.md)
   - [ICommandHandler.cs](src/TutoringCentre.Application/Common/Cqrs/ICommandHandler.cs.md)
   - [IQuery.cs](src/TutoringCentre.Application/Common/Cqrs/IQuery.cs.md)
   - [IQueryHandler.cs](src/TutoringCentre.Application/Common/Cqrs/IQueryHandler.cs.md)
   - [Unit.cs](src/TutoringCentre.Application/Common/Cqrs/Unit.cs.md)
2. Ports:
   - [IUnitOfWork.cs](src/TutoringCentre.Application/Common/Ports/IUnitOfWork.cs.md)
   - [IClock.cs](src/TutoringCentre.Application/Common/Ports/IClock.cs.md)
3. [docs/architecture/overview.md](docs/architecture/overview.md.md), bottom half ("Dispatcher design").
4. [Dispatcher.cs](src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs.md). Read this one slowly, twice.
5. [HandlerRegistration.cs](src/TutoringCentre.Application/Common/Cqrs/HandlerRegistration.cs.md)
6. Security:
   - [Actor.cs](src/TutoringCentre.Application/Common/Security/Actor.cs.md)
   - [ICurrentActor.cs](src/TutoringCentre.Application/Common/Security/ICurrentActor.cs.md)
   - [CurrentActorContext.cs](src/TutoringCentre.Application/Common/Security/CurrentActorContext.cs.md)
7. [DependencyInjection.cs (Application)](src/TutoringCentre.Application/DependencyInjection.cs.md): how all of the above is registered.
8. [AssemblyMarker.cs (Application)](src/TutoringCentre.Application/AssemblyMarker.cs.md): note that it is also used for handler scanning.

Then the tests, which prove the pipeline:

9. [docs/notes/pipeline-cases.md](docs/notes/pipeline-cases.md.md): the spec, cases C1–C9.
10. [FakeUnitOfWork.cs](tests/TutoringCentre.Application.Tests/Fakes/FakeUnitOfWork.cs.md) → [TestRequests.cs](tests/TutoringCentre.Application.Tests/Cqrs/TestRequests.cs.md) → [DispatcherTests.cs](tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs.md)
11. [CurrentActorContextTests.cs](tests/TutoringCentre.Application.Tests/Security/CurrentActorContextTests.cs.md)

**Checkpoint:**
- Trace `SendAsync` for: an invalid command, a handler failure, a handler exception, and a success. Which unit-of-work calls happen in each?
- Why does validation run before `BeginAsync`?
- Why does rollback use `CancellationToken.None`?
- Where will tenant checks go?
- Why can handlers never set the actor?

---

## Stage 4 — Infrastructure: the adapters (≈1 h)

How the ports from stage 3 are implemented with EF Core, PostgreSQL and the system clock.

1. [Time/SystemClock.cs](src/TutoringCentre.Infrastructure/Time/SystemClock.cs.md) → [SystemClockTests.cs](tests/TutoringCentre.Infrastructure.Tests/Time/SystemClockTests.cs.md). Start here because it is the simplest adapter.
2. [Persistence/Schemas.cs](src/TutoringCentre.Infrastructure/Persistence/Schemas.cs.md)
3. [Persistence/AppDbContext.cs](src/TutoringCentre.Infrastructure/Persistence/AppDbContext.cs.md)
4. [Persistence/UnitOfWork.cs](src/TutoringCentre.Infrastructure/Persistence/UnitOfWork.cs.md). Compare it line by line with `IUnitOfWork` and the `Dispatcher`.
5. [Persistence/Interceptors/TimestampInterceptor.cs](src/TutoringCentre.Infrastructure/Persistence/Interceptors/TimestampInterceptor.cs.md)
6. [DependencyInjection.cs (Infrastructure)](src/TutoringCentre.Infrastructure/DependencyInjection.cs.md): wires everything, including the health check.
7. [AssemblyMarker.cs (Infrastructure)](src/TutoringCentre.Infrastructure/AssemblyMarker.cs.md)
8. [.config/dotnet-tools.json](.config/dotnet-tools.json.md): the EF CLI for future migrations.

**Checkpoint:**
- How does a query end up in a PostgreSQL read-only transaction?
- What does `RollbackAsync` clean up?
- What happens at startup when `ConnectionStrings:Postgres` is empty, and what happens at the first database call?
- Why does the EF model currently have zero entities?

---

## Stage 5 — Api: the host and how it starts (≈30 min)

1. [Program.cs](src/TutoringCentre.Api/Program.cs.md): read it with the startup sequence diagram in [PROJECT_OVERVIEW §3](../PROJECT_OVERVIEW.md).
2. [appsettings.json](src/TutoringCentre.Api/appsettings.json.md) → [appsettings.Development.json](src/TutoringCentre.Api/appsettings.Development.json.md)
3. [Properties/launchSettings.json](src/TutoringCentre.Api/Properties/launchSettings.json.md)
4. [AssemblyMarker.cs (Api)](src/TutoringCentre.Api/AssemblyMarker.cs.md)
5. [HealthEndpointTests.cs](tests/TutoringCentre.Api.Tests/Health/HealthEndpointTests.cs.md)

**Checkpoint:**
- Difference between `/health` and `/health/ready`, and why liveness runs zero checks?
- Why `public partial class Program`?
- Why does the readiness test use port 1 instead of an empty string?

---

## Stage 6 — Architecture tests: how the rules are enforced (≈30 min)

Now that you know the layers, see how they are policed.

1. [ProjectFiles.cs](tests/TutoringCentre.Architecture.Tests/ProjectFiles.cs.md)
2. [ProjectReferenceTests.cs](tests/TutoringCentre.Architecture.Tests/ProjectReferenceTests.cs.md)
3. [DependencyRuleTests.cs](tests/TutoringCentre.Architecture.Tests/DependencyRuleTests.cs.md)
4. [tests/.semantic.md](tests/.semantic.md.md): its counts are stale, but its "why two techniques" section and the experiments are still the best explanation.

**Checkpoint:**
- Which kind of violation does each test catch that the other can't?
- What doesn't either test catch, for example the Api project?

---

## Stage 7 — Frontend (≈1.5 h)

Follow the same order the app boots in, then the one feature, then the shared pieces.

1. [frontend/README.md](frontend/README.md.md): the conventions. Read them first; the code follows them.
2. Tooling:
   - [package.json](frontend/package.json.md)
   - [vite.config.ts](frontend/vite.config.ts.md): note the dev proxy.
   - [tsconfig.json](frontend/tsconfig.json.md) → [tsconfig.app.json](frontend/tsconfig.app.json.md) → [tsconfig.node.json](frontend/tsconfig.node.json.md)
   - [eslint.config.js](frontend/eslint.config.js.md)
3. Boot path:
   - [index.html](frontend/index.html.md)
   - [src/main.tsx](frontend/src/main.tsx.md)
   - [src/app/queryClient.ts](frontend/src/app/queryClient.ts.md)
   - [src/routes/__root.tsx](frontend/src/routes/__root.tsx.md)
   - [src/routes/index.tsx](frontend/src/routes/index.tsx.md)
4. The status feature, network to screen:
   - [api.ts](frontend/src/features/status/api.ts.md)
   - [useReadiness.ts](frontend/src/features/status/useReadiness.ts.md)
   - [StatusCard.tsx](frontend/src/features/status/StatusCard.tsx.md)
5. The test setup, then the feature test:
   - [setup.ts](frontend/src/test/setup.ts.md)
   - [msw/handlers.ts](frontend/src/test/msw/handlers.ts.md)
   - [msw/server.ts](frontend/src/test/msw/server.ts.md)
   - [render.tsx](frontend/src/test/render.tsx.md)
   - [StatusCard.test.tsx](frontend/src/features/status/StatusCard.test.tsx.md)
6. The shared error model, prepared for future API calls:
   - [errors.ts](frontend/src/api/errors.ts.md)
   - [problemDetails.ts](frontend/src/api/problemDetails.ts.md) → [problemDetails.test.ts](frontend/src/api/problemDetails.test.ts.md)
   - [errorMessages.ts](frontend/src/api/errorMessages.ts.md) → [errorMessages.test.ts](frontend/src/api/errorMessages.test.ts.md)
7. Styling and UI kit. Skim these; they are mostly generated by shadcn:
   - [components.json](frontend/components.json.md)
   - [index.css](frontend/src/index.css.md)
   - [button-variants.ts](frontend/src/components/ui/button-variants.ts.md) → [button.tsx](frontend/src/components/ui/button.tsx.md)
   - [alert.tsx](frontend/src/components/ui/alert.tsx.md)
   - [card.tsx](frontend/src/components/ui/card.tsx.md)
   - [skeleton.tsx](frontend/src/components/ui/skeleton.tsx.md)
   - [sonner.tsx](frontend/src/components/ui/sonner.tsx.md)
   - [lib/utils.ts](frontend/src/lib/utils.ts.md)
   - [.prettierrc](frontend/.prettierrc.md)
   - [.prettierignore](frontend/.prettierignore.md)

**Checkpoint:**
- Trace a click on Retry from `StatusCard` to the API and back.
- Why is a 503 *data* but a network failure a *thrown error*?
- How do tests guarantee no real network call happens?
- How will `messageFor` turn a backend error code into Arabic text?

---

## Stage 8 — Local dev and CI (≈30 min)

1. [.env.example](.env.example.md) → [compose.yaml](compose.yaml.md): the local database.
2. [.github/workflows/ci.yml](.github/workflows/ci.yml.md)
3. [.github/workflows/codeql.yml](.github/workflows/codeql.yml.md)
4. [.github/workflows/secret-scan.yml](.github/workflows/secret-scan.yml.md)
5. [.github/dependabot.yml](.github/dependabot.yml.md)
6. [later.md](later.md.md): what was deliberately postponed.

Now actually run the project with the README quick start, and open `/health/ready` and the status page.

**Checkpoint:**
- What must pass before a PR can merge?
- Where does the database password live, and where does the connection string live?

---

## Stage 9 — Leftovers and reflection (≈30 min)

1. The test project files. Read one; the rest differ by a single package:
   [Domain.Tests.csproj](tests/TutoringCentre.Domain.Tests/TutoringCentre.Domain.Tests.csproj.md)
   ([Application](tests/TutoringCentre.Application.Tests/TutoringCentre.Application.Tests.csproj.md),
   [Infrastructure](tests/TutoringCentre.Infrastructure.Tests/TutoringCentre.Infrastructure.Tests.csproj.md),
   [Api](tests/TutoringCentre.Api.Tests/TutoringCentre.Api.Tests.csproj.md),
   [Architecture](tests/TutoringCentre.Architecture.Tests/TutoringCentre.Architecture.Tests.csproj.md))
2. Repo hygiene:
   - [.gitattributes](.gitattributes.md)
   - [.gitignore](.gitignore.md)
   - [frontend/.gitignore](frontend/.gitignore.md)
3. The generated, stale docs. Read them only to see how the project evolved:
   - [.semantic-manifest.json](.semantic-manifest.json.md)
   - [OVERVIEW.semantic.md](OVERVIEW.semantic.md.md)
   - [ARCHITECTURE.semantic.md](ARCHITECTURE.semantic.md.md)
   - [src/.semantic.md](src/.semantic.md.md)
4. [../PROJECT_OVERVIEW.md](../PROJECT_OVERVIEW.md) §4–§10: re-read the concepts, quality assessment and glossary. They will mean much more now.

**Final self-test:** explain out loud how a future "Create centre" request would flow. It goes from a frontend form, through the API endpoint, the `Dispatcher` (validation, then the tenant check, then `BeginAsync`), the handler and `Centre.Create`, then `SaveChanges`, the `TimestampInterceptor` and `Commit`, and back as either a success or an error code that `messageFor` translates. If you can do that, you understand the project.

---

## Tips while reading

- Keep [INDEX.md](INDEX.md) open to jump to any file.
- Each explanation has a **Related files** section. Use it when you want to follow a thread, but come back to this order afterwards.
- Read every **Gotchas and issues** section; together they are your to-do list for future work.
