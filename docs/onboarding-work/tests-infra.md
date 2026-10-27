# Testing, infrastructure and operations: verified teaching material

This material describes the current checked-out repository. Source citations use repository-relative paths and one-based line numbers. The test catalogue and read ledger alongside this file distinguish declarations read from results executed. Comments containing dates, task numbers or future-tense statements are historical context, not proof that a feature remains unimplemented. In particular, several old comments still say there are no tenant entities even though the current model contains an Academics module.

<a id="infra-test-strategy"></a>
## 6.60 Testing as a hierarchy of different proofs

#### What it means

A test is executable evidence for a specific claim. A domain test can prove that an invalid capacity is refused. It cannot prove that two simultaneous HTTP requests never take the same last seat. A browser test can prove that a person can find the enrollment form and use it. It cannot establish every boundary of the capacity rule. These are different questions, and choosing a test means choosing the smallest environment that can answer the question honestly.

The repository has five backend test projects. Domain tests call business objects. Application tests call handlers and the dispatcher with hand-written doubles. Architecture tests inspect project references, compiled types, source text and the EF model. Infrastructure tests use real PostgreSQL. API tests host the real HTTP application in-process. Separate browser projects exercise development journeys and an already-running production-style instance. This separation is visible in the test project files, fixtures and `smoke/playwright.config.ts:14`.

#### The problem it solves

Without this hierarchy, either everything becomes a slow browser test or everything is mocked. The first option makes a one-character rule change expensive to check. The second can report success while a migration omits a unique index or the runtime database role bypasses isolation.

```csharp
// Hypothetical: this proves only the test double returns the value we arranged.
var repository = new FakeRepository { SeatsAvailable = true };
Assert.True(await repository.HasSeatAsync(groupId));
```

The actual last-seat tests instead run competing commands through real transactions and count persisted rows. Their assertions are not inferred from a mock's configured answer.

#### How it works

Arrange the state that matters, execute the operation under the chosen boundary, then assert observable outcomes. Keep unrelated components outside that boundary. A pure name rule needs no HTTP host. A claim that an error is HTTP 409 requires HTTP or a direct test of the mapping. A claim about PostgreSQL SQLSTATE requires PostgreSQL itself.

Tests should also establish that their experiment could detect failure. An independence test removes one defense before checking another. A race test measures whether requests overlap. A model test introduces a synthetic entity to avoid passing merely because its production scan found nothing. These controls make passing tests more informative than broad success banners.

#### How THIS project implements it

`tests/TutoringCentre.Domain.Tests` covers entities, results, phone numbers, search normalization, permissions and scheduling generation. `tests/TutoringCentre.Application.Tests/Fakes` supplies repositories, read services, a clock, actor context and unit of work. `tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs:61` creates the database environment. `tests/TutoringCentre.Api.Tests/Fixtures/ApiFactory.cs:43` creates the HTTP environment.

The infrastructure fixture configures the same Application and Infrastructure registration methods used by the real host. This matters: calling a repository directly would not prove the dispatcher opens a transaction, installs the tenant setting or saves audit entries. Conversely, handler tests deliberately build handlers without production infrastructure so a failure points toward business orchestration rather than Docker startup.

#### Execution example

To verify a student's enrollment, the suite asks several questions at different levels. `EnrollmentTests.cs` checks entity behavior. `EnrollStudentHandlerTests.cs` checks handler branches through fakes. `EnrollmentUseCaseTests.cs` dispatches against real storage. `LastSeatConcurrencyTests.cs:33` runs ten contenders for one remaining seat across twenty repetitions. `LastSeatHttpTests.cs` checks the HTTP result. Frontend journeys then exercise the visible enrollment workflow.

These tests are complementary. If a domain test passes and the HTTP test fails, inspect endpoint binding, authorization, transaction wiring and error mapping before rewriting the entity. If only the parallel test fails, inspect locking order and constraint translation rather than form validation.

#### Why this project needs it

This system handles isolated centres, staff privileges and limited seats. Those guarantees cross boundaries: a valid domain object is insufficient if the database stores a cross-centre relationship. The layered suite makes those boundaries explicit and keeps most rule checks fast.

#### Alternatives

A single end-to-end suite has fewer test technologies but weaker failure localization. A suite using only mocks is fast but cannot prove database behavior. SQLite and EF's in-memory provider can test some queries but cannot substitute for PostgreSQL policies, SQLSTATE values or exclusion constraints. The repository's use of PostgreSQL-specific assertions explains why its database tests need PostgreSQL.

#### Important details

A test name is a statement of intent; its assertions are the evidence. Counts of `[Fact]` and `[Theory]` declarations are not counts of executed cases because theories expand. A passing suite is evidence for its current assertions, not proof of absent bugs or production deployment. The run results later in this report use the test runner's totals rather than source declaration counts.

#### What I should understand before modifying this code

Choose the test boundary from the claim. Add a pure test for a rule, a real database test for a constraint, and an HTTP test when status or authorization matters. Preserve the independence and overlap controls. Do not replace a real PostgreSQL assertion with a fake merely to make the suite faster.

<a id="infra-test-doubles"></a>
## 6.61 Test doubles, call records and controllable time

#### What it means

A test double replaces a dependency during an experiment. A stub supplies a prepared answer. A fake supplies a small working implementation, such as an in-memory list. An interaction-recording double records calls so a test can check their sequence. The repository writes these classes directly instead of introducing a mocking library.

The important distinction is between arranging an input and assuming the conclusion. Returning a known membership is a reasonable way to isolate a handler's response to membership data. Returning a known successful transaction is not evidence that PostgreSQL rollback works. Doubles help isolate one responsibility; they do not reproduce every guarantee of the replaced system.

#### The problem it solves

Without substitution, every handler test would need a database, identity users and a running host. Failures would be slower and harder to attribute. Without a controllable clock, a schedule test could change behavior when the calendar crosses a week or daylight-saving transition.

```csharp
// Hypothetical dependency that is difficult to test deterministically.
var today = DateTimeOffset.UtcNow;

// The repository's approach: ask an injected clock or provider.
var clock = new FakeClock();
```

The second form lets a test choose the date that exposes the rule. It also avoids waiting for real calendar events.

#### How it works

A handler accepts an interface in its constructor. Production composition provides the real adapter; the test supplies a fake implementing that same interface. The handler executes unchanged. Assertions inspect the returned result, fake state, or recorded calls.

For pipeline behavior, each dependency appends a recognizable event to a shared list. Comparing that list against an expected sequence detects an extra save, a missing rollback or a transaction that starts before validation. The fake need not implement a database engine because its purpose is to expose the dispatcher's decisions.

#### How THIS project implements it

`tests/TutoringCentre.Application.Tests/Fakes/FakeUnitOfWork.cs:9` records `Begin(ro)`, `Begin(rw)`, `Save`, `Commit` and `Rollback`. `ThrowOnSave` simulates an unexpected save exception. `SaveFailure` simulates an expected persistence error returned as a failed `Result`.

```csharp
// Real code: FakeUnitOfWork.cs, SaveChangesAsync.
Calls.Add("Save");
if (ThrowOnSave)
{
    throw new InvalidOperationException("Simulated SaveChanges failure.");
}
return Task.FromResult(SaveFailure is null ? Result.Success() : Result.Failure(SaveFailure));
```

The distinction matters because the production dispatcher handles failure values and thrown faults through different branches. One fake supports both experiments without pretending they are the same outcome.

`ApiFactory.cs:32` exposes `FixableTimeProvider`. Its test registration replaces `IClock`, while leaving the framework's `TimeProvider` real. The comment at `ApiFactory.cs:111` explains why: fixing the business date must not accidentally expire cookies or invalidate antiforgery infrastructure.

#### Execution example

`ScheduleDoubleBookingHttpTests.cs:28` fixes the business clock at 2027-04-20 08:00 UTC. The test creates a teacher and two groups, then submits overlapping rules. Because the generation horizon starts from a stable date, the winner generates exactly four sessions. The test's disposal restores real time so later tests do not inherit the fixed calendar.

At the Application layer, a dispatcher test sets `SaveFailure`, sends a successful handler result, and checks that the dispatcher returns the persistence failure and rolls back instead of committing. This isolates pipeline behavior from the SQL constraint that might produce the failure in production.

#### Why this project needs it

Scheduling is date-dependent, and commands have a specific transaction protocol. Both would be painful to verify with only live services. Small explicit doubles make the inputs and observations visible to a reader learning the architecture.

#### Alternatives

Moq or NSubstitute could generate configurable substitutes. A test-specific subclass could override behavior but would couple tests to implementation inheritance. Delegates can replace one-method interfaces. The repository does not explicitly state why hand-written fakes were chosen. The following is an interpretation based on the implementation: explicit classes make state and call order easy to inspect and avoid a second configuration language.

#### Important details

The fake's behavior can drift from production. A list does not enforce a PostgreSQL composite foreign key. A completed task does not model cancellation or lock contention. Keep integration tests for those properties. Sharing a mutable fake between tests also creates order dependence, which is why the unit-of-work comment says to use one instance per test.

#### What I should understand before modifying this code

Read the fake together with its interface and production adapter. Extend only the behavior the test needs. Keep `IClock` replacement separate from framework time. When a test changes the clock, restore it. Treat call-list strings as part of the test harness contract when editing dispatcher assertions.

<a id="infra-postgres-fixtures"></a>
## 6.62 Real PostgreSQL fixtures, isolated databases and least privilege

#### What it means

An integration fixture creates and owns expensive shared resources for a group of tests. Here the resource is a throwaway PostgreSQL 17 container. The fixture bootstraps roles, applies the real migrations, builds production services and resets data between tests. Its lifetime is longer than one test method, but each test receives a clean data state.

Least privilege means the application connects with only the permissions needed for normal work. This is also a testing requirement. A test run as a superuser cannot prove that row-level security or denied DDL protects the runtime login, because the superuser has exceptional authority.

#### The problem it solves

Without controlled fixtures, tests could depend on a developer's local database or each other's rows. Without distinct roles, isolation tests could pass for an application predicate while silently failing to exercise PostgreSQL's real security boundary.

```csharp
// Hypothetical weak experiment: the application is tested with administrator credentials.
services.AddInfrastructure(ConfigurationWithSuperuserConnection());

// Actual fixture design: application services receive AppConnectionString.
["ConnectionStrings:Postgres"] = connectionString;
["ConnectionStrings:PostgresMigrations"] = OwnerConnectionString;
```

The final two assignments are from `PostgresFixture.CreateServiceProvider`, starting at line 143. The default `connectionString` is the runtime login.

#### How it works

Testcontainers asks Docker to start a real database on an available port. The fixture executes the repository's own bootstrap script inside that container. It then applies migrations as `tutoring_owner`, while normal application services use `tutoring_app`. A separate superuser connection is retained only for preparation, cleanup and independent assertions.

Respawn discovers relationships and deletes application rows between tests while preserving the migration history. Cleanup deliberately uses the superuser because forced row-level security would hide rows from an owner connection without a tenant setting. This is a controlled exception in the harness, not the application's runtime configuration.

#### How THIS project implements it

`PostgresFixture.cs:61` starts the container and invokes `RoleBootstrap.RunAsync`. Lines 67-68 build services and apply migrations. Lines 75-80 configure Respawn for `platform`, `identity`, `academics`, `probe` and `audit`, ignoring `platform.__ef_migrations_history`. `ResetAsync` at line 222 opens the superuser connection and calls `_respawner.ResetAsync`.

