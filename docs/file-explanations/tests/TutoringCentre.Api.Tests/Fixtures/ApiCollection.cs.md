# `tests/TutoringCentre.Api.Tests/Fixtures/ApiCollection.cs`

## Purpose

Source summary (intent, not independent execution proof): All database-backed API tests share one factory/container; they run sequentially.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ApiCollection.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 14 lines and 550 bytes. Source hash: `9d476d57711755842897022ac0b97e63826ea87978ab37aba7a145d434f07903`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 11](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ApiCollection.cs#L11): `public sealed class ApiCollection : ICollectionFixture<ApiFactory>`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Api.Tests/Academics/EnrollmentCrossTenantTests.cs:17](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentCrossTenantTests.cs#L17)
- [tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs:18](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L18)
- [tests/TutoringCentre.Api.Tests/Academics/EnrollmentTeacherScopeTests.cs:15](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentTeacherScopeTests.cs#L15)
- [tests/TutoringCentre.Api.Tests/Academics/GroupCrossTenantTests.cs:15](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupCrossTenantTests.cs#L15)
- [tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs:14](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs#L14)
- [tests/TutoringCentre.Api.Tests/Academics/GroupTeacherScopeTests.cs:15](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupTeacherScopeTests.cs#L15)
- [tests/TutoringCentre.Api.Tests/Academics/LastSeatHttpTests.cs:20](../../../../../tests/TutoringCentre.Api.Tests/Academics/LastSeatHttpTests.cs#L20)
- [tests/TutoringCentre.Api.Tests/Academics/ParentCrossTenantProbeTests.cs:13](../../../../../tests/TutoringCentre.Api.Tests/Academics/ParentCrossTenantProbeTests.cs#L13)
- [tests/TutoringCentre.Api.Tests/Academics/ParentEndpointTests.cs:9](../../../../../tests/TutoringCentre.Api.Tests/Academics/ParentEndpointTests.cs#L9)
- [tests/TutoringCentre.Api.Tests/Academics/ScheduleCrossTenantTests.cs:16](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleCrossTenantTests.cs#L16)
- [tests/TutoringCentre.Api.Tests/Academics/ScheduleDoubleBookingHttpTests.cs:19](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleDoubleBookingHttpTests.cs#L19)
- [tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs:18](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs#L18)
- [tests/TutoringCentre.Api.Tests/Academics/ScheduleTeacherScopeTests.cs:15](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleTeacherScopeTests.cs#L15)
- [tests/TutoringCentre.Api.Tests/Academics/StudentCrossTenantProbeTests.cs:15](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentCrossTenantProbeTests.cs#L15)
- [tests/TutoringCentre.Api.Tests/Academics/StudentEndpointTests.cs:9](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentEndpointTests.cs#L9)
- [tests/TutoringCentre.Api.Tests/Academics/SubjectCrossTenantProbeTests.cs:13](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectCrossTenantProbeTests.cs#L13)
- [tests/TutoringCentre.Api.Tests/Academics/SubjectEndpointTests.cs:9](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectEndpointTests.cs#L9)
- [tests/TutoringCentre.Api.Tests/Academics/SubjectPermissionHttpTests.cs:13](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectPermissionHttpTests.cs#L13)
- [tests/TutoringCentre.Api.Tests/Academics/TeacherCrossTenantTests.cs:15](../../../../../tests/TutoringCentre.Api.Tests/Academics/TeacherCrossTenantTests.cs#L15)
- [tests/TutoringCentre.Api.Tests/Academics/TeacherEndpointTests.cs:9](../../../../../tests/TutoringCentre.Api.Tests/Academics/TeacherEndpointTests.cs#L9)
- 15 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;

namespace TutoringCentre.Api.Tests.Fixtures;

/// <summary>All database-backed API tests share one factory/container; they run sequentially.</summary>
[SuppressMessage(
    "Naming",
    "CA1711:Identifiers should not have incorrect suffix",
    Justification = "xUnit collection-definition convention; 'Collection' here names an xUnit concept, not .NET's ICollection.")]
[CollectionDefinition(Name)]
public sealed class ApiCollection : ICollectionFixture<ApiFactory>
{
    public const string Name = "api";
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
