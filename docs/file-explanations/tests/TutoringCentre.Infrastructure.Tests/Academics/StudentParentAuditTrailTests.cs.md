# `tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentAuditTrailTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 38.6: the audit trail for a parent's phone change, proven through a real command against real PostgreSQL — the counterpart to Month 2's AuditTrailTests for Subject and Membership. Every assertion reads audit.audit_entries through the superuser connection, never through the application's own filtered view of it.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentAuditTrailTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 48 lines and 2,793 bytes. Source hash: `f5625771987af5d8da52e8731e780015ad0aa84a2a6676b3ea5cae02ac35c382`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Application.Common.Security;`
- Line 3: `using TutoringCentre.Domain.Common;`
- Line 4: `using TutoringCentre.Domain.Identity;`
- Line 5: `using TutoringCentre.Infrastructure.Tests.Fixtures;`
- Line 6: `using TutoringCentre.Infrastructure.Tests.Tenancy;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 16](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentAuditTrailTests.cs#L16): `public sealed class StudentParentAuditTrailTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)`
- [Line 19](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentAuditTrailTests.cs#L19): `public async Task UpdateParentPhone_WritesOneUpdatedAuditRowWithMaskedValuesOnly()`
- [Line 47](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentAuditTrailTests.cs#L47): `private async Task<StaffActor> StaffActorInAsync(Guid centreId) => new(await Fixture.SeedUserAsync(), centreId, StaffRole.Teacher);`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;
using TutoringCentre.Infrastructure.Tests.Tenancy;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Task 38.6: the audit trail for a parent's phone change, proven through a real command against real
/// PostgreSQL — the counterpart to Month 2's <c>AuditTrailTests</c> for Subject and Membership. Every
/// assertion reads audit.audit_entries through the superuser connection, never through the application's own
/// filtered view of it.
/// </summary>
public sealed class StudentParentAuditTrailTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    [Fact]
    public async Task UpdateParentPhone_WritesOneUpdatedAuditRowWithMaskedValuesOnly()
    {
        var actor = await StaffActorInAsync(NileCentreId);
        var parentId = (await Fixture.SendAsAsync<AddParentCommand, Guid>(actor, new AddParentCommand(NileCentreId, "Omar Hassan", "01012345678"))).Value;
        var countAfterCreate = await Fixture.ScalarAsync<long>("select count(*) from audit.audit_entries");

        var result = await Fixture.SendAsAsync<UpdateParentPhoneCommand, Unit>(actor, new UpdateParentPhoneCommand(parentId, "01012349999"));

        Assert.True(result.IsSuccess);
        Assert.Equal(countAfterCreate + 1, await Fixture.ScalarAsync<long>("select count(*) from audit.audit_entries"));
        Assert.Equal(
            1,
            await Fixture.ScalarAsync<long>(
                $"select count(*) from audit.audit_entries where entity_id = '{parentId}' and entity_type = 'parent' and action = 'updated'"));

        var before = await Fixture.ScalarAsync<string>(
            $"select changes->'phone'->>'before' from audit.audit_entries where entity_id = '{parentId}' and action = 'updated'");
        var after = await Fixture.ScalarAsync<string>(
            $"select changes->'phone'->>'after' from audit.audit_entries where entity_id = '{parentId}' and action = 'updated'");
        Assert.Equal(PhoneNumber.Create("01012345678").Value.Masked, before);
        Assert.Equal(PhoneNumber.Create("01012349999").Value.Masked, after);

        var changesText = await Fixture.ScalarAsync<string>(
            $"select changes::text from audit.audit_entries where entity_id = '{parentId}' and action = 'updated'");
        Assert.DoesNotContain("+201012345678", changesText, StringComparison.Ordinal);
        Assert.DoesNotContain("+201012349999", changesText, StringComparison.Ordinal);
    }

    private async Task<StaffActor> StaffActorInAsync(Guid centreId) => new(await Fixture.SeedUserAsync(), centreId, StaffRole.Teacher);
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