`tests/TutoringCentre.Infrastructure.Tests/Fixtures/RoleBootstrap.cs:15` reads `db/bootstrap-roles.sh`, copies it into the container, makes it executable and invokes it with test-only passwords. The API project has a corresponding harness helper. This reuses the real role setup instead of writing a weaker test-only imitation.

`RuntimeRolePrivilegeTests.cs:22` directly queries `current_user`, `rolsuper` and `rolbypassrls`. Other tests assert SQLSTATE `42501` for forbidden schema changes, centre deletion, migration-history insertion and Data Protection key updates/deletes. These are observations about the actual runtime role.

#### Execution example

A persistence test begins. The collection fixture has already started PostgreSQL, bootstrapped roles and migrated the schema. The test base resets data. Test preparation inserts known rows through the superuser connection. A fresh application scope sets a staff actor and dispatches a query using `tutoring_app`. PostgreSQL applies the transaction's tenant setting and policies. The result is asserted. A separate superuser count confirms what physically remains, even if the application's filters would hide it. The next test resets all application rows again.

#### Why this project needs it

The project relies on PostgreSQL-specific enforcement: read-only transactions, role grants, forced policies, composite foreign keys, partial unique indexes and scheduling exclusion constraints. A different engine or privileged login would change the very properties being tested.

#### Alternatives

A shared permanent test database is faster to start but risks stale rows and accidental use of real data. Recreating a container for every test gives strong isolation but is expensive. An in-memory provider is useful for some object behavior but cannot establish PostgreSQL permissions. The current shared-container/reset approach balances startup cost with clean test state.

#### Important details

`PostgresFixture.ProbeServices` is a separate service provider whose `AppDbContext` resolves to the same scoped `TenantProbeDbContext`. This lets test-only tenant probes pass through the real unit of work. A separate context instance would silently lose the transaction and tenant setting under test.

The API fixture's Respawn schema list differs: `ApiFactory.cs:58` lists `platform`, `identity` and `academics`, whereas the infrastructure fixture explicitly includes `audit` and `probe`. Do not describe those reset configurations as identical. Whether dependent audit rows are cleaned through relationship discovery must be checked against actual fixture behavior rather than inferred from the list alone.

#### What I should understand before modifying this code

Keep the application login unprivileged. Use superuser preparation only to establish independent known state. Add newly owned schemas to the appropriate reset plan. Preserve the migration history table. When a tenant test seems inexplicably empty, inspect actor assignment, transaction setup and database role before changing the query.

<a id="infra-http-tests"></a>
## 6.63 In-process HTTP tests and explicit endpoint inventories

#### What it means

An HTTP integration test sends a request through the real application's binding, middleware, authorization, endpoint filters and response mapping. `WebApplicationFactory<Program>` hosts that application inside the test process, so no public port is required. It is more complete than calling a handler, and narrower than controlling a browser.

An endpoint inventory is a separate security proof: every route the application exposes must be assigned an intentional access category. New endpoints cannot silently escape the test matrix merely because nobody remembered to add an assertion for them.

#### The problem it solves

Calling `HandleAsync` directly never checks whether an invalid JSON body becomes the right HTTP error or whether an endpoint forgot its permission binding. Hand-maintaining a list of protected examples is also insufficient: the list may stay green while a newly added route has no test.

```csharp
// Hypothetical incomplete test: one known endpoint remains protected.
Assert.Equal(HttpStatusCode.Unauthorized, (await client.GetAsync("/api/subjects")).StatusCode);

// Actual inventory design: compare all running routes with all classified routes.
var unclassified = actualRoutes.Except(expectedSet, StringComparer.Ordinal).ToList();
var missing = expectedSet.Except(actualRoutes, StringComparer.Ordinal).ToList();
```

The real comparison is in `EndpointInventoryTests.cs:50`.

#### How it works

The factory starts the real `Program` with environment `Testing` and controlled configuration. It replaces selected test services before host construction. Requests are handled by the in-memory server, but use the normal request and response pipeline. Database-backed tests still talk to real PostgreSQL.

The inventory resolves `EndpointDataSource`, walks route endpoints, reads method metadata and constructs keys such as `GET /api/me`. Routes without a method restriction use `ANY`. It excludes endpoints explicitly hidden from API description, such as fallbacks, then compares the discovered set against explicit anonymous, authenticated and permission-matrix categories.

#### How THIS project implements it

`ApiFactory.cs:100` selects environment `Testing`, supplies both connection strings, sets a throwaway seed password, disables the positive session-validation cache and enables scope/build validation. The controlled environment prevents developer user-secrets from deciding the database under test.

`EndpointInventoryTests.cs:20` begins the anonymous route entries: health, readiness, system info, antiforgery and login. Lines 29-35 list any-authenticated routes: logout, profile, centre selection and password change. Other routes come from `PermissionMatrixTests.MatrixRoutes`. `EveryRunningEndpoint_IsClassifiedExactlyOnce` at line 38 checks duplicate expectations, unclassified routes and expected routes that no longer run.

`AntiforgeryTestHelper.cs` fetches actual token pairs rather than bypassing CSRF. API tests therefore verify that ordinary unsafe requests work through the same token mechanism as the browser.

#### Execution example

A developer adds a new Academics endpoint. The normal host maps it. `EndpointDataSource` now contains its method and raw route pattern. The inventory's actual set grows, but its expected categories do not. The assertion reports the new route as unclassified. The developer must deliberately put it in the right category and extend the permission matrix rather than receiving accidental coverage from a wildcard.

For a protected mutation, a test signs in with a real cookie, fetches an antiforgery token, sends JSON, and observes a concrete status and Problem Details code. That checks the endpoint's contract as well as the underlying use case.

#### Why this project needs it

The API now contains many staff, academic and scheduling operations with different access rules. An inventory prevents growth from weakening the original fail-closed intent. A route that no longer exists also fails the inventory, keeping tests from claiming protection for obsolete names.

#### Alternatives

Static source scanning could inspect mapping calls but would have to understand extension methods and conditionally registered routes. OpenAPI inventory is useful but may omit hidden routes. MVC attribute reflection works for controller systems; this repository uses minimal APIs and runtime route metadata. Wildcard expectations are concise but would conceal the very omissions this test seeks.

#### Important details

The inventory does not itself prove permission behavior; the matrix performs that behavioral work. Hidden fallbacks are excluded deliberately and need their own tests. Development-only OpenAPI is absent under environment `Testing`, so do not describe the inventory as a list of every route in every environment.

#### What I should understand before modifying this code

Add explicit route expectations with the new endpoint. Keep method and raw pattern exact. Do not derive expected permissions from the same role map that the implementation uses, because then a wrong implementation can produce a matching wrong expectation. Preserve real CSRF handling in HTTP tests.

<a id="infra-architecture-tests"></a>
## 6.64 Architecture tests as executable design constraints

#### What it means

An architecture test checks the structure of code rather than one business answer. It can ask which project references which, which namespaces a compiled type uses, whether every request has exactly one handler, or whether every tenant-owned entity has the expected database keys. These constraints become part of the build instead of relying only on diagrams and code review.

Different inspection methods answer different questions. Project XML reveals declared references. Compiled assembly inspection reveals used types. Source scanning can recognize forbidden calls. EF model inspection reveals configured keys and filters. No one method sees all four categories.

#### The problem it solves

Without executable constraints, a hurried endpoint could inject a repository, an Application handler could use EF directly, or a new tenant request could omit its permission declaration. All might compile. A new developer could reasonably mistake a missing convention for an allowed pattern.

```csharp
// Hypothetical structural regression: a query depends on a write repository.
internal sealed class GetSomethingHandler(ICentreRepository centres) { }

// Actual rule intent: query handlers must use read ports instead.
// CqrsRuleTests examines handler interfaces and constructor parameters.
```

The benefit is detection at the point of change, before that coupling spreads.

#### How it works

The tests obtain assemblies through known marker types. They enumerate requests and handlers, inspect generic interface arguments, examine constructor parameters, and use NetArchTest for forbidden dependencies. Source scans read production `.cs` files for calls that compiled type dependencies alone cannot distinguish. EF tests build the model without opening a database connection.

Several rules add non-vacuity guards: a scan finding no types must not silently pass. Synthetic tenant probe entities also prove conventions apply to an unfamiliar type, rather than merely matching today's named entities.

#### How THIS project implements it

`ProjectReferenceTests.cs:8` defines the allowed four-project graph: Domain has no project references; Application references Domain; Infrastructure references Application and Domain; API references Application and Infrastructure. `DependencyRuleTests.cs:15` forbids framework and outer-layer namespaces in inner layers.

`QueryFilterBypassTests.cs:14` forbids calls beginning with `IgnoreQueryFilters`, `FromSqlRaw`, `FromSql`, `SqlQuery`, `ExecuteSqlRaw` and `ExecuteSql`, outside a narrow allow-list and migrations. The pattern also accepts suffixes and generic calls, so `ExecuteSqlRawAsync` is not a loophole.

`TenantScopeRuleTests.cs:22` enumerates tenant Application namespaces. `AuthorizationRuleTests.cs:42` requires tenant requests to implement `IRequirePermission`; another test reads their permission keys and checks the catalogue. `ModuleBoundaryRuleTests.cs:27` defines Platform, Identity, Academics and Audit namespace groups and forbids access to other modules' internal ports and entities while allowing narrow contracts.

#### Execution example

A new Academics query omits `ITenantScoped`. The type compiles and can have a perfectly valid handler. `EveryRequestInATenantModule_DeclaresTenantScoped` identifies its namespace and interface set, then reports the missing marker. If the marker is added but no permission declaration follows, `EveryTenantScopedRequest_AlsoDeclaresIRequirePermission` fails next. A misspelled permission then fails catalogue validation.

This chain protects the dispatcher contract at registration time. It is separate from HTTP tests because the same request can be invoked by a CLI or another trusted front door.

#### Why this project needs it

The repository keeps several functional modules inside four assemblies. Namespace boundaries therefore carry real responsibility that the compiler alone does not enforce. Tests make those boundaries visible and reduce the chance that convenience erases the architecture.

#### Alternatives

Separate assemblies per module give stronger compile-time barriers but increase project count. Roslyn analyzers can show editor diagnostics but require analyzer development. ArchUnitNET offers another dependency-testing library. Human review remains useful but is not repeatable enforcement. The ADRs document the choice of a small project set backed by architecture tests.

#### Important details

Name-based repository detection can miss a differently named port. Regex source scanning can match comments or miss code written in an unexpected style. The current raw-SQL allow-list is a deliberate exception list, not proof that raw SQL is always unsafe. It contains the unit of work, migration support, runtime privilege health check and repositories needing database locking operations.

`TenantModelTests.cs` contains historical comments saying production tenant entities or relationships do not exist. The current test inspects the real model regardless of those comments; do not reproduce their old state as current fact.

#### What I should understand before modifying this code

Treat a rule failure as a design signal. Change the production code first unless the design itself is intentionally changing. Keep exceptions named narrowly. Add new module namespaces to the map and add non-vacuity checks when a rule could otherwise pass on an empty set. Read source, compiled dependency and model tests together.

### Deeper dive: what each architecture experiment actually sees

It helps to trace one change through all the mechanisms rather than treating “architecture tests” as one scanner. Suppose a developer adds an Academics query that looks up a staff account and returns an Identity entity. Several different design mistakes are possible. The query could depend directly on an Identity repository. Its DTO could expose an Infrastructure type. It could omit tenant scoping or permission metadata. Its database entity could have a plain foreign key that ignores centre ownership. No single reflection test sees all those problems, because they live in different representations of the program.

