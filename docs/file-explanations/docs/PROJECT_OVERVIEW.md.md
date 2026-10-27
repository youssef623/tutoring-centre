# `docs/PROJECT_OVERVIEW.md`

## Purpose

Documentation covering Tutoring Centre Manager — Project Overview; 1. Project Overview; 1.1 What the application is (and is not yet); 1.2 Implementation state, feature by feature; 1.3 What I ran and what I could not run. Treat historical claims according to the revision described in the file.

## Where it fits

Category: **documentation**. [Open current file](../../PROJECT_OVERVIEW.md). [Complete file index](../INDEX.md). [Current teaching overview](../../PROJECT_OVERVIEW3.md).

The reviewed file has 2634 lines and 245,622 bytes. Source hash: `bcf0aa13e8e3c585911b804d119e101f8f8bf9af35bb12f7f763ae59647fbd71`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 306](../../PROJECT_OVERVIEW.md#L306): `13. public partial class Program; at the bottom exposes the compiler-generated Program class so WebApplicationFactory<Program> can reference it.`
- [Line 388](../../PROJECT_OVERVIEW.md#L388): `internal sealed class CreateCentreHandler(ICurrentActor currentActor, ICentreRepository centres)`
- [Line 446](../../PROJECT_OVERVIEW.md#L446): `- "Interface" does not mean "abstraction for its own sake" here: every interface in Application exists because an outer layer must implement it.`
- [Line 447](../../PROJECT_OVERVIEW.md#L447): `- Putting an interface in the *wrong project* silently defeats the idea (an ICentreRepository inside Infrastructure would make Application depend on Infrastructure to use it).`
- [Line 454](../../PROJECT_OVERVIEW.md#L454): `**Dependency Injection (DI)** is the practice of a class *receiving* its collaborators (usually through its constructor) instead of creating them. A **DI container** (IServiceCollection → IServiceProvider in .NET) holds **registrations** (a map "when someone asks for type X, build type Y") and performs **resolution** (building the object graph by recursively satisfying constructor parameters). **Lifetime** says how long a built instance lives: **singleton** (one for the whole app), **scoped** (one per scope — in ASP.NET one per HTTP request), **transient** (new each time). **Inversion of control**: the class no longer controls what it depends on; the container does.`
- [Line 479](../../PROJECT_OVERVIEW.md#L479): `**Two registrations for one object (ICurrentActor).** AddScoped<ICurrentActor>(provider => provider.GetRequiredService<CurrentActorContext>()) is a *factory registration*: when something asks for ICurrentActor, the container *resolves the concrete CurrentActorContext* and returns that same scoped instance. Result: edge code (seed, tests) asks for CurrentActorContext and calls Set(...); handlers ask for ICurrentActor and can only read. One object, two views, enforced by interface segregation.`
- [Line 481](../../PROJECT_OVERVIEW.md#L481): `**Assembly scanning (HandlerRegistration.AddCqrsHandlers).** It calls assembly.GetTypes(), keeps concrete non-generic classes, and for each implemented interface that is a closed ICommandHandler<,> or IQueryHandler<,> registers AddScoped(handlerInterface, implementation). So CreateCentreHandler (an internal class) becomes the answer to ICommandHandler<CreateCentreCommand, CreateCentreResult>. Adding a new handler class requires **no registration code** — the scan finds it (verified: no explicit registration of CreateCentreHandler exists anywhere).`
- [Line 496](../../PROJECT_OVERVIEW.md#L496): `- A handler must be a concrete class implementing a closed generic handler interface, or the scan will not register it.`
- [Line 502](../../PROJECT_OVERVIEW.md#L502): `Think of a restaurant kitchen. A chef (your class) needs ingredients (dependencies). Either the chef drives to the market every time (class creates its own dependencies with new), or a **supplier delivers exactly what the recipe lists** (the container injects them). The chef never needs to know which farm the tomatoes came from.`
- [Line 504](../../PROJECT_OVERVIEW.md#L504): `- **Dependency**: another object your class needs (ICentreRepository for CreateCentreHandler).`
- [Line 505](../../PROJECT_OVERVIEW.md#L505): `- **Injection**: the dependency is *handed to* the class from outside - here through the constructor.`
- [Line 506](../../PROJECT_OVERVIEW.md#L506): `- **Inversion of control (IoC)**: normally the class controls its collaborators; with DI, control is inverted - something else (the container) decides.`
- [Line 515](../../PROJECT_OVERVIEW.md#L515): `internal sealed class CreateCentreHandler(ICurrentActor currentActor, ICentreRepository centres)`
- [Line 563](../../PROJECT_OVERVIEW.md#L563): `Constructor parameters make dependencies **mandatory and visible**: an object cannot exist half-configured, and a quick glance at the constructor shows what the class needs. Property injection hides requirements; method injection suits per-call data.`
- [Line 662](../../PROJECT_OVERVIEW.md#L662): `SendAsync<TCommand, TResponse>(command) is a *generic method*. For CreateCentreCommand the compiler fills in TCommand = CreateCentreCommand, TResponse = CreateCentreResult. The dispatcher then asks the container for the **closed generic type** ICommandHandler<CreateCentreCommand, CreateCentreResult>. The container returns the class registered for exactly that type (CreateCentreHandler, registered by the scan). The dispatcher has never heard of centres; it only knows the *shape* "a command has a handler for its type". That is why adding a use case requires **zero** dispatcher changes.`
- [Line 826](../../PROJECT_OVERVIEW.md#L826): `- **Shape validation** (cheap, no business knowledge, no I/O): required, max length, enum defined. Done by **FluentValidation** in CreateCentreValidator, run by the dispatcher *before* any transaction.`
- [Line 852](../../PROJECT_OVERVIEW.md#L852): `| **Shape** | name not empty, at most 120 chars; locale value is a defined enum member | no | CreateCentreValidator (FluentValidation) |`
- [Line 1088](../../PROJECT_OVERVIEW.md#L1088): `- Each new query needs a new interface method and implementation.`
- [Line 1117](../../PROJECT_OVERVIEW.md#L1117): `| DefaultLocale HasConversion(ValueConverter<SupportedLocale,string>) | enum stored as 'ar'/'en' (varchar(2)); converter throws on unknown values |`
- [Line 1176](../../PROJECT_OVERVIEW.md#L1176): `2. dotnet ef migrations add Name compares the **current model** with the **model snapshot** (AppDbContextModelSnapshot.cs) and writes a new migration class with Up (apply) and Down (undo) plus an updated snapshot.`
- [Line 1265](../../PROJECT_OVERVIEW.md#L1265): `- Quirk to know: SystemActor declares public override Guid? CentreId { get; } = CentreId; — the positional record parameter and the overriding property share a name; this is how the record satisfies the abstract property. SystemActor(null) means "platform-wide".`
- [Line 1390](../../PROJECT_OVERVIEW.md#L1390): `Because one class sets content type, status, traceId and correlationId, a test (ProblemDetailsTests) can assert the shape once and trust it everywhere.`
- [Line 1396](../../PROJECT_OVERVIEW.md#L1396): `An exception message can contain connection strings, SQL, file paths, or internal names (a classic *information leakage*). GlobalExceptionHandler logs the full exception **server-side only** and answers with a generic message. ProblemDetailsTests.UnhandledException_Returns500WithoutLeakingInternals plants Password=hunter2 in an exception message and asserts it never appears in the response.`
- [Line 1408](../../PROJECT_OVERVIEW.md#L1408): `Different failure paths (validation, exceptions, unknown routes, malformed JSON) would answer in different shapes, X-Correlation-Id would be missing on some, and the frontend's single messageFor(error) function could not exist.`
- [Line 1475](../../PROJECT_OVERVIEW.md#L1475): `Support would have no way to connect a client's error with server logs; the 500 handler would have nowhere to record the real exception.`
- [Line 1482](../../PROJECT_OVERVIEW.md#L1482): `The **options pattern** binds configuration to a typed class and can validate it. **ValidateOnStart** runs validation when the host starts rather than at first use (fail fast). **Health checks** are endpoints used by orchestrators: **liveness** ("is the process alive?") vs **readiness** ("can it serve traffic, i.e., are dependencies up?").`
- [Line 1541](../../PROJECT_OVERVIEW.md#L1541): `3. **Assembly handles:** each project has internal sealed class AssemblyMarker; and an InternalsVisibleTo for TutoringCentre.Architecture.Tests, so tests can write typeof(TutoringCentre.Domain.AssemblyMarker).Assembly — compile-time-checked, unlike Assembly.Load("string").`
- [Line 1647](../../PROJECT_OVERVIEW.md#L1647): `- A **new instance of the test class is created per test method**, so tests do not share fields. Anything expensive is therefore placed in a **fixture** shared via ICollectionFixture<T> + [Collection("postgres")]. Tests in one collection run **sequentially**, which is required because they share one database.`
- [Line 1677](../../PROJECT_OVERVIEW.md#L1677): `A **component** is a function returning UI. **Props** are its inputs. **State** is data React remembers between renders; changing it makes React call the component function again (**re-render**) and update the DOM with the difference. A **hook** (useQuery) is a function that plugs a component into React features. **Server state** (data owned by the API) differs from UI state: it needs caching, refetching, loading/error states — **TanStack Query** manages that.`
- [Line 1681](../../PROJECT_OVERVIEW.md#L1681): `2. main.tsx creates the router (createRouter({ routeTree })), registers its type via module augmentation (declare module "@tanstack/react-router" { interface Register … } so links are type-checked), throws if #root is missing, and renders <StrictMode><QueryClientProvider client={queryClient}><RouterProvider router={router}/></QueryClientProvider></StrictMode>. StrictMode intentionally double-invokes some functions in development to expose impurities.`
- [Line 1688](../../PROJECT_OVERVIEW.md#L1688): `9. **What causes a re-render:** the observer created by useQuery subscribes the component to the query cache entry. When the 15-second timer fires refetch, or Retry's onClick calls readiness.refetch(), the query's state changes (fetching → success/error); the observer notifies React, which schedules a render of StatusCard; the function runs again, reads the new readiness value, and the returned JSX differs (e.g., a new checkedAt Date) so the DOM text updates.`
- [Line 1704](../../PROJECT_OVERVIEW.md#L1704): `- **JSX** is syntax that looks like HTML inside TypeScript; it compiles to function calls that create a description of the UI (a tree of "elements"). Nothing touches the browser DOM yet.`
- [Line 1705](../../PROJECT_OVERVIEW.md#L1705): `- A **component** is a function that takes **props** (inputs) and returns that description. React calls it ("**render**") whenever it needs to know what the UI should look like.`
- [Line 1706](../../PROJECT_OVERVIEW.md#L1706): `- **State** is data React remembers for a component between renders (useState). **Calling a state setter does not change the variable immediately**; it tells React "something changed, schedule another render". On that render React calls your function again, this time returning the new state value.`
- [Line 1709](../../PROJECT_OVERVIEW.md#L1709): `- **StrictMode** (in main.tsx) deliberately renders components twice in development to expose code that is not a pure function of its inputs. It does nothing in production.`
- [Line 1711](../../PROJECT_OVERVIEW.md#L1711): `This repo has **no useState** and no useEffect. All changing data comes from useQuery, and StatusCard is a pure function of the query result - the preferred style for server data.`
- [Line 1738](../../PROJECT_OVERVIEW.md#L1738): `- **Type predicate** value is ProblemDetails (isProblemDetails): a function that proves a type to the compiler after a runtime check - the safe way to read untrusted JSON.`
- [Line 1740](../../PROJECT_OVERVIEW.md#L1740): `- **Module augmentation** (declare module "@tanstack/react-router" { interface Register ... }): tells the router library about *your* route tree so links are type-checked.`
- [Line 1747](../../PROJECT_OVERVIEW.md#L1747): `**Utility-first CSS (Tailwind)**: instead of writing .card {...} you compose small classes (w-full max-w-md). **shadcn/ui** copies component source into src/components/ui, so you own it. **cva** maps a prop like variant="outline" to a class string. **Logical properties** (ms-, me-) mean "margin at the start/end of the line", which flips automatically for right-to-left Arabic, unlike ml-/mr-.`
- [Line 1770](../../PROJECT_OVERVIEW.md#L1770): `- **Implementation**: a class that fulfils a contract (CentreRepository, FakeCentreRepository).`
- [Line 1776](../../PROJECT_OVERVIEW.md#L1776): `Polymorphism through interfaces is the mechanism behind almost everything: DI (the container returns *an implementation* of the interface you ask for), testing (swap fakes), and architecture (inner layers own interfaces; outer layers implement them).`
- [Line 1785](../../PROJECT_OVERVIEW.md#L1785): `| sealed | almost every class is sealed (cannot be inherited): the design says "extend by composition, not by subclassing", and sealing lets the runtime optimise calls |`
- [Line 1790](../../PROJECT_OVERVIEW.md#L1790): `Advantages: swap behaviour without changing callers; enforce dependency direction; make tests easy. Disadvantages: indirection (you must find the implementation); an interface with exactly one production implementation can feel like ceremony - here justified because a second implementation always exists in tests and because of the layering rule.`
- [Line 1793](../../PROJECT_OVERVIEW.md#L1793): `Concrete classes everywhere (simple, but untestable and tightly coupled); delegates/function parameters instead of one-method interfaces; inheritance-based template methods (more rigid).`
- [Line 1796](../../PROJECT_OVERVIEW.md#L1796): `Add new capabilities by defining an interface in the layer that needs it, and implement it in the outer layer. Prefer composition; the only inheritance hierarchies are small and intentional.`
- [Line 1812](../../PROJECT_OVERVIEW.md#L1812): `public async Task<Result<CreateCentreResult>> HandleAsync(CreateCentreCommand command, CancellationToken cancellationToken)`
- [Line 1846](../../PROJECT_OVERVIEW.md#L1846): `A **record** (public sealed record Error(...)) is a class with auto-generated value equality, ToString, deconstruction and with-copy. **Immutable** means "cannot change after creation": records' positional properties are init-only. Immutability makes messages (commands, results, errors) safe to pass around and compare, and it is why Assert.Equal(TestErrors.HandlerFailure, result.Error) works. readonly record struct Unit is the value-type form.`
- [Line 1862](../../PROJECT_OVERVIEW.md#L1862): `Primary constructors (class CentreRepository(AppDbContext db)), collection expressions ([], ["ready"]), file-scoped namespaces (namespace X;), top-level statements (Program.cs), partial classes (needed by [GeneratedRegex] and [LoggerMessage]), extension methods (AddApplication(this IServiceCollection ...)) that let layers add themselves to the container fluently.`
- [Line 1997](../../PROJECT_OVERVIEW.md#L1997): `| 2 | UseSerilogRequestLogging | After #1 so its single completion line carries CorrelationId (CorrelationId_AppearsInRequestLogLine). **Before** the exception handler so it wraps it and records the *final* status (a handled exception turns into a 500, and the level function logs Error for >= 500). Health paths log at Verbose. |`
- [Line 2015](../../PROJECT_OVERVIEW.md#L2015): `Every HTTP request must pass through common chores: assign an id, log, catch exceptions, check identity. Instead of writing these in each endpoint, ASP.NET Core arranges them as a **chain of small components**. Each component is a function of the form "receive the request context and a handle to *the rest of the chain* (next); do something; optionally call next; do something after next returns". The framework type for that handle is RequestDelegate.`
- [Line 2019](../../PROJECT_OVERVIEW.md#L2019): `public async Task InvokeAsync(HttpContext context)`
- [Line 2060](../../PROJECT_OVERVIEW.md#L2060): `GetSystemInfoAsync(Dispatcher dispatcher, CancellationToken ct): neither parameter appears in the route or body. The framework recognises Dispatcher as a registered service (resolved from the request-scope container) and CancellationToken as the request-aborted token. Had the signature included a record type with no registration, it would have been read from the JSON body.`
- [Line 2163](../../PROJECT_OVERVIEW.md#L2163): `Five real actions are traced end to end. Each names the exact file and function at every stage and shows what the data looks like as it moves. File explanations are linked as [file]; line numbers refer to the files as committed.`
- [Line 2179](../../PROJECT_OVERVIEW.md#L2179): `| 9 | Validate | Dispatcher.ValidateAsync (Dispatcher.cs:109) | CreateCentreValidator runs: not empty, max lengths, enum defined. No errors -> continue. |`
- [Line 2261](../../PROJECT_OVERVIEW.md#L2261): `| 11 | Re-render | React | StatusCard function runs again; isPending false; branch chosen; DOM updated (skeleton replaced by the Card or an Alert). |`
- [Line 2451](../../PROJECT_OVERVIEW.md#L2451): `| Result/Error | ResultTests, ErrorTests | constructor 'success with error' guard, record equality |`
- [Line 2452](../../PROJECT_OVERVIEW.md#L2452): `| Validation | validator unit tests; dispatcher grouping (C9); HTTP 400 (EmptyName_FailsValidation_Returns400) | time-zone length, enum validity, name over 120 at validator level |`
- [Line 2529](../../PROJECT_OVERVIEW.md#L2529): `| Command / Query / Handler | Intent to change / read; class that executes one | CreateCentreCommand, GetSystemInfoQuery, *Handler |`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```text
# Tutoring Centre Manager — Project Overview

> **How to read this document.** Every statement about the repository was checked against a file I opened (commit `371be5c` plus this documentation). Where the repository does not say *why* something was done, I say so and label my interpretation. Things I could **not** verify are listed explicitly (for example, I could not run the .NET test suite — see [§1.3](#13-what-i-ran-and-what-i-could-not-run)).
>
> Companion documents: [`file-explanations/INDEX.md`](file-explanations/INDEX.md) (every file, grouped by directory) and the per-file explanation files it links to; area summaries `file-explanations/01…07-*.md` also exist.
>
> **Scope.** This document follows the section list you specified (1-15). Appendix A summarises the frontend and Appendix B is a practical 'working on this repo' guide. Every source file also has its own explanation under [`file-explanations/`](file-explanations/INDEX.md) (one `.md` per file, mirroring the repository paths).

---

<!-- deeper-dive-pass -->
# 1. Project Overview

## 1.1 What the application is (and is not yet)

The README states the **intended** product: *"A multi-tenant management platform for Egyptian tutoring centres: students and parents, groups and schedules, attendance (manual and QR), fees and payments, and a WhatsApp assistant that answers parents' questions in Egyptian Arabic and English through the same authorized use cases as the staff web app."* (`README.md`). ADR 0001 adds that the same use cases must serve three "front doors": a staff web app, a WhatsApp assistant, and background jobs.

**What actually exists in code today is a foundation, not those features.** I searched `src/` and `frontend/src/`: there are no students, parents, groups, schedules, attendance, fees, payments, WhatsApp or QR code. The only business concept is a **Centre** (the tenant), and the only business operation is **create a centre**, reachable **only from a CLI command**, not over HTTP.

Intended users (from README/ADRs): tutoring-centre staff (web app), parents (WhatsApp assistant), and the platform operator (the "system" actor). The repository's ADRs state it is also a **portfolio project aimed at .NET job roles**, so the architecture itself is part of what is being demonstrated (ADR 0001, Context). That explains why there is far more architectural machinery than product.

## 1.2 Implementation state, feature by feature

| Area | State | Evidence |
```

The excerpt stops at line 24; use the original file for the remaining 2610 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
