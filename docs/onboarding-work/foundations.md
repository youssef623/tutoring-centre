<a id="foundation-interfaces"></a>
## 6.17 Interfaces, abstraction, polymorphism, inheritance and composition

#### What it means

An interface names capabilities without prescribing their implementation. A class implements that contract. A caller using the interface can remain unaware of whether the work uses a database, memory or another service. That separation is abstraction. When the same interface call selects different concrete behaviour depending on the supplied object, the mechanism is polymorphism. These are language mechanisms; they do not require a dependency injection framework.

Inheritance and composition organize reuse differently. Inheritance says that one type is a specialized form of another. Composition gives an object collaborators and lets it delegate work. `Centre` derives its identity behaviour from `Entity`. `EnrollStudentHandler` instead receives an actor, clock and repositories. It composes those capabilities to run an operation. It does not derive from a large “business service” base class containing every feature.

#### The problem it solves

Without a port, a handler would know its database adapter:

```csharp
// Hypothetical: tightly coupled construction, not repository code.
var repository = new GroupRepository(databaseContext);
var group = await repository.FindAsync(groupId, cancellationToken);
```

That caller must now know the concrete class, how to construct its context and how long the context should live. A test of the handler must also arrange those details. With an interface, construction moves to the composition root and the handler names only the capability it uses.

#### How it works

The compiler checks that an implementation provides the interface members. At runtime the interface variable refers to a concrete object. Calling the member invokes that object's implementation. The type of the variable and the concrete type of the object are therefore different pieces of information. This is the bridge between the inward project dependency and the outward runtime call described in the architecture section.

An interface does not remove coupling entirely. The caller remains coupled to the contract's names, argument meanings, return types and behavioural assumptions. If `GetForEnrollmentAsync` promises a locked group with a count measured after locking, a fake returning an arbitrary count does not establish the database guarantee. Contract tests and real database tests address that difference.

#### How THIS project implements it

`src/TutoringCentre.Application/Academics/Enrollments/Commands/EnrollStudent/EnrollStudentHandler.cs:19` declares collaborators through `ICurrentActor`, `IClock`, `IGroupRepository`, `IStudentRepository` and `IEnrollmentRepository`. The handler's repository dependency is an Application port. `src/TutoringCentre.Infrastructure/Repositories/GroupRepository.cs:14` implements the group port using EF. `Infrastructure/DependencyInjection.cs` registers the port and adapter with a scoped lifetime.

The actual constructor declaration is:

```csharp
internal sealed class EnrollStudentHandler(
    ICurrentActor currentActor,
    IClock clock,
    IGroupRepository groups,
    IStudentRepository students,
    IEnrollmentRepository enrollments)
    : ICommandHandler<EnrollStudentCommand, EnrollStudentResult>
```

The `internal` visibility makes the concrete handler an assembly implementation detail. `sealed` prevents deriving another handler from it. The dispatcher addresses its implemented handler interface instead. Tests can exercise the same handler through supplied collaborators; the container is convenient construction machinery, not an essential property of the language pattern.

#### Execution example

An enrollment endpoint dispatches the command. The dispatcher resolves the closed handler interface. The service provider constructs `EnrollStudentHandler` with registered collaborators. The handler calls `groups.GetForEnrollmentAsync`. The concrete object is the Infrastructure adapter, so its EF query runs. The handler receives the port's result and continues without mentioning `DbContext`, SQL or Npgsql. A test can provide a different object implementing the same port and exercise the decision branches without opening a database.

#### Why this project needs it

Application rules are reused by HTTP, operator commands and test harnesses. Narrow collaborators keep transport and persistence details from spreading into those rules. The group port also gives a meaningful name to an important capability: obtaining a locked group and a capacity count is more specific than a generic `Get` method.

#### Alternatives

Direct concrete calls reduce files and indirection, but make construction and substitution harder. Delegates can represent a small capability without defining an interface. An abstract base class can combine a contract and shared implementation, but then subclasses inherit that implementation's assumptions. The repository does not state a universal reason for every individual interface. The interpretation supported by the project layout is that ports preserve boundaries and allow independent logic tests.

#### Important details