The declared project graph is checked before any such detail. `ProjectReferenceTests` asks `ProjectFiles.ReadProjectReferences` to parse project XML and compare exact sorted project-name arrays. Application's only permitted project reference is Domain. Adding an unused Infrastructure reference still changes the XML and fails this check. That unused reference may not survive as a meaningful compiled type dependency, which is why examining assemblies alone would be insufficient. Conversely, importing a forbidden framework through another permitted dependency can appear in compiled code without adding the exact project reference the XML checker recognizes. Both representations matter.

`DependencyRuleTests` obtains assemblies through marker types, then passes them to NetArchTest. The library examines dependencies of the compiled types against forbidden namespaces. Before asserting absence, the helper checks the scanned type set is nonempty. That small control prevents a renamed or wrong assembly from making a rule pass by inspecting nothing. The failure message names the assembly and failing types. A returning developer should use that list to locate the actual coupling rather than weakening the namespace list until the test turns green.

Handler conventions require more specific discovery. A command is recognized by its implemented closed generic `ICommand<T>` interface, not merely by a class name ending in Command. Handler discovery similarly reads `ICommandHandler<TRequest,TResponse>` or `IQueryHandler<TRequest,TResponse>`. The cardinality rule builds the relationship between each concrete request type and its matching handler types, then requires exactly one. Zero handlers would otherwise fail only when DI resolution occurs at runtime; two handlers could be hidden by the container's last-registration behavior. The test also requires handlers to be sealed and nonpublic, encoding their intended status as implementation details reached through dispatch.

Repository conventions use names in a way generic handler discovery does not. `ArchitectureSupport` identifies an interface beginning `I` and ending Repository, and a concrete class ending Repository. Placement tests then require ports in Application and implementations in Infrastructure. Query-handler constructor inspection refuses repository interfaces as dependencies, directing read logic toward DTO-returning read services. This is useful enforcement, but its name convention is also a limit: an equivalent persistence port named Store would need the classifier updated or could evade this particular rule. Explaining the matching mechanism is more useful than describing the rule as an impenetrable compiler barrier.

The module map adds a second boundary inside the layers. `ModuleBoundaryRuleTests` groups Application namespaces into Platform/Centres, Identity/Staff, Academics and Audit. It computes the other modules' repository interfaces, read-service interfaces, handlers, validators and Domain types, then asserts the current module does not depend on those forbidden types. Shared common namespaces and a small explicitly named set of cross-cutting Domain types are exempt. This is how four projects can still host several modules without making every module's internal persistence interfaces globally acceptable.

Published contracts are checked separately. Types in namespaces ending `.Contracts` must be interfaces or records. The record test looks for the compiler-generated `<Clone>$` method, which is appropriate to the reference-type records used here. A contract must also avoid depending on its own module's repositories, read services or entities. Otherwise a supposedly narrow contract could simply wrap an internal entity and re-export the coupling. The tests require a nonempty contract set so this allowance remains exercised. This is the mechanism behind the explicit Identity directory contract used by academic teacher-account linking.

Tenant markers are discovered from interface membership, then checked against the declared namespace map. `TenantScopeRuleTests` requires requests in tenant modules to implement `ITenantScoped`, while older platform/login queries must remain outside that marker. The second rule also asserts the non-tenant request set is nonempty. `AuthorizationRuleTests` requires every tenant-scoped request to declare `IRequirePermission` and validates each permission key against `Permissions.All`. It obtains that key from an uninitialized request instance because current implementations return constants. This is safe only while the property remains independent of instance state; adding computed permission logic would require revisiting the test mechanism, not merely its expected data.

The role-reference scan addresses a different question. It reads Application command/query source for literal `StaffRole.Owner`, Secretary or Teacher references outside a narrow allow-list. Its intent is that authorization goes through permissions rather than role checks scattered in handlers. Some staff handlers legitimately manipulate roles as data, and teacher-account linking legitimately asks a directory about the Teacher role, so those files are named exceptions. The regex sees text, not semantic intent; comments can match and differently written logic can evade it. That makes code review complementary rather than obsolete.

Finally, `TenantModelTests` builds the actual EF model without connecting. It finds every CLR type implementing `ITenantOwned`, requires a query filter, a nonnullable `CentreId`, a Restrict relationship to Centre and a key containing exactly CentreId and Id. For every relationship whose principal is tenant-owned, it checks CentreId appears on both sides. Synthetic parent and child probes prove the convention applies to unfamiliar types and actually exercises the tenant-to-tenant relationship assertion. These probes remain valuable even though comments dating from before Academics are stale: their purpose is a control against an accidentally vacuous or special-cased convention, not a claim that production still lacks relationships.

When changing architecture, locate which representation the failure concerns: project XML, compiled dependencies, generic interfaces, constructor types, source text or EF metadata. Each assertion proves only what that representation exposes. Together they provide a much stronger fence than any one, while keeping the remaining assumptions visible and reviewable.

<a id="infra-isolation-experiments"></a>
## 6.65 Defense-in-depth tests and removing one layer at a time

#### What it means

Two security controls are independently useful only if either still works when the other is bypassed. A test that leaves both enabled can show the combined system refuses access, but cannot identify which control actually did the refusing. A stronger experiment deliberately disables one control and tests the remaining one.

The repository uses this method for EF tenant query filters and PostgreSQL row-level security. It also tests write guards, database role privileges and composite foreign keys separately. These are different protections: hiding a row, refusing a mismatched write, limiting SQL authority and preventing a cross-centre reference.

#### The problem it solves

Suppose an EF filter is accidentally removed but RLS still hides other-centre rows. An ordinary query test passes. If the RLS policy is later weakened, the supposedly redundant filter is already gone. Testing both layers together never revealed the earlier loss.

```csharp
// Hypothetical incomplete assertion: both controls are enabled.
Assert.All(await NormalQuery(), row => Assert.Equal(nileId, row.CentreId));

// Actual experiment names state which control is bypassed.
// BypassingTheEfFilter_AsNileActor_StillReturnsOnlyNileRows
// EfFilterAlone_WithSuperuserConnectionInPlaceOfApp_StillReturnsOnlyNileRows
```

Those tests are in `TenantIsolationIndependenceTests.cs:17` and line 31.

#### How it works

Seed rows for two centres using a preparation connection independent of the controls under test. For the database-policy experiment, use a query that ignores the EF filter but still connects as `tutoring_app`. For the EF-filter experiment, replace the connection with a superuser, which bypasses database RLS, while leaving the EF model filter enabled. Each result should contain only the actor's centre.

A third experiment runs raw SQL inside a dispatched query. Raw SQL bypasses LINQ filters, but the transaction's tenant setting and PostgreSQL policy remain. This tests a different path from generated ORM SQL.

#### How THIS project implements it

`TenantIsolationIndependenceTests.cs:19` seeds Nile and Maadi probes. `ListProbeCentresIgnoringEfFilterQuery` exercises database isolation alone. The second test constructs services with `appConnectionString: Fixture.SuperuserConnectionString` and a probe context, leaving EF as the only filtering control. The third uses `ListProbeCentresByRawSqlQuery` through the dispatcher.

`TenantModelTests.cs:55` checks that tenant relationships include `CentreId` on both sides of their foreign key. That is a storage invariant independent of read filtering. `AuditImmutabilityTests.cs:21` tests UPDATE, DELETE and TRUNCATE denials as SQLSTATE `42501`; it separately tests cross-centre reads and inserts.

#### Execution example

A Nile actor queries probes after one Nile row and one Maadi row were inserted. The test-only handler bypasses EF filters. The dispatcher begins the transaction and the unit of work establishes `app.current_centre`. PostgreSQL applies the policy as `tutoring_app`, returns only Nile, and the test asserts the exact centre-id list. If that policy were absent, both rows would be visible and the test would fail even though the normal application query remained filtered.

The complementary test uses a superuser database connection. PostgreSQL no longer hides Maadi, so a passing exact-list assertion demonstrates that the EF predicate itself still scopes to Nile.

#### Why this project needs it

The design explicitly invests in multiple tenant controls. Without independent experiments, that investment could degrade into one functioning control and several decorative configurations. The tests make the intended redundancy measurable.

#### Alternatives

Mutation testing could systematically remove predicates and policies to see which tests fail. Manual penetration tests can inspect broader attack surfaces but are harder to repeat for every change. A single access-denied test is cheaper but provides weaker evidence about independence. The repository implements direct controlled experiments for the most important tenant boundaries.

#### Important details

Superuser use is acceptable here only because removing RLS is the explicit experiment. It would invalidate a test claiming to prove RLS. Preparation counts must also be independent: if a query is filtered incorrectly, using that same query to count leaked rows can hide the defect.

#### What I should understand before modifying this code

State which defense each new test isolates. Keep the other layer genuinely bypassed. Seed both allowed and forbidden rows. Assert exact identities or counts rather than merely nonempty results. Preserve the probe context's shared transaction wiring, otherwise a test may exercise a different tenant lifecycle from production.

<a id="infra-concurrency-tests"></a>
## 6.66 Concurrency tests, synchronized starts and invariant assertions

#### What it means

Concurrency means operations overlap in time. A race occurs when their outcome depends on interleaving. A sequential test cannot establish a last-seat or double-booking guarantee because every second request sees the first request's completed state. A meaningful race test must create competing scopes and connections, overlap their critical operations, and inspect the persisted invariant.

The repository's races are bounded experiments, not full load tests. They target specific correctness properties: one last seat, one active enrollment, no capacity underflow, one owner remaining, unique names and no overlapping teacher sessions.

#### The problem it solves

```csharp
// Hypothetical sequential test: useful, but does not exercise a race.
await EnrollAsync(firstStudent);
var second = await EnrollAsync(secondStudent);
Assert.Equal("group.full", second.Error!.Code);
```

Both requests could still oversubscribe if run simultaneously. A parallel test has them wait at a start gate, then releases them together. It checks actual rows after completion, not only successful response counts.

#### How it works

Create independent clients or DI scopes before opening the gate. Build requests and fetch CSRF tokens beforehand so preparation does not serialize the contested interval. Use `TaskCompletionSource` to release contenders. Await every attempt. Assert one winner, expected loser codes and a correct database count. Repeat with fresh entities so state from earlier repetitions cannot force the later outcome.

For HTTP experiments, record start and finish times and verify that the latest start precedes the earliest completion. This establishes actual overlap in the observed request interval. It does not prove every SQL instruction overlaps, but rules out a wholly sequential request execution masquerading as a race.

#### How THIS project implements it

`LastSeatConcurrencyTests.cs:19` declares twenty repetitions and ten last-seat contenders. Line 25 checks the runtime connection pool can support the largest parallelism; a smaller pool would serialize before the database lock. Line 62 tests fifty contenders for a one-seat group over five repetitions. Line 83 tests duplicate enrollment of the same student, and line 106 races enrollment against shrinking capacity.

`ScheduleDoubleBookingHttpTests.cs:38` signs in separate owner and secretary clients, prepares overlapping rules and records each attempt's elapsed interval. It requires one 201 and one 409 with `schedule.teacher_double_booked`, then independently counts exactly one rule and four sessions through the fixture's superuser connection.

#### Execution example

Nineteen students occupy a twenty-seat group. Ten fresh students compete for the final seat. Each command uses its own scope and connection. The repository lock serializes access to the group row. One transaction sees nineteen active enrollments and inserts the twentieth. Later contenders acquire the lock after that commit, see twenty, and return `group.full`. The test requires one successful result, nine specific failures and exactly twenty active rows.

