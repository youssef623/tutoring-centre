# Backend teaching material for the current repository

This working chapter records implementation evidence inspected during this session. Source references use repository-relative paths and one-based line numbers. Code marked hypothetical teaches an alternative; it is not a quotation from the repository. Where a reason is inferred rather than documented, the distinction is explicit. Runtime results are not implied by the existence of tests.

<a id="backend-architecture"></a>
## 6.1 Clean Architecture, dependency inversion, and the composition root

#### What it means

A dependency is something a piece of code needs to compile or execute. A handler that directly constructs a database context depends on that context's implementation, provider, configuration, and lifetime. Dependency inversion changes ownership of that relationship. The handler's layer declares an interface describing the capability it needs. An outer layer implements that interface. The handler can call the implementation at runtime while its compiled assembly refers only to the interface.

Clean Architecture applies this distinction to whole projects. Business objects describe valid business state. Application handlers coordinate an operation. Infrastructure supplies databases, Identity, and other adapters. The API translates HTTP into application calls and assembles the implementations at process startup. A composition root is that assembly point. It chooses which concrete object fulfills each interface; business handlers do not make that choice.

#### The problem it solves

Consider this hypothetical endpoint:

```csharp
app.MapPost("/students", async (StudentBody body) =>
{
    var db = new AppDbContext(/* connection settings */);
    // Validate names, find parents, create rows, and decide HTTP status here.
    await db.SaveChangesAsync();
    return Results.Ok();
});
```

The endpoint owns HTTP, business rules, persistence, construction, and transaction management. A CLI cannot reuse the operation without also importing HTTP concerns. A unit test must construct a database context merely to check a business decision. Repeating this style across staff, enrollment, and scheduling spreads the same infrastructure decisions through every feature.

The current project instead separates a command, its handler, the domain objects it changes, and interfaces for loading and storing those objects. For example, `EnrollStudentHandler` receives `IGroupRepository`, `IStudentRepository`, and `IEnrollmentRepository`; its constructor contains no EF Core type (`src/TutoringCentre.Application/Academics/Enrollments/Commands/EnrollStudent/EnrollStudentHandler.cs:19`). That is a practical boundary, not just a naming convention.

#### How it works

The compiler knows an interface has particular methods. An object implementing that interface can be passed to any consumer requiring the interface. A call through the interface dispatches to the actual object's implementation. Consequently, the compile-time reference direction and the runtime call direction can differ. Application can compile against its own repository port while the runtime repository object comes from Infrastructure.

Composition happens before requests execute. Registrations tell the container that resolving a repository interface should construct a particular Infrastructure class. Constructor parameters recursively describe the rest of the object graph. The handler does not locate a connection string, build a provider, or decide whether a repository is real or fake.

#### How THIS project implements it

`Program.cs:68` calls `AddApplication().AddInfrastructure(builder.Configuration)`. Infrastructure registers repositories and read services at `DependencyInjection.cs:124` through `:157`, Identity adapters at `:177` through `:179`, and the unit of work at `:122`. Application owns the `IUnitOfWork` port consumed by `Dispatcher`; Infrastructure implements it over EF Core (`src/TutoringCentre.Infrastructure/Persistence/UnitOfWork.cs:14`).

The context's mapping remains outside Domain. `AppDbContext.OnModelCreating` calls `ApplyConfigurationsFromAssembly`, so persistence rules are discovered from Infrastructure configuration classes (`src/TutoringCentre.Infrastructure/Persistence/AppDbContext.cs:43`). Domain entities such as `Enrollment` express state transitions without importing EF Core. Its `End` method accepts an instant and reason and returns a `Result`; it does not open a transaction or execute an update (`src/TutoringCentre.Domain/Academics/Enrollment.cs:70`).

#### Execution example

An enrollment request reaches its API endpoint and is dispatched as `EnrollStudentCommand`. `Dispatcher.SendAsync` validates the command, checks tenant selection and permission, resolves the handler, and opens a transaction (`Dispatcher.cs:41`–`:70`). The resolved handler receives repository implementations constructed from the same scoped context. It loads and locks the group, checks the student, asks the group whether a seat remains, constructs an enrollment, and calls the enrollment repository's `Add` method. That method only attaches the entity to the context (`EnrollmentRepository.cs:44`). The dispatcher then asks the unit of work to save and commit. HTTP mapping runs after the application result returns.

Notice the two directions. The handler calls outward into a repository object, but the source dependency still points inward because the handler names the Application interface. A test can replace the repository object without changing the handler's business orchestration.

#### Why this project needs it

Current features share several mechanisms: tenant isolation, permission checks, optimistic concurrency, transaction management, and auditing. Keeping these mechanisms outside individual domain methods permits pure domain rules and lets one dispatch pipeline wrap many use cases. Staff creation also illustrates why adapters matter: Application requests an account through `IStaffAccountService`, while Infrastructure uses `UserManager<ApplicationUser>` and joins its writes to the existing transaction (`StaffAccountService.cs:9`).

#### Alternatives

A small application can place all code in one project and rely on discipline. A conventional service layer can depend directly on EF Core and expose business methods. A vertical-slice design can put endpoint, handler, persistence, and tests together by feature. These approaches reduce indirection but place different weight on compilation boundaries. The repository's current implementation chooses separate ports and adapters. Do not infer that every alternative would be wrong or untestable; the important comparison is which boundaries the code makes explicit.

#### Important details

An interface alone does not prove decoupling. Its parameters and return types must also avoid exposing the implementation. A port returning `IQueryable<ApplicationUser>` would still expose persistence and Identity decisions. Current read adapters project to application DTOs rather than returning tracked entities. Conversely, an entity's lack of EF imports does not mean EF cannot populate it: persistence uses mapping and materialization separately from its public factory.

The composition root is deliberately allowed to know concrete layers. Removing all Infrastructure knowledge from `Program.cs` would leave no place to choose adapters. The design goal is concentrated knowledge, not the absence of concrete types everywhere.

#### What I should understand before modifying this code

Follow an existing feature across its boundary before introducing a new abstraction. Put a persistence capability in an Application port when a handler needs it; implement it in Infrastructure; register it once. Keep domain state rules independent of HTTP status and database operations. When a change requires several entities, understand whether the existing dispatch transaction should own all of them. Do not create an independent context inside the handler: doing so breaks the shared transaction, interception, and audit behavior explained below.

<a id="backend-di"></a>
## 6.2 Dependency injection, lifetimes, and shared scope ownership

#### What it means

Dependency injection means an object receives its collaborators rather than constructing them. A DI container stores construction recipes and resolves them recursively. A lifetime defines reuse: singleton means one object for the provider's lifetime; scoped means one object within a scope; transient means a new object for each resolution. Disposal follows the ownership established by these registrations.

The essential idea is ordinary constructor injection. A container automates calling constructors; it does not remove those constructors or make dependencies optional. `UnitOfWork(AppDbContext db, ILogger<UnitOfWork> logger)` still states exactly what a unit of work needs (`UnitOfWork.cs:20`). Passing a different context changes the object it operates on.

#### The problem it solves

Without DI, each endpoint could build its own context, repositories, clock, interceptor, and handler. Two repositories might accidentally receive different contexts. Then a transaction opened by one object would not cover the other's write. Hand-written lifetime management could also keep a context alive across requests or dispose it before asynchronous work finishes.

With DI, Infrastructure registers `AppDbContext` once and the adapters receive that scoped registration. The unit of work and repositories therefore cooperate on one tracked set of changes. The lifetime decision is part of correctness, not merely an allocation optimization.

```csharp
// Real registrations, Infrastructure/DependencyInjection.cs:121-125.
services.AddScoped<IUnitOfWork, UnitOfWork>();
services.AddScoped<ICentreRepository, CentreRepository>();
services.AddScoped<ICentreReadService, CentreReadService>();
```

#### How it works

Registration creates service descriptors containing a service type, an implementation or factory, and a lifetime. Building the provider makes those descriptors usable. Resolving a handler finds its registration, selects its constructor, resolves each parameter, constructs the handler, and caches it according to lifetime. A request scope provides a common cache for scoped objects. Resolving the same scoped context several times within that scope returns the same instance.

A singleton must not capture a request-specific collaborator. Such a dependency keeps request state alive for the singleton's lifetime. A context is also not a shared concurrent work queue: using one context across simultaneous requests mixes tracking and transactions. The correct lifetime keeps ownership aligned with one request or explicit operator operation.

#### How THIS project implements it

Infrastructure registers `TimeProvider.System` and `IClock` as singletons (`DependencyInjection.cs:55`–`:57`). Timestamp and tenant-guard interceptors are singleton because their state comes from the context being saved or a thread-safe clock. Audit is scoped because it reads the current actor and correlation context (`:83`–`:88`). These different registrations explain why copying every interceptor registration with one lifetime would be incorrect.

`AddDbContext<AppDbContext>` supplies options through a factory (`:103`). That factory resolves validated database options and the three interceptors. `AppDbContext` separately receives `ICurrentActor`; `CurrentCentreId` reads that actor live rather than caching the centre at construction (`AppDbContext.cs:25`–`:38`). This matters because authentication can resolve infrastructure before login replaces the actor.

The same configuration registers all academic repositories, read services, staff account operations, teacher scope resolution, Identity, and the database-backed Data Protection key ring. The connection available through the registered runtime context is deliberately the runtime connection, while migration construction uses a separate path, as documented at `DependencyInjection.cs:100`.

#### Execution example

A command resolution asks for `ICommandHandler<AddScheduleRuleCommand, AddScheduleRuleResult>`. The container constructs `AddScheduleRuleHandler` with actor, clock, zone provider, group repository, rule repository, and session repository (`AddScheduleRuleHandler.cs:21`). The repositories receive one context. When the session repository takes a transaction-scoped advisory lock, that SQL runs inside the transaction already opened through the unit of work. When the rule repository tracks a rule and the session repository tracks occurrences, the dispatcher's save sees both changes. No handler manually passes the context between these objects; the shared scope establishes that relationship.

#### Why this project needs it

The audit interceptor must observe the same actor as the handler. The tenant query filter must read the same selected centre that the dispatcher checked. Repositories must save through the same transaction that owns row and advisory locks. These guarantees rely on the object graph sharing its scoped dependencies. Singleton clocks and stateless guards avoid unnecessary construction without sharing tenant state.

#### Alternatives

Manual composition can express exactly the same ownership with explicit `new` calls and `using` blocks. It becomes verbose as a feature's graph grows. A service locator lets arbitrary code resolve dependencies late, but hides requirements from constructors. Here `Dispatcher` deliberately resolves the closed generic handler because its type depends on the request. That localized lookup should not become a reason for domain objects to request their own services.

#### Important details

Scoped does not inherently mean HTTP. A CLI can create an explicit scope. Conversely, resolving a scoped graph from the root provider can effectively extend its lifetime and defeat the intended boundary. Shared lifetime also does not permit nested transactions: `UnitOfWork.BeginAsync` rejects an already active transaction (`UnitOfWork.cs:28`). Two handlers independently dispatched inside one scope therefore cannot each own a transaction.

The live actor property protects against early context construction, but it does not make arbitrary actor replacement safe. A scope is still intended to execute as one actor, except for the explicit authentication transition. Never treat the context as something to reuse for several unrelated users.

#### What I should understand before modifying this code

Identify a new service's state and dependencies before choosing its lifetime. Use scoped registration for a service that depends on the context, actor, or correlation context. A singleton interceptor must read per-save context rather than capture one request's identity. If a use case needs multiple persistence adapters, verify that all receive the registered context. If you create a special context outside DI, use the same configurator and supply a deliberately chosen actor; otherwise its guards may differ from runtime behavior.

<a id="backend-dispatcher"></a>
## 6.3 CQRS and the current dispatcher pipeline

#### What it means

CQRS separates commands that change state from queries that read state. It does not necessarily mean separate databases, separate processes, event sourcing, or a message broker. In this repository it means separate request and handler interfaces and different execution rules. A dispatcher is the common entry point that finds a handler and wraps it with cross-cutting steps.

The distinction gives a query a stronger contract than a method whose name starts with `Get`. Queries execute in read-only database transactions. Commands execute in read-write transactions, may track changes, and are saved once by the dispatcher. A request can additionally declare that it needs a selected tenant and a permission.

#### The problem it solves

Without a common pipeline, every endpoint and CLI operation must remember validation, centre selection, permission checks, transaction start, rollback, save, commit, and logging. Omitting one step may compile successfully while creating a security or consistency defect. Putting the same checks into each handler duplicates their ordering and error behavior.

Hypothetical repeated orchestration would look like `Validate; CheckTenant; CheckPermission; Begin; Handle; Save; Commit`, with a separate catch and rollback at every call site. Current callers instead invoke `SendAsync` or `QueryAsync`. The dispatcher determines the sequence, so adding a feature means supplying its message, handler, validator, and declared capabilities.

#### How it works

Generic request and response parameters identify a closed handler interface. For `EnrollStudentCommand`, the dispatcher resolves `ICommandHandler<EnrollStudentCommand, EnrollStudentResult>`. It need not contain a switch statement listing academic features. Validation obtains every `IValidator<TRequest>`, executes them, groups their failures, and returns a structured validation error before opening a transaction.

The current command pipeline is: validate; check tenant scope; check permission; resolve handler; begin read-write transaction; execute handler; rollback on failed result; save; rollback on failed save result; commit on success; log outcome. The query pipeline has the same first checks, then begins read-only, executes, and commits without a save. Exception cleanup uses a non-cancelled token to preserve rollback even if the request was cancelled.

#### How THIS project implements it

`Dispatcher.cs:35` declares the command method, and `:100` the query method. `CheckTenantScope` only activates for `ITenantScoped` (`:152`). It returns `auth.not_authenticated` for anonymous actors and `tenant.not_selected` when an actor has no centre. `CheckPermission` only activates for `IRequirePermission` (`:174`). System actors pass; staff pass only if `RolePermissions.Holds` returns true; other cases return `auth.permission_denied`.