An interface is not a security boundary by itself. A forged actor supplied by trusted edge code would still influence the handler. Likewise, a class being sealed does not prove its dependencies are correct. Visibility, registration, tests and database constraints address different properties. Read them together rather than treating one keyword as the whole design.

#### What I should understand before modifying this code

Define a capability in the layer that needs it. Keep EF and HTTP types out of Application ports. Update every implementation and meaningful fake when a contract changes. Check whether the new method promises merely a value or a stronger condition such as a lock, tenant scope or consistent read. The stronger condition needs proof at the layer that actually provides it.

<a id="foundation-async"></a>
## 6.18 Asynchronous work, tasks, cancellation and disposal

#### What it means

Database and network operations spend time waiting for another process. Blocking code holds a thread during that wait. Asynchronous code represents completion as a `Task` and can suspend the method while the operation is incomplete. The compiler turns an `async` method into a state machine that remembers where to continue. This helps a web server use its threads for other requests while database I/O is pending.

Calling an asynchronous method and awaiting its returned task are separate events. Many methods begin work when called. `await` observes completion and arranges continuation; it does not generally act as a universal start switch. A LINQ expression describes a query until a terminal operation executes it. Calling `AnyAsync` or `ToListAsync` is such an operation; awaiting the returned task receives its eventual result.

#### The problem it solves

```csharp
// Hypothetical blocking call; do not introduce this into the async path.
var count = repository.CountAsync(cancellationToken).Result;

// Asynchronous composition preserves the task chain.
var count = await repository.CountAsync(cancellationToken);
```

Both forms can produce a number, but the blocking version occupies the calling thread while waiting. It can also introduce deadlocks in environments with synchronization contexts. Merely writing `async` does not make CPU work parallel or give a database operation a second independent transaction.

#### How it works

At an incomplete `await`, the method returns an incomplete task to its caller. When the awaited operation completes, the continuation resumes with either a value or an exception. Exceptions propagate through the awaited chain. A `CancellationToken` adds a cooperative signal: code must pass it to operations and those operations must observe it. Cancellation is a request to stop, not an instant thread termination.

Resource ownership remains necessary. A database context, transaction and DI scope must be disposed even when a task faults. `await using` waits for asynchronous disposal at scope exit. A `finally` block runs when normal execution, return or exception unwinding leaves its associated `try`.

#### How THIS project implements it

`Application/Common/Cqrs/Dispatcher.cs:35` and its query method return `Task<Result<TResponse>>`. They await validators, transaction operations and handlers. `Infrastructure/Repositories/GroupRepository.cs:31` awaits the locked group load and then awaits the active enrollment count. That order is part of the capacity guarantee: the count must happen after the row lock is held.

The dispatcher deliberately uses a non-cancelled token for exception cleanup:

```csharp
catch
{
    await _unitOfWork.RollbackAsync(CancellationToken.None);
    throw;
}
```

This is current source. A request cancellation should not prevent the attempt to release its transaction. The normal failure-result path initially passes its existing token to rollback; if cleanup throws, the catch still handles exception cleanup. `UnitOfWork.RollbackAsync` stores and clears the transaction reference before awaiting rollback, then disposes and clears tracking in its `finally` block.

#### Execution example

An enrollment command begins a transaction and calls the handler. The group adapter starts the SQL operation that requests `FOR UPDATE`. If another transaction holds the row, the database waits while the application task remains incomplete. The handler does not count seats yet. After the lock operation completes, execution resumes and counts active enrollment rows. The handler returns a result. The dispatcher saves and commits or rolls back. A cancellation can interrupt an observed operation and propagate to the dispatcher, whose exception cleanup attempts rollback with `CancellationToken.None`.

#### Why this project needs it

HTTP, credential verification, database reads and transaction work already form asynchronous chains. Preserving those chains prevents avoidable thread blocking. Passing a cancellation token through the boundary avoids continuing expensive work after a disconnected client when the provider can stop it. Explicit cleanup keeps a cancelled request from leaving an application-owned transaction object alive in the scope.

#### Alternatives

Synchronous repository methods are easier to follow for small scripts, but tie a waiting request to a thread. Callbacks and events can represent asynchronous completion without `await`, with more manual control-flow management. Parallel tasks can improve independent work, but this context and transaction are not a license to issue concurrent EF operations through the same `DbContext`.