It also counts audit entries for the group. Only the winning enrollment should be audited; losing commands must roll back their associated audit work. This links the seat invariant to atomic transaction behavior rather than checking them in isolation.

#### Why this project needs it

Seat counts and teacher availability are shared resources. Two staff members can act at once. Friendly validation before a write is insufficient because both requests can observe the same earlier state. Tests therefore exercise the database mechanism that arbitrates concurrent work.

#### Alternatives

Deterministic database barriers inside a test-specific adapter can align the exact race window more tightly. Stress tools can increase contention but make failures harder to localize. Model checking can explore interleavings for a simplified model. The repository uses repeated real-database races with explicit gates and invariant checks as practical evidence for its chosen locks and constraints.

#### Important details

Repetition reduces the chance of missing timing-sensitive faults but is not mathematical proof. A failure accepting any exception would be too weak: a loser should receive the expected business outcome, not a random 500. These tests assert concrete error codes and persisted state. Keep the connection pool large enough that contenders reach PostgreSQL simultaneously.

#### What I should understand before modifying this code

Read the lock acquisition order together with every competing mutation. Capacity changes must participate in the same protocol as enrollment. Keep fresh groups or teachers per repetition. Do not relax an exact expected conflict into a broad exception assertion to make a race pass. Confirm both outcome and physical storage state.

<a id="infra-options-health"></a>
## 6.67 Typed configuration, startup validation and health probes

#### What it means

Configuration is input supplied by the environment rather than business users. A connection string, trusted proxy network or telemetry collector address can change between a developer machine and a container without changing compiled source. Typed options turn named settings into objects. Validation checks those objects before the application accepts traffic.

Health probes answer operational questions. Liveness asks whether this process is alive. Readiness asks whether it can safely serve its intended work. A process can be alive while its database is unreachable or while its database credentials are dangerously privileged. Treating those as different states prevents an operator from confusing a restart problem with a dependency or security problem.

#### The problem it solves

```csharp
// Hypothetical: a misspelled key fails only when this request first runs.
var value = configuration["ConnectionStrings:Postgress"];
await OpenDatabaseAsync(value);

// Current composition registers typed options and startup validation.
services.AddOptions<ProxyOptions>()
    .Bind(configuration.GetSection("Proxy"))
    .ValidateOnStart();
```

The second form appears in `ForwardedHeadersSetup.cs:18`. Its validator can reject an empty Production proxy list and malformed CIDR values at startup. A failure is therefore tied to deployment configuration, instead of surfacing later as misleading client behavior.

#### How it works

The host builds configuration from providers. Later providers override earlier values. Options binding reads a section into a class. Registered validators inspect the result; `ValidateOnStart` arranges validation when the host starts. Application code receives the resulting options through DI rather than reconstructing strings everywhere.

Health-check registration is separate. The host associates checks with tags. A mapped health endpoint chooses which checks run. In this application `/health` deliberately selects none, while `/health/ready` selects checks tagged `ready`. Failure responses stay generic because readiness is anonymous.

#### How THIS project implements it

`Program.cs:57` registers proxy options. `ProxyOptionsValidator.cs:20` requires at least one trusted network in Production and parses each configured CIDR. `Program.cs:61` binds demo options and validates on start. Infrastructure validates database options; the runtime and migrations connection strings have different roles in operation.

`Program.cs:204` maps liveness with `Predicate = _ => false`. Readiness follows with `c.Tags.Contains("ready")`. Besides connectivity, `RuntimeRolePrivilegeHealthCheck.cs:18` opens the runtime connection and queries PostgreSQL role metadata. It returns unhealthy when the login is superuser, has `BYPASSRLS`, or is the named owner role.

```sql
-- Real query from RuntimeRolePrivilegeHealthCheck.cs.
select rolsuper or rolbypassrls or rolname = @ownerRole
from pg_roles where rolname = current_user;
```

This is a configuration tripwire. A working connection with the wrong authority is not considered ready merely because `SELECT 1` succeeds.

#### Execution example

A container starts with `ConnectionStrings__Postgres` mistakenly pointing to `tutoring_owner`. The process can bind its HTTP port and liveness can remain healthy. Readiness opens that configured connection, finds the owner role and returns unhealthy. A deployment gate polling readiness refuses to treat that instance as fit to serve. The test family `ReadinessPrivilegeTests.cs` exercises privileged versus ordinary credentials, while `RuntimeRolePrivilegeTests.cs` directly inspects grants.

For a malformed proxy CIDR, the failure happens earlier: options validation rejects it during host startup. There is no need to wait for an incoming forwarded request to discover it.

#### Why this project needs it

Tenant isolation depends on operational choices as well as code. Supplying an owner connection can defeat assumptions beneath otherwise correct handlers. Trusted network configuration also determines whether client addresses and HTTPS scheme are interpreted safely behind Caddy. Explicit startup and readiness checks make those assumptions observable.

#### Alternatives

Direct configuration reads are simple but spread key names and defer errors. `IOptionsMonitor` supports changing settings while running, but a runtime change to security configuration needs careful lifecycle handling. One combined health endpoint is easier to name but cannot distinguish restart from traffic eligibility. An external database probe can check connectivity but would not necessarily use the application's actual configured login.

#### Important details

Readiness does not repair configuration or revoke database privileges. An orchestrator or deployment gate must act on its result. No Azure gate exists yet; CI and local Compose demonstrate parts of the intended process. An options object with a default value does not automatically prove the value is appropriate for production.

The build-time OpenAPI path supplies placeholder options in `Program.cs:33`, but actual execution during this audit still logged a database key-ring access error. Therefore the source comment that nothing connects during generation is stronger than observed behavior. The build completed; its diagnostic side effect remains separately documented.

#### What I should understand before modifying this code

Add settings where their ownership belongs, validate values that would make startup unsafe, and decide whether a failure means dead or merely unready. Keep anonymous probe output generic. Test readiness with the actual runtime role. Preserve the distinction between required runtime settings and owner-only migration settings.

<a id="infra-docker-build"></a>
## 6.68 Docker images, multi-stage builds and static frontend packaging

#### What it means

An image is the packaged filesystem and launch configuration used to create containers. A container is a running process using that image with isolation provided by the container platform. Building and running are different phases: compilers and package managers are needed during the build, but the running service needs only the compiled application and its static assets.

A multi-stage Dockerfile makes that separation explicit. Each `FROM` starts a stage with its own filesystem. Later stages copy selected outputs from earlier stages. The final image can therefore contain the runtime application without containing the SDK, npm dependencies or source tree used to build it.

#### The problem it solves

```dockerfile
# Hypothetical single-stage image: all build tooling remains in production.
FROM mcr.microsoft.com/dotnet/sdk:10.0.401
COPY . /src
WORKDIR /src
RUN dotnet publish -c Release
CMD ["dotnet", "src/TutoringCentre.Api/bin/Release/net10.0/TutoringCentre.Api.dll"]
```

That approach also does not build the React bundle. The repository's actual image builds both toolchains and copies only their outputs into an ASP.NET runtime stage.

#### How it works

Docker caches build instructions and their inputs. Copying manifests before source lets dependency installation reuse a layer when application code changes but dependencies do not. Building React produces static HTML, JavaScript, CSS and fonts. Publishing the API produces assemblies and runtime files. The final stage combines those outputs under `/app`, placing the frontend in `wwwroot` for same-origin serving.

The process runs as a non-root user. That is a different protection from database least privilege: one limits operating-system authority; the other limits SQL authority. Both matter, and neither replaces application authorization.

#### How THIS project implements it

`Dockerfile:7` starts `node:22.12-alpine` as `frontend-build`. It copies `frontend/package.json` and the lockfile, runs `npm ci`, copies frontend source and runs `npm run build`.

`Dockerfile:18` starts the .NET SDK stage pinned to `10.0.401`. It accepts `COMMIT_SHA`, copies solution-wide props and each source project file, restores the API project, then copies source and publishes Release output. `SourceRevisionId` carries the supplied commit into assembly informational versions.

```dockerfile
# Real final-stage structure, Dockerfile.
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=backend-build /app/publish .
COPY --from=frontend-build /frontend/dist ./wwwroot
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
USER $APP_UID
CMD ["dotnet", "TutoringCentre.Api.dll"]
```

`.dockerignore` excludes Git metadata, `.env`, build outputs, node_modules, test source, docs and deployment-support files not needed in the image build. This reduces context size and prevents the local environment file from being sent as build context.

#### Execution example

CI supplies the commit SHA and builds `tutoring-centre:ci`. The frontend stage installs the lockfile and produces `dist`. The backend stage restores and publishes. The final image receives the API and `dist` as `wwwroot`. A later `docker run` starts Kestrel on port 8080. Requests to `/` receive the built frontend from the same process that serves `/api`.

The container CI job also invokes the image with `dotnet TutoringCentre.Api.dll migrate`. Because the Dockerfile uses `CMD` rather than an immutable entrypoint, the command can replace normal web hosting for a one-shot operation. The same packaged code therefore performs migration before serving.

#### Why this project needs it

The application has two build systems but one deployment unit. Packaging both outputs together preserves the same-origin cookie design and gives CI an artifact resembling the intended production service. A source-only .NET image would miss the frontend; a separate static host would introduce an additional origin and hosting boundary.

#### Alternatives

Build outside Docker and copy ready outputs into a runtime image; this can be fast but depends more on the external build environment. Host the frontend independently behind a shared reverse proxy; that adds routing and release coordination. Publish a self-contained .NET executable; the repository currently uses a framework-dependent publish and the ASP.NET runtime image. Single-stage builds are easier to sketch but retain unnecessary tooling.

#### Important details

An `EXPOSE` instruction documents a container port; it does not publish that port to the host. Compose or `docker run` decides publication. `USER $APP_UID` affects the final runtime stage, not earlier image-building commands. A non-root process can still misuse a privileged database connection, so runtime SQL credentials remain separately restricted.

Image tags such as `aspnet:10.0` and `postgres:17` identify version lines, not immutable content digests. The repository does not pin every base image by digest. Record that accurately rather than claiming fully immutable builds merely because an npm lockfile exists.

#### What I should understand before modifying this code

Keep restore inputs before source to preserve cache usefulness. Include new required project files in the restore stage. Verify the final `wwwroot` location against SPA hosting. Do not copy local secrets into the image. Preserve command replacement if the same image must keep supporting migrate and seed jobs.

<a id="infra-compose-roles"></a>
## 6.69 Compose topology, role bootstrap and migrate-before-serve ordering

#### What it means

Compose describes several cooperating containers, their environment, networks, volumes and startup dependencies. It is not just a shorter `docker run`: it can express that a database must be healthy before migration starts, migration must complete successfully before seeding, and seeding must succeed before the web app starts.

Database bootstrap is separate from schema migration. Bootstrap creates logins and grants authority to perform future work. Migration evolves application tables. The runtime application should not receive the owner credential merely because someone needs that credential earlier in deployment.

#### The problem it solves

A naive setup starts the web process and asks it to create schema on every startup. Multiple instances can then race, hold locks unexpectedly, or serve code before the schema is ready. Using one superuser login everywhere also erases the authority boundary tenant isolation depends on.

```yaml
# Real ordering from compose.prod.yaml, simplified to the relevant keys.
seed:
  depends_on:
    migrate:
      condition: service_completed_successfully
app:
  depends_on:
    seed:
      condition: service_completed_successfully
```

These conditions express completed work rather than merely created containers.

#### How it works

The base Compose file starts PostgreSQL and mounts `db/` into the image's initialization directory. On a new empty data volume, the Postgres entrypoint runs the bootstrap script. The override file adds migrate, seed, app and Caddy services on a fixed network.

