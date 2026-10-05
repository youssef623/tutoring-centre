# tests/TutoringCentre.Application.Tests/Fakes/FakeMembershipReadService.cs

## Purpose

In-memory fake of `IMembershipReadService` that returns whatever the test assigned to three properties.

## Where It Fits

Application.Tests/Fakes. Used by the three identity handler test classes. Lets Application handlers be tested without a database.

## Walkthrough

`FakeMembershipReadService` (6-21). Three settable properties - `Profile` (`StaffProfileDto?`), `ActiveMembership` (`ActiveMembershipDto?`), `SessionState` (`SessionStateDto?`) - and the three interface methods `GetProfileAsync`, `GetActiveMembershipAsync`, `GetSessionStateAsync`, each returning `Task.FromResult(<property>)` and ignoring its arguments. Default is `null` for all three, which is what the real service returns for unknown user / no active membership.

## Concepts Used

### Test doubles: fakes vs real dependencies

#### What it means

A *fake* is a small working substitute (in-memory repository). Real-dependency (integration) tests run actual components such as PostgreSQL. Fakes are fast and focused; integration tests prove behaviour only the real system provides.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#616-test-doubles-vs-real-database-tests](../../../../PROJECT_OVERVIEW2.md#616-test-doubles-vs-real-database-tests).)

#### Where it appears in this file

The whole class.

#### How it works here

The whole class (lines 6-20).

#### Why it matters here

A hand-written fake is enough: three canned answers, no mocking library; it ignores parameters, so a test cannot check *which* user id was asked for (the real service's filtering is tested against PostgreSQL in `MembershipReadServiceTests`).

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

Because the fake ignores its arguments, handler tests cannot detect a handler passing the wrong `userId` or `centreId` to the read service.

## Related Files

- [`src/TutoringCentre.Application/Identity/IMembershipReadService.cs`](../../../src/TutoringCentre.Application/Identity/IMembershipReadService.cs.md)
- [`tests/TutoringCentre.Application.Tests/Identity/GetActiveMembershipHandlerTests.cs`](../Identity/GetActiveMembershipHandlerTests.cs.md)
- [`tests/TutoringCentre.Application.Tests/Identity/GetMyMembershipsHandlerTests.cs`](../Identity/GetMyMembershipsHandlerTests.cs.md)
- [`tests/TutoringCentre.Application.Tests/Identity/ValidateStaffSessionHandlerTests.cs`](../Identity/ValidateStaffSessionHandlerTests.cs.md)