#### Important details

Concurrency means operations overlap in time; parallelism means work executes simultaneously. Two requests waiting for the same row lock are concurrent even though their capacity decisions are serialized. Retrying a cancelled command is also a new business action unless idempotency exists; cancellation alone does not prove the server committed nothing before the client stopped waiting.

#### What I should understand before modifying this code

Pass the request token down to I/O operations. Do not replace awaits with `.Result` or `.Wait()`. Preserve the lock-then-count order. Keep explicit ownership of scopes and transactions. When adding parallel work, first prove that it uses independent resources and does not share a non-thread-safe context or depend on a common transaction order.

<a id="foundation-types"></a>
## 6.19 Generics, records, nullability and pattern matching

#### What it means

Generics define a type or method with type parameters. `Result<T>` is one definition that can carry many different successful values. A generic constraint restricts the legal substitutions. The dispatcher pairs a command type and response type through `where TCommand : ICommand<TResponse>` rather than passing untyped objects and hoping the result can be cast later.

Records provide value-oriented equality and generated members. They are suitable for request messages and response data whose meaning comes from their contents. Entities instead retain identity as state changes. Nullable reference annotations describe where `null` is possible to the compiler. Pattern matching inspects type and structure while optionally binding matched values to local variables.

#### The problem it solves

```csharp
// Hypothetical untyped dispatcher: mismatches become runtime errors.
object response = await SendObjectAsync(command);
var created = (EnrollStudentResult)response;

// Typed dispatch makes the request/response relationship explicit.
Result<EnrollStudentResult> created =
    await dispatcher.SendAsync<EnrollStudentCommand, EnrollStudentResult>(command, ct);
```

The typed form still permits business failure, represented by `Result`. It removes an unrelated class of unsafe casts. Type safety and business correctness are different: the compiler can verify that the result type matches while the domain still has to verify that a group has room.

#### How it works

The compiler substitutes concrete types for generic parameters and checks constraints. Reflection-based registration discovers the handler interfaces and registers closed combinations. The dispatcher asks for the exact interface corresponding to its type parameters. No switch statement listing every feature is required.

Pattern matching can both test and extract. In `StaffActor { Role: { } role }`, the first pattern requires a staff actor, the nested non-null pattern requires a role value, and the variable `role` becomes available in the selected branch. Nullable analysis then understands that branch differently from a path where the value could be absent.

#### How THIS project implements it

`Application/Common/Cqrs/Dispatcher.cs` uses generic handler resolution and result types. Its permission switch distinguishes a trusted `SystemActor`, a `StaffActor` with a role and all other cases:

```csharp
var allowed = actor switch
{
    SystemActor => true,
    StaffActor { Role: { } role } => RolePermissions.Holds(role, permission),
    _ => false,
};
```

`Api/Program.cs` uses list patterns for operator commands: `args is ["migrate"]` matches exactly one argument, while `args is ["reset-password", var resetEmail, var resetPassword]` matches exactly three and captures two values. This is not a substring check or prefix check; additional arguments prevent that branch from matching.

The current actor and result model also contain nullable data intentionally. A staff user can be signed in without a selected centre. An error is absent on a successful result. Callers must establish which state they hold before reading the associated value. Null-forgiving syntax is a compiler assertion by the author, not a runtime validation step.

#### Execution example

At login, credentials produce an authenticated user identifier and stamp. A `StaffActor` is created with null centre and role. The profile query is dispatched using its concrete query and DTO type. If exactly one membership can be selected, the endpoint creates a new principal with a centre and role. Later, a tenant-scoped command reaches `CheckTenantScope`, which refuses the null-centre state before `CheckPermission` uses the actor's role. The optional values model a real intermediate state in the workflow.

#### Why this project needs it

Many requests share one dispatcher without sharing result shapes. Strong type pairs keep that common pipeline reusable. Explicit nullability describes session states that cannot be represented correctly by inventing an empty centre GUID or default owner role. Pattern matching makes those state distinctions visible near the decisions they control.

#### Alternatives