Migrate receives the owner connection and runs one command. Seed is explicitly enabled for the demo environment and runs only after migrate succeeds. App receives the runtime connection and no owner connection. Caddy publishes the HTTPS entry point and forwards requests to app:8080. A named database volume preserves data across ordinary container recreation.

#### How THIS project implements it

`compose.yaml:3` uses `postgres:17`. Its environment requires administrator, owner and app passwords. `db/bootstrap-roles.sh:20` enables strict shell handling and requires the owner/app password variables. It creates missing roles and always resets their attributes to login, no superuser, no createdb, no createrole and no bypassrls.

The script grants database CREATE/CONNECT to the owner and CONNECT to the app. It does not make the app a member of the owner. Existing platform and identity schema objects can be transferred to the owner; identifier formatting uses SQL `format` with `%I` rather than concatenating arbitrary identifiers.

`compose.prod.yaml` separates four application-related roles: migrate, seed, app and Caddy. The `prod` network fixes subnet `172.28.0.0/16`, which matches `Proxy__KnownNetworks__0`. Caddy uses `tls internal` for `localhost:8443` and forwards to `app:8080`.

#### Execution example

On an empty volume, `docker compose -f compose.yaml -f compose.prod.yaml up --build` starts PostgreSQL initialization. The role script creates `tutoring_owner` and `tutoring_app`. Health succeeds. The migrate container applies schema using the owner credential and exits successfully. Seed runs with demo mode enabled, creates the demonstration data and exits. App then starts with only `ConnectionStrings__Postgres`. Caddy accepts browser HTTPS traffic and forwards the request to app over the internal network.

Restarting app alone does not repeat migration or regenerate its database. Persistent Data Protection keys allow a cookie created before the restart to remain readable afterward, provided the same database and application identity are used.

#### Why this project needs it

This is a locally executable production topology. It exercises packaging, TLS termination, forwarded headers, owner/runtime credentials and startup ordering without inventing a cloud deployment that does not exist. It also gives the smoke test a stable HTTPS target.

#### Alternatives

An initialization sidecar could migrate before app startup in another platform. A deployment pipeline could invoke the migration image independently, then release app. A single container running shell scripts plus the web process is simpler initially but conflates lifecycle and credentials. Installing PostgreSQL locally avoids containers but does not prove the packaged topology.

#### Important details

The role script runs automatically only when the database volume is initialized. Editing its passwords in `.env` does not automatically rewrite an existing volume's logins. README explicitly calls out this first-start behavior.

The production override inherits the base database port publication `5432:5432`. Therefore the current local stack publishes PostgreSQL as well as Caddy. The deployment document's claim that only Caddy has a published port is incorrect for this merged configuration. Fixed networks and container dependencies do not themselves remove inherited port mappings.

#### What I should understand before modifying this code

Inspect the merged Compose configuration, not one file in isolation. Keep owner credentials out of app. Preserve success-based job dependencies. Treat volume removal as destructive data deletion, and do not use it as an automatic fix for a configuration mismatch. Update trusted proxy networks if network topology changes.

<a id="infra-proxy-spa"></a>
## 6.70 Reverse proxies, forwarded scheme, security headers and SPA fallbacks

#### What it means

A reverse proxy accepts a client request and sends it to an internal application. TLS can end at the proxy, so the internal hop is HTTP even though the browser used HTTPS. Forwarded headers describe the original scheme and client address. Those headers are claims: the application must decide which immediate peers are allowed to supply them.

Same-origin SPA hosting means HTML, assets and API share the browser's origin. A deep link such as `/groups/...` must return the SPA shell so the client router can render it. A missing JavaScript file must return 404, and an unknown `/api` route must remain JSON. These cases look similar to the server but require different responses.

#### The problem it solves

Without forwarded scheme handling, secure antiforgery checks may see HTTP behind TLS termination. Without client-address handling, a per-IP login limit groups every user behind the proxy together. Without careful fallback ordering, `/api/missing` can receive HTML or `/assets/missing.js` can receive a false-success app shell.

```csharp
// Hypothetical broad fallback: returns HTML for mistakes in API and asset paths.
app.MapFallbackToFile("index.html");

// Current Program.cs first reserves unknown API paths for JSON 404.
app.MapFallback("/api/{**path}", () =>
    Error.NotFound("route.not_found", "The requested route does not exist.").ToProblemResult());
```

The real mapping adds anonymous and description-exclusion metadata.

#### How it works

Forwarded-header processing runs before logic that depends on scheme or client IP. Security-header callbacks register before downstream work and set response headers immediately before sending, including after exception handling clears a response. Static-file middleware runs before route selection so existing files can be served without authentication work.

Explicit routing then selects API, health or fallback endpoints. The SPA fallback exists only if a physical `index.html` is present. Its nonfile constraint excludes paths that look like missing assets. A final unconstrained anonymous fallback returns the uniform JSON 404 for anything remaining.

#### How THIS project implements it

`ForwardedHeadersSetup.cs:38` configures `X-Forwarded-For` and `X-Forwarded-Proto` with `ForwardLimit = 1`, populating known IP networks from options. `Program.cs:162` calls it before HSTS, headers, correlation and logging. `SecurityHeadersSetup.cs:37` uses `OnStarting` to set CSP, nosniff, referrer policy, cross-origin opener policy and permissions policy. API responses receive `Cache-Control: no-store` unless already specified.

`SpaHostingSetup.cs:20` installs static files. `index.html` receives `no-cache`; other static assets receive one year of public immutable caching. The fallback checks for the built index before mapping. `Program.cs` explicitly places `UseRouting` after static files, with a comment explaining why implicit early selection could otherwise choose fallback endpoints before file serving.

#### Execution example

The browser opens `https://localhost:8443/subjects`. Caddy terminates TLS and forwards to app:8080 with forwarding headers. Middleware interprets trusted network information, registers security headers and correlation, then fails to find a physical `/subjects` file. Routing selects the SPA fallback and sends index HTML with no-cache. The browser loads hashed assets, which static files serve with immutable caching. React's router then applies the page guard and fetches protected data.

If JavaScript requests `/api/nope`, the API-specific fallback returns `application/problem+json` with `route.not_found`. If the browser requests `/assets/missing.js`, the nonfile SPA constraint does not match; the last fallback returns 404 rather than HTML.

#### Why this project needs it

The cookie and antiforgery design relies on correct scheme interpretation and a coherent browser origin. The packaged frontend also needs deep links to work after refresh. These deployment details are observable application behavior, not cosmetic infrastructure choices.

#### Alternatives

Caddy could serve static files and proxy only API paths, but cache and fallback rules would then belong partly outside the application. Separate frontend and API origins require CORS and explicit cookie considerations. A backend-rendered app would not need the same client-routing fallback. Per-endpoint header setting is omission-prone compared with one middleware.

#### Important details

The CSP permits inline styles because the repository records a Sonner runtime-style requirement; script sources remain self-only. This relaxation is documented, not an assertion that every inline style is harmless. HSTS is Production-only with an initial five-minute lifetime, and loopback hosts are excluded by framework behavior that the smoke configuration explicitly accounts for.

The empty known-network behavior needs independent verification: the source comment claims no trust when both framework known lists are empty, while existing forwarding tests use a configured `10.0.0.0/8` network. Do not present the empty-list security claim as runtime-proven merely from that comment.

#### What I should understand before modifying this code

Trace middleware order before changing it. Test existing static files, deep links, missing assets, unknown API routes and error responses separately. Keep trusted proxy configuration narrow. Confirm framework behavior for an empty trust list before relying on it in Development or Testing. Preserve no-cache HTML and content-hashed asset assumptions together.

<a id="infra-ci"></a>
## 6.71 Continuous integration, reproducible inputs and drift gates

#### What it means

Continuous integration automatically builds and checks a change in a clean runner environment. A workflow defines triggers; a job owns a runner; steps inside that job execute in order. Jobs can run independently or wait for prerequisites. CI is evidence that a particular revision satisfies configured checks. It is not the same as deploying that revision.

A drift gate checks whether a committed generated artifact matches its source. A normal compile can succeed while the OpenAPI document or frontend client is stale. A `git diff --exit-code` after generation makes that mismatch fail explicitly. This treats generated files as reviewed contracts rather than incidental developer output.

#### The problem it solves

Without CI, one developer can build the backend while accidentally breaking TypeScript. Without lockfiles, dependency resolution can change between machines. Without drift checks, a correctly changed API can leave a generated client representing yesterday's endpoint shape.

```yaml
# Real backend gate from .github/workflows/ci.yml.
- name: OpenAPI document is up to date
  run: git diff --exit-code -- frontend/openapi

# Real frontend gate runs generation before comparing committed output.
run: |
  npm run generate:api
  git diff --exit-code -- src/api/generated
```

These checks do not infer freshness from timestamps. They compare actual content.

#### How it works

A pull request or push to main triggers the CI workflow. Backend and frontend jobs run separately. Backend restores packages, builds Release, compares OpenAPI, restores the EF tool, checks model/snapshot agreement and runs tests. Frontend installs exact lockfile dependencies, generates the client, compares it, then runs lint, typecheck, translation-key parity, tests and build.

The e2e, container and smoke jobs depend on backend and frontend. Each creates its own relevant environment. This avoids treating a unit-tested assembly as proof that the production image or TLS-proxied browser workflow works. Concurrency settings cancel superseded runs for the same ref so obsolete revisions do not consume runners indefinitely.

#### How THIS project implements it

`.github/workflows/ci.yml:1` defines the workflow and its five jobs: backend, frontend, e2e, container and smoke. Default token permissions are `contents: read`. The backend uses `global.json` through setup-dotnet. The frontend uses Node LTS and npm caching keyed by `frontend/package-lock.json`.

The container job builds the production Dockerfile, runs migrate from that image with owner credentials, starts app with runtime credentials, polls readiness and checks HTML plus CSP. The smoke job starts the complete local production Compose stack and runs `smoke/` against Caddy HTTPS, then tears down that job's disposable volume.

Separate workflows run CodeQL for C# and JavaScript/TypeScript, and gitleaks across full Git history. Dependabot checks NuGet, frontend npm, GitHub Actions and Docker weekly. These are separate kinds of signals: static analysis, secret scanning, dependency proposals and executable behavior checks.

#### Execution example

A backend developer adds a response property and commits C# but forgets generated OpenAPI. CI's build regenerates the document. The following diff step sees a changed file and fails. After the document is committed, frontend generation may still alter `src/api/generated`, causing the frontend drift gate to fail until that client is regenerated and committed too.

A later image build can still fail independently, for example because the Dockerfile forgot a new project file in its restore stage. That is why the container job exists even though the ordinary Release build already succeeded.

#### Why this project needs it

Two languages and multiple generated contracts increase coordination cost. Security-sensitive storage also needs migration/model agreement. CI puts those checks beside every proposed change rather than leaving them to a developer's memory.

#### Alternatives

GitLab CI, Azure Pipelines or Jenkins could perform the same commands. Generating all contracts only during deployment removes committed drift but makes review harder and moves failure later. One monolithic job reduces setup but gives slower, less specific feedback. The repository's jobs separate concerns while using dependencies for expensive cross-layer gates.

#### Important details

The workflow triggers on pull requests and pushes to main, not every arbitrary branch push. Required branch-protection checks are a GitHub setting; README's protection claim cannot be proven from workflow source. `needs` establishes job ordering, not shared files or shared databases between jobs. Every job must create its own prerequisites.