Save failures are now values too. `IUnitOfWork.SaveChangesAsync` returns a `Result`; the dispatcher checks it at `:78`, rolls back, and converts its error into the command's generic result. This is an important change from a pipeline that assumed every expected failure occurred inside the handler. Concurrency and database uniqueness can fail after the handler already returned success.

```csharp
// Real sequence, Dispatcher.cs:78-88.
var saveResult = await _unitOfWork.SaveChangesAsync(ct);
if (saveResult.IsFailure)
{
    await _unitOfWork.RollbackAsync(ct);
    var failedSave = Result<TResponse>.Failure(saveResult.Error!);
    LogOutcome(CommandKind, typeof(TCommand).Name, failedSave, started);
    return failedSave;
}
await _unitOfWork.CommitAsync(ct);
```

#### Execution example

A staff member requests a schedule write. Shape validation first rejects malformed input. The tenant marker then rejects a signed-in user who has not selected a centre. Permission checking rejects a teacher without `schedule.manage`, before any transaction or business handler lookup is needed. An authorized owner reaches `AddScheduleRuleHandler`, which tracks a rule and generated sessions. A collision may only be discovered during save by PostgreSQL's exclusion constraint. The unit of work translates that failure; the dispatcher rolls back the rule and every generated occurrence. The endpoint receives a conflict result even though the handler's earlier return was success.

#### Why this project needs it

Tenant and permission enforcement now protects real product writes. Scheduling demonstrates atomic multi-row work; transfers demonstrate atomic history plus a new enrollment; staff creation combines Identity and membership changes. One pipeline prevents each feature from inventing a different transaction boundary. Outcome logging records request kind and type, success or failure, error code, and elapsed milliseconds without logging request values (`Dispatcher.cs:263`).

#### Alternatives

MediatR behaviors can package a similar pipeline. Service classes can perform orchestration directly. Endpoint filters can perform HTTP-specific validation, but cannot on their own make CLI calls follow the same rules. A full CQRS deployment with a separate read database adds synchronization concerns absent here. The repository does not explicitly state why every detail of the handwritten dispatcher was chosen; the following is an interpretation based on the implementation: keeping its order in one small class makes the security and transaction mechanics directly inspectable.

#### Important details

Marker-based enforcement is opt-in at the application request type. A request lacking `IRequirePermission` does not receive a permission check merely because its handler changes data. HTTP authentication still applies, but it answers a different question. The tenant check also does not itself prove membership; it checks the trusted actor already built from the validated session. Database RLS and write guards provide further protection.

`BeginAsync` runs before the dispatcher's try block. Setup failures must therefore clean up their own resources where appropriate. Queries commit even when their handler returns a failed `Result`, because they have no application save and are database-read-only. Do not mechanically make command and query failure handling identical without considering that distinction.

#### What I should understand before modifying this code

Understand all early exits before adding a pipeline step. Put checks requiring no transaction before `BeginAsync`. Do not make a handler dispatch another command in the same scope; nested units of work are rejected. Keep expected save conflicts as results and unexpected persistence faults as exceptions. When adding a tenant feature, inspect the request markers, permission constant, role matrix, handler, repository, entity mapping, migration, and tests together. A missing marker cannot be repaired by a hidden UI button.

<a id="backend-results"></a>
## 6.4 Results, expected failures, and exception boundaries

#### What it means

An expected failure is a normal possible outcome: a group is full, a name is taken, a requested row is not visible, or an input field is invalid. An exception describes a programming or infrastructure fault that ordinary business branching cannot resolve. A `Result<T>` represents either a produced value or an error; the caller can inspect that outcome without unwinding the call stack.

The distinction is semantic. A database exception can represent an expected race, and a domain-level method can throw for a programmer error. Location alone does not decide classification. Current persistence translation explicitly identifies a few exception families that belong to normal conflict handling and leaves all other faults loud.

#### The problem it solves

If every business refusal throws an exception, callers must know every exception class and remember its mapping. Forgetting to catch a duplicate-name exception turns a normal user outcome into HTTP 500. If every fault becomes a generic failed result, genuine bugs disappear into ordinary validation messages. The project needs both mechanisms with a clear boundary.

```csharp
// Hypothetical exception-based business branching.
try { await Enroll(); }
catch (GroupFullException) { return Results.Conflict(); }

// Real domain refusal, Group.cs:153-155.
if (activeEnrollmentCount >= Capacity)
{
    return Result.Failure(Error.Conflict("group.full", "This group has no free seats."));
}
```

#### How it works

`Result` enforces two legal states in its constructor: success with no error, or failure with an error. An invalid combination throws immediately (`Result.cs:8`). `Result<T>` adds a value that can only be read on success. Reading a failed value throws an `InvalidOperationException` because the caller violated the API contract (`Result.cs:61`).

`Error` carries a stable machine code, developer message, kind, and optional field dictionary (`Error.cs:14`). The kind chooses an HTTP status; the code lets frontend translation choose a specific message. Field keys connect a failure to an input. A code such as `student.parent_already_linked` is therefore part of a cross-layer contract, not an incidental logging string.

#### How THIS project implements it

Domain factories return `Result<Entity>`. State transitions commonly return non-generic `Result`. Application handlers preserve an error while changing the success value into a feature result. The dispatcher preserves that same error through rollback. The API maps it only at the transport boundary.

The persistence translator recognizes `DbUpdateConcurrencyException` first because it derives from `DbUpdateException` (`PersistenceErrorTranslator.cs:31`). It returns `concurrency.stale`. Unique violations use SQLSTATE `23505` and an exact constraint-name catalogue. Exclusion violations use `23P01`. A foreign-key or CHECK failure remains an exception; the translator deliberately does not normalize every database error into a pleasant conflict.

#### Execution example

Two clients edit a versioned enrollment. The first saves. The second supplies the older version. The repository loads the entity and sets the expected original version (`EnrollmentRepository.cs:25`). The domain transition may still be valid in memory. EF then issues an update whose concurrency condition no longer matches, reports zero affected rows, and raises `DbUpdateConcurrencyException`. The unit of work catches it, receives `concurrency.stale` from the translator, and returns failure. The dispatcher rolls back the whole command and returns a conflict through the standard HTTP mapper. No endpoint needs a dedicated concurrency catch.

#### Why this project needs it

Failures now arise at several depths: shape validation, permission checking, entity rules, relationship checks, row versions, unique constraints, and scheduling exclusions. A uniform error value lets the same rollback and HTTP logic handle them. The distinction also protects observability: violating the one-primary-parent constraint remains an unexpected fault because Domain is supposed to make that state impossible.

#### Alternatives

Custom exception types can express business outcomes but require a mapping convention. Nullable returns lose the reason for failure. Discriminated unions can make success and failure branching more explicit, but C# callers still need a consistent project convention. Libraries such as a Result package can add composition helpers; this implementation keeps a small local type. The repository does not explicitly state why every Result design detail was chosen. The following is an interpretation based on the implementation: simple factories and a small closed kind enum make the transport mapping easy to audit.

#### Important details

The compiler does not force every `Result` to be inspected. A handler can accidentally ignore a failed domain transition. Review early returns and test both success and refusal. `Error.Fields` is optional and can appear on conflicts or rules as well as validation; field attachment and error kind are separate axes. Never infer that every field-keyed response must be HTTP 400.

The database catalogue deliberately excludes `ux_student_parents_one_primary` and `ux_class_sessions_rule_date` (`ConstraintErrorCatalog.cs:62`). An unknown unique constraint has a generic duplicate fallback and warning; an unknown exclusion constraint has no fallback. These are intentional different policies, not equivalent catch-all behavior.

#### What I should understand before modifying this code

Classify a new failure according to whether callers can reasonably encounter it, then choose an existing kind and stable code. Keep exception internals out of `Error.Message`. When introducing a unique or exclusion constraint, decide whether violation is a normal business race or proof of a bug, and update the catalogue accordingly. Add translations for a new public code. Preserve the original error when passing it between generic result types so field keys and specificity survive.

<a id="backend-validation"></a>
## 6.5 Validation, invariants, and multiple lines of defense

#### What it means

Validation determines whether input has an acceptable shape. An invariant determines whether business state remains valid. A state-dependent rule compares a proposed change with other persisted records. A database constraint enforces a condition even when application checks race or a different writer accesses the database. These mechanisms overlap intentionally but answer different questions.

For a group, capacity must be within a supported range, but reducing capacity also requires knowing how many active enrollments exist. The first condition is intrinsic to the proposed value. The second concerns concurrent state in other rows. Treating both as one form-field check hides the synchronization requirement.

#### The problem it solves

Without early shape checks, obviously invalid requests open transactions and consume database resources. Without domain checks, a non-HTTP caller can bypass a form validator and create invalid state. Without database enforcement, two individually valid requests can jointly violate uniqueness or overlap rules.

Hypothetical `if (await Count() < capacity) await Insert()` looks like validation but is unsafe unless competing requests serialize the count and insert. The current enrollment handler first locks the group, then performs the count and decision inside the dispatcher's transaction (`EnrollStudentHandler.cs:32`). Validation alone cannot supply this guarantee.

#### How it works

The dispatcher executes registered FluentValidation validators before tenant and permission checks and before transaction start (`Dispatcher.cs:41`). It collects every failure rather than returning the first, groups messages by camel-cased property names, and removes duplicate messages (`:240`). Domain factories then validate business values before construction. Handlers check existence, active state, and relationships using repositories. Database constraints arbitrate concurrent conflicts at save.

The order gives different failure contracts. A malformed shape may produce `validation.failed` with several fields. A validly shaped request that violates an entity rule may produce a more specific code. Both can be normal outcomes; they are not evidence that one layer duplicated the other's entire responsibility.

#### How THIS project implements it

`ScheduleRule.Create` enforces duration from 30 to 240 minutes in five-minute steps (`ScheduleRule.cs:75`). It checks start-time ticks against a five-minute boundary, which also rejects hidden seconds and fractional precision (`:81`). It trims an optional room and limits it to 40 characters. These rules protect construction regardless of transport.

`Group.ChangeCapacity` checks the supported range and refuses a value below the active enrollment count (`Group.cs:107`). The caller supplies that count because it is not a field on the group. `ChangeGroupCapacityHandler` takes the same lock as enrollment, checks the expected version, and then calls the domain method (`ChangeGroupCapacityHandler.cs:17`). This joins validation with the correct consistency boundary.

#### Execution example

An owner attempts to reduce a group's capacity to ten. The request validator first rejects any malformed ID or unsupported shape. Tenant and permission checks pass. The handler acquires the group lock and reads twelve active enrollments. It confirms that the client's version still matches. `ChangeCapacity(10, 12)` returns `group.capacity_below_enrolled` with a `capacity` field message. The dispatcher rolls back without a save. A concurrent enrollment must wait on the same lock and therefore cannot slip between the count and reduction.

#### Why this project needs it

The product now edits state that other features depend on. Archiving a student affects enrollment rules; assigning a teacher affects future sessions; changing staff roles affects authorization. Keeping intrinsic rules in entities and relational checks in handlers makes the source of each decision visible. Database constraints remain necessary because an earlier friendly check does not guarantee a later insert succeeds.

#### Alternatives

Data annotations can express simple shape checks. Endpoint-only validation provides convenient HTTP errors but leaves CLI or direct dispatcher callers exposed. Domain-only validation can preserve correctness while losing aggregated field feedback and cheap pre-transaction rejection. Database-only validation is authoritative but typically provides lower-level messages. The current system combines mechanisms rather than expecting one to replace all others.

#### Important details

User input normalization is also a business decision. Name cleanup and search normalization are different operations: display text can preserve spelling while a separate normalized field supports matching. Phone parsing creates a canonical value before uniqueness comparison. Do not apply a generic trim or lowercasing function to every value and assume equivalent semantics.

Some rules depend on time or external state. The current scheduling handlers calculate today in the centre's zone, not the server's calendar. Their range decisions therefore need both the injected clock and zone provider. Moving that rule into an HTTP validator without those explicit dependencies changes its meaning and testability.

#### What I should understand before modifying this code

Write down what fact a rule needs before choosing its layer. Pure input shape belongs before transaction start. An entity's own state transition belongs on the entity. Cross-row rules need a handler and, when concurrent changes can invalidate them, a lock or constraint. Keep field keys aligned with generated request properties and frontend forms. Do not translate every database invariant failure into validation: some indicate broken application guarantees and should remain observable faults.

<a id="backend-domain"></a>
## 6.6 Entities, value objects, aggregates, and historical state

#### What it means

An entity has a stable identity even when its attributes change. A value object is identified by its contents and is typically immutable. An aggregate is a consistency boundary: one root controls a group of related objects and enforces invariants across them. These ideas solve different problems. A student is an entity; a canonical phone number is a value; the student's parent links form a small aggregate managed through the student.

Encapsulation means callers cannot arbitrarily rewrite state. Factories establish valid initial state. Transition methods determine which changes are permitted. Private setters alone are insufficient unless every legitimate mutation flows through methods that preserve the required relationships.

#### The problem it solves

Publicly mutable links could leave a student with no parent, two primary parents, or duplicate links. Mutating an enrollment's group ID would erase where it originally belonged. Editing a recurring rule in place would make already generated sessions appear to come from a rule that no longer describes them.

Hypothetical `enrollment.GroupId = targetId` is shorter than a transfer operation, but destroys history. The current model ends the old enrollment with reason `Transferred` and creates a new enrollment in the destination (`TransferEnrollmentHandler.cs:78`). The extra row preserves the business event rather than overwriting it.

#### How it works

`Entity` assigns a UUIDv7 during construction (`Entity.cs:6`). Domain factories validate before returning an object. The EF materialization constructor is separate: persistence can recreate an existing row without pretending to execute a new business operation. Transition methods accept all needed inputs explicitly and return results for expected refusals.

Value objects normalize once. `PhoneNumber.Create` maps Arabic digits, strips recognized separators, accepts selected local and international spellings, and returns a canonical E.164 representation (`PhoneNumber.cs:36`). Equality uses that canonical string, so different accepted spellings of the same number compare equal. Display masking is a separate representation and does not change identity.

#### How THIS project implements it

