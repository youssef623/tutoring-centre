# `src/TutoringCentre.Api/Cli/SeedCommand.cs`

## Purpose

Source summary (intent, not independent execution proof): Creates the two demo centres and their demo subjects through the dispatcher. A CLI is just another client of the Application layer.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Api/Cli/SeedCommand.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 674 lines and 36,963 bytes. Source hash: `12a8f8ff4b53e2aa61d82c81d128eed2e8b79685fd3d93f7bc709214d82dc658`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using Microsoft.AspNetCore.Identity;`
- Line 3: `using Microsoft.EntityFrameworkCore;`
- Line 4: `using Microsoft.Extensions.DependencyInjection;`
- Line 5: `using Microsoft.Extensions.Hosting;`
- Line 6: `using Microsoft.Extensions.Logging;`
- Line 7: `using TutoringCentre.Application.Academics.Enrollments.Commands.EnrollStudent;`
- Line 8: `using TutoringCentre.Application.Academics.Groups;`
- Line 9: `using TutoringCentre.Application.Academics.Groups.Commands.CreateGroup;`
- Line 10: `using TutoringCentre.Application.Academics.Groups.Queries.ListGroups;`
- Line 11: `using TutoringCentre.Application.Academics.Parents;`
- Line 12: `using TutoringCentre.Application.Academics.Parents.Queries.LookupParentByPhone;`
- Line 13: `using TutoringCentre.Application.Academics.Scheduling;`
- Line 14: `using TutoringCentre.Application.Academics.Scheduling.Commands.AddScheduleRule;`
- Line 15: `using TutoringCentre.Application.Academics.Scheduling.Queries.ListGroupScheduleRules;`
- Line 16: `using TutoringCentre.Application.Academics.Students;`
- Line 17: `using TutoringCentre.Application.Academics.Students.Commands.RegisterStudent;`
- Line 18: `using TutoringCentre.Application.Academics.Students.Queries.ListStudents;`
- Line 19: `using TutoringCentre.Application.Academics.Subjects;`
- Line 20: `using TutoringCentre.Application.Academics.Subjects.Commands.CreateSubject;`
- Line 21: `using TutoringCentre.Application.Academics.Subjects.Queries.ListSubjects;`
- Line 22: `using TutoringCentre.Application.Academics.Teachers;`
- Line 23: `using TutoringCentre.Application.Academics.Teachers.Commands.CreateTeacher;`
- Line 24: `using TutoringCentre.Application.Academics.Teachers.Commands.SetTeacherAccount;`
- Line 25: `using TutoringCentre.Application.Academics.Teachers.Queries.GetTeacher;`
- Line 26: `using TutoringCentre.Application.Academics.Teachers.Queries.ListTeachers;`
- Line 27: `using TutoringCentre.Application.Centres.Commands.CreateCentre;`
- Line 28: `using TutoringCentre.Application.Centres.Contracts;`
- Line 29: `using TutoringCentre.Application.Common.Cqrs;`
- Line 30: `using TutoringCentre.Application.Common.Ports;`
- Line 31: `using TutoringCentre.Application.Common.Security;`
- Line 32: `using TutoringCentre.Application.Common.Time;`
- Line 33: `using TutoringCentre.Domain.Academics;`
- Line 34: `using TutoringCentre.Domain.Centres;`
- Line 35: `using TutoringCentre.Domain.Common;`
- Line 36: `using TutoringCentre.Infrastructure.Identity;`
- Line 37: `using TutoringCentre.Infrastructure.Persistence;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 42](../../../../../src/TutoringCentre.Api/Cli/SeedCommand.cs#L42): `public static class SeedCommand`
- [Line 83](../../../../../src/TutoringCentre.Api/Cli/SeedCommand.cs#L83): `// The same person (the two-centre teacher account) is linked to one teacher record per centre, so the demo`
- [Line 150](../../../../../src/TutoringCentre.Api/Cli/SeedCommand.cs#L150): `public static async Task<int> RunAsync(IServiceProvider services)`
- [Line 331](../../../../../src/TutoringCentre.Api/Cli/SeedCommand.cs#L331): `private static async Task<bool> SeedEnrollmentsAsync(IServiceProvider services, Dictionary<string, Guid> centreIdsBySlug, ILogger logger)`
- [Line 412](../../../../../src/TutoringCentre.Api/Cli/SeedCommand.cs#L412): `private static async Task<bool> SeedTeachersAndGroupsAsync(IServiceProvider services, Dictionary<string, Guid> centreIdsBySlug, ILogger logger)`
- [Line 562](../../../../../src/TutoringCentre.Api/Cli/SeedCommand.cs#L562): `private static async Task<bool> SeedScheduleRulesAsync(IServiceProvider services, Dictionary<string, Guid> centreIdsBySlug, ILogger logger)`
- [Line 652](../../../../../src/TutoringCentre.Api/Cli/SeedCommand.cs#L652): `private static AsyncServiceScope CreateCentreScope(IServiceProvider services, Guid centreId, out Dispatcher dispatcher)`
- [Line 660](../../../../../src/TutoringCentre.Api/Cli/SeedCommand.cs#L660): `private sealed record SeedTeacher(string CentreSlug, string FullName, bool LinkTwoCentreTeacher);`
- [Line 662](../../../../../src/TutoringCentre.Api/Cli/SeedCommand.cs#L662): `private sealed record SeedGroup(string CentreSlug, string Name, string SubjectName, string TeacherName, int Capacity);`
- [Line 664](../../../../../src/TutoringCentre.Api/Cli/SeedCommand.cs#L664): `private sealed record SeedEnrollment(string CentreSlug, string GroupName, string[] StudentNames);`
- [Line 666](../../../../../src/TutoringCentre.Api/Cli/SeedCommand.cs#L666): `private sealed record SeedScheduleRule(`
- [Line 669](../../../../../src/TutoringCentre.Api/Cli/SeedCommand.cs#L669): `private sealed record SeedCentre(string Name, string Slug, string TimeZoneId, SupportedLocale DefaultLocale);`
- [Line 671](../../../../../src/TutoringCentre.Api/Cli/SeedCommand.cs#L671): `private sealed record SeedSubjects(string CentreSlug, string[] Names);`
- [Line 673](../../../../../src/TutoringCentre.Api/Cli/SeedCommand.cs#L673): `private sealed record SeedFamily(string CentreSlug, string ParentFullName, string ParentPhone, string[] StudentNames);`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/reconcile_backend_ledger.py:17](../../../../onboarding-work/reconcile_backend_ledger.py#L17)
- [src/TutoringCentre.Api/Program.cs:142](../../../../../src/TutoringCentre.Api/Program.cs#L142)
- [tests/TutoringCentre.Api.Tests/Academics/EnrollmentCrossTenantTests.cs:25](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentCrossTenantTests.cs#L25)
- [tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs:31](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L31)
- [tests/TutoringCentre.Api.Tests/Academics/EnrollmentTeacherScopeTests.cs:25](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentTeacherScopeTests.cs#L25)
- [tests/TutoringCentre.Api.Tests/Academics/GroupCrossTenantTests.cs:21](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupCrossTenantTests.cs#L21)
- [tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs:22](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs#L22)
- [tests/TutoringCentre.Api.Tests/Academics/GroupTeacherScopeTests.cs:23](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupTeacherScopeTests.cs#L23)
- [tests/TutoringCentre.Api.Tests/Academics/LastSeatHttpTests.cs:32](../../../../../tests/TutoringCentre.Api.Tests/Academics/LastSeatHttpTests.cs#L32)
- [tests/TutoringCentre.Api.Tests/Academics/ParentCrossTenantProbeTests.cs:19](../../../../../tests/TutoringCentre.Api.Tests/Academics/ParentCrossTenantProbeTests.cs#L19)
- [tests/TutoringCentre.Api.Tests/Academics/ParentEndpointTests.cs:18](../../../../../tests/TutoringCentre.Api.Tests/Academics/ParentEndpointTests.cs#L18)
- [tests/TutoringCentre.Api.Tests/Academics/ScheduleCrossTenantTests.cs:27](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleCrossTenantTests.cs#L27)
- [tests/TutoringCentre.Api.Tests/Academics/ScheduleDoubleBookingHttpTests.cs:27](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleDoubleBookingHttpTests.cs#L27)
- [tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs:36](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs#L36)
- [tests/TutoringCentre.Api.Tests/Academics/ScheduleTeacherScopeTests.cs:25](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleTeacherScopeTests.cs#L25)
- [tests/TutoringCentre.Api.Tests/Academics/StudentCrossTenantProbeTests.cs:21](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentCrossTenantProbeTests.cs#L21)
- [tests/TutoringCentre.Api.Tests/Academics/StudentEndpointTests.cs:17](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentEndpointTests.cs#L17)
- [tests/TutoringCentre.Api.Tests/Academics/SubjectCrossTenantProbeTests.cs:19](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectCrossTenantProbeTests.cs#L19)
- [tests/TutoringCentre.Api.Tests/Academics/SubjectEndpointTests.cs:17](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectEndpointTests.cs#L17)
- [tests/TutoringCentre.Api.Tests/Academics/SubjectPermissionHttpTests.cs:21](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectPermissionHttpTests.cs#L21)
- 15 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TutoringCentre.Application.Academics.Enrollments.Commands.EnrollStudent;
using TutoringCentre.Application.Academics.Groups;
using TutoringCentre.Application.Academics.Groups.Commands.CreateGroup;
using TutoringCentre.Application.Academics.Groups.Queries.ListGroups;
using TutoringCentre.Application.Academics.Parents;
using TutoringCentre.Application.Academics.Parents.Queries.LookupParentByPhone;
using TutoringCentre.Application.Academics.Scheduling;
using TutoringCentre.Application.Academics.Scheduling.Commands.AddScheduleRule;
using TutoringCentre.Application.Academics.Scheduling.Queries.ListGroupScheduleRules;
using TutoringCentre.Application.Academics.Students;
using TutoringCentre.Application.Academics.Students.Commands.RegisterStudent;
using TutoringCentre.Application.Academics.Students.Queries.ListStudents;
using TutoringCentre.Application.Academics.Subjects;
using TutoringCentre.Application.Academics.Subjects.Commands.CreateSubject;
using TutoringCentre.Application.Academics.Subjects.Queries.ListSubjects;
using TutoringCentre.Application.Academics.Teachers;
using TutoringCentre.Application.Academics.Teachers.Commands.CreateTeacher;
using TutoringCentre.Application.Academics.Teachers.Commands.SetTeacherAccount;
using TutoringCentre.Application.Academics.Teachers.Queries.GetTeacher;
using TutoringCentre.Application.Academics.Teachers.Queries.ListTeachers;
using TutoringCentre.Application.Centres.Commands.CreateCentre;
using TutoringCentre.Application.Centres.Contracts;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Common.Time;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Domain.Common;
using TutoringCentre.Infrastructure.Identity;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Api.Cli;

/// <summary>Creates the two demo centres and their demo subjects through the dispatcher. A CLI is just another client of the Application layer.</summary>
public static class SeedCommand
{
    private static readonly SeedCentre[] Centres =
    [
        new("Nile Tutoring Centre", "nile-centre", "Africa/Cairo", SupportedLocale.Ar),
        new("Maadi Learning Hub", "maadi-hub", "Africa/Cairo", SupportedLocale.En),
    ];

    // Empty on purpose: no subjects, staff (beyond its owner), students or groups, so the full-setup journey builds
    // everything itself through the UI. Development and Testing only (SeedGate.IncludesSetupCentre).
    private static readonly SeedCentre SetupCentre =
        new("مركز الجيزة الجديد", DevelopmentIdentitySeeder.SetupCentreSlug, "Africa/Cairo", SupportedLocale.Ar);

    private static readonly SeedSubjects[] Subjects =
    [
        new("nile-centre", ["رياضيات", "فيزياء", "كيمياء"]),
        new("maadi-hub", ["Mathematics", "Physics"]),
    ];

    // Phones are synthetic, in a block no real Egyptian number could collide with (010 + six zeros + a running
    // index), never a value that looks like it could belong to an actual person. Three families per centre list
    // two student names (siblings sharing a parent); the rest list one.
    private static readonly SeedFamily[] Families =
    [
        new("nile-centre", "محمود حسن", "01000000001", ["أحمد محمود", "سارة محمود"]),
        new("nile-centre", "علي سعيد", "01000000002", ["إبراهيم علي", "ياسمين علي"]),
        new("nile-centre", "حسن عبدالله", "01000000003", ["فاطمة حسن", "يوسف حسن"]),
        new("nile-centre", "السيد محمد", "01000000004", ["مصطفى السيد"]),
        new("nile-centre", "أحمد فوزي", "01000000005", ["نور الدين أحمد"]),
        new("nile-centre", "طارق سمير", "01000000006", ["ريم طارق"]),
        new("nile-centre", "عادل منصور", "01000000007", ["كريم عادل"]),
        new("nile-centre", "وليد جمال", "01000000008", ["هبة وليد"]),
        new("nile-centre", "خالد رضا", "01000000009", ["عمر خالد"]),
        new("maadi-hub", "Robert Wilson", "01000000010", ["James Wilson"]),
        new("maadi-hub", "Michael Clark", "01000000011", ["Emma Clark"]),
        new("maadi-hub", "David Brown", "01000000012", ["Oliver Brown"]),
        new("maadi-hub", "Daniel Davis", "01000000013", ["Sophia Davis"]),
        new("maadi-hub", "Thomas Miller", "01000000014", ["Lucas Miller"]),
        new("maadi-hub", "Christopher Anderson", "01000000015", ["Mia Anderson"]),
    ];

    // The same person (the two-centre teacher account) is linked to one teacher record per centre, so the demo
    // shows the ownership guard: one account, different groups per selected centre. In Nile the linked teacher
    // owns exactly one group and the unlinked one the other three, including the two-seat last-seat demo group.
    private static readonly SeedTeacher[] Teachers =
    [
        new("nile-centre", "كريم فتحي", LinkTwoCentreTeacher: true),
        new("nile-centre", "منى السيد", LinkTwoCentreTeacher: false),
        new("maadi-hub", "Karim Fathy", LinkTwoCentreTeacher: true),
```

The excerpt stops at line 90; use the original file for the remaining 584 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
