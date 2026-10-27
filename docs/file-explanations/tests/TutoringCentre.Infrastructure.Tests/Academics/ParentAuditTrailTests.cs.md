# `tests/TutoringCentre.Infrastructure.Tests/Academics/ParentAuditTrailTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 39.9: the audit trail for the production parent commands, proven through real HTTP-shaped use cases against real PostgreSQL — the counterpart to Month 2's AuditTrailTests. Every assertion reads audit.audit_entries through the superuser connection.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ParentAuditTrailTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 71 lines and 3,953 bytes. Source hash: `85ae339db1546ec278497261551b38925cae2a510a431c2a936e752351f1fe1a`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Parents.Commands.CreateParent;`
- Line 2: `using TutoringCentre.Application.Academics.Parents.Commands.UpdateParent;`
- Line 3: `using TutoringCentre.Application.Common.Cqrs;`
- Line 4: `using TutoringCentre.Application.Common.Security;`
- Line 5: `using TutoringCentre.Domain.Common;`
- Line 6: `using TutoringCentre.Domain.Identity;`
- Line 7: `using TutoringCentre.Infrastructure.Tests.Fixtures;`
- Line 8: `using TutoringCentre.Infrastructure.Tests.Tenancy;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 17](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ParentAuditTrailTests.cs#L17): `public sealed class ParentAuditTrailTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)`
- [Line 20](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ParentAuditTrailTests.cs#L20): `public async Task CreateParent_WritesOneCreatedAuditRowWithMaskedPhoneOnly()`
- [Line 42](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ParentAuditTrailTests.cs#L42): `public async Task UpdateParentPhone_WritesOneUpdatedAuditRowWithMaskedValuesOnly()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Parents.Commands.CreateParent;
using TutoringCentre.Application.Academics.Parents.Commands.UpdateParent;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;
using TutoringCentre.Infrastructure.Tests.Tenancy;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Task 39.9: the audit trail for the production parent commands, proven through real HTTP-shaped use cases
/// against real PostgreSQL — the counterpart to Month 2's AuditTrailTests. Every assertion reads
/// audit.audit_entries through the superuser connection.
/// </summary>
public sealed class ParentAuditTrailTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    [Fact]
    public async Task CreateParent_WritesOneCreatedAuditRowWithMaskedPhoneOnly()
    {
        var actor = new StaffActor(await Fixture.SeedUserAsync(), NileCentreId, StaffRole.Owner);

        var created = await Fixture.SendAsAsync<CreateParentCommand, CreateParentResult>(
            actor, new CreateParentCommand("Omar Hassan", "01012345678"));
        Assert.True(created.IsSuccess);

        Assert.Equal(
            1,
            await Fixture.ScalarAsync<long>(
                $"select count(*) from audit.audit_entries where entity_id = '{created.Value.ParentId}' and entity_type = 'parent' and action = 'created'"));
        var phoneAfter = await Fixture.ScalarAsync<string>(
            $"select changes->'phone'->>'after' from audit.audit_entries where entity_id = '{created.Value.ParentId}'");
        Assert.Equal("+20••••••5678", phoneAfter);

        var changesText = await Fixture.ScalarAsync<string>(
            $"select changes::text from audit.audit_entries where entity_id = '{created.Value.ParentId}'");
        Assert.DoesNotContain("+201012345678", changesText, StringComparison.Ordinal);
    }

    [Fact]
    public async Task UpdateParentPhone_WritesOneUpdatedAuditRowWithMaskedValuesOnly()
    {
        var actor = new StaffActor(await Fixture.SeedUserAsync(), NileCentreId, StaffRole.Owner);
        var created = await Fixture.SendAsAsync<CreateParentCommand, CreateParentResult>(
            actor, new CreateParentCommand("Omar Hassan", "01012345678"));
        var version = uint.Parse(
            await Fixture.ScalarAsync<string>($"select xmin::text from academics.parents where id = '{created.Value.ParentId}'"),
            System.Globalization.CultureInfo.InvariantCulture);

        var updated = await Fixture.SendAsAsync<UpdateParentCommand, Unit>(
            actor, new UpdateParentCommand(created.Value.ParentId, "Omar Hassan", "01112345678", version));
        Assert.True(updated.IsSuccess);

        Assert.Equal(
            1,
            await Fixture.ScalarAsync<long>(
                $"select count(*) from audit.audit_entries where entity_id = '{created.Value.ParentId}' and action = 'updated'"));
        var before = await Fixture.ScalarAsync<string>(
            $"select changes->'phone'->>'before' from audit.audit_entries where entity_id = '{created.Value.ParentId}' and action = 'updated'");
        var after = await Fixture.ScalarAsync<string>(
            $"select changes->'phone'->>'after' from audit.audit_entries where entity_id = '{created.Value.ParentId}' and action = 'updated'");
        Assert.Equal("+20••••••5678", before);
        Assert.Equal("+20••••••5678", after);

        var changesText = await Fixture.ScalarAsync<string>(
            $"select changes::text from audit.audit_entries where entity_id = '{created.Value.ParentId}' and action = 'updated'");
        Assert.DoesNotContain("+201012345678", changesText, StringComparison.Ordinal);
        Assert.DoesNotContain("+201112345678", changesText, StringComparison.Ordinal);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
