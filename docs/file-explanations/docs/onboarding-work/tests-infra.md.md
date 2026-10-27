# `docs/onboarding-work/tests-infra.md`

## Purpose

Documentation covering Testing, infrastructure and operations: verified teaching material; 6.60 Testing as a hierarchy of different proofs; 6.61 Test doubles, call records and controllable time; 6.62 Real PostgreSQL fixtures, isolated databases and least privilege; 6.63 In-process HTTP tests and explicit endpoint inventories. Treat historical claims according to the revision described in the file.

## Where it fits

Category: **documentation**. [Open current file](../../../onboarding-work/tests-infra.md). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 1155 lines and 115,971 bytes. Source hash: `088f2461499004b3d6991b357ff5b034fb71a6bfa5867cb8c3496ac45c2fe33d`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 85](../../../onboarding-work/tests-infra.md#L85): `A handler accepts an interface in its constructor. Production composition provides the real adapter; the test supplies a fake implementing that same interface. The handler executes unchanged. Assertions inspect the returned result, fake state, or recorded calls.`
- [Line 127](../../../onboarding-work/tests-infra.md#L127): `Read the fake together with its interface and production adapter. Extend only the behavior the test needs. Keep IClock replacement separate from framework time. When a test changes the clock, restore it. Treat call-list strings as part of the test harness contract when editing dispatcher assertions.`
- [Line 264](../../../onboarding-work/tests-infra.md#L264): `internal sealed class GetSomethingHandler(ICentreRepository centres) { }`
- [Line 274](../../../onboarding-work/tests-infra.md#L274): `The tests obtain assemblies through known marker types. They enumerate requests and handlers, inspect generic interface arguments, examine constructor parameters, and use NetArchTest for forbidden dependencies. Source scans read production .cs files for calls that compiled type dependencies alone cannot distinguish. EF tests build the model without opening a database connection.`
- [Line 288](../../../onboarding-work/tests-infra.md#L288): `A new Academics query omits ITenantScoped. The type compiles and can have a perfectly valid handler. EveryRequestInATenantModule_DeclaresTenantScoped identifies its namespace and interface set, then reports the missing marker. If the marker is added but no permission declaration follows, EveryTenantScopedRequest_AlsoDeclaresIRequirePermission fails next. A misspelled permission then fails catalogue validation.`
- [Line 318](../../../onboarding-work/tests-infra.md#L318): `Handler conventions require more specific discovery. A command is recognized by its implemented closed generic ICommand<T> interface, not merely by a class name ending in Command. Handler discovery similarly reads ICommandHandler<TRequest,TResponse> or IQueryHandler<TRequest,TResponse>. The cardinality rule builds the relationship between each concrete request type and its matching handler types, then requires exactly one. Zero handlers would otherwise fail only when DI resolution occurs at runtime; two handlers could be hidden by the container's last-registration behavior. The test also requires handlers to be sealed and nonpublic, encoding their intended status as implementation details reached through dispatch.`
- [Line 320](../../../onboarding-work/tests-infra.md#L320): `Repository conventions use names in a way generic handler discovery does not. ArchitectureSupport identifies an interface beginning I and ending Repository, and a concrete class ending Repository. Placement tests then require ports in Application and implementations in Infrastructure. Query-handler constructor inspection refuses repository interfaces as dependencies, directing read logic toward DTO-returning read services. This is useful enforcement, but its name convention is also a limit: an equivalent persistence port named Store would need the classifier updated or could evade this particular rule. Explaining the matching mechanism is more useful than describing the rule as an impenetrable compiler barrier.`
- [Line 324](../../../onboarding-work/tests-infra.md#L324): `Published contracts are checked separately. Types in namespaces ending .Contracts must be interfaces or records. The record test looks for the compiler-generated <Clone>$ method, which is appropriate to the reference-type records used here. A contract must also avoid depending on its own module's repositories, read services or entities. Otherwise a supposedly narrow contract could simply wrap an internal entity and re-export the coupling. The tests require a nonempty contract set so this allowance remains exercised. This is the mechanism behind the explicit Identity directory contract used by academic teacher-account linking.`
- [Line 326](../../../onboarding-work/tests-infra.md#L326): `Tenant markers are discovered from interface membership, then checked against the declared namespace map. TenantScopeRuleTests requires requests in tenant modules to implement ITenantScoped, while older platform/login queries must remain outside that marker. The second rule also asserts the non-tenant request set is nonempty. AuthorizationRuleTests requires every tenant-scoped request to declare IRequirePermission and validates each permission key against Permissions.All. It obtains that key from an uninitialized request instance because current implementations return constants. This is safe only while the property remains independent of instance state; adding computed permission logic would require revisiting the test mechanism, not merely its expected data.`
- [Line 416](../../../onboarding-work/tests-infra.md#L416): `For HTTP experiments, record start and finish times and verify that the latest start precedes the earliest completion. This establishes actual overlap in the observed request interval. It does not prove every SQL instruction overlaps, but rules out a wholly sequential request execution masquerading as a race.`
- [Line 830](../../../onboarding-work/tests-infra.md#L830): `The smoke Subjects assertion checks the first row is visible; it does not explicitly distinguish a data row from a table header. Therefore describe the assertion literally rather than claiming it proves at least one subject record exists. The project intends read-mostly operation, and its spec performs no explicit creation calls.`
- [Line 962](../../../onboarding-work/tests-infra.md#L962): `Azure App Service offers a simpler web-app footprint but no equivalent first-class one-shot job design in this ADR's comparison. A VM provides control at the cost of patching, TLS and process management. Stored service-principal secrets would authenticate deployment but require storage and rotation. Those alternatives are documented considerations; none is an active deployment in the current repository.`
- [Line 1042](../../../onboarding-work/tests-infra.md#L1042): `The Application project tests orchestration. Academics handler tests control read answers and repository state to explore missing records, invalid transitions and allowed operations. Dispatcher-focused files check authorization and tenant prerequisites independently of entity rules. Staff tests cover creation, listing, role changes, deactivation, reactivation and validators. Centre tests cover settings and centre-local calendar behavior. Audit tests cover query validation and authorization. Security tests cover actor/correlation state and teacher scope. Fakes are per-interface adapters whose state is visible in the test rather than generated through a mocking DSL.`
- [Line 1152](../../../onboarding-work/tests-infra.md#L1152): `5. smoke/tests/smoke.spec.ts asserts visibility of the first table row after opening Subjects. A header row can satisfy that assertion. The current test therefore does not explicitly establish that a subject data record rendered, despite prose describing that intent.`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

````text
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
````

The excerpt stops at line 24; use the original file for the remaining 1131 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