`Student` owns a private list and exposes `IReadOnlyCollection<StudentParentLink>` (`Student.cs:20`, `:56`). Its documented rules require one to four parents, no duplicate parent, and exactly one primary link. `LinksRevision` changes on successful parent-link changes (`:53`), ensuring aggregate-level state can participate in concurrency even when child rows change.

`Enrollment` keeps centre, student, and group fixed after start. `End` refuses a second end and an end before its start (`Enrollment.cs:70`). `ScheduleRule` stores local recurrence and can be ended once, but has no ordinary edit method (`ScheduleRule.cs:99`). `ClassSession` stores UTC instants and a teacher snapshot; reassignment affects only future scheduled sessions (`ClassSession.cs:95`). These are concrete examples of preserving historical truth.

#### Execution example

A transfer loads a versioned active enrollment, rejects a destination equal to the source, locks the destination group, verifies the same subject and available capacity, then obtains one `now` instant from the clock. It calls `current.End(now, Transferred)` and `Enrollment.Start(..., now)`. Both objects are tracked in one context. If any subsequent save fails, both changes roll back. If successful, history contains an ended source enrollment and active destination enrollment sharing the transfer instant.

#### Why this project needs it

Tutoring operations have histories that should not be silently rewritten. Staff need to distinguish withdrawal from transfer. A past session must retain the teacher who taught it even if the group's current teacher changes. Parent-link rules must remain valid regardless of which screen initiates a change. Modeling these constraints on domain objects makes them reusable by current handlers and future delivery mechanisms.

#### Alternatives

An anemic model places all rules in services and exposes mutable properties. Event sourcing records every state transition as an event and rebuilds state, adding more machinery than this row-based history. Value objects could also be represented by records, but a private constructor plus factory is useful when canonicalization can fail. A separate parent-link service could manipulate child rows; it would have to reconstruct the same aggregate invariants currently concentrated in `Student`.

#### Important details

Immutability differs by object. `PhoneNumber` has a get-only value; `Enrollment` can transition from active to ended. A record type does not automatically make all nested collections immutable. A read-only collection interface prevents ordinary callers from using mutation methods through that interface, while the root still owns the mutable list internally.

Database restrictions still matter. Domain methods protect ordinary calls, but materialization and direct database access bypass factories. Conversely, database constraints alone cannot express every aggregate rule conveniently. The unit of work includes explicit reconciliation for removed parent links because EF's required relationship with Restrict does not automatically delete those rows (`UnitOfWork.cs:89`).

#### What I should understand before modifying this code

Identify whether a field describes current configuration or historical fact. Prefer a new transition or history row when overwriting would erase meaning. Do not expose setters to simplify a form submission. For aggregate child changes, update the root's revision consistently and preserve minimum, maximum, duplicate, and primary constraints. Treat canonical phone values as values; use their masked form only for output requiring privacy, never as a uniqueness key or change comparison.

<a id="backend-transactions"></a>
## 6.7 Unit of work, transaction-local tenancy, and cleanup

#### What it means

A database transaction groups work into a commit or rollback boundary. Atomicity means its writes either persist together or are discarded together. A unit of work collects tracked object changes and saves them as one operation. EF already supplies change tracking; this project's port exposes explicit transaction control to Application without exposing EF types.

Transactions also own locks and session-local settings. That ownership is crucial here: enrollment capacity locks, schedule advisory locks, and the PostgreSQL current-centre setting must apply to all reads and writes in one use case, then disappear before a pooled connection serves another request.

#### The problem it solves

Wrapping only `SaveChanges` leaves earlier reads and locks outside the use-case transaction. A transfer could end the source enrollment and fail to create the destination. Staff creation could leave a user without the requested membership. Audit rows could describe changes that were never committed.

Without transaction-local tenant configuration, a pooled connection could retain a previous request's centre setting. The current implementation explicitly sets the value for every transaction and scopes it locally, including an empty value when there is no centre (`UnitOfWork.cs:54`). It never assumes a freshly borrowed connection is already correct.

#### How it works

The dispatcher opens the transaction before calling the handler. The unit of work first rejects nested use. For a query, it issues `SET TRANSACTION READ ONLY` before any other SQL, because PostgreSQL requires that ordering. It then executes parameterized `set_config('app.current_centre', centreId, true)`. The third argument makes the setting transaction-local.

Command save runs EF interception and SQL generation inside that open transaction. Commit persists writes and releases locks. Rollback discards database changes, disposes the transaction, and clears tracked objects so failed in-memory state cannot be accidentally saved later. Exception rollback uses `CancellationToken.None` from the dispatcher so cancellation does not prevent cleanup.

#### How THIS project implements it

`UnitOfWork.BeginAsync` contains the nested check at `:30`, opens at `:36`, configures query read-only at `:45`, and applies tenant state at `:60`. `SaveChangesAsync` first reconciles removed student-parent links, then calls EF and translates expected database failures (`:66`). Commit disposes its transaction in `finally` (`:133`). Rollback takes the current transaction, clears its field, rolls back, disposes, and clears the tracker (`:147`).

```csharp
// Real tenant assignment, UnitOfWork.cs:59-63.
var centreId = _db.CurrentCentreId?.ToString() ?? string.Empty;
await _db.Database.ExecuteSqlRawAsync(
    "SELECT set_config('app.current_centre', {0}, true)",
    [centreId],
    ct);
```

The placeholder is a database parameter, not string concatenation. A centre ID never becomes SQL source. This explicit transaction exists on both query and command paths, so RLS sees the selected centre even when a read adapter contains no handwritten centre predicate.

#### Execution example

An owner adds a schedule rule producing several sessions. The dispatcher starts a write transaction and sets its centre. The handler locks the teacher schedule, tracks the rule, and tracks occurrences. Timestamp and audit interceptors run during save. PostgreSQL finds one generated session overlaps an existing teacher session. Save returns the mapped conflict. The dispatcher rolls back: no rule, no generated sessions, and no audit descriptions of those attempted changes survive. The advisory lock and current-centre setting end with the transaction.

#### Why this project needs it

Real features now change multiple rows and sometimes multiple aggregates. Data isolation is also implemented in PostgreSQL rather than only C#. The transaction provides one boundary for both consistency and tenant settings. Identity account creation uses the same scoped context and existing transaction, despite `UserManager` saving its own changes during the handler (`StaffAccountService.cs:9`). The user row remains provisional until commit.

#### Alternatives

EF's implicit save transaction is adequate when all relevant work occurs in one save and no earlier transaction-local settings or locks are needed. `TransactionScope` supplies an ambient transaction but introduces different lifetime and propagation rules. Distributed sagas coordinate work across independent stores; this application instead uses one context and one database transaction. A transaction per repository call would be smaller but could not guarantee all-or-nothing transfer or creation flows.

#### Important details

Read-only does not mean no transaction. The query path explicitly starts one and commits to end it. PostgreSQL is the backstop against accidental writes, including raw SQL. A transaction also holds resources while the handler awaits; do not put long external network calls inside it without considering connection and lock duration.

Removed parent links require special handling. `ReconcileOrphanedStudentParentLinks` temporarily disables automatic change detection while comparing each tracked student's current link IDs to tracked link entities. It marks missing links Deleted before ordinary detection can report a severed required relationship. A `finally` restores the previous detection setting (`UnitOfWork.cs:106`). This is narrow persistence adaptation, not permission to call global detection switches casually.

#### What I should understand before modifying this code

Know which transaction owns every read, write, lock, and audit row. Never open a second independent context to perform part of an atomic operation. Preserve the first-statement requirement for query read-only setup. Use transaction-local settings rather than persistent connection settings. If a new child-collection mutation changes EF relationship behavior, test actual persistence rather than assuming an in-memory collection change deletes the row. Keep rollback idempotent and clear failed tracking state.

<a id="backend-persistence"></a>
## 6.8 Repositories, DTO projections, and EF Core's execution model

#### What it means

A repository presents domain persistence capabilities such as finding an entity, taking a business lock, or tracking a new aggregate. A read service answers a query with a DTO shaped for its caller. An ORM translates between relational rows and objects. EF's context contains mapping metadata and a change tracker; its provider translates supported LINQ expressions into database operations.

An `IQueryable` is a query description, not the results. Building `Where` and `Select` calls constructs an expression. A terminal asynchronous operation such as `ToListAsync`, `SingleOrDefaultAsync`, or `AnyAsync` executes it. Tracked entity queries support subsequent mutation; `AsNoTracking` projections avoid that mutation machinery when the result is read-only data.

#### The problem it solves

If handlers return entities directly, transport consumers learn private domain and persistence details. If read handlers load full aggregates solely to show a table, unnecessary objects and tracking consume resources. If repositories save independently, the common transaction loses ownership. The current separation lets write handlers call entity methods while read handlers obtain exact display data without exposing EF interfaces to Application.

Without understanding deferred execution, a developer can accidentally load a whole table before filtering. Without understanding tracking, a developer can expect a detached DTO edit to become an update. These are mechanism errors, not merely style disagreements.

#### How it works

`AppDbContext.OnModelCreating` applies mapping classes, an extension point, then the tenant-filter convention (`AppDbContext.cs:43`). EF uses that model to translate queries and map rows. Write repositories return tracked entities; an entity's state change becomes an update during save. New entities become Added when `Add` is called; no insert is sent until save.

Read services select DTO fields and use no-tracking queries. Joins are expressed in LINQ and executed in the database where supported. Some operations deliberately materialize a limited set before computing a value EF cannot translate; those exceptions must be understood from source rather than assumed to be database-side.

#### How THIS project implements it

`EnrollmentRepository.Add` calls `db.Set<Enrollment>().Add` and never saves (`EnrollmentRepository.cs:44`). Its versioned load sets the shadow Version property's original value from the client (`:25`). `GroupReadService.Project` joins groups, subjects, and teachers, returns names, capacity, active enrollment count, status, and version (`GroupReadService.cs:72`). The count comes from enrollment rows, not a second stored counter.

`StudentReadService` builds a filtered and ordered query, takes one extra row for pagination, then loads primary parents for only the page IDs (`StudentReadService.cs:53`). Its phone-fragment search is a documented exception: converted phone properties could not support the attempted SQL LIKE translation, so it loads tenant-scoped phone projections and matches their canonical values in memory (`:118`). This code should not be described as an indexed database substring search.

#### Execution example

A student list query begins as `Set<Student>().AsNoTracking`. If archived rows are excluded, the adapter adds a status predicate. A name search normalizes and escapes the term and adds SQL LIKE. A cursor adds a lexicographic predicate on search name and ID. Ordering and `Take(pageSize + 1)` execute in PostgreSQL. The adapter trims the extra row, fetches primary parent information for page IDs, projects DTOs, and encodes the next cursor. The dispatcher commits its read-only transaction and the endpoint serializes that page.

#### Why this project needs it

The UI now shows joined data: group subject and teacher names, roster student names, student parent contact information, and session history. DTO projections keep those screen requirements out of domain aggregates. Write repositories expose operations with consistency meaning, such as locking a group for enrollment. The common context still allows all adapters to participate in one transaction.

#### Alternatives

Handlers can directly depend on EF, reducing files while weakening the boundary. Generic CRUD repositories reduce repeated code but may hide important operations such as lock-before-count. Dapper or raw SQL gives explicit query control at the cost of manual mapping. Database views can centralize complex read projections. The current approach uses use-case-specific ports and EF implementations, with raw SQL reserved for operations lacking a suitable LINQ representation.

#### Important details

Projection alone does not prove every expression executes server-side. Inspect where `ToListAsync` occurs. `StudentReadService` intentionally performs phone matching after materialization. Likewise, the absence of a manual centre predicate does not mean a query is unscoped: global filters and RLS add isolation outside the local expression. Conversely, bypassing filters or using privileged raw SQL deserves explicit review.

EF mappings include shadow properties such as Version and timestamps. Those fields can appear in DTOs through `EF.Property` without a public domain property. A domain object being free of persistence metadata therefore does not mean the table lacks concurrency or audit support.

#### What I should understand before modifying this code

Locate terminal query operations and identify which work runs in SQL versus memory. Preserve no-tracking projections on read paths unless mutation is intended. Never add `SaveChanges` to an ordinary repository method. Keep version ownership explicit: a client's expected version belongs in EF's original value, not merely a comparison against a DTO. When optimizing a query, preserve tenant and teacher scope predicates and verify that any new raw SQL stays within the dispatch transaction.

<a id="backend-concurrency"></a>
## 6.9 Optimistic concurrency, uniqueness, row locks, and advisory locks

#### What it means

Concurrency means several operations can overlap in time. A race occurs when their interleaving changes the outcome. Four mechanisms in this repository solve different concurrency problems. Optimistic concurrency rejects an update based on an outdated version. Unique constraints reject duplicate keys. Row locks serialize decisions based on a shared record. Advisory locks serialize an application-defined resource, such as a teacher's schedule, even when no single row is the whole resource.

These mechanisms are complementary. A version token cannot enforce a group capacity count if enrollment inserts do not update the same group row. A row lock does not replace a unique index for duplicate enrollment pairs. A friendly pre-check cannot make a later insert immune to races. Understanding which invariant each mechanism protects is more useful than saying that a feature is simply thread-safe.

#### The problem it solves

Imagine two users reading the same group. One changes its capacity while the other changes its name using an old form. An unconditional update can overwrite a newer value. Optimistic concurrency requires the update to match the version originally read. Separately, two staff members can both see one free seat and each enroll a different student. Their enrollment rows have different IDs, so entity row versions alone do not prevent overbooking.

The unsafe hypothetical enrollment sequence is: count seats, see one free, insert. Both transactions can honestly pass the count before either commits. The current handler starts by taking the group lock, so the second transaction waits and reads a count that includes the first committed enrollment. The invariant is enforced across the decision window, not merely checked before it.

#### How it works

Optimistic concurrency stores a version in the data returned to the client. The client submits that expected version with a mutation. EF includes it in the update condition; zero matching rows means another writer changed the row. PostgreSQL's `xmin` system column supplies the underlying token. The project maps it as a shadow `uint` named `Version` so domain objects do not expose database-specific system columns.