Untyped dictionaries and `object` values move checks to runtime. A separate dispatcher method per request creates repetitive code. Class inheritance can replace some record hierarchies, but does not automatically provide value semantics. Optional-value types can be more explicit than `null`, at the cost of another abstraction. The repository's approach follows the C# features used throughout its current source; motivations for every syntax choice are not independently documented.

#### Important details

A record is not deeply immutable merely because its positional properties are init-only. A collection referenced by a record can still be mutable. Equality of nested reference values also depends on those values' equality implementations. Likewise, nullable warnings are compile-time analysis; deliberately supplying `null` from reflection or external data still requires guards and validation.

#### What I should understand before modifying this code

Preserve request/response generic relationships. Do not give an unselected session a default role just to remove nullable warnings. Update pattern branches when introducing a new actor kind. Inspect the fallback branch: denying unknown actor types is a deliberate security property. Treat `!` as a proof obligation and identify the earlier branch or marker that makes the assertion true.

<a id="foundation-exceptions"></a>
## 6.20 Exceptions, result values and guaranteed cleanup

#### What it means

An exception interrupts normal control flow and searches outward for a matching catch. A result value is ordinary data returned to the caller. This repository uses both because an expected business refusal and an unexpected programming or infrastructure fault have different meanings. A full group is an ordinary conflict; an invalid service registration is a fault. Both require correct transaction cleanup, but they should not necessarily produce the same HTTP response or log severity.

#### The problem it solves

```csharp
// Hypothetical: silently converting every fault destroys the distinction.
try { return await handler.HandleAsync(command, ct); }
catch { return Result<EnrollStudentResult>.Failure(someBusinessError); }
```

This code would turn a database outage or null dereference into a misleading business refusal. At the other extreme, throwing an exception for every full group would force every caller to know and catch that exception class. The current dispatcher branches explicitly on failed results and rethrows unexpected exceptions after cleanup.

#### How it works

An awaited task rethrows its failure into the awaiting method. A bare `throw;` inside a catch preserves the original exception's stack information. `throw exception;` establishes a different throw location and can obscure the useful origin. `finally` runs on both successful and exceptional exit, so it is the proper place to release owned resources whose lifetime must end either way.

Disposal and rollback are related but distinct. Rollback asks the database to undo the transaction. Disposal releases the transaction object and its resources. Clearing EF tracking discards the in-memory pending state. Keeping only one of those actions can leave another part of the scope inconsistent.

#### How THIS project implements it

`Infrastructure/Persistence/UnitOfWork.cs:129` obtains the active transaction before committing. The `finally` disposes it and clears `_transaction`. `RollbackAsync` copies and nulls the field before awaiting rollback and clears the change tracker in its cleanup block. A second call sees no transaction and returns. The dispatcher uses these operations for failed handler results, failed save results and exceptions.

Persistence save exceptions have a narrow translation boundary. `SaveChangesAsync` catches EF update/concurrency exception types, calls `PersistenceErrorTranslator.TryTranslate`, and rethrows when the translator returns null. A known unique or concurrency conflict can therefore become a stable business error; an unrelated foreign-key or check failure is not automatically mislabeled as that same conflict.

#### Execution example

An enrollment handler adds a tracked entity and returns success. Saving reaches PostgreSQL and encounters a recognized conflict. The translator returns a failed `Result`. The unit of work returns it to the dispatcher. The dispatcher rolls back and returns a failed response result. If saving instead throws an untranslated exception, the dispatcher catches it, attempts rollback with a non-cancelled token and rethrows. The HTTP exception boundary then produces the generic unexpected-error contract. The original exception remains available to server-side logging rather than being sent to the browser verbatim.

#### Why this project needs it

Commands may touch several entities and audit rows. Cleanup must apply regardless of whether failure is represented by a value or exception. The translation boundary also protects error semantics: the frontend can react to `group.full` or a stale version without treating every failure as an internal server error, while real faults remain visible to maintainers.

#### Alternatives

Exception-only business APIs reduce result boilerplate but hide ordinary failure possibilities in signatures. Result-only APIs must still decide how to handle runtime faults and cancellation. An ambient `TransactionScope` changes how a transaction is discovered but does not remove the need to understand ownership and cleanup. Database retries are another separate policy; catching an exception does not make replaying a command safe automatically.