The `smoke` npm package has its own lockfile but no separate Dependabot npm entry; the npm update entry targets `/frontend`. That is a maintenance gap visible in `.github/dependabot.yml`, not a claim that the smoke dependencies are currently vulnerable.

#### What I should understand before modifying this code

Run the relevant generation before committing. Keep CI's commands consistent with package scripts and project configuration. Add a new independent service to the appropriate job rather than assuming another job's service survives. Distinguish a passing source build from a passing image, browser journey and deployment.

<a id="infra-playwright"></a>
## 6.72 Browser journeys and deployed smoke tests

#### What it means

A browser journey drives a real browser through user-visible behavior. It tests interactions across frontend rendering, routing, API calls, cookies and database persistence. A smoke test is narrower: it asks whether an already-running revision is fit to serve a small essential workflow. It should be quick, low-impact and tied to the expected revision.

The repository has two Playwright projects because those purposes require different setup. `frontend/e2e` starts or reuses development servers and creates test data. `smoke/` starts no server, accepts a target URL and performs five checks against that instance.

#### The problem it solves

Component tests with MSW cannot establish that a real browser accepts the real cookies, that the API sees HTTPS, or that a proxy preserves the contract. An HTTP test cannot establish that translated labels and visible controls let a staff member complete a workflow.

```ts
// Hypothetical narrow component assertion: no real API or cookie is involved.
expect(screen.getByRole("button", { name: "Sign in" })).toBeVisible();

// Real smoke journey, smoke/tests/smoke.spec.ts.
await page.getByLabel("Email").fill(email);
await page.getByLabel("Password").fill(password);
await page.getByRole("button", { name: "Sign in" }).click();
```

The browser's resulting page and later authenticated actions determine success.

#### How it works

Playwright creates browser contexts, locates elements by accessible role or label, waits for observable conditions and asserts rendered outcomes. Its webServer configuration starts the development API and Vite server, waiting for configured URLs before tests run. The API uses the HTTPS profile; the readiness probe uses that profile's plain HTTP sibling to avoid Node certificate trust problems.

The smoke project instead requires `SMOKE_BASE_URL`, `SMOKE_EXPECTED_SHA` and `SMOKE_PASSWORD`. It turns off trace, screenshot and video because its credential can belong to a running demo instance. HSTS expectation defaults on and is explicitly disabled only for the loopback local stack.

#### How THIS project implements it

`frontend/playwright.config.ts:17` enables fully parallel tests, one CI retry, Chromium and HTML reports. It starts API at the `https` launch profile and Vite at port 5173 with `API_TARGET=https://localhost:7197`. Test files require `SEED_PASSWORD` and fail immediately if it is empty.

The ten current spec files declare nineteen journeys: auth, subjects, staff access, staff audit, first login, students, teachers/groups, enrollments, scheduling and full centre setup. The full setup journey uses Arabic locale and begins with an empty Giza centre. Credential-sensitive journeys override trace/video settings. These declarations are source facts; executed results must be reported separately.

`smoke/tests/smoke.spec.ts:29` defines five checks: readiness, HTML/security headers, expected commit plus demo mode, unauthenticated Subjects 401, and owner sign-in/Subjects/sign-out. `smoke/playwright.config.ts:14` sets no retries, serial execution, thirty-second timeout and no media capture.

#### Execution example

CI's e2e job starts a PostgreSQL service, runs the real role bootstrap, generates a development certificate, seeds data, installs Chromium and launches the nineteen journeys. It raises the login permit limit to 100 for that job because repeated UI sign-ins share one runner IP. This is an environment-specific test budget, not the production default.

CI's smoke job builds and starts the Compose production stack, waits for HTTPS readiness on port 8443 and passes the current commit SHA to smoke. The version check requires `applicationVersion` to end with that SHA. A browser can therefore load a working page and still fail smoke if the wrong revision is serving it.

#### Why this project needs it

Authentication, tenant switching and bilingual workflows have seams that isolated tests cannot prove. The smoke project adds a release identity check and packaging/proxy proof, while the broader journeys exercise actual staff tasks.

#### Alternatives

Cypress provides another browser automation stack. Manual walkthroughs reveal usability issues but do not repeat reliably for every change. Reusing all e2e journeys as smoke would create data and slow deployment gates. A health-only smoke test is cheap but cannot catch a broken frontend or sign-in flow.

#### Important details

The smoke Subjects assertion checks the first row is visible; it does not explicitly distinguish a data row from a table header. Therefore describe the assertion literally rather than claiming it proves at least one subject record exists. The project intends read-mostly operation, and its spec performs no explicit creation calls.

Development journeys create uniquely named data and can leave it behind. They should not be aimed at a real production dataset. `reuseExistingServer` is convenient locally but can mean a preexisting API ignores changed test environment settings, which is why README tells readers to stop their own API before relying on the raised login limit.

#### What I should understand before modifying this code

Keep browser assertions tied to accessible names. Preserve credential capture restrictions. Use cross-layer journeys for seams, leaving exhaustive rule matrices in faster tests. Supply the correct seed password and expected revision. Do not report source declarations or historical runbook results as current execution results.

<a id="infra-telemetry"></a>
## 6.73 OpenTelemetry signals, opt-in exporters and correlation

#### What it means

Observability is the ability to investigate behavior from emitted evidence. Logs describe events. Traces describe a chain of timed operations belonging to a request. Metrics aggregate measurements over time. OpenTelemetry supplies a common instrumentation and export model for those signals, so the application can emit them without hard-coding one cloud monitoring product throughout business code.

An exporter sends signals to a collector. A resource identifies the emitting service and version. A span represents one timed operation; tags add searchable context. Correlation connects a client-visible error reference with logs and trace context.

#### The problem it solves

Without correlation, a report that enrollment failed at some time must be matched against many unrelated logs. Without common instrumentation, request timing and database work can be observed through incompatible tools. Hard-coding exporter dependencies in handlers would also couple business logic to monitoring infrastructure.

```csharp
// Hypothetical tight coupling inside a business handler.
cloudMonitor.TrackEvent("EnrollmentCreated", userData);

// Actual setup remains at the API composition edge.
builder.Services.AddApiTelemetry(builder.Configuration);
```

The second line is in `Program.cs:65`; Domain and Application do not receive OpenTelemetry dependencies.

#### How it works

At startup, the setup reads an optional collector endpoint. If absent or whitespace, it returns without registering providers. If present, it registers a service resource, ASP.NET request instrumentation, Npgsql spans, metrics and logging export. The configured OTLP endpoint is the destination.

Request instrumentation observes framework activities. Npgsql instrumentation observes the provider's own activity source. This avoids sprinkling timing code around each repository. Correlation middleware can enrich the current request activity while retaining the existing structured log property and response reference.

#### How THIS project implements it

`TelemetrySetup.cs:30` reads `Otel:Endpoint`; lines 31-34 disable export entirely when unset. Lines 37-39 create the service resource named `tutoring-centre-api` with assembly informational version. Lines 41-48 register ASP.NET tracing, exclude paths under `/health`, add Npgsql instrumentation and configure OTLP. Lines 49-51 register ASP.NET metrics. Lines 53-60 add OpenTelemetry logging with the same resource.

```csharp
// Real trace configuration, TelemetrySetup.cs:41.
.WithTracing(tracing => tracing
    .AddAspNetCoreInstrumentation(options =>
        options.Filter = context => !context.Request.Path.StartsWithSegments("/health"))
    .AddNpgsql()
    .AddOtlpExporter(otlp => otlp.Endpoint = endpointUri))
```

`TelemetrySetupTests.cs:11` checks no endpoint yields no `TracerProvider`; line 23 checks an endpoint yields one. Those tests prove registration behavior, not delivery to an external collector or the full content of emitted telemetry.

#### Execution example

An operator configures `Otel__Endpoint` for a local OTLP viewer and starts the API. A staff action reaches correlation middleware, which associates its request reference with logs and the current activity. ASP.NET instrumentation records request timing. A repository query causes Npgsql instrumentation to emit a database span. Exporters send the collected signals to the configured endpoint. The operator can use a client-visible correlation reference to investigate associated evidence if the collector receives and indexes those fields.

The final collector lookup is not exercised by the two setup tests. The operations documents explicitly mark independent trace lookup as not verified in their historical manual session, so this report retains that limit.

#### Why this project needs it

Commands now contain authorization, database work, auditing and sometimes lock contention. A single elapsed request log can say work was slow but not show which operation dominated. Tracing supplies that additional structure while keeping monitoring ownership at the composition root.

#### Alternatives

Serilog-only logging is simpler and remains useful here, but does not provide a complete timed operation tree. A vendor SDK can integrate deeply but ties code to one backend. Custom `ActivitySource` spans can add business detail; current setup primarily uses framework/provider instrumentation. Azure Monitor is documented as a future destination, not an active cloud service.

#### Important details

The repository comments state that Npgsql records statement text without parameter values, and this setup adds no parameter-enrichment callback. That does not by itself prove every SQL literal is harmless; sensitive values should still be parameters. Export is opt-in, but a malformed nonempty endpoint can fail when constructing its URI.

There is no live cloud telemetry result in this audit. Resource registration and source configuration are established. Delivery, retention, sampling and collector access controls require a running destination and remain operational questions.

#### What I should understand before modifying this code

Keep telemetry packages outside Domain and Application. Preserve body and parameter-value exclusion. Test provider registration when changing the opt-in switch. If adding custom spans, identify exactly which operation they explain and avoid attaching personal data. Verify collector delivery separately from compilation and registration.

<a id="infra-cloud-operations"></a>
## 6.74 Deployment design, revision identity, rollback and honest operational status

#### What it means

Deployment changes the revision serving users. Delivery automation coordinates image creation, schema preparation, traffic eligibility and verification. Rollback changes which application revision serves traffic again; it does not automatically undo database changes. Operational documentation must distinguish an implemented local mechanism from an unprovisioned cloud design.

OIDC federation is an identity design in which GitHub presents a short-lived signed token to Azure, and Azure trusts only an explicitly configured repository/environment identity. It avoids a long-lived client secret stored in GitHub. In this repository that cloud trust is an accepted design, not a provisioned integration.

#### The problem it solves

A successful build can still deploy the wrong image, start before migration or pass health while sign-in is broken. A naive rollback can restore old code that no longer understands a changed schema. A design document written as though resources already exist gives a returning developer false confidence about production state.

```text
Hypothetical unsafe sequence:
  update app image
  hope schema is compatible
  mark deploy complete when process starts

Repository's designed sequence:
  build tagged image
  migrate successfully
  update revision
  wait for readiness
  smoke-test expected revision
  restore previous traffic on smoke failure
```

The latter sequence appears in `docs/architecture/deployment.md` and ADR 0009. Only its local/image/smoke pieces are currently implemented.

#### How it works

The build embeds a source revision in assembly metadata. System info exposes the resulting version string. Smoke compares the suffix with the expected commit. A deployment gate can therefore verify identity as well as functionality.

Migration runs under owner authority before new code serves traffic. Runtime uses the unprivileged login. A previous image can be selected again only if the current schema remains compatible with it. Expand-and-contract schema changes are one way to preserve that compatibility, but this repository has no automated production rollback workflow yet.

#### How THIS project implements it

`Dockerfile:23` accepts `COMMIT_SHA`; the publish command passes it as `SourceRevisionId`. `smoke/tests/smoke.spec.ts` asserts the reported version ends with `+${expectedSha}`. CI builds and tests both an image and the complete local production stack.