A unique constraint checks the database's final state. The handler may first query for a friendly error, but simultaneous writers still rely on the constraint. Row locks instead make competitors wait while one transaction checks and changes shared state. Advisory locks use an arbitrary key agreed upon by application code; they do not themselves validate an overlap, but make conflicting writers acquire resources in a controlled order before exclusion constraints arbitrate.

#### How THIS project implements it

`RowVersionConfiguration.HasRowVersion` maps `Version` to `xmin`, type `xid`, generated on add or update, and marks it as a row version (`src/TutoringCentre.Infrastructure/Persistence/Configurations/RowVersionConfiguration.cs:16`). Versioned repository loads assign the client's token to `OriginalValue`, not to the current stored value. For example, `ClassSessionRepository.cs:27` establishes the expected update version after loading a session.

`PersistenceErrorTranslator` returns `concurrency.stale` for EF concurrency failures. Exact unique names map to feature conflicts: centre slug, subject name, membership pair, parent phone, student-parent pair, teacher account, group name, and active enrollment pair (`ConstraintErrorCatalog.cs:14`). The one-primary-parent and rule-date unique indexes remain deliberately untranslated because the application treats their violation as a broken invariant, not an ordinary conflict.

Teacher schedule writes call `LockTeacherSchedulesAsync`. It de-duplicates teacher IDs, sorts them, creates a namespaced string key, and executes `pg_advisory_xact_lock(hashtextextended(key, 0))` once per teacher (`ClassSessionRepository.cs:60`). Sorting means operations that need several teachers acquire locks in the same order. The transaction owns the lock, so commit or rollback releases it.

```csharp
// Real advisory lock acquisition, ClassSessionRepository.cs:69-73.
foreach (var teacherId in teacherIds.Distinct().Order())
{
    var key = $"teacher-schedule:{teacherId}";
    await db.Database.ExecuteSqlAsync($"SELECT pg_advisory_xact_lock(hashtextextended({key}, 0))", ct);
}
```

#### Execution example

Two commands attempt to fill the final seat. The first acquires the target group's enrollment lock and counts active rows. It passes `EnsureCanEnroll`, tracks a new enrollment, saves, and commits. The second has been waiting on that group. Once released, it counts again and finds capacity reached. `Group.EnsureCanEnroll` returns `group.full`, which propagates through rollback and HTTP conflict mapping. There is no persistent enrolled-count field to increment or repair.

A different race occurs when two users edit one session. Both forms carry the same Version. The first cancellation updates the row. The second command may still load current state but sets the original concurrency token from its stale form. If it produces an update, save affects zero matching versions and becomes `concurrency.stale`. These two examples use different tools because one invariant concerns a collection count and the other concerns a specific edited record.

#### Why this project needs it

Staff management must preserve an active owner. Enrollment must preserve capacity. Group edits must not overwrite one another. Scheduling must prevent teacher and group overlap. Each is a real multi-user operation. A single application process would still need database enforcement because requests overlap, and database constraints remain valid even if another process writes through the same permitted account.

#### Alternatives

Serializable isolation can detect some conflicting transaction histories, but callers must handle retries. A stored counter on groups can make seat updates atomic but introduces a second source of truth. `INSERT ... ON CONFLICT` can be useful for idempotent inserts, but does not directly express every business refusal here. Distributed locks add another infrastructure dependency and do not replace relational constraints. An in-process mutex only coordinates one process and would fail across multiple instances.

The repository explicitly documents why teacher locks use sorted IDs and transaction scope in `ClassSessionRepository.cs:64`. It does not explicitly state every reason for choosing `xmin` over an application-managed token. The following is an interpretation based on the implementation: using PostgreSQL's existing version value avoids maintaining a separate domain field while still providing a client-visible stale-write contract.

#### Important details

An optimistic token only matters when an update or delete checks it. A mutation that becomes a true no-op may produce no SQL update, so understanding the entity transition is necessary before predicting a stale response. The capacity handler therefore performs an explicit `IsAtVersion` check after its locked load (`ChangeGroupCapacityHandler.cs:23`). Do not assume every command detects staleness at the same line.

Advisory-lock hash collisions reduce concurrency by making unrelated teachers wait; they do not grant access or disable constraints. Conversely, acquiring the advisory lock is not the overlap check. Exclusion constraints remain the final arbiter. Error classification uses SQLSTATE and constraint names, never localized database message text, which avoids coupling to server wording.

#### What I should understand before modifying this code

State the invariant precisely. For one-record lost updates, preserve the version contract. For counts, identify the row every competing operation must lock before counting. For uniqueness, keep the database constraint even if a pre-check exists. For scheduling, preserve sorted teacher lock acquisition and the exclusion constraints. Review transfer operations for lock cycles: the current transfer locks only its destination because the source only loses a seat (`TransferEnrollmentHandler.cs:15`). Adding an unnecessary source lock changes deadlock behavior.

<a id="backend-tenancy"></a>
## 6.10 Multi-tenancy through actor scope, EF filters, write guards, and PostgreSQL RLS

#### What it means

A tenant is one customer's data boundary. Here the tenant is a centre. Multi-tenancy means one running application and database hold data for several centres while ensuring an operation can only see or change the selected centre's records. Authentication identifies the person. Centre selection determines the current tenant. Permission checking determines which capabilities that person has there. Tenant isolation then constrains which rows those capabilities can reach.

The current design uses several independent checks. The dispatcher requires a selected centre for tenant-scoped requests. EF automatically adds read filters. A save interceptor checks tracked writes. PostgreSQL row-level security applies the boundary inside the database. Composite foreign keys prevent a child in one centre from referring to a parent row belonging to another centre. None of these layers should be described as a substitute for the others.

#### The problem it solves

The dangerous hypothetical query is `db.Students.Where(s => s.CentreId == request.CentreId)`. The caller chooses a centre ID, so every endpoint must separately prove that the ID is permitted. Even if most handlers add the correct predicate, one forgotten filter leaks data. Raw SQL and direct writes may bypass ORM-level checks entirely.

The project instead takes the active centre from trusted actor state created from a validated session. Tenant-owned entities have a fixed `CentreId`. A repository can query by entity ID without a local centre predicate because the context and database both add the tenant boundary. A foreign ID appears absent rather than returning another centre's data.

#### How it works

At the HTTP edge, cookie claims become `StaffActor(userId, centreId, role)` only after authentication. The dispatcher examines `ITenantScoped`. A missing tenant fails before a transaction. The context's `CurrentCentreId` reads the actor live. During model construction, the filter convention finds every mapped `ITenantOwned` type and builds `entity.CentreId == context.CurrentCentreId`. During save, the guard examines Added, Modified, and Deleted tenant entries and refuses absent or mismatched centre ownership and attempted centre changes.

During transaction start, the unit of work sets `app.current_centre` transaction-locally. RLS policies compare each row's `centre_id` to that setting. `USING` restricts visible or mutable existing rows; `WITH CHECK` restricts newly written row values. An absent or empty setting resolves to no matching tenant. `FORCE ROW LEVEL SECURITY` also binds the table owner except for PostgreSQL bypass privileges described by the helper's documentation.

#### How THIS project implements it

`TenantQueryFilterConvention.Apply` iterates every mapped type and recognizes `ITenantOwned` (`TenantQueryFilterConvention.cs:19`). It refuses owned or keyless mappings because those cannot carry this filter (`:26`). Its expression reads `CurrentCentreId` from the context rather than capturing a centre value at model creation (`:44`). `AppDbContext.OnModelCreating` runs it after all ordinary mappings and the test extension point (`AppDbContext.cs:55`), so a later configuration cannot overwrite the filter in that method.

`TenantWriteGuardInterceptor` checks audit immutability first, then tenant write ownership (`TenantWriteGuardInterceptor.cs:43`). An actor with a centre is not sufficient if the entity belongs to a different centre. A Modified entry whose CentreId property is marked modified is rejected even if the current value happens to match the actor (`:65`). Tenant identity is not an editable business field.

The migration helper emits three statements: enable RLS, force RLS, and create the `tenant_isolation` policy. Its policy expression is exact:

```sql
centre_id = nullif(current_setting('app.current_centre', true), '')::uuid
```

`TenantRowLevelSecurity.cs:19` defines that expression once. Table and schema identifiers are validated against a safe unquoted-identifier pattern before interpolation (`:65`). This validation is distinct from value parameterization: SQL identifiers cannot use ordinary value parameters, while centre values can and do.

#### Execution example

A user selects Nile through the membership gate. The next group query has a StaffActor carrying Nile's ID. The dispatcher accepts its tenant marker. The unit of work begins read-only and sets that ID in the database transaction. The EF global filter restricts groups, subjects, and teachers to Nile. The read projection joins those already scoped sets. PostgreSQL applies RLS again as SQL executes. A group ID copied from Maadi yields no visible group. If a buggy handler constructs a new entity with Maadi's centre, the write guard throws before save; if a direct SQL statement bypasses EF, the RLS `WITH CHECK` still rejects the foreign tenant row on the runtime role.

#### Why this project needs it

Subjects, teachers, parents, students, groups, enrollments, rules, sessions, and audit records now carry tenant data. The original groundwork is therefore a real security boundary. A central convention lowers the chance that adding a new query omits a filter. Database enforcement protects paths that do not execute ordinary LINQ. Composite relationships keep tenant ownership consistent across graphs as well as individual rows.

#### Alternatives

One database per tenant provides stronger operational separation but adds provisioning and migration complexity. A schema per tenant separates names but also complicates schema evolution. Handwritten predicates are explicit but omission-prone. EF filters alone cannot protect raw SQL or external writers. RLS alone does not provide early friendly errors and requires correct runtime privileges and tenant settings. The current system intentionally combines application and database layers.

#### Important details

Membership is deliberately not `ITenantOwned` because it must be read before a centre is selected. It is queried through explicit user and centre predicates (`MembershipReadService.cs:31`). Centre itself is the tenant rather than a tenant-owned child. Platform Data Protection keys are also not tenant-owned (`AppDbContext.cs:40`). These exceptions are part of the design, not evidence that every table should receive the same filter.

`ConfigureTenantOwned` adds a required centre field, Restrict foreign key to centres, and alternate key `(centre_id, id)` (`TenantOwnedConfiguration.cs:17`). Child composite foreign keys reference that key so they prove parent and child share a centre. A foreign key on parent ID alone proves only that the parent exists somewhere. Runtime credentials must also lack superuser and BYPASSRLS privileges; otherwise database isolation is bypassable. Infrastructure registers a readiness tripwire specifically for that condition (`DependencyInjection.cs:75`).

#### What I should understand before modifying this code

For a new tenant table, implement `ITenantOwned`, apply tenant mapping, add the migration's RLS and runtime grants, declare request scope and permission, and test cross-centre reads and writes. Do not copy centre IDs from the request body into actor state. Do not add `IgnoreQueryFilters`, raw SQL, or bulk updates casually. A bulk operation can bypass the change tracker and therefore auditing and the write guard, even while RLS still applies. Preserve transaction-local settings on every dispatch and keep privileged migration connections separate from runtime DI.

<a id="backend-authentication"></a>
## 6.11 Cookie authentication, antiforgery, revalidation, and password change

#### What it means

HTTP does not remember a user between requests. A session mechanism supplies a credential that can be returned on each request. This application uses an encrypted and signed authentication ticket in a browser cookie. The browser sends the cookie automatically. The server authenticates the ticket, revalidates its current account and membership facts, then builds Application actor state.

Because browsers send cookies automatically, authentication alone cannot prove a state-changing request came from the application's own script. Antiforgery requires a separate request token tied to a cookie token. Password verification is a different concern again: Identity hashes credentials, tracks lockout, and updates the security stamp. A changed stamp makes older tickets fail revalidation.

#### The problem it solves

Without a server-issued credential, a client could claim a user ID in a header and impersonate anyone. Without antiforgery, another origin could try to make a browser issue authenticated actions. Without revalidation, a self-contained ticket could continue carrying a revoked membership or old role until expiry. Without a password-change gate, a temporary password would remain a normal long-term credential.

The hypothetical `LoadUser(request.Headers["X-User-Id"])` trusts client data as identity. The actual login endpoint verifies credentials through `IAuthenticationService`, reads the profile, and only then issues a principal (`AuthEndpoints.cs:97`). This order ensures a failed profile read does not leave a newly issued session cookie.

#### How it works

Sign-in asks ASP.NET Core's cookie handler to serialize the principal into a protected ticket. Cookie authentication later decrypts the ticket, checks expiry, and invokes `OnValidatePrincipal`. This application's revalidation hook asks whether the stored stamp, membership activity, and role still match. A valid answer can be cached briefly; an invalid answer rejects the principal and sends cookie deletion.

Antiforgery token issue is a public GET returning the request token while the framework writes the cookie half. An endpoint filter on the `/api` group validates unsafe methods before the handler. Password changes call Identity's password operations, clear the pending-change flag, rotate the security stamp, and issue a new session with no selected centre. The current browser must select a centre again; other tickets now carry the old stamp.

#### How THIS project implements it

`AuthenticationSetup.cs:23` names the cookie `__Host-tcm.session`. It is HttpOnly, Secure always, SameSite=Lax, and Path `/`. The ticket lifetime is eight hours with sliding expiration (`:29`). Login and access-denied redirects are replaced with 401 and 403 statuses so the SPA receives an API response rather than HTML navigation. The fallback policy requires an authenticated user unless an endpoint explicitly allows anonymous access (`:48`).

The Data Protection key ring is persisted through `PersistKeysToDbContext<AppDbContext>` in Infrastructure (`DependencyInjection.cs:119`). This corrects an earlier local-key-ring limitation. Application name remains `TutoringCentre`; the registration comment explains that changing the name invalidates existing cookies. Keys are platform-level data rather than tenant rows.

`SessionRevalidationHandler` includes role in its cache key and query (`SessionRevalidationHandler.cs:43`). `MembershipReadService.GetSessionStateAsync` returns the stored stamp, activity, and current role (`MembershipReadService.cs:49`). Do not repeat the older claim that only membership activity and stamp are checked while role remains stale indefinitely. The current query receives the role as an explicit input.

#### Execution example

