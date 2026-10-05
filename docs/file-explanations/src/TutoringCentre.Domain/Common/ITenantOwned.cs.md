# src/TutoringCentre.Domain/Common/ITenantOwned.cs

## Purpose

Marker interface for future entities that belong to one centre (tenant).

## Where It Fits

Domain/Common. **No class implements it yet** and no code references it (verified by search). Comment says Infrastructure will apply tenant filtering to implementers in 'Month 2'.

## Walkthrough

`interface ITenantOwned { Guid CentreId { get; } }`. `Centre` deliberately does not implement it because it is the tenant. When entities such as students are added they would implement this and EF global query filters could use it.

## Concepts Used

### Actor and tenancy groundwork

#### What it means

An *actor* is who executes a use case; a *tenant* is one customer's isolated data slice (here a Centre). The actor is supplied by trusted edge code, never by request data.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#611-the-actor-model-and-the-tenancy-groundwork](../../../../PROJECT_OVERVIEW2.md#611-the-actor-model-and-the-tenancy-groundwork).)

#### Where it appears in this file

Tenancy marker.

#### How it works here

Interface declaration.

#### Why it matters here

Prepares automatic tenant filtering; today it is unused groundwork.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

Dead code until a tenant-owned entity exists (status: planned, not implemented).

## Related Files

- [`src/TutoringCentre.Domain/Centres/Centre.cs`](../Centres/Centre.cs.md)
- [`src/TutoringCentre.Application/Common/Security/Actor.cs`](../../TutoringCentre.Application/Common/Security/Actor.cs.md)