`docs/architecture/deployment.md` explicitly states that Azure resources, OIDC trust and `deploy.yml` are not provisioned. Its Month 3 addendum retains that status. ADR 0009 discusses Container Apps, Registry, managed PostgreSQL, Application Insights and Jobs as design choices. No corresponding live environment is inferred from those names.

`docs/operations/runbook.md` labels cloud commands not executed and records historical local drills separately. It explains that Caddy requires the `app` network alias when swapping an image manually, and that database compatibility bounds rollback safety.

#### Execution example

The implemented CI smoke path builds the current image with the commit SHA, starts migrate and seed, launches app behind Caddy, waits for readiness and checks the expected suffix. That establishes a local production-style packaged revision works under the configured checks.

The intended Azure path would additionally authenticate via federated identity, push to a registry, update a Container Apps revision and control traffic rollback. Those steps need actual resource identifiers, permissions and environment protections. This audit does not claim they ran.

#### Why this project needs it

The code now includes deployable packaging and operational commands. A returning developer needs to know those are usable locally while the public deployment remains unfinished. Keeping this distinction precise prevents accidental reliance on a nonexistent release pipeline or cloud backup/rollback capability.

#### Alternatives

Azure App Service offers a simpler web-app footprint but no equivalent first-class one-shot job design in this ADR's comparison. A VM provides control at the cost of patching, TLS and process management. Stored service-principal secrets would authenticate deployment but require storage and rotation. Those alternatives are documented considerations; none is an active deployment in the current repository.

#### Important details

Historical runbook observations are not results from this session. They can explain intended procedures and known caveats, but current execution results are reported separately. README's branch-protection claim and future required reviewers are external settings, not verified repository facts. No live URL is present or tested.

Operator commands also deserve precise scope. The repository's reset-password CLI accepts a password argument; that is an implemented interface, and command-line exposure must be considered when using it. Re-running seed does not necessarily rotate existing passwords; the runbook explicitly explains this distinction.

#### What I should understand before modifying this code

Identify the serving revision through system info, not only a local Git branch. Review schema compatibility before rollback. Keep owner credentials confined to jobs. Label unexecuted cloud work as such. When cloud provisioning happens, verify each documented identifier and gate against actual resource state before changing its status from designed to built.

<a id="infra-cross-tenant-probes"></a>
## 6.75 Cross-tenant attack probes and proving absence of side effects

#### What it means

A negative security test checks that forbidden behavior does not occur. That requires more than seeing an error status. An endpoint could modify a row and then return 404, so a response-only test would pass while data was corrupted. A stronger test snapshots protected state before and after the attempted operation through a connection that can see the real stored row.

There are also two distinct attacks. A direct resource attack targets another centre's URL. A reference attack targets the attacker's own resource but puts another centre's identifier inside the request. A direct 404 test does not establish that the second case is safe, because the attacker's URL itself is valid.

#### The problem it solves

```csharp
// Hypothetical weak test: the endpoint could mutate first, then answer 404.
Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

// Real CrossTenantProbe.RunAsync checks storage before and after as well.
var before = await snapshotVictimRowAsync();
await SendAndAssertAsync(attackerClient, method, url, body, expectedStatus, expectedCode);
var after = await snapshotVictimRowAsync();
Assert.Equal(before, after);
```

The actual helper is `tests/TutoringCentre.Api.Tests/Fixtures/CrossTenantProbe.cs:18`. Its snapshot callback is required, not optional.

#### How it works

Arrange two centres with known data. Sign in a real attacker client in one centre. Capture the victim's stored state through a fixture SQL snapshot. Send the attempted cross-centre request with an actual antiforgery token when unsafe. Check exact HTTP status and, where required, the stable Problem Details code. Capture the victim state again and compare it.

For a reference attack, snapshot both attacker's and victim's rows. A bad relationship could corrupt the attacker's row without changing the victim at all, so observing only the victim would miss it. After the request, assert neither snapshot changed.

#### How THIS project implements it

`CrossTenantProbe.RunAsync` implements direct attacks. `RunReferenceAsync` starts at line 45 and requires separate attacker and victim snapshot callbacks. Its private request helper serializes optional JSON bodies, fetches CSRF tokens for non-GET requests, sends the request, checks status and parses the `code` property when an expected code was supplied.

Features use dedicated probe test files: subjects, students, parents, staff, teachers, groups, enrollments and schedules have cross-tenant cases under API tests. Infrastructure tests independently check row policies, tenant filters and structural integrity, so the HTTP probe is not the only evidence.

`CrossTenantProbeSelfTests.cs:23` deliberately arranges a supposedly attacking client that is actually in the same centre and expects the probe to fail rather than pass silently. This is a control for the test helper itself: if the probe accepted the legitimate mutation as a successful attack refusal, the security harness would be untrustworthy.

#### Execution example

A Nile owner edits a Nile group but supplies a Maadi teacher identifier. A direct request to a Maadi group would simply be hidden and return 404, which cannot establish reference validation. `RunReferenceAsync` instead targets the Nile group, captures both sides, submits the foreign teacher id, checks the expected refusal, then verifies the Nile group still points to its original teacher and the Maadi teacher remains unchanged.

The guarantee comes from the entire chain: endpoint permission, dispatcher tenant context, scoped lookup and the database's composite relationship. The probe observes behavior without assuming which individual layer blocked the attempt. Independence tests elsewhere establish the separate layers.

#### Why this project needs it

The academic model now has many identifiers in bodies: student, parent, subject, teacher, group and enrollment references. Tenant isolation must protect these links, not only list queries. Before-and-after snapshots make those protection claims concrete.

#### Alternatives

An external security scanner can discover broad classes of issues but may not understand centre-specific relationship invariants. Assertions against a fake repository inspect handler decisions but do not establish storage effects. Transaction logs or direct SQL counts can supplement snapshots when a request affects several rows. The repository's generic helper keeps the common HTTP and side-effect proof reusable while each feature supplies the meaningful snapshot.

#### Important details

Snapshots must include every field whose mutation matters. An unchanged name does not prove an unchanged relationship. Use the owner/superuser assertion connection so tenant filtering cannot hide corrupted data. A refused request may still legitimately emit logs; the invariant is no unauthorized business mutation, not literally no work of any kind.

The helper treats all non-GET methods as needing CSRF in its current implementation. That is appropriate for its mutation probes but is not a complete general model of HTTP safe methods. Do not reuse it for a new HEAD or OPTIONS experiment without understanding that distinction.

#### What I should understand before modifying this code

Add direct and reference attacks separately. Choose independent snapshots for both affected sides. Keep the helper self-test. Assert specific failure codes when information-hiding behavior is part of the contract. Do not conclude a negative security test proves atomicity merely because it received an error response.

# Testing reference for main section 12

## What each backend project actually verifies

The Domain project tests data rules without HTTP or PostgreSQL. `Common` covers entity identifiers, Result/Error invariants, Egyptian phone normalization and search normalization. `Identity` covers membership changes, the permission catalogue, the role-permission table and teacher-scope decisions. `Academics` covers subjects, students, parents, teachers, groups, enrollments and scheduling objects. Scheduling tests include generated occurrences and daylight-saving boundaries. These tests establish deterministic rule behavior; they do not prove ORM mapping or API binding.

The Application project tests orchestration. Academics handler tests control read answers and repository state to explore missing records, invalid transitions and allowed operations. Dispatcher-focused files check authorization and tenant prerequisites independently of entity rules. Staff tests cover creation, listing, role changes, deactivation, reactivation and validators. Centre tests cover settings and centre-local calendar behavior. Audit tests cover query validation and authorization. Security tests cover actor/correlation state and teacher scope. Fakes are per-interface adapters whose state is visible in the test rather than generated through a mocking DSL.

The Architecture project has structural coverage beyond references. It inspects handler cardinality and visibility, query-handler dependencies, repository placement, API-layer use of Infrastructure, Identity containment, raw-SQL/filter bypass, tenant model keys and filters, request tenant markers, permission declarations and module boundaries. It is executable documentation of where a new feature may place each concern. Its tests cannot prove algorithmic correctness or production settings.

The Infrastructure project exercises storage with real PostgreSQL and real migrations. Tenant tests isolate EF filters, write guards, transaction-local settings, RLS and composite references. Persistence tests cover read-only commands, transactions, migration state, expected error translation, options and role grants. Academics tests cover rows, use-case dispatch, audit trails, cross-centre behavior, teacher scope, races, stale versions, last-seat capacity, transfers, schedule exclusions and DST persistence. Staff tests exercise account and membership interactions, including owner-related races. Audit tests verify diff/masking policy, reads and immutability. Time tests use a fake time provider for conversion behavior.

The API project exercises the real HTTP host with PostgreSQL where required. It covers authentication, CSRF, rate limits, password change, centre settings, staff, audit and the full Academics surface. Newer suites verify runtime endpoint classification, literal permission matrices, forwarded headers, proxy validation, security headers, static SPA behavior, persistent Data Protection keys and telemetry registration. CLI suites cover migrate, seed and reset-password. Cross-tenant probes assert both refusal and unchanged storage.

This division is intentional. A module feature should not need to reproduce its entire test matrix in each layer. Instead, each layer adds the proof available only at that boundary: rules, orchestration, storage semantics, HTTP semantics and browser behavior.

## Coverage limits that remain after reading the tests

The suite contains targeted concurrency tests, but that is not a broad performance or capacity benchmark. Ten or fifty contenders for a seat reveal a correctness race; they do not establish throughput under a sustained mixed workload. There is no evidence in these test files of a production workload model, latency objective or long-running soak test. A returning developer should keep those claims separate.

PostgreSQL integration tests establish behavior for the configured `postgres:17` test image and the test roles. They do not prove a managed cloud database has identical extension availability, network rules, certificates, backups or connection limits. In particular, the AddSchedule migration's PostgreSQL extension needs must be accounted for when cloud provisioning eventually happens.

Persistent-key tests establish that a cookie issued by one host is accepted by another sharing its database. They do not test every operational lifecycle: backup restoration, lost key rows, key encryption at rest, disaster recovery or a mixed-version rolling deployment. Those remain infrastructure questions even though the basic cross-host ticket proof is now present.

Telemetry tests establish whether a provider is registered for a configured endpoint. They do not verify collector delivery, a correlation query in a real viewer, retention, access control or whether a cloud exporter is configured. Historical operation notes explicitly mark trace lookup as unfinished. Avoid turning a registration test into a claim that observability is fully operational.

HTTP security tests verify chosen cases of roles, tenant references and session changes. A passing matrix is meaningful because expected cells are explicit, but it is not a formal proof that all combinations of malformed inputs and concurrent identity changes are safe. The inventory helps ensure new routes are classified; it does not enumerate every request payload.

The browser project uses Chromium. The configuration does not include Firefox or WebKit projects. Logical CSS and Arabic flows are exercised, but semantic assertions and `dir` checks do not replace a full visual regression suite across screen sizes, fonts and browser engines. A screenshot-free credential-sensitive journey also intentionally trades diagnostic media for reduced credential retention.

The smoke test targets an already-running instance and verifies only its essential path. It does not create or edit academic records, intentionally. Its first-row visibility check can succeed on a header row. A successful smoke run therefore means its five explicit assertions passed; it should not be described as exhaustive feature validation.

## How to select tests when returning to a feature

For a domain-rule edit, start with the entity test class. Read its boundary cases before changing the factory or transition method. If the rule changes a stored invariant, inspect the matching configuration and migration tests next. Changing only a domain error code can still require API translation and bilingual frontend updates because codes are contract keys.