A newly created staff account logs in with its temporary password. Credentials verify, and `Reauthenticate` establishes a staff actor without a centre. The profile includes `MustChangePassword=true`. Login deliberately disables single-membership auto-selection in that case (`AuthEndpoints.cs:116`). The browser receives a session without centre and role, so tenant-scoped commands fail with `tenant.not_selected`. Attempting centre selection also checks `MustChangePasswordAsync` and returns `auth.password_change_required` (`GetActiveMembershipHandler.cs:24`).

The user submits current and new passwords to `/api/auth/change-password`. Request validation and the dedicated rate-limit policy apply. The authentication adapter rejects an equal old and new password, calls `UserManager.ChangePasswordAsync`, distinguishes password mismatch from policy failure, clears `MustChangePassword`, and updates the stamp (`IdentityAuthenticationService.cs:50`). The endpoint issues a new ticket without centre (`AuthEndpoints.cs:196`). Centre selection can now proceed through the ordinary membership gate.

#### Why this project needs it

Staff membership can now be changed or deactivated through product operations. Sessions therefore need current authorization facts rather than only cryptographic validity. Staff creation returns a temporary password once and sets a pending-change flag; the gate turns that flag into enforced behavior. Persisted keys allow tickets to remain decryptable across the deployment lifecycle described by the registration, while still requiring the same application name and database availability.

#### Alternatives

A server-side session store keeps a small random ID in the cookie and revokes by deleting a row. JWT bearer tokens can suit independent API clients but require explicit storage and revocation design. A JWT stored in a cookie still needs CSRF protection. An external identity provider can own credentials but does not replace per-centre memberships. These alternatives change credential mechanics; the centre and permission model would still need enforcement.

#### Important details

The cookie is a self-contained ticket, not a session row. Persisting Data Protection keys does not create a server-side session store. Logout calls `SignOutAsync` and removes the browser's cookie (`AuthEndpoints.cs:138`); it does not itself rotate the security stamp. A copied ticket can therefore remain valid until expiry or revalidation detects a changed account or membership fact. This is a direct implication of the current logout code and ticket design, not proof of an observed attack.

The authentication service's dummy hash prevents a trivially cheap unknown-account path, but its comment should not be promoted into a measured timing guarantee. Locked-out users follow a different branch before password verification. The source proves uniform error codes and deliberate dummy work; only timing measurement could establish the actual response-time distributions.

The password-change adapter performs more than one Identity save. The repository code explicitly clears the flag and rotates through `UpdateSecurityStampAsync` after the initial password operation. Its return status is not checked in the shown code (`IdentityAuthenticationService.cs:77`). That behavior deserves separate review rather than an unsupported claim that the entire method is one explicit transaction.

#### What I should understand before modifying this code

Keep session-changing actions synchronized with frontend CSRF refresh and session-cache updates. Preserve centre and role pairing in claims. Revisit the validation query whenever a new authorization fact is placed in a ticket. A password operation must consider other sessions, current ticket issuance, pending-change state, rate limiting, and error privacy. Never log the request body or a temporary password. Do not confuse key persistence with ticket revocation; they solve independent problems.

<a id="backend-permissions"></a>
## 6.12 Permissions, role freshness, and teacher ownership scope

#### What it means

Authentication proves identity. Authorization decides whether that identity can perform a capability. Ownership scope narrows which records an otherwise permitted read can return. A role groups permissions; a permission names an action such as `groups.manage`; a teacher scope describes whether the current caller can see every group, one teacher's groups, or none.

These layers answer different questions. A teacher can possess `schedule.view` while only seeing sessions assigned to that teacher. An owner can view the whole centre but cannot view another centre. Hiding a button communicates these rules in the frontend, but the API and application pipeline remain the security boundary.

#### The problem it solves

Role comparisons scattered through handlers duplicate policy and drift. One handler might treat Secretary as a manager while another treats it as read-only. A simple permission check is also insufficient for teachers because a successful `groups.view` decision must not expose every group's roster. Conversely, silently returning all data when no teacher link exists turns incomplete setup into a data leak.

Hypothetical `if (actor.Role == Teacher) ...` inside each feature repeats role meaning. Current dispatch permission checking delegates to `RolePermissions`, and ownership resolution delegates to `TeacherScopeProvider`. Features receive an explicit scope that fails closed for missing or unknown cases.

#### How it works

Each permission-requiring request declares its required key. The dispatcher reads the role already carried by trusted actor state and consults the central map. It makes that decision before a database transaction begins. Revalidation ensures a ticket's role still corresponds to membership state, subject to the configured positive-cache window.

Read handlers that require ownership ask `ITeacherScopeProvider`. Owners and secretaries get All because `RoleScopes.IsLimitedToOwnGroups` only returns true for Teacher. A teacher's linked active teacher record produces Own(teacherId). Missing or archived teacher links produce None. Read adapters apply the teacher predicate in SQL rather than loading all centre rows and filtering them afterward.

#### How THIS project implements it

`Permissions.cs:6` through `:21` defines keys for subjects, staff, audit, centre settings, students, teachers, groups, enrollments, and schedule. `RolePermissions` grants Owner every listed capability. Secretary receives subject, student, group, enrollment, and schedule management plus teacher view, but not staff management, audit view, centre settings management, or teacher management (`RolePermissions.cs:27`). Teacher receives subject view, group view, enrollment view, and schedule view (`:41`). Unknown roles and keys are denied by the lookup behavior.

`TeacherScopeProvider` caches its resolved result in the scoped instance (`TeacherScopeProvider.cs:20`). It returns All for system actors and staff roles not limited to own groups, None for other unknown cases, and resolves teachers through `FindIdByUserAsync` (`:34`). `GroupReadService.Scoped` chooses either every tenant-filtered group, a teacher-ID predicate, or no query (`GroupReadService.cs:60`).

Schedule scope is intentionally historical. Rules belong to a group and use the group's current teacher. Timetable sessions carry their own teacher and are scoped by that session field (`ScheduleReadService.cs:11`). A past session can therefore remain visible to its historical teacher after the group's teacher changes.

#### Execution example

A teacher requests the timetable. The dispatcher first sees a selected centre and verifies `schedule.view`. The query handler obtains Own(linkedTeacherId). `ScheduleReadService.GetTimetableAsync` applies `session.TeacherId == ownTeacherId`, then filters the requested local-date week and any optional group or teacher filters. A teacher-supplied filter for someone else intersects with the ownership predicate; it does not replace it. The returned projection joins group, subject, and the session's teacher. Every join is also tenant-filtered and subject to RLS.

If the account has no active teacher link, scope is None and the read adapter returns an empty collection without asking for all timetable rows. If the teacher tries a write, `schedule.manage` is absent and the dispatcher rejects before transaction start.

#### Why this project needs it

The product distinguishes owner administration, secretary operations, and teacher read access. Staff can work in multiple centres and have a different role in each. Teachers have both login accounts and business teacher records, which are not interchangeable identifiers. The scope provider is the bridge between authenticated account identity and the academic teacher record that owns groups or sessions.

#### Alternatives

ASP.NET named policies can enforce HTTP capabilities but do not automatically cover direct application or CLI dispatch. Resource-based authorization can evaluate each row separately but may be expensive for lists and can leak data if filtering happens too late. A claims-only list of permissions becomes stale unless renewed or revalidated. A database permission table supports configurable roles but adds schema and administration complexity absent from the current static matrix.

The repository does not explicitly state why every capability grouping was chosen. The following is an interpretation based on the implementation: stable capability keys allow the UI to present available actions without duplicating the role matrix, while keeping the actual decision on the server.

#### Important details

Permission and ownership scope are not identical types of filter. `RolePermissions` is a synchronous map requiring no lookup. Teacher scope requires an active linked academic record and is therefore resolved asynchronously. Caching that result once per request avoids repeated identical lookup, but it is not a cross-request authorization cache.

Session revalidation has its own positive cache. A role change can therefore take effect after that cache expires rather than at the exact write instant. Returning fresh permissions from `/api/me` does not itself change an already issued ticket. Understand both the current profile projection and cookie validation when predicting what a concurrent demotion does.

#### What I should understand before modifying this code

Add a new capability to the key set, matrix, application request, UI presentation, and tests together. Decide whether a read needs ownership scope as well as permission. Preserve All/Own/None semantics and default to None on unknown input. Do not accept a teacher ID from the request as proof of ownership. When a feature displays historical data, choose deliberately whether scope follows the current group teacher or the teacher recorded on each historical row.

<a id="backend-audit"></a>
## 6.13 Transactional auditing, allowlists, masking, and append-only history

#### What it means

An audit log records business changes with enough context to answer who changed which record and what changed. It differs from operational logging, which records process behavior and failures. Audit entries should correspond to committed state changes, preserve history, and avoid turning sensitive data into a second uncontrolled copy.

An allowlist says which fields may enter the audit document. Masking changes how an allowed sensitive field is represented. Append-only means existing audit rows cannot be edited or deleted through ordinary application behavior. These are separate policies: an allowlist limits content, masking limits exposure, and append-only rules protect integrity.

#### The problem it solves

If every handler writes its own audit row, one handler eventually forgets, writes the wrong actor, or records the event before the business transaction fails. Logging entire entity objects may leak passwords, tokens, or personal contact details. Auditing every EF property automatically also records technical noise and makes newly added sensitive fields visible without an explicit decision.

Hypothetical `logger.LogInformation("Updated {@Student}", student)` is neither transactional nor safely minimized. Current auditing runs at save, examines known audited types and allowed fields, and adds an audit entity to the same context. If the transaction rolls back, that audit entity rolls back too.

#### How it works

A save interceptor runs before EF generates its write commands. It inspects the tracked entity states and their original and current property values. A policy chooses eligible entity types, actions, fields, and tenant ownership. A pure diff builder constructs a JSON document. The interceptor adds an audit entry containing the actor, tenant, timestamp, entity identity, action, changes, and correlation ID.

The interceptor snapshots the changed entries before adding audit entities, avoiding collection mutation while enumerating. A later tenant write guard checks the newly added audit entries as well as business entries. Modifying or deleting an existing audit row is rejected independently of whether its centre matches the actor.

#### How THIS project implements it

`AuditInterceptor` is scoped because it receives `IClock`, `ICurrentActor`, and `ICorrelationContext` (`AuditInterceptor.cs:22`). It runs after timestamp stamping and before the tenant write guard, as stated in its class documentation. Platform bootstrap with no actor centre is deliberately logged rather than audited (`:64`). For an actor with a centre, it resolves Staff or System actor type and optional user ID, captures one timestamp and correlation ID, and snapshots tracked changes (`:78`).

Each changed object must appear in `AuditPolicy.Entries`. The entity's resolved centre must match the actor centre. State maps to Created, Updated, or Deleted. Policy can exclude particular actions. The interceptor then passes property names, original values, current values, and `IsModified` flags to `AuditDiffBuilder` (`:114`). A null diff skips the audit row entirely.

`AuditDiffBuilder.Build` iterates only allowed fields (`AuditDiffBuilder.cs:37`). Created actions write null-before/current-after; Deleted writes original-before/null-after; Updated writes only fields marked modified whose real values differ. The output field names are camelCase. Masking happens after the real-value comparison, so two phone numbers sharing a visible suffix still produce a change (`:75`). Phone values are written through `PhoneNumber.Masked`, not canonical full digits.

#### Execution example

An owner updates a parent's phone. The handler validates and canonicalizes the new number, loads the versioned parent, and calls its domain update. During save, EF identifies the modified phone property. Timestamp interception updates technical time metadata. Audit interception finds the parent policy, compares actual old and new phone objects, and builds before/after masked values for the allowed field. It adds an audit row with centre, actor ID, timestamp, entity ID, and correlation ID. The tenant guard then verifies both rows belong to the acting centre. Save and commit persist them together. A unique-phone conflict instead rolls back both the attempted update and audit row.

#### Why this project needs it

Staff and academic records now have meaningful administrative changes. A centre needs a history of role changes, archives, contact edits, enrollments, and schedule modifications. A centralized interception point avoids asking each feature author to remember audit plumbing. An allowlist ensures adding a new entity property does not silently expand data collection.

#### Alternatives

Domain events can describe higher-level business actions and be persisted transactionally, but require deliberate emission and dispatch mechanics. Database triggers can guarantee coverage for every SQL writer but have less direct access to application actor and correlation context unless these are supplied through connection settings. An external event stream can preserve immutable history but introduces delivery guarantees and infrastructure. Ordinary structured logs are useful for troubleshooting but are not a replacement for the transactionally stored audit table.

The repository explicitly states the allowlist and save-interception contract in the inspected classes. It does not explicitly state every tradeoff against these alternatives. The following is an interpretation based on the implementation: using tracked property changes gives broad coverage with minimal handler code, while policy and masking limit the information captured.

#### Important details

Auditing only sees tracked operations. A bulk SQL update or EF `ExecuteUpdate` does not produce ordinary tracked property changes. `ClassSessionRepository` explicitly avoids bulk updates for that reason (`ClassSessionRepository.cs:9`). RLS may still protect those writes, but audit and tenant-interceptor coverage would differ.

No-op changes should not produce misleading history. Updated actions with no actual allowed-field differences return null (`AuditDiffBuilder.cs:66`). A row might change only technical fields without an audit record if policy does not allow them. Conversely, Created and Deleted representations include allowed fields even when a value is null, because the action describes the whole allowed snapshot.

The guard prevents Modified or Deleted audit entities (`TenantWriteGuardInterceptor.cs:43`). Database access grants add another layer through migration helpers; application interception alone does not make a table immutable to privileged operators. Keep those scopes distinct when explaining append-only behavior.

#### What I should understand before modifying this code

Choose audited fields deliberately and review privacy before adding them. Compare real values before masking. Do not add an audit save independent of the business transaction. Preserve interceptor ordering so audit rows are created before the guard runs. When adding bulk persistence, explicitly address the auditing gap. Treat changes to policy, JSON representation, action names, and entity names as changes to the stored historical contract, because existing rows remain after code changes.

<a id="backend-academics"></a>
## 6.14 Academic records, aggregate relationships, and archive workflows

#### What it means

