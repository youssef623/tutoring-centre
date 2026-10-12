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
