# src/TutoringCentre.Infrastructure/Persistence/Schemas.cs

## Purpose

Constants naming the PostgreSQL schemas used per feature module.

## Where It Fits

Infrastructure/Persistence, `internal`. `Schemas.Platform` is used by `CentreConfiguration` and `DependencyInjection`. `Schemas.Identity` is unused (planned identity module).

## Walkthrough

`Platform = "platform"`, `Identity = "identity"`. Doc: every entity configuration uses these in `ToTable(name, schema)`. Test fixtures list both schema names for Respawn although the `identity` schema does not exist yet.

## Concepts Used

This file introduces no concept that needs a tutorial beyond what its walkthrough already explains.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

Unused constant `Identity`.

## Related Files

- [`src/TutoringCentre.Infrastructure/Persistence/Configurations/Centres/CentreConfiguration.cs`](Configurations/Centres/CentreConfiguration.cs.md)
- [`src/TutoringCentre.Infrastructure/DependencyInjection.cs`](../DependencyInjection.cs.md)