The academic model describes the centre's operational records rather than just its sign-in infrastructure. A subject is something the centre teaches. A teacher is a teaching record, optionally linked to a login account. A parent is a contact shared by linked students. A student owns its parent relationships. A group joins a subject and teacher with a capacity. An enrollment records a student's seat in a group over time. Schedule rules and sessions then describe when that group meets.

These distinctions prevent misleading shortcuts. A teacher is not necessarily a staff account. A parent is not a student-owned copy of a phone number. An enrollment is not just membership in a mutable list. An archive is not deletion. Each concept has a different identity, lifecycle, and set of references, which determines where changes belong.

#### The problem it solves

Combining teacher and staff records would require every listed teacher to have a login and would make account role changes erase academic identity. Copying parent contacts onto each student would create inconsistent phone numbers when siblings share a contact. Deleting archived records would break historical enrollments and sessions. Storing a group's enrollment count would duplicate information already present in active enrollment rows.

The unsafe hypothetical student registration writes a parent, saves, then writes a student in a second transaction. If student validation fails, an unwanted parent remains. Current registration resolves or constructs the parent and student in one dispatch transaction. The common save commits both aggregates and the link or rolls everything back (`RegisterStudentHandler.cs:10`).

#### How it works

Domain entities own intrinsic rules and transitions. Handlers load related records through tenant-scoped repositories and verify cross-aggregate conditions. Database foreign keys prevent invalid references, including cross-tenant composite relationships. Archiving changes status while retaining identity. Read adapters can exclude archived rows by default and expose them only when requested.

Account linking uses an explicit contract across functional areas. The academic handler asks `IStaffDirectory` whether a user is an active Teacher-role member of the current centre. It does not load `ApplicationUser` or parse password and claim data. A separate uniqueness check ensures one account is not linked to another teacher in the centre, with database translation covering a concurrent race.

#### How THIS project implements it

`Teacher.cs:6` explains that teachers can exist without signing in, and role changes do not remove the teaching record. `UserId` is optional. `LinkUser` and `UnlinkUser` succeed without change when already at the requested link state; both refuse editing archived teachers through the shared archived check. Archive retains the account link so restoration can restore ownership scope (`Teacher.cs:115`).

`SetTeacherAccountHandler` first loads the versioned teacher. A null UserId means unlink. A non-null ID must pass `IsActiveMemberWithRoleAsync(centreId, userId, Teacher)` and the account-linked uniqueness check (`SetTeacherAccountHandler.cs:36`). Unknown users, foreign-centre users, wrong roles, and inactive memberships all produce the same eligibility refusal, limiting information disclosure.

`Subject` cleans internal whitespace and uppercases its normalized name (`Subject.cs:91`). By contrast, student, parent, teacher, and group search fields use `SearchText.Normalize`. These are not the same algorithm. `SearchText` removes selected Arabic diacritics and tatweel, lowercases, normalizes selected Arabic letters, and maps Arabic digit sets (`SearchText.cs:12`). Display names remain distinct from those matching fields.

Student registration accepts either an existing parent ID or nested parent details. `RegisterStudentHandler` canonicalizes an optional student phone, resolves the existing parent or validates a new parent's phone and uniqueness, constructs the parent if needed, then registers the student and primary link (`RegisterStudentHandler.cs:25`). It tracks a new parent before `Student.Register`; if that later factory fails, the dispatcher rollback clears the provisional tracked parent. Therefore its summary comment that nothing is added before every failing check has passed is broader than the actual statement order.

#### Execution example

An authorized secretary registers a learner whose parent is already known. The request supplies an existing parent ID, student details, and relationship. The dispatcher validates shape and permission, starts a tenant transaction, and invokes the handler. An optional student phone is parsed into the canonical value object. The repository finds the parent only within the actor's centre. `Student.Register` validates the learner's name and school year, allocates its ID, and creates exactly one primary link to the resolved parent. Tracking the student graph prepares both student and link writes. Timestamp, audit, and tenant guard processing runs at save. Composite foreign keys enforce that the link refers to records in the same centre. On commit, the result returns student and parent IDs.

#### Why this project needs it

The product now has real relationships that staff must maintain over time. Parents can be reused across students without copying contact data. Teachers can be managed independently of login provisioning. Enrollment history supports transfer and withdrawal. Archive rules prevent removing operational records while dependents remain active. These are business mechanisms visible in source, not future features inferred from file names.

#### Alternatives

Hard deletion makes cleanup easy but loses identity and historical references. A separate table of arbitrary person roles can unify people but introduces more generality than these focused records. Embedding parent contacts under each student simplifies one form while making shared updates difficult. Storing counters or denormalized names can speed reads but requires synchronization rules. The current joins and historical rows choose explicit relationships and source-of-truth data.

The repository does not explicitly state why every normalization difference was chosen. The following is an interpretation based on the implementation: subject names use a simpler case-and-spacing uniqueness rule, while people and groups receive Arabic-friendly matching suited to search. Do not silently unify those behaviors as a cleanup without deciding whether uniqueness contracts should change.

#### Important details

Archive refusals belong to specific features. `ArchiveTeacherHandler` refuses a teacher with active groups (`ArchiveTeacherHandler.cs:20`). `ArchiveStudentHandler` refuses a student with an active enrollment (`ArchiveStudentHandler.cs:21`). `ArchiveGroupHandler` locks the group, checks the expected version and no active enrollments, then checks no schedule rule remains active from the centre's today (`ArchiveGroupHandler.cs:26`). Parent entities have no archive status in the inspected model (`Parent.cs:6`). Describing a universal archive implementation would therefore be inaccurate.

Student-parent changes increment `LinksRevision` only on success. Removing the last parent is refused. Removing the primary parent requires making another link primary first. Setting primary clears the old link and marks the new one in one aggregate operation (`Student.cs:177`). These details matter when building forms: an arbitrary sequence of unlink and relink calls is not equivalent to the provided primary-change operation.

#### What I should understand before modifying this code

Follow the identity and lifecycle of the specific record. Use teacher business IDs for groups and sessions, user IDs for login linkage, and membership IDs for staff lifecycle. Preserve parent reuse and aggregate rules. Before adding an archive path, identify every active dependent relationship and whether it needs a lock. When modifying search normalization, consider persisted normalized values and migration or backfill needs. Never assume changing display text automatically updates independently stored matching fields outside the entity's own methods.

<a id="backend-scheduling"></a>
## 6.15 Local recurrence, UTC sessions, DST, and exclusion constraints

#### What it means

A wall-clock time answers what a person sees on a clock, such as Saturday at 18:00 in Cairo. An instant identifies one point on the global timeline. They are not interchangeable because a time zone's offset can change with daylight saving. A recurrence rule describes local intent. A dated session records the resulting instants for one occurrence.

The current model deliberately separates these representations. `ScheduleRule` stores weekday, local `TimeOnly`, duration, room, and effective local dates. It stores no UTC offset or time zone; the centre owns the zone. `ClassSession` stores UTC start and end, the local date it was generated for, and a teacher snapshot. The local date can differ from the UTC date near midnight.

#### The problem it solves

If recurring sessions are generated by taking one UTC instant and repeatedly adding seven days, the local start time shifts when the zone offset changes. If duration is calculated by subtracting local clock readings across a backward transition, a session's elapsed length can be wrong. If the timetable filters by UTC midnight boundaries, an early Cairo session can appear in the wrong day or disappear from the intended week.

Hypothetical `nextStart = firstStart.AddDays(7)` preserves a global interval but not necessarily the intended wall-clock hour. Current generation reconstructs each occurrence's local date and start time separately, converts that wall-clock value through the centre's zone, then adds duration to the UTC instant (`SessionGenerator.cs:34`). This preserves local scheduling intent and elapsed duration as separate requirements.

#### How it works

`ScheduleRule.OccursOn` accepts a date when its weekday matches and it lies within inclusive effective bounds. The generator iterates the requested inclusive date range and skips non-occurrences. It builds an Unspecified-kind local `DateTime`, because the value is neither server-local nor UTC. Conversion then handles normal, skipped, and repeated times explicitly.

For a skipped local time, the generator computes the offset before and after the gap, shifts forward by the gap, and uses the later offset. For a repeated time, it chooses the larger offset, corresponding to the first occurrence. Ordinary times use the zone's offset on that date. End is start plus elapsed minutes. The result becomes a session through an internal factory so ordinary callers cannot invent arbitrary start/end combinations.

#### How THIS project implements it

`SessionGenerator.Generate` receives the rule, teacher ID, `TimeZoneInfo`, and from/to dates (`SessionGenerator.cs:17`). It reads no clock, repository, or machine time zone. `ToUtc` contains the three transition cases (`:50`). `ClassSession.Schedule` is internal and copies centre, group, rule, room, and teacher into the occurrence (`ClassSession.cs:68`). Teacher identity is therefore a generation snapshot, later mutable only under the future-session reassignment rule.

`CentreCalendar.Today` converts `IClock.UtcNow` into the supplied centre zone before extracting a date (`CentreCalendar.cs:12`). `ScheduleHorizon.Days` is 28, so adding a rule generates through today plus 27 days. New rules can take effect at most 366 days ahead, while explicit extension can reach at most 92 days ahead (`ScheduleHorizon.cs:7`). A valid rule outside the initial 28-day horizon may initially create no sessions; that follows from separate accepted-start and generation-window limits.

PostgreSQL supplies the overlap guarantee. `20261024105240_AddSchedule.cs` installs `btree_gist`, creates rules and sessions, then adds GiST exclusion constraints for group and teacher overlap. Each compares centre, resource ID, and `tstzrange(starts_at, ends_at, '[)')`. The half-open interval means a session ending at 19:00 does not overlap one starting at 19:00. Constraints apply only to rows whose status is `scheduled`, so cancellation frees a slot. Centre ID participates, so one centre's schedule cannot block another's.

#### Execution example

An owner adds a Saturday rule at 18:00 for 90 minutes. The handler loads the visible group and refuses archived groups. It resolves the centre zone and today, rejects an effective date too far ahead, constructs the rule, and takes the teacher's advisory schedule lock (`AddScheduleRuleHandler.cs:35`). The generation range starts at the later of today and effective-from and ends at today plus 27 days. Each matching Saturday becomes a UTC session using that Saturday's own offset. The handler tracks the rule and sessions and returns their count.

The dispatcher saves. If any occurrence overlaps another scheduled session for the same group or teacher, the exclusion constraint rejects save. `PersistenceErrorTranslator` maps a known constraint to `schedule.group_double_booked` or `schedule.teacher_double_booked`. Rollback removes the entire rule and batch. There is no partial success list that silently drops collisions.

#### Why this project needs it

The application's centres carry IANA zones, and scheduling is expressed by people in local dates and times. Accurate instants are necessary for overlap checks, future-only cancellation, and historical teacher assignment. Explicit DST handling removes a class of bugs that ordinary weekly date arithmetic hides. Database overlap enforcement remains authoritative when two requests generate or reassign sessions concurrently.

#### Alternatives

Storing only UTC is adequate for one-off appointments but loses recurrence intent. Storing only local time requires resolving ambiguity on every comparison and cannot directly enforce absolute overlap. A recurrence library could implement richer calendars, but its DST policy would still need to match the product contract. Application overlap pre-checks can improve previews but cannot replace database exclusion constraints under concurrency. A room-overlap constraint is a separate policy; the inspected migration defines teacher and group constraints, not a room resource constraint.

The migration comments explicitly explain half-open ranges, partial scheduled status, and group-constraint creation before teacher. It states the group-first order aims to return the more precise group collision when both could fail. The repository does not establish a universal room-booking policy from the presence of the optional room string; do not claim room uniqueness merely because sessions record rooms.

#### Important details

Extension is explicit and idempotent for existing rule/date occurrences. `ExtendScheduleHandler` lists active rules for active groups, resolves their current teachers, acquires all relevant teacher locks, obtains existing local dates per rule, and adds only missing dates (`ExtendScheduleHandler.cs:37`). Existing dates include occurrences regardless of whether regenerating them would be desirable; a cancelled occurrence still occupies the rule/date identity. The unique index `ux_class_sessions_rule_date` is deliberately untranslated because duplicate generation is treated as a bug.

Ending a rule does not delete generated rows. The handler sets its inclusive last date and cancels later sessions only if still scheduled and not started (`EndScheduleRuleHandler.cs:26`). Sessions on or before the last date remain unchanged. Reassigning a group's teacher locks both teachers and moves only future scheduled sessions, preserving past and cancelled history (`AssignGroupTeacherHandler.cs:45`). A conflict on the new teacher rolls back the group and session changes together.

#### What I should understand before modifying this code

Keep local date, wall-clock time, zone, and instant distinct in names and types. Do not use server `DateTime.Today` for centre rules. Preserve explicit skipped and repeated-time policy when changing generation. Do not edit a rule in place to alter history; end it and add a replacement through the existing model. Before adding room scheduling, holidays, exceptions, or background extension, define their collision, identity, and audit contracts. Retain teacher lock ordering and database constraints when optimizing batch generation.

<a id="backend-foundations"></a>
## 6.16 C# foundations behind the architecture: contracts, generics, async, records, reflection, and cleanup

#### What it means

Professional architectural vocabulary rests on language mechanisms. An interface declares callable members without selecting an implementation. Polymorphism lets the same interface call execute different implementations. Composition means an object holds collaborators. Inheritance means a type derives behavior or identity from a base type. Generics parameterize types while retaining compile-time relationships. Async methods represent incomplete work through tasks. Records provide generated value-oriented behavior. Reflection inspects type metadata at runtime. None of these is magic; each has a concrete role in the inspected code.

The repository uses interfaces for ports, composition for handlers and adapters, small inheritance relationships for entities and framework integration, and generic request/handler pairs for dispatch. Reading those constructs fluently makes the architecture much easier to follow than memorizing folder names alone.

#### The problem it solves

Without interfaces, `EnrollStudentHandler` would name concrete EF repositories and unit tests would need those implementations. Without generics, the dispatcher might accept `object`, switch on runtime types, and cast results, moving errors from compile time to runtime. Without async I/O, every database wait would block a thread. Without deterministic disposal, failed requests could leave transactions or connections alive. Without reflection or generated registration, every new handler would require another explicit dispatcher branch.

