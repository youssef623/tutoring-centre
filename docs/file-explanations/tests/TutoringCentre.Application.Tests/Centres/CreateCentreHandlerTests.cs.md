# tests/TutoringCentre.Application.Tests/Centres/CreateCentreHandlerTests.cs

## Purpose

Unit tests for `CreateCentreHandler` branches using fakes.

## Where It Fits

Application.Tests/Centres. Constructs the `internal` handler directly (allowed by `InternalsVisibleTo`).

## Walkthrough

- `HandleAsync_AnonymousActor_ReturnsForbiddenAndDoesNotAdd`: Arrange - fresh `CurrentActorContext` (anonymous), repository pre-seeded with an existing `nile-centre` (so a duplicate exists too); Act - `HandleAsync`; Assert - failure `centre.create_forbidden`, kind Forbidden, `Added` empty. The pre-seeded duplicate proves authorization is checked *before* uniqueness.
- `..._SlugAlreadyExists_ReturnsConflictAndDoesNotAdd`: actor set to `SystemActor(null)`; Assert `centre.slug_taken`/Conflict.
- `..._InvalidSlug_ReturnsDomainErrorAndDoesNotAdd`: slug `"Bad Slug"` -> `centre.slug_invalid`.
- `..._SystemActorValidInput_AddsCentreAndReturnsResult`: one `Added`, name and slug match, `result.Value.CentreId == Added[0].Id`.
Not tested: invalid time zone path through the handler (covered in Infrastructure tests), cancellation.

## Concepts Used

### Test doubles: fakes vs real dependencies

#### What it means

A *fake* is a small working substitute (in-memory repository). Real-dependency (integration) tests run actual components such as PostgreSQL. Fakes are fast and focused; integration tests prove behaviour only the real system provides.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#616-test-doubles-vs-real-database-tests](../../../../PROJECT_OVERVIEW2.md#616-test-doubles-vs-real-database-tests).)

#### Where it appears in this file

Fake repository.

#### How it works here

`FakeCentreRepository`.

#### Why it matters here

No database needed.

### Actor and tenancy groundwork

#### What it means

An *actor* is who executes a use case; a *tenant* is one customer's isolated data slice (here a Centre). The actor is supplied by trusted edge code, never by request data.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#611-the-actor-model-and-the-tenancy-groundwork](../../../../PROJECT_OVERVIEW2.md#611-the-actor-model-and-the-tenancy-groundwork).)

#### Where it appears in this file

Anonymous vs System.

#### How it works here

Tests 1 and others.

#### Why it matters here

Verifies the authorization rule.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreHandler.cs`](../../../src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreHandler.cs.md)
- [`tests/TutoringCentre.Application.Tests/Fakes/FakeCentreRepository.cs`](../Fakes/FakeCentreRepository.cs.md)
- [`src/TutoringCentre.Application/Common/Security/CurrentActorContext.cs`](../../../src/TutoringCentre.Application/Common/Security/CurrentActorContext.cs.md)
