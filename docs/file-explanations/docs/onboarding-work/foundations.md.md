# `docs/onboarding-work/foundations.md`

## Purpose

Documentation covering 6.17 Interfaces, abstraction, polymorphism, inheritance and composition; 6.18 Asynchronous work, tasks, cancellation and disposal; 6.19 Generics, records, nullability and pattern matching; 6.20 Exceptions, result values and guaranteed cleanup; 6.21 Attributes, reflection, scanning and generated code. Treat historical claims according to the revision described in the file.

## Where it fits

Category: **documentation**. [Open current file](../../../onboarding-work/foundations.md). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 339 lines and 32,247 bytes. Source hash: `172d8c5674061665dd6307060a9835f734754f1132b853cc2e0d19cb79b618cf`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../onboarding-work/foundations.md#L6): `An interface names capabilities without prescribing their implementation. A class implements that contract. A caller using the interface can remain unaware of whether the work uses a database, memory or another service. That separation is abstraction. When the same interface call selects different concrete behaviour depending on the supplied object, the mechanism is polymorphism. These are language mechanisms; they do not require a dependency injection framework.`
- [Line 8](../../../onboarding-work/foundations.md#L8): `Inheritance and composition organize reuse differently. Inheritance says that one type is a specialized form of another. Composition gives an object collaborators and lets it delegate work. Centre derives its identity behaviour from Entity. EnrollStudentHandler instead receives an actor, clock and repositories. It composes those capabilities to run an operation. It does not derive from a large “business service” base class containing every feature.`
- [Line 24](../../../onboarding-work/foundations.md#L24): `The compiler checks that an implementation provides the interface members. At runtime the interface variable refers to a concrete object. Calling the member invokes that object's implementation. The type of the variable and the concrete type of the object are therefore different pieces of information. This is the bridge between the inward project dependency and the outward runtime call described in the architecture section.`
- [Line 26](../../../onboarding-work/foundations.md#L26): `An interface does not remove coupling entirely. The caller remains coupled to the contract's names, argument meanings, return types and behavioural assumptions. If GetForEnrollmentAsync promises a locked group with a count measured after locking, a fake returning an arbitrary count does not establish the database guarantee. Contract tests and real database tests address that difference.`
- [Line 35](../../../onboarding-work/foundations.md#L35): `internal sealed class EnrollStudentHandler(`
- [Line 44](../../../onboarding-work/foundations.md#L44): `The internal visibility makes the concrete handler an assembly implementation detail. sealed prevents deriving another handler from it. The dispatcher addresses its implemented handler interface instead. Tests can exercise the same handler through supplied collaborators; the container is convenient construction machinery, not an essential property of the language pattern.`
- [Line 56](../../../onboarding-work/foundations.md#L56): `Direct concrete calls reduce files and indirection, but make construction and substitution harder. Delegates can represent a small capability without defining an interface. An abstract base class can combine a contract and shared implementation, but then subclasses inherit that implementation's assumptions. The repository does not state a universal reason for every individual interface. The interpretation supported by the project layout is that ports preserve boundaries and allow independent logic tests.`
- [Line 60](../../../onboarding-work/foundations.md#L60): `An interface is not a security boundary by itself. A forged actor supplied by trusted edge code would still influence the handler. Likewise, a class being sealed does not prove its dependencies are correct. Visibility, registration, tests and database constraints address different properties. Read them together rather than treating one keyword as the whole design.`
- [Line 150](../../../onboarding-work/foundations.md#L150): `The typed form still permits business failure, represented by Result. It removes an unrelated class of unsafe casts. Type safety and business correctness are different: the compiler can verify that the result type matches while the domain still has to verify that a group has room.`
- [Line 154](../../../onboarding-work/foundations.md#L154): `The compiler substitutes concrete types for generic parameters and checks constraints. Reflection-based registration discovers the handler interfaces and registers closed combinations. The dispatcher asks for the exact interface corresponding to its type parameters. No switch statement listing every feature is required.`
- [Line 185](../../../onboarding-work/foundations.md#L185): `Untyped dictionaries and object values move checks to runtime. A separate dispatcher method per request creates repetitive code. Class inheritance can replace some record hierarchies, but does not automatically provide value semantics. Optional-value types can be more explicit than null, at the cost of another abstraction. The repository's approach follows the C# features used throughout its current source; motivations for every syntax choice are not independently documented.`
- [Line 189](../../../onboarding-work/foundations.md#L189): `A record is not deeply immutable merely because its positional properties are init-only. A collection referenced by a record can still be mutable. Equality of nested reference values also depends on those values' equality implementations. Likewise, nullable warnings are compile-time analysis; deliberately supplying null from reflection or external data still requires guards and validation.`
- [Line 292](../../../onboarding-work/foundations.md#L292): `Find the reader of any annotation before changing it. Keep generated source changes tied to their inputs. Add or adjust schema metadata when custom JSON converters change wire shapes. Confirm that a new handler is concrete, implements the expected closed interface and is discoverable in the scanned assembly. Preserve tests that reject duplicate or missing handlers.`
- [Line 299](../../../onboarding-work/foundations.md#L299): `Serialization converts values into a transferable representation; deserialization builds values from that representation. HTTP model binding supplies an endpoint's parameters from route segments, query strings, request bodies or registered services. A JSON record is not automatically trusted merely because deserialization succeeded. Shape validation, permission checks and business rules still have work to do.`
- [Line 301](../../../onboarding-work/foundations.md#L301): `Dates, times and instants are different kinds of values. A DateOnly is a calendar date. A TimeOnly is a local time of day. A DateTimeOffset identifies an instant with an offset representation. Weekly scheduling needs all three concepts because “Saturday at 18:00 in Cairo” is a recurrence rule, while a particular dated class session has UTC start and end instants.`
- [Line 317](../../../onboarding-work/foundations.md#L317): `Api/Program.cs:76 configures UnmappedMemberHandling.Disallow, string enum conversion, HourMinuteTimeOnlyJsonConverter and UtcInstantJsonConverter. It separately enables RouteHandlerOptions.ThrowOnBadRequest. The OpenAPI schema transformer later declares string types for the custom converted CLR types, preserving null in nullable non-parameter schemas.`
- [Line 319](../../../onboarding-work/foundations.md#L319): `The backend converters and generated client serve complementary roles. The converter determines actual runtime JSON. The contract tells tooling what shape to expect. TypeScript can check compiled consumers of that contract, but a network response is still runtime data. The shared API wrapper validates error payloads through its own runtime guard rather than assuming that a generated interface has checked incoming bytes.`
- [Line 323](../../../onboarding-work/foundations.md#L323): `The schedule form collects a weekday, local time, duration and effective range. The generated function serializes a request body. The endpoint binds the JSON into its request type. A malformed startTime is refused by conversion; a well-formed but invalid range reaches request validation or domain checks. A valid rule is passed to the generator with an explicit centre zone. Generated sessions return UTC instants, which the browser formats using the selected centre's time zone. No stage should reinterpret a local time as an already-UTC instant.`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

````text
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
````

The excerpt stops at line 24; use the original file for the remaining 315 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