Hypothetical `object Send(object message)` permits passing a request whose result is cast to the wrong type. The actual `SendAsync<TCommand,TResponse>` constrains the command to `ICommand<TResponse>` (`Dispatcher.cs:35`). That relationship tells the compiler which result type belongs to the request and which closed handler interface must resolve.

#### How it works

An interface reference stores an object whose runtime type implements the contract. Calling its method selects that implementation. Generics let one dispatcher algorithm operate on many request types while retaining typed results. `Result<EnrollStudentResult>` and `Result<AddScheduleRuleResult>` use the same result implementation but expose different success types. Nullable annotations mark references that may be absent; they do not themselves add runtime checks.

An async method returns a `Task` that represents eventual completion. At an incomplete `await`, the compiler-generated state machine records the continuation and returns control instead of blocking the caller's thread. The continuation resumes after completion. A cancellation token is cooperative: every layer must pass or observe it. Throwing an exception unwinds calls to a matching catch while `finally` and disposal code execute. `throw;` rethrows the same failure without deliberately replacing its original location.

#### How THIS project implements it

`Dispatcher` depends on `IServiceProvider`, `IUnitOfWork`, and `ILogger<Dispatcher>` through its constructor (`Dispatcher.cs:24`). The generic method constraint enforces request/result pairing. Repository methods return `Task<Entity?>` when a visible row may be absent, and handlers explicitly branch on null before reading the entity. The actor check uses pattern matching to capture a staff actor only when its type matches (`GetMyMembershipsHandler.cs:19`).

`AuditInterceptor` uses a switch expression to choose actor metadata and entity-state action (`AuditInterceptor.cs:71`). `TeacherScopeProvider` uses property patterns and nullable values to distinguish cached, all, own, and none states. `Program.cs` uses list patterns to recognize exact CLI argument shapes (`Program.cs:123`, `:131`, `:148`). The reset-password branch only matches the three-element command shape; its values come from the matched positions rather than general string parsing.

The source also uses attributes with distinct readers. `GeneratedRegex` requests compile-time generated regex code, while `LoggerMessage` requests generated typed logging methods. EF migration attributes and configuration discovery are framework metadata mechanisms. An attribute does nothing merely because it is written; a compiler component or runtime consumer must read it. Partial classes and methods allow generated code to complete the declared type.

#### Execution example

Follow one `await` through enrollment. The endpoint awaits `SendAsync`, which awaits the handler, which awaits `groups.GetForEnrollmentAsync`. The repository asynchronously sends the locking query. While waiting for a competing transaction, application execution is suspended rather than a C# busy loop polling the database. When the database returns the group, execution resumes, performs the count, checks the student and capacity, then returns a typed result. The dispatcher awaits save and commit. If any await throws, its catch awaits rollback with `CancellationToken.None` and rethrows (`Dispatcher.cs:91`). The API's outer exception machinery can then translate unexpected faults.

The cancellation token follows that call chain into EF operations. Identity's `UserManager` methods in the credential adapter do not take the adapter's cancellation-token parameter in the shown calls (`IdentityAuthenticationService.cs:23`). Therefore a method accepting a token is not proof that every internal operation observes cancellation. Read actual calls before claiming end-to-end cancellation coverage.

#### Why this project needs it

The same handler shape appears across many features. Generics remove a type switch; interfaces make adapters replaceable; async operations make database waits scalable; nullable checks make absent tenant-visible rows explicit; pattern matching makes the actor and command variants readable. Deterministic cleanup protects transaction ownership. Source-generated logging and regex keep repeated infrastructure behavior typed without hand-writing its boilerplate.

#### Alternatives

Concrete classes and manual composition can be simpler for a small program. Delegates can replace some single-method interfaces. Callbacks or reactive streams can represent asynchronous work but change the execution style. Runtime reflection can replace some source generation at the cost of later validation. A discriminated union language can make variant handling more exhaustive than ordinary nullable references and classes. These are language and design choices, not necessarily competing application architectures.

The repository does not explicitly state why every modern C# syntax choice was selected. The following is an interpretation based on the implementation: primary constructors, records, patterns, and generic interfaces reduce boilerplate while leaving the transaction and authorization algorithm in ordinary readable methods.

#### Important details

`!` is a compile-time nullability suppression, not a runtime non-null check. The handler's `currentActor.Actor.CentreId!.Value` relies on the dispatcher having enforced the tenant marker. Calling that handler directly with an unselected actor can violate its precondition. A record's generated equality does not recursively make mutable dictionary contents value-equal; an immutable-looking error can still contain a reference to a collection.

`async` does not mean a new thread is created, nor that work is automatically parallel. Two sequential awaits remain sequential. `Task.WhenAll` would overlap operations but is not automatically safe when they share one EF context or have ordered locking requirements. Avoid parallelizing repository calls merely to reduce wall-clock time without checking context and transaction constraints.

`finally` establishes cleanup even on failure, but code within cleanup can itself throw. The inspected unit of work disposes and clears state deliberately; this does not prove every external failure mode has been tested. Keep language mechanism, source intention, and executed verification distinct when reporting behavior.

#### What I should understand before modifying this code

Trace types before tracing frameworks. Determine which interface is requested, which implementation is registered, which scope owns it, and which terminal await executes I/O. Preserve generic response pairing and explicit null branches. Do not use `.Result` or `.Wait()` to simplify an async method. Pass cancellation where supported and retain non-cancelled rollback. Understand who reads an attribute before copying it. Keep business operation sequencing explicit when introducing parallelism, because lock ordering and one-context ownership are correctness constraints.

<a id="backend-http"></a>
## 6.17 HTTP binding, strict JSON, Problem Details, and generated contracts

#### What it means

HTTP separates a method, path, headers, and optional body from a response status, headers, and body. Model binding converts those transport values into endpoint parameters. Serialization converts result objects back to JSON. Neither operation should decide domain permission or business state. A stable API contract defines these shapes so frontend generation and clients can agree on the same fields and statuses.

Problem Details provides a common error envelope. This project enriches it with a stable machine code and identifiers linking the response to logs. A field dictionary can associate a refusal with a form input. HTTP status and field detail are independent: a duplicate-name conflict can be 409 and still name the `name` field.

#### The problem it solves

Without strict binding, a client typo such as `teacherId` on a schedule-rule body might be silently ignored, leaving the user believing it selected a teacher. Without a single failure mapper, one endpoint could return `{error:...}`, another plain text, and an unexpected exception HTML. Without explicit time formats, generated TypeScript could accept values that the server interprets differently.

The hypothetical endpoint returns `Results.BadRequest(exception.Message)`, leaking implementation detail and bypassing common enrichment. Actual endpoints dispatch application requests and call shared result mapping. Unexpected exceptions become generic `server.unexpected` responses while the full exception is logged server-side (`GlobalExceptionHandler.cs:27`).

#### How it works

ASP.NET builds endpoint delegates from their parameter types and route metadata. Route values, query parameters, request bodies, services, and cancellation tokens bind from different sources. JSON options reject unknown members and serialize enums as camelCase strings. Required body properties distinguish an omitted value from a legitimate default. Binding failures throw because `ThrowOnBadRequest` is enabled, allowing the common exception handler to produce a consistent malformed-request response.

Endpoint metadata describes success and error schemas for OpenAPI. Custom converters can hide the JSON shape from schema inference, so the application supplies a schema transformer for `TimeOnly` and `DateTimeOffset`. That transformer preserves string formats and nullability. Generation describes compile-time shapes; it does not turn every incoming or outgoing JSON payload into runtime schema validation.

#### How THIS project implements it

`Program.cs:75` rejects unmapped JSON fields, adds camelCase enum conversion, and registers the local-time and UTC-instant converters. `HourMinuteTimeOnlyJsonConverter` accepts exactly `HH:mm` with invariant culture and rejects seconds, offsets, and instants (`HourMinuteTimeOnlyJsonConverter.cs:17`). `UtcInstantJsonConverter` reads through the standard DateTimeOffset reader and writes a normalized UTC string ending in `Z` (`UtcInstantJsonConverter.cs:17`). These directions are asymmetric by design: UTC output normalization does not imply input offsets are forbidden.

`ResultHttpExtensions.ToProblemResult` maps Validation to 400, NotFound to 404, Conflict to 409, Rule to 422, Forbidden to 403, and Unauthenticated to 401 (`ResultHttpExtensions.cs:58`). Any error carrying fields becomes `ValidationProblemDetails` while retaining its mapped status. `ProblemResult.ExecuteAsync` adds trace and correlation IDs, sets status, and writes `application/problem+json` (`ProblemResult.cs:11`). Framework-generated errors use the shared enrichment through configured Problem Details services rather than necessarily instantiating this custom result themselves.

#### Execution example

A client sends a schedule rule with an extra `teacherId`. The route supplies the group ID. The body type contains weekday, local start, duration, optional room, and effective-from only. Global unmapped-member rejection makes the extra field a binding failure before the business handler runs. `ThrowOnBadRequest` exposes that as `BadHttpRequestException`; `GlobalExceptionHandler` logs a malformed-request warning and writes 400 `request.malformed`. No rule or session is tracked.

A validly bound body with an unsupported duration reaches application validation or `ScheduleRule.Create`, producing a normal validation result. An overlapping valid rule instead fails at save with a conflict. These failures share an envelope but retain different causes and statuses, enabling the frontend to show field feedback or a collision message appropriately.

#### Why this project needs it

The API now serves several resource families, versioned state transitions, local date and time input, and UTC session output. Shared conventions keep generated clients and forms consistent. Exact request bodies also prevent callers from supplying authoritative fields such as centre or teacher when those belong to actor or group state. A predictable error shape lets the client centralize translation, field application, and session-expiry handling.

#### Alternatives

Lenient JSON tolerates unknown fields and can improve forward compatibility, but hides mistakes. MVC automatic model validation supplies another binding model, while these endpoints use minimal APIs. Protobuf or GraphQL can define different contracts but require corresponding tooling. Per-endpoint catch blocks can map errors but duplicate the same security and shape decisions. The current design centralizes conversion while keeping feature endpoints short.

#### Important details

Creation does not always mean one Location header contract. Schedule-rule creation returns 201 and points to the group's rule list because the rule has no separate page. Enrollment creation points to the roster. Transfer returns 200 with the new enrollment ID because one request ends an old resource and starts another. Extend returns 200 with a count because it can create many sessions or none (`ScheduleEndpoints.cs` and `EnrollmentEndpoints.cs`).

The common creation helper's comment says Location uses only server-generated result values, yet nested resource endpoints deliberately capture a validated route group ID after the handler succeeds. The endpoint comments explain the narrower security reason: the ID has been resolved inside the tenant, and the route constraint restricts it to a GUID. Document the actual endpoint behavior rather than copying the broader helper comment as universal fact.

#### What I should understand before modifying this code

Choose the source of every field: route, body, actor, database, or server-generated result. Keep authoritative centre and teacher selection out of bodies that do not own them. Add endpoint names and complete response metadata, rebuild the contract, and regenerate the client. Preserve exact local-time and UTC-output formats. Test malformed binding separately from domain validation and save conflicts, because they execute at different points and require different guarantees about side effects.

# Backend action walkthroughs

These walkthroughs follow current implementation paths. They are source-derived executions, not claims that a live server was exercised during this subtask. Their purpose is to make the concepts above concrete enough that a returning developer can set breakpoints and verify each transition.

## Registering a student with a new parent

The important inputs are the student's display details, optional student phone, relationship, and either a known parent or new parent details. The active centre does not come from the request. The dispatcher has already checked the tenant marker before `RegisterStudentHandler` reads `currentActor.Actor.CentreId!.Value` (`RegisterStudentHandler.cs:22`). That null-forgiving expression is a documented precondition, not a second authorization check.

The handler first attempts student-phone conversion only when a nonblank string was supplied. `PhoneNumber.Create` can reject input or produce the canonical value. A failure is wrapped with the student field context rather than allowing a persistence converter to fail later. For new-parent input, the handler separately parses the parent phone, checks uniqueness in the current tenant, and constructs `Parent` with the actor centre. The parent factory cleans its display name and builds a search field (`Parent.cs:40`). A matching phone produces a field-specific refusal before tracking a second parent.

The newly constructed parent receives its UUID immediately through the entity base constructor. Therefore the student can reference the parent ID before any SQL insert. `parents.Add` tracks the parent without saving. `Student.Register` then validates the learner and constructs exactly one primary link using that parent ID. `students.Add` tracks the student graph. The handler returns student and parent IDs as success data, but that success is provisional until the dispatcher completes save and commit.

At save, timestamps are applied, audited changes are converted to allowed documents, and the tenant guard checks every tracked tenant object. The parent's centre, student's centre, and link centre must agree with actor state. Composite foreign keys additionally require the link's referenced parent and student to exist in that same centre. EF writes the tracked graph in an order satisfying its relationships. A duplicate-parent-phone race can still lose at the unique index; the catalogue translates it into the expected parent conflict. Rollback discards parent, learner, link, and audit entries together.

One subtle source-reading lesson appears here. The handler summary says nothing is added before every potentially failing check. The actual code tracks the parent before `Student.Register`. Tracking is not a committed insert, so transaction behavior remains safe, but the comment's literal statement order is inaccurate. The correct explanation is that no provisional tracked parent survives a failed registration because dispatcher rollback clears the tracker and transaction. This distinction helps avoid treating comments as stronger evidence than executable statements.

To modify this operation safely, decide whether a new field belongs to student identity, parent identity, or the relationship. Relationship-specific facts belong on the link rather than duplicated on parent. New input validation must match the correct nested field names. Additional writes must use the same context. Do not implement nested parent creation by dispatching `CreateParentCommand`; the nested transaction guard would reject that pattern and it would obscure the intended one-operation consistency boundary.

## Creating staff and enforcing the first-password transition