For a handler edit, start with its handler tests and the corresponding dispatcher permission/tenant tests. Check whether a fake supplies answers that the real read service can produce. If the handler relies on a database lock or optimistic version, run the actual use-case/race tests; a fake cannot validate the SQL protocol.

For a relationship edit, inspect both structural and access tests. Composite keys protect references at rest, while query filters and RLS protect access. Add a reference-attack HTTP probe in addition to direct foreign-resource access. Use snapshots that include the relationship fields so a 404 cannot hide mutation.

For a new endpoint, add operation metadata, inventory classification and permission-matrix coverage. Then choose focused HTTP assertions for binding, success and stable failure codes. Do not copy the whole authentication test suite into every endpoint class; rely on shared middleware tests while proving the new route is wired to them.

For a migration or grant edit, run real PostgreSQL tests under the runtime login. Review the Down method as code even when rollback is not executed. Verify model/snapshot agreement through the EF tool. If the change affects production images, run the image migration gate as well as the ordinary source build.

For a deployment or proxy edit, run static-file, forwarding, security-header and readiness tests, then exercise the local production stack smoke path. The development Vite proxy is not a substitute for Caddy's TLS termination and forwarded headers. Preserve distinct checks for loopback HSTS behavior and real-hostname requirements.

# Infrastructure reference for main section 13

## File map and ownership

| Path | Type | Responsibility | Inputs | Consumers |
| --- | --- | --- | --- | --- |
| `global.json` | configuration | SDK feature line and roll-forward policy | SDK installation | local CLI, CI, Docker SDK stage |
| `Directory.Build.props` | configuration | net10.0, nullability, implicit usings, warnings-as-errors, analyzer level | MSBuild | all .NET projects |
| `Directory.Packages.props` | configuration | central NuGet versions | package choices | source and test projects |
| `.config/dotnet-tools.json` | configuration | local dotnet-ef version | tool restore | developers, CI model check |
| `Dockerfile` | Docker/infrastructure | frontend and backend build, runtime image | source, manifests, commit SHA | container and smoke jobs, local production stack |
| `.dockerignore` | configuration | build-context exclusions | repository layout | Docker build |
| `compose.yaml` | Docker/infrastructure | local PostgreSQL, bootstrap mount, persistent volume | `.env` variables | ordinary development and production override |
| `compose.prod.yaml` | Docker/infrastructure | ordered jobs, runtime app and Caddy, fixed network | base Compose, image build, secrets | local production-style stack and CI smoke |
| `Caddyfile` | Docker/infrastructure | localhost TLS termination and app proxy | Caddy internal CA, app network alias | browser requests to port 8443 |
| `db/bootstrap-roles.sh` | script | create/converge owner/runtime roles | administrator connection, role passwords | fresh Compose DB, integration fixtures, CI service DB |
| `database/demos/week4-rls.sql` | database/schema demonstration | manual demonstration material for RLS | a suitable database | developer learning; not app startup |
| `.github/workflows/ci.yml` | CI/CD | five correctness and packaging jobs | Git revision, pinned manifests | pull requests and main pushes |
| `.github/workflows/codeql.yml` | CI/CD | C# and JS/TS static analysis | source build | GitHub security findings |
| `.github/workflows/secret-scan.yml` | CI/CD | full-history secret scan | Git history | pull requests and main pushes |
| `.github/dependabot.yml` | configuration | scheduled dependency update proposals | NuGet, frontend npm, Actions, Docker | repository maintenance |
| `smoke/package.json` | configuration | smoke test commands and range dependency | npm | deployed/local stack smoke runner |
| `smoke/package-lock.json` | lockfile | exact smoke Playwright dependency graph | npm resolution | `npm ci` |
| `smoke/playwright.config.ts` | configuration | target-only Chromium smoke, no media capture | `SMOKE_*` environment | smoke tests |
| `smoke/tests/smoke.spec.ts` | test | five essential instance checks | target, expected SHA, demo credential | CI smoke and future release gate |
| `docs/architecture/deployment.md` | documentation | built-versus-designed topology and secrets inventory | implementation and planned cloud design | returning developers and operators |
| `docs/operations/runbook.md` | documentation | operational procedures with execution status | local mechanisms and future cloud resources | operators |

## Configuration keys and the boundary that reads them

`POSTGRES_DB`, `POSTGRES_USER` and `POSTGRES_PASSWORD` belong to Compose/PostgreSQL initialization. `TUTORING_OWNER_PASSWORD` and `TUTORING_APP_PASSWORD` are passed to the bootstrap script. They do not become .NET connection strings automatically. README instructs the developer to provide both .NET strings separately through user-secrets. The local `.env` file is not automatically an ASP.NET configuration provider.

`ConnectionStrings:Postgres` is the runtime SQL login. Its environment form is `ConnectionStrings__Postgres`. `ConnectionStrings:PostgresMigrations` is the owner connection, passed only where schema operations are needed. Keeping these names distinct is essential: a healthy connection as owner is still the wrong runtime configuration. Readiness checks that assumption.

`Seed:Password` supplies the development/demo password used to create missing staff accounts. `Demo:Enabled` opts Production into demo seeding and is also surfaced by system info for the frontend banner. `DEMO_SEED_PASSWORD` is Compose's external value for the seed job; it is not the name the .NET service reads directly. `SEED_PASSWORD` is the browser suite's own input. The two test layers need matching values because they are different processes with different readers.

`Proxy:KnownNetworks` binds a collection of trusted CIDR networks. Compose supplies element zero through `Proxy__KnownNetworks__0`. Production validation requires a nonempty list; every element must parse. This key concerns the immediate reverse-proxy network, not a list of every allowed browser address. Client IP derivation then reads one forwarded hop.

`SessionValidation:CacheDuration` controls positive session-check caching and defaults in the options class. API fixtures set it to zero to avoid a sixty-second timing window in revocation tests. This does not change cookie lifetime. The cookie handler's own framework time remains real even when scheduling tests replace `IClock`.

`Auth:LoginRateLimit:PermitLimit` is configurable and CI e2e raises it to 100 for repeated logins from one runner. `Otel:Endpoint` enables OTLP telemetry only when nonempty. `API_TARGET` belongs to the Vite Node process and selects the development proxy destination; it is not a browser-bundle secret. `ASPNETCORE_HTTP_PORTS=8080` belongs to the runtime image's listening configuration.

`SMOKE_BASE_URL` selects the already-running instance. `SMOKE_EXPECTED_SHA` binds success to a revision. `SMOKE_PASSWORD` is read into the sign-in field without capture media. `SMOKE_EXPECT_HSTS` defaults true; the local loopback stack explicitly sets false. These keys are not ordinary application runtime settings; they configure the checker.

## Current versions verified from configuration

The SDK pin is 10.0.401 with `latestFeature` roll-forward. The shared target is net10.0. Central Microsoft EF/Identity/OpenAPI/Data Protection and related extension packages are 10.0.13 in `Directory.Packages.props`; dotnet-ef remains 10.0.12 in the tool manifest. Npgsql EF provider is 10.0.3, naming conventions 10.0.1, FluentValidation 12.1.1, Serilog.AspNetCore 10.0.0, Testcontainers.PostgreSql 4.15.0, Respawn 7.0.0, xUnit 2.9.3 and runner 4.0.0. These are configured package versions, not a claim that every transitive dependency has the same line.

OpenTelemetry hosting/export packages are 1.15.3, ASP.NET instrumentation 1.16.0, and Npgsql.OpenTelemetry 10.0.1. The health-check package remains 9.0.0. Their differing version lines do not by themselves prove incompatibility; actual build/test outcomes are a separate observation.

The Docker frontend build image is Node 22.12 Alpine. The backend build image pins SDK 10.0.401, while the final runtime image uses the ASP.NET 10.0 tag. PostgreSQL uses major tag 17, and Caddy uses 2.9-alpine. Smoke's package range is `@playwright/test ^1.63.0`, while its lockfile resolves 1.64.0 with matching Playwright/core 1.64.0. CI Node configuration uses `lts/*`, so do not infer its exact future runtime version from that moving selector.

## Changes from the v2 infrastructure and test account

The old overview described no Dockerfile, app container, reverse proxy or deployment packaging. Those claims are now false: the repository has a multi-stage image, local TLS stack, migration/seed job ordering, SPA hosting and image/smoke CI gates. Cloud deployment still remains unprovisioned, so replacing the old absence claim with an unqualified claim of deployed production would be another error.

The old overview described Data Protection persistence as future work. The current repository persists keys in PostgreSQL and includes a two-host cookie-acceptance test. The old overview described forwarded headers and security headers as missing; setup and HTTP tests now exist. The old overview also described only a connectivity readiness probe; current readiness checks runtime-role privilege as well.

The architecture suite has grown from layer/reference checks into tenant-model, marker, raw-SQL, permission and module-boundary rules. Source comments retaining the old no-tenant-entity state are stale; they must be read beside the current model. The test suite now contains academic and scheduling families rather than only centre/identity foundations.

The browser suite grows from three authentication journeys to nineteen declarations across ten files. The separate five-check smoke project now targets a built running instance. Current execution results belong in the test-run record; no historical count or past runbook pass is reused as if it were observed today.

ADR 0003 now explicitly gives reasons for the hand-written dispatcher, including visible pipeline ownership and its comparison with MediatR. The old overview's statement that the repository does not say why is no longer accurate. ADR 0004 has an addendum explaining `SaveChangesAsync` returning `Result`, closing the earlier inability to represent expected persistence conflicts as ordinary failure values.

## Verified findings and operational questions

1. `docs/architecture/deployment.md` claims Caddy is the only published container and PostgreSQL is internal-only. `compose.yaml` publishes `5432:5432`, and `compose.prod.yaml` does not remove that inherited mapping. The claim is false for the merged local production stack. This is a documentation inconsistency and a real local exposure choice; no network exploit is inferred.
2. `Program.cs:29` says no connection occurs during OpenAPI generation. The observed Release build logged a failed PostgreSQL Data Protection key-ring read against placeholder database `openapi_generation`, then generated the document and completed successfully. The generation guard therefore does not suppress every startup side effect. The run's final MSBuild summary had zero warnings and errors; log errors and build diagnostics are different signals.
3. `TenantModelTests.cs` and ADR 0006 retain comments saying no production tenant-owned entities or relationships exist. Current Academics mappings and tests contradict that historical state. Keep their design explanations, but do not reuse those statements as current inventory.
4. `.github/dependabot.yml` has an npm update entry only for `/frontend`, although `smoke/` owns another manifest and lockfile. Smoke dependency updates are not covered by that npm entry. This is a maintenance coverage observation, not a vulnerability assertion.
5. `smoke/tests/smoke.spec.ts` asserts visibility of the first table row after opening Subjects. A header row can satisfy that assertion. The current test therefore does not explicitly establish that a subject data record rendered, despite prose describing that intent.
6. `ApiFactory.cs:58` and `PostgresFixture.cs:78` use different Respawn schema lists. The API list omits audit while the infrastructure list includes it. The difference is verified; a claim of leaked audit rows requires an additional behavior experiment, so this report does not label that effect proven.
7. Forwarded-header setup's empty-list comment needs framework/runtime verification. Existing trusted/untrusted tests configure a nonempty network. This is an open question here until a direct empty-list experiment or inspected framework implementation establishes behavior.
8. Cloud resource state, GitHub branch protection, required reviewers, live telemetry delivery, public TLS/DNS, cloud backups and actual production rollback cannot be established from local repository files. Deployment documentation explicitly marks the cloud work unprovisioned; no external state is fabricated.