#### Important details

Cleanup can itself fail. A rollback exception can obscure an earlier error unless explicitly managed. The current implementation's ordering and idempotent no-transaction branch are concrete source behaviours, not a proof that the network can never fail during rollback. Likewise, a failed HTTP response after commit does not imply the database was rolled back; the transaction might already have succeeded before the response was lost.

#### What I should understand before modifying this code

Do not catch broadly and invent a business code for every exception. Keep known persistence translation centralized. Preserve `throw;` when rethrowing the same fault. Check every early return for resource ownership. When adding a new transactional path, identify the exact begin, save, commit, rollback and disposal operations and prove their behaviour with success and failure tests.

<a id="foundation-metadata"></a>
## 6.21 Attributes, reflection, scanning and generated code

#### What it means

An attribute is metadata attached to a declaration. It has an effect only when a compiler, generator, framework or runtime reader interprets it. Reflection inspects types and members at runtime. A source generator runs while compiling and creates code that becomes part of the build. These are different mechanisms that can look equally “automatic” when reading only a decorated class.

#### The problem it solves

Without scanning, every handler requires a manual registration:

```csharp
// Hypothetical explicit registration style.
services.AddScoped<ICommandHandler<EnrollStudentCommand, EnrollStudentResult>, EnrollStudentHandler>();
```

That is clear but repetitive across many requests, and a forgotten registration fails only when resolution is attempted unless another check catches it. Scanning locates implementations systematically. Tests still need to establish that each request has exactly one valid handler; scanning is not a substitute for that invariant.

#### How it works

The scanner obtains assembly types, filters concrete classes, examines their implemented generic interfaces and registers the closed handler contracts. A container later resolves the selected implementation. The scanner does not execute the handler at registration time. It only records how the object should be built.

Test attributes are read by test discovery. Option-validation attributes are read by validation infrastructure. Generated regex and logging attributes are read by compiler tooling. Endpoint metadata such as operation names and declared responses is consumed by routing and OpenAPI generation. Each apparent annotation therefore needs an identified consumer before its effect is understood.

#### How THIS project implements it

`Application/DependencyInjection.cs` delegates handler registration to `Application/Common/Cqrs/HandlerRegistration.cs`. The dispatcher later resolves `ICommandHandler<TCommand,TResponse>` or `IQueryHandler<TQuery,TResponse>`. `Api/Program.cs` registers OpenAPI document and schema transformers. The schema transformer recognizes `TimeOnly` and `DateTimeOffset` so the contract describes their custom converter output as strings instead of leaving the generator with unknown shapes.

The frontend also has generated source. The route tree connects file routes, and Orval consumes the committed OpenAPI document to generate API types/functions/hooks. Those generated artifacts are not independent design authorities: their inputs and generator configuration explain how they should change.

#### Execution example

A new endpoint declares an operation name and a response DTO. Building the API executes the OpenAPI tooling path. `Program.cs` recognizes the export host, supplies generation configuration and still maps the endpoints. The exporter reads endpoint and schema metadata. Orval then reads the emitted contract and produces TypeScript. A component importing the affected type is checked by TypeScript. The CI drift checks compare generated files with committed artifacts, catching a developer who changes the backend but forgets the matching generated output.

#### Why this project needs it

The repository has many use cases and a substantial client contract. Discovering registrations and generating types reduces repeated manual lists. More importantly, it moves some errors into build or test checks: a changed contract can produce a compiler failure rather than an unnoticed runtime mismatch. The generation path still has startup side effects and configuration requirements, which is why it deserves its own explained branch.

#### Alternatives

Manual composition is explicit and compiler-visible but longer. A mediator library can supply scanning and pipeline behaviours. Source-generated DI can move object graph checks earlier. Handwritten client types remove generation tooling but create another copy of API shapes. None of these alternatives removes the need to keep transport definitions, runtime conversion and consumers consistent.

#### Important details

An attribute does nothing by its name alone. Verify the registration that enables its consumer. Scanning depends on which assembly is supplied and what the filter admits. Generated code can be stale even when it looks correct. A test that scans an empty collection can pass vacuously; architecture tests use non-empty guards to make their scope meaningful.