Staff creation coordinates a login identity and centre membership. `CreateStaffHandler` obtains the current centre from actor state, then asks `IStaffAccountService.EnsureAccountAsync` for an account (`CreateStaffHandler.cs:23`). The handler never depends directly on Identity's user type. Infrastructure checks email through `UserManager`. If the account already exists, it returns the user ID with no temporary password. It does not overwrite that person's existing display name or locale in the inspected existing-account branch (`StaffAccountService.cs:26`).

For a new account, Infrastructure generates 16 characters using `RandomNumberGenerator.GetString` over its fixed password alphabet. It sets username and email, confirms email in this provisioning path, sets display name and preferred locale, and marks `MustChangePassword=true`. `UserManager.CreateAsync` performs the credential creation and can return errors. Those errors are converted to a field-bearing `staff.email_invalid` result. The plaintext temporary password is returned only as the creation result and is not a field stored on `ApplicationUser` by this code.

The handler then checks whether any membership already exists for that user and centre, including an inactive membership. Existing membership returns `staff.already_member`; reactivation is a distinct operation. If no row exists, `Membership.Create` constructs the active membership and the repository tracks it. The result includes membership ID, user ID, and a temporary password only when the account adapter created one. A concurrent membership race receives the same error through `ux_memberships_centre_user` translation.

Although `UserManager` saves during account creation, its store uses the same scoped context and the already open dispatch transaction. The account is provisional until the whole command commits. If membership creation or the final save fails, a newly created identity row does not become an orphaned permanent account. This is why replacing the account adapter with a separately constructed context would change business correctness, even if every individual Identity call still appeared to succeed.

The first login verifies the temporary credential normally, builds the profile, and sees the pending-change flag. Single-centre auto-selection is suppressed. The resulting ticket proves the user identity but carries no centre or role. That permits the password-change flow while keeping tenant feature dispatches blocked. Centre selection explicitly checks the flag too, so posting a centre ID directly does not bypass the UI gate.

Password replacement validates the old and new values, rejects reusing the current password, and invokes Identity's change operation. Successful code clears the flag and updates the security stamp, then returns the new stamp to the endpoint. The endpoint issues a fresh centreless ticket. The user chooses a centre again through active-membership verification. Other tickets fail stamp validation after their positive cache windows expire. Thus the transition spans identity persistence, ticket contents, membership gate, application tenant marker, and frontend navigation; a flag alone would not establish it.

## Protecting the last owner during staff changes

Changing a role and deactivating staff follow parallel orchestration, but they do not directly edit a global user role. The target is the membership row inside the current centre. The handler loads that target with an expected version and explicit centre (`ChangeStaffRoleHandler.cs:22`). If no visible row exists, the operation returns a not-found error. This ordering prevents a foreign membership ID from reaching owner-count logic and revealing another centre's administrative state.

Next, the handler refuses changes to the caller's own membership. It compares user identity after loading the target rather than trusting a submitted user ID (`:28`). A system actor does not match that staff self-check, which reflects the trusted operator model. For an active owner losing the Owner role, the handler calls `LockActiveOwnersAsync`. It then refuses if the count is at most one. Deactivation uses the same owner protection when the target is an active owner, rather than applying the expensive lock to every secretary or teacher change.

The lock is necessary because two administrators could each observe two owners and simultaneously demote a different one. An unlocked count allows both requests to conclude that another owner remains, leaving none. The repository's locking operation is the coordination point, while the domain method handles the membership's own status and role rules. `Membership.ChangeRole` refuses inactive memberships and an unchanged role. `Deactivate` refuses a second deactivation. Neither domain method can independently count other owner rows, so that state-dependent invariant belongs in the handler and repository.

After a successful transition, the ordinary unit of work saves the membership versioned update, and audit interception records allowed changes. No login account is deleted. Other memberships held by the same person are untouched by these handler statements. A deactivated membership will no longer pass session revalidation for its centre. A changed role is now part of revalidation input, correcting the earlier design where role claims could remain stale while only activity was compared.

The practical modification boundary is therefore broader than the membership method. Adding another privileged role requires defining whether it can satisfy the last-administrator invariant, updating permission policy, and deciding how role freshness affects existing tickets. Removing the self-check or moving it before a tenant-scoped load changes both business behavior and disclosure order. Keep these checks as an ordered decision sequence rather than a collection of interchangeable `if` statements.

## Enrolling, withdrawing, and transferring without rewriting history

Enrollment creation is nested under a group route. The request body contains only the student ID; the group comes from the route and the centre from actor state (`EnrollmentEndpoints.cs`, `EnrollStudentRequest`). This transport shape prevents conflicting body and route group IDs. The handler does not accept an enrolled-count or capacity value from the client. It obtains both group state and count through the locking repository operation.

`GroupRepository.GetForEnrollmentAsync` uses parameterized `SELECT *, xmin ... FOR UPDATE`, allowing EF to compose its tenant filter and map the shadow version (`GroupRepository.cs:34`). Naming xmin explicitly matters because `SELECT *` omits PostgreSQL system columns. Only after the row lock is held does it count active enrollments. That count is then the input to `Group.EnsureCanEnroll`, whose `>=` comparison refuses a completely full group.

The handler resolves the student after the group. An unknown or foreign group therefore returns the group not-found outcome before anything about the student is disclosed. A visible archived student is refused. An existing active enrollment of that student in the group returns `enrollment.already_enrolled`. A unique partial index remains the race backstop for that same pair. If all checks pass, `Enrollment.Start` records the injected clock's current instant and active state. The endpoint returns 201 and points Location to the roster, since an individual enrollment has no separate read page in this route design.

Withdrawal and transfer are actions rather than deletion. The enrolled row remains, with an end instant and reason. A transfer first loads the versioned enrollment, verifies it is still active, and rejects a destination equal to its current group. It locks only the target group. The source only loses a seat, so locking it as well would add an unnecessary opportunity for opposing transfers to wait in a cycle. The handler requires the source and target to share a subject and checks the target duplicate and capacity rules.

One `now` value ends the old enrollment as Transferred and starts the new destination enrollment. If a stale version is detected during save, the new row also rolls back. If the destination fills before the command owns its lock, the command refuses without ending the source. A successful transfer returns 200 with the new enrollment ID, not a redirect or a mutated source ID. History can therefore show both stages rather than a row whose group mysteriously changed.

Teacher read access uses a separate scope. Roster reads first establish a visible owned group, then project active students and enrollment versions. Student enrollment history joins scoped groups, so a teacher sees only enrollments belonging to permitted groups. Being able to view enrollment history does not grant `students.view` or enrollment management. Keep permission and ownership checks distinct when adding links between student and group screens.

## Reassigning a teacher while preserving old sessions

A group's current teacher and a session's historical teacher can diverge intentionally. `AssignGroupTeacherHandler` loads a versioned group, resolves a visible teacher, and refuses an archived teacher (`AssignGroupTeacherHandler.cs:28`). Before changing either teacher's schedule, it acquires advisory locks for old and new teacher IDs. The repository sorts and de-duplicates those IDs, which gives concurrent operations a shared acquisition order.

The group transition changes its current TeacherId if the group is editable. The handler captures one current instant and asks for future scheduled sessions of that group. Every returned entity passes through `ReassignTeacher`; that domain method only changes a session when it is Scheduled and StartsAt is strictly after now (`ClassSession.cs:102`). Started or cancelled sessions preserve their prior teacher. No bulk update skips the tracked entities, so audit and version checks still run for each affected session.

The new teacher may already be scheduled at one of those times. The handler does not perform a potentially stale overlap pre-check. PostgreSQL's exclusion constraint rejects the save. Translation produces the teacher collision code, and rollback restores the group's old teacher and every session assignment. On success, future sessions move together while historical rows keep their original teaching identity.

This has a visible authorization consequence. Group detail scope follows the group's current teacher. Timetable history scope follows the teacher stored on each session. After reassignment, the new teacher can see the group's future work, while past sessions remain associated with the former teacher. A query that joins current group TeacherId to filter all sessions would silently rewrite visibility history, even if the underlying session rows remained untouched.

To add replacement teachers for a single session, define a separate transition rather than broadening group reassignment casually. Decide whether the group should change, whether old sessions remain fixed, which advisory resources must be locked, and which ownership scope should follow the one-off substitution. The current code supplies a coherent group-wide future reassignment contract; it does not imply every possible substitution workflow already exists.

## Applying migrations without giving the web process owner privileges

The long-running runtime context is registered from `ConnectionStrings:Postgres`. Migration configuration uses `ConnectionStrings:PostgresMigrations`. `MigrationRunner` deliberately constructs a separate short-lived context rather than resolving the runtime context (`MigrationRunner.cs:10`). It requires a nonblank migrations string, configures the context through the shared configurator, supplies an anonymous actor, applies pending migrations, and disposes the context asynchronously.

`Program.cs` recognizes an exact `migrate` CLI argument before building the HTTP pipeline. `MigrateCommand.RunAsync` applies migrations, logs a successful up-to-date result, and returns zero; an exception is logged and returns one (`MigrateCommand.cs:20`). Normal Development startup also applies migrations. Normal Production web startup does not. The source comment on `MigrationRunner` still says it is called by Development and seed only, although `MigrateCommand` now calls it too; treat that comment as an incomplete caller list.

The privilege split matters to RLS. Runtime readiness checks detect a connection running as a superuser, BYPASSRLS, or owner role because those privileges can undermine intended isolation. Migration operations need broader schema rights, extension installation, grants, and policy creation. Keeping those credentials out of the runtime DI context reduces the set of request paths capable of performing schema work. The code constructs owner access only in short-lived migration or operator tooling paths.

The schedule migration illustrates why reviewing generated files matters. EF creates the rule and session tables, ordinary keys, indexes, and CHECK constraints. Handwritten SQL installs `btree_gist` and creates exclusion constraints because those are not ordinary EF model declarations in the inspected implementation. It then grants runtime select/insert/update, revokes delete, and enables tenant RLS on both tables. The Down path removes those policies and constraints before dropping tables but intentionally leaves the extension installed because other objects might depend on it.

Do not regenerate a migration and assume the snapshot tells the whole story. The snapshot describes EF's model, while custom SQL contains operational security and exclusion behavior that model comparison cannot fully represent. When adding a new tenant table, review grants and RLS explicitly. When adding a custom constraint, connect its name to error translation. When planning rollback, inspect dependency order and whether historical data would be destroyed; a Down method existing in source is not proof that rollback is operationally safe for a populated production database.

## Following student and audit cursors through keyset pagination

Pagination divides a potentially large result into bounded pages. Offset pagination asks the database to skip an increasing number of rows. Keyset pagination instead remembers the ordered key of the last returned row and asks for rows after that key. The key must fully define order; a timestamp or name alone is insufficient when several rows have the same value. Current student pages order by normalized search name then ID, while audit pages order by occurrence time then ID, both descending for audit (`StudentReadService.cs:53`; `AuditReadService.cs:65`).

The student cursor encodes a fixed-width GUID, separator, and search name as UTF-8, then Base64 with URL-safe replacements and padding removal (`StudentCursor.cs:15`). The fixed ID position means a separator inside the search name does not confuse decoding. The cursor is opaque as an API convention: clients should carry it back rather than constructing their own. It is not encrypted, signed, or proof of authorization. Tenant filtering and permissions still apply to every page, even when a caller modifies the token.

For a student page, the read service first applies archive and search criteria, then decodes the cursor and adds a lexicographic predicate: search name greater than the cursor name, or equal name and greater ID. It orders by those same fields and takes `pageSize + 1`. The extra row tells the adapter whether another page exists without a separate total count. If extra data exists, it returns only `pageSize` rows and encodes the final returned row as the next cursor. Using the extra row itself as the next cursor would skip that row on the following request.

Primary parent information is loaded after the student page has been bounded. The service extracts page student IDs, queries primary links and parents for those IDs, and builds a dictionary. This avoids performing an individual parent query inside a loop for every returned student. The resulting list item includes the primary parent's name and canonical phone, while the full detail path returns all linked parents. This is one concrete reason to keep list DTOs separate from aggregate or detail DTOs: the query can fetch only what the table needs.

Audit cursor encoding uses UTC ticks plus a GUID (`AuditCursor.cs:15`). On the next page, the adapter selects entries with an earlier timestamp, or the same timestamp and smaller ID. The two comparisons match the descending ordering exactly. Actor filtering, entity filtering, and occurrence range predicates are applied before cursor paging. The user display name is obtained through a left join, allowing system or missing-user cases to retain the audit record rather than disappearing through an inner join. The projection intentionally does not return the actor's email (`AuditReadService.cs:11`).

Audit storage and API output have different shapes. The stored Changes document is an object keyed by field with before and after values. `ParseChanges` turns that object into a list of `AuditChangeDto` entries. Null stays null; booleans become lowercase text; numbers use their raw JSON text; strings use their text value (`AuditReadService.cs:95`). That conversion means a frontend display can receive one consistent before/after representation even though stored JSON contains several primitive kinds. It also means a new structured object value would require deliberate adaptation; the current fallback expects a string for kinds not handled explicitly.

A cursor decoder must treat its token as untrusted input. `TryDecode` restores Base64 characters and padding, catches format errors, checks its structural fields, and returns a parsed key. The name of a Try method does not itself guarantee every malformed value returns false. In the inspected audit decoder, a parsed `long` is passed to `new DateTimeOffset(ticks, TimeSpan.Zero)` without a date-range guard (`AuditCursor.cs:56`). Unsupported tick values can therefore throw after numeric parsing succeeds. This is a source-level boundary worth checking through the validator and HTTP tests rather than assuming the method's name proves total parsing safety.

Keyset paging is stable relative to its keys, but it is not a snapshot spanning several HTTP requests. If a student's name changes between pages, its sort position changes and a client may see movement. Audit entries are append-only and use occurrence-plus-ID order, giving a different practical behavior from mutable student names. No source here establishes a repeatable-read snapshot persisted across page requests. Explain that limit instead of promising a frozen result set.

When modifying either list, keep order, cursor encoding, and comparison direction synchronized. Adding a new sort mode requires a cursor capable of identifying that mode's complete key. Changing page size must not alter which row supplies the next cursor. Preserve tenant and ownership predicates before materialization. If a UI needs an exact total count, treat it as a new query and cost decision rather than assuming the existing cursor response already contains one.
