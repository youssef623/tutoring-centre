# tests/TutoringCentre.Application.Tests/Security/CurrentActorContextTests.cs

## Purpose

Tests the actor holder's default and set-once behaviour.

## Where It Fits

Application.Tests/Security.

## Walkthrough

`Actor_ByDefault_IsAnonymous` (`Assert.IsType<AnonymousActor>`); `Set_ThenActor_ReturnsTheSetActor` (`Assert.Same`); `Set_CalledTwice_ThrowsInvalidOperationException`; new `Reauthenticate_AfterSet_ReplacesTheActor` (set an owner actor, `Reauthenticate` with a staff actor, `Assert.Same`) and `Reauthenticate_ThenSet_ThrowsInvalidOperationException` (the scope still counts as set). Not tested: `Set(null)` and `Reauthenticate(null)`.

## Concepts Used

### Actor and tenancy groundwork

#### What it means

An *actor* is who executes a use case; a *tenant* is one customer's isolated data slice (here a Centre). The actor is supplied by trusted edge code, never by request data.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#611-the-actor-model-and-the-tenancy-groundwork](../../../../PROJECT_OVERVIEW2.md#611-the-actor-model-and-the-tenancy-groundwork).)

#### Where it appears in this file

Set-once rule.

#### How it works here

Third test.

#### Why it matters here

Prevents identity swapping within a scope.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`src/TutoringCentre.Application/Common/Security/CurrentActorContext.cs`](../../../src/TutoringCentre.Application/Common/Security/CurrentActorContext.cs.md)