#### What I should understand before modifying this code

Find the reader of any annotation before changing it. Keep generated source changes tied to their inputs. Add or adjust schema metadata when custom JSON converters change wire shapes. Confirm that a new handler is concrete, implements the expected closed interface and is discoverable in the scanned assembly. Preserve tests that reject duplicate or missing handlers.

<a id="foundation-json"></a>
## 6.22 JSON, parameter binding and the wire contract

#### What it means

Serialization converts values into a transferable representation; deserialization builds values from that representation. HTTP model binding supplies an endpoint's parameters from route segments, query strings, request bodies or registered services. A JSON record is not automatically trusted merely because deserialization succeeded. Shape validation, permission checks and business rules still have work to do.

Dates, times and instants are different kinds of values. A `DateOnly` is a calendar date. A `TimeOnly` is a local time of day. A `DateTimeOffset` identifies an instant with an offset representation. Weekly scheduling needs all three concepts because “Saturday at 18:00 in Cairo” is a recurrence rule, while a particular dated class session has UTC start and end instants.

#### The problem it solves

```json
{"startTime":"18:00","durationMinutes":90}
```

This shape communicates a local time and duration. Sending a machine-local date/time string instead would silently depend on the browser or server zone. Similarly, accepting misspelled JSON members without feedback could leave a DTO property at its default while the user believes it was set. The API configures strict unknown-member handling and explicit time conversion to keep those failures visible.

#### How it works

The endpoint delegate signature identifies its parameter types. The framework selects the binding source according to the route and available services. JSON options control body conversion. If binding fails and `ThrowOnBadRequest` is enabled, the failure reaches the common exception handler instead of becoming an unrelated empty response. A custom converter must be paired with contract schema metadata when the exporter cannot infer its representation.

#### How THIS project implements it

`Api/Program.cs:76` configures `UnmappedMemberHandling.Disallow`, string enum conversion, `HourMinuteTimeOnlyJsonConverter` and `UtcInstantJsonConverter`. It separately enables `RouteHandlerOptions.ThrowOnBadRequest`. The OpenAPI schema transformer later declares string types for the custom converted CLR types, preserving null in nullable non-parameter schemas.

The backend converters and generated client serve complementary roles. The converter determines actual runtime JSON. The contract tells tooling what shape to expect. TypeScript can check compiled consumers of that contract, but a network response is still runtime data. The shared API wrapper validates error payloads through its own runtime guard rather than assuming that a generated interface has checked incoming bytes.

#### Execution example

The schedule form collects a weekday, local time, duration and effective range. The generated function serializes a request body. The endpoint binds the JSON into its request type. A malformed `startTime` is refused by conversion; a well-formed but invalid range reaches request validation or domain checks. A valid rule is passed to the generator with an explicit centre zone. Generated sessions return UTC instants, which the browser formats using the selected centre's time zone. No stage should reinterpret a local time as an already-UTC instant.

#### Why this project needs it

The API and SPA are compiled separately and communicate through JSON. Scheduling adds values whose meaning cannot be recovered from a plain string without a contract. Strict conversion, schema transformation, generated client types and tests cooperate to make the contract both machine-readable and understandable to a maintainer.

#### Alternatives

Lenient unknown-member handling eases some client/server rollout sequences but hides typos. Unix timestamps represent instants compactly but cannot replace recurrence dates and wall times. Protobuf or another schema-driven protocol offers different compatibility rules. Runtime response schemas can validate all successful bodies as well as errors, at the cost of another validation layer; generated TypeScript alone does not provide that runtime proof.

#### Important details

Changing a converter changes the wire contract even if the C# property type stays the same. A schema-only change can also make the client believe a shape that the runtime never emits. Unknown-member rejection has a forward-compatibility cost: a newer client sending a newly introduced field to an older server can fail. Keep the server, contract and generated client updates coordinated.

#### What I should understand before modifying this code

Identify whether a value is a local date, local time or instant. Preserve the configured JSON representation. Update the schema transformer when required. Add endpoint response metadata and regenerate the contract/client. Keep malformed-binding tests distinct from domain validation tests: they demonstrate different layers and may legitimately produce different machine codes.
