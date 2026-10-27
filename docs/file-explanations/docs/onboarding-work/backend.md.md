# `docs/onboarding-work/backend.md`

## Purpose

Documentation covering Backend teaching material for the current repository; 6.1 Clean Architecture, dependency inversion, and the composition root; 6.2 Dependency injection, lifetimes, and shared scope ownership; 6.3 CQRS and the current dispatcher pipeline; 6.4 Results, expected failures, and exception boundaries. Treat historical claims according to the revision described in the file.

## Where it fits

Category: **documentation**. [Open current file](../../../onboarding-work/backend.md). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 1056 lines and 146,162 bytes. Source hash: `11f8974e7803018fa9cfcf928983686017c373403fd3f6717ec793d2867ebec9`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../onboarding-work/backend.md#L10): `A dependency is something a piece of code needs to compile or execute. A handler that directly constructs a database context depends on that context's implementation, provider, configuration, and lifetime. Dependency inversion changes ownership of that relationship. The handler's layer declares an interface describing the capability it needs. An outer layer implements that interface. The handler can call the implementation at runtime while its compiled assembly refers only to the interface.`
- [Line 34](../../../onboarding-work/backend.md#L34): `The compiler knows an interface has particular methods. An object implementing that interface can be passed to any consumer requiring the interface. A call through the interface dispatches to the actual object's implementation. Consequently, the compile-time reference direction and the runtime call direction can differ. Application can compile against its own repository port while the runtime repository object comes from Infrastructure.`
- [Line 36](../../../onboarding-work/backend.md#L36): `Composition happens before requests execute. Registrations tell the container that resolving a repository interface should construct a particular Infrastructure class. Constructor parameters recursively describe the rest of the object graph. The handler does not locate a connection string, build a provider, or decide whether a repository is real or fake.`
- [Line 60](../../../onboarding-work/backend.md#L60): `An interface alone does not prove decoupling. Its parameters and return types must also avoid exposing the implementation. A port returning IQueryable<ApplicationUser> would still expose persistence and Identity decisions. Current read adapters project to application DTOs rather than returning tracked entities. Conversely, an entity's lack of EF imports does not mean EF cannot populate it: persistence uses mapping and materialization separately from its public factory.`
- [Line 176](../../../onboarding-work/backend.md#L176): `MediatR behaviors can package a similar pipeline. Service classes can perform orchestration directly. Endpoint filters can perform HTTP-specific validation, but cannot on their own make CLI calls follow the same rules. A full CQRS deployment with a separate read database adds synchronization concerns absent here. The repository does not explicitly state why every detail of the handwritten dispatcher was chosen; the following is an interpretation based on the implementation: keeping its order in one small class makes the security and transaction mechanics directly inspectable.`
- [Line 199](../../../onboarding-work/backend.md#L199): `If every business refusal throws an exception, callers must know every exception class and remember its mapping. Forgetting to catch a duplicate-name exception turns a normal user outcome into HTTP 500. If every fault becomes a generic failed result, genuine bugs disappear into ordinary validation messages. The project needs both mechanisms with a clear boundary.`
- [Line 235](../../../onboarding-work/backend.md#L235): `Custom exception types can express business outcomes but require a mapping convention. Nullable returns lose the reason for failure. Discriminated unions can make success and failure branching more explicit, but C# callers still need a consistent project convention. Libraries such as a Result package can add composition helpers; this implementation keeps a small local type. The repository does not explicitly state why every Result design detail was chosen. The following is an interpretation based on the implementation: simple factories and a small closed kind enum make the transport mapping easy to audit.`
- [Line 288](../../../onboarding-work/backend.md#L288): `User input normalization is also a business decision. Name cleanup and search normalization are different operations: display text can preserve spelling while a separate normalized field supports matching. Phone parsing creates a canonical value before uniqueness comparison. Do not apply a generic trim or lowercasing function to every value and assume equivalent semantics.`
- [Line 337](../../../onboarding-work/backend.md#L337): `Immutability differs by object. PhoneNumber has a get-only value; Enrollment can transition from active to ended. A record type does not automatically make all nested collections immutable. A read-only collection interface prevents ordinary callers from using mutation methods through that interface, while the root still owns the mutable list internally.`
- [Line 514](../../../onboarding-work/backend.md#L514): `State the invariant precisely. For one-record lost updates, preserve the version contract. For counts, identify the row every competing operation must lock before counting. For uniqueness, keep the database constraint even if a pre-check exists. For scheduling, preserve sorted teacher lock acquisition and the exclusion constraints. Review transfer operations for lock cycles: the current transfer locks only its destination because the source only loses a seat (TransferEnrollmentHandler.cs:15). Adding an unnecessary source lock changes deadlock behavior.`
- [Line 647](../../../onboarding-work/backend.md#L647): `Read handlers that require ownership ask ITeacherScopeProvider. Owners and secretaries get All because RoleScopes.IsLimitedToOwnGroups only returns true for Teacher. A teacher's linked active teacher record produces Own(teacherId). Missing or archived teacher links produce None. Read adapters apply the teacher predicate in SQL rather than loading all centre rows and filtering them afterward.`
- [Line 665](../../../onboarding-work/backend.md#L665): `The product distinguishes owner administration, secretary operations, and teacher read access. Staff can work in multiple centres and have a different role in each. Teachers have both login accounts and business teacher records, which are not interchangeable identifiers. The scope provider is the bridge between authenticated account identity and the academic teacher record that owns groups or sessions.`
- [Line 675](../../../onboarding-work/backend.md#L675): `Permission and ownership scope are not identical types of filter. RolePermissions is a synchronous map requiring no lookup. Teacher scope requires an active linked academic record and is therefore resolved asynchronously. Caching that result once per request avoids repeated identical lookup, but it is not a cross-request authorization cache.`
- [Line 688](../../../onboarding-work/backend.md#L688): `An audit log records business changes with enough context to answer who changed which record and what changed. It differs from operational logging, which records process behavior and failures. Audit entries should correspond to committed state changes, preserve history, and avoid turning sensitive data into a second uncontrolled copy.`
- [Line 706](../../../onboarding-work/backend.md#L706): `AuditInterceptor is scoped because it receives IClock, ICurrentActor, and ICorrelationContext (AuditInterceptor.cs:22). It runs after timestamp stamping and before the tenant write guard, as stated in its class documentation. Platform bootstrap with no actor centre is deliberately logged rather than audited (:64). For an actor with a centre, it resolves Staff or System actor type and optional user ID, captures one timestamp and correlation ID, and snapshots tracked changes (:78).`
- [Line 730](../../../onboarding-work/backend.md#L730): `No-op changes should not produce misleading history. Updated actions with no actual allowed-field differences return null (AuditDiffBuilder.cs:66). A row might change only technical fields without an audit record if policy does not allow them. Conversely, Created and Deleted representations include allowed fields even when a value is null, because the action describes the whole allowed snapshot.`
- [Line 830](../../../onboarding-work/backend.md#L830): `The application's centres carry IANA zones, and scheduling is expressed by people in local dates and times. Accurate instants are necessary for overlap checks, future-only cancellation, and historical teacher assignment. Explicit DST handling removes a class of bugs that ordinary weekly date arithmetic hides. Database overlap enforcement remains authoritative when two requests generate or reassign sessions concurrently.`
- [Line 836](../../../onboarding-work/backend.md#L836): `The migration comments explicitly explain half-open ranges, partial scheduled status, and group-constraint creation before teacher. It states the group-first order aims to return the more precise group collision when both could fail. The repository does not establish a universal room-booking policy from the presence of the optional room string; do not claim room uniqueness merely because sessions record rooms.`
- [Line 853](../../../onboarding-work/backend.md#L853): `Professional architectural vocabulary rests on language mechanisms. An interface declares callable members without selecting an implementation. Polymorphism lets the same interface call execute different implementations. Composition means an object holds collaborators. Inheritance means a type derives behavior or identity from a base type. Generics parameterize types while retaining compile-time relationships. Async methods represent incomplete work through tasks. Records provide generated value-oriented behavior. Reflection inspects type metadata at runtime. None of these is magic; each has a concrete role in the inspected code.`
- [Line 861](../../../onboarding-work/backend.md#L861): `Hypothetical object Send(object message) permits passing a request whose result is cast to the wrong type. The actual SendAsync<TCommand,TResponse> constrains the command to ICommand<TResponse> (Dispatcher.cs:35). That relationship tells the compiler which result type belongs to the request and which closed handler interface must resolve.`
- [Line 865](../../../onboarding-work/backend.md#L865): `An interface reference stores an object whose runtime type implements the contract. Calling its method selects that implementation. Generics let one dispatcher algorithm operate on many request types while retaining typed results. Result<EnrollStudentResult> and Result<AddScheduleRuleResult> use the same result implementation but expose different success types. Nullable annotations mark references that may be absent; they do not themselves add runtime checks.`
- [Line 903](../../../onboarding-work/backend.md#L903): `Trace types before tracing frameworks. Determine which interface is requested, which implementation is registered, which scope owns it, and which terminal await executes I/O. Preserve generic response pairing and explicit null branches. Do not use .Result or .Wait() to simplify an async method. Pass cancellation where supported and retain non-cancelled rollback. Understand who reads an attribute before copying it. Keep business operation sequencing explicit when introducing parallelism, because lock ordering and one-context ownership are correctness constraints.`
- [Line 928](../../../onboarding-work/backend.md#L928): `Program.cs:75 rejects unmapped JSON fields, adds camelCase enum conversion, and registers the local-time and UTC-instant converters. HourMinuteTimeOnlyJsonConverter accepts exactly HH:mm with invariant culture and rejects seconds, offsets, and instants (HourMinuteTimeOnlyJsonConverter.cs:17). UtcInstantJsonConverter reads through the standard DateTimeOffset reader and writes a normalized UTC string ending in Z (UtcInstantJsonConverter.cs:17). These directions are asymmetric by design: UTC output normalization does not imply input offsets are forbidden.`
- [Line 1048](../../../onboarding-work/backend.md#L1048): `Audit cursor encoding uses UTC ticks plus a GUID (AuditCursor.cs:15). On the next page, the adapter selects entries with an earlier timestamp, or the same timestamp and smaller ID. The two comparisons match the descending ordering exactly. Actor filtering, entity filtering, and occurrence range predicates are applied before cursor paging. The user display name is obtained through a left join, allowing system or missing-user cases to retain the audit record rather than disappearing through an inner join. The projection intentionally does not return the actor's email (AuditReadService.cs:11).`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

````text
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
````

The excerpt stops at line 24; use the original file for the remaining 1032 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
