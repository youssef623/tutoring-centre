using Npgsql;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Tenancy;

/// <summary>
/// Task 21.6: the composite foreign key on probe.tenant_probe_children makes a cross-centre reference impossible
/// to store — proven as superuser, so nothing about role privilege (Day 19) or row-level security (Task 21.3) is
/// what's actually stopping it.
/// </summary>
public sealed class TenantProbeCompositeForeignKeyTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    [Fact]
    public async Task InsertChildWithNilesCentreAndAMaadiProbesId_AsSuperuser_Throws23503()
    {
        var maadiProbeId = await Fixture.SeedProbeAsync(MaadiCentreId, "maadi-1");

        await using var connection = new NpgsqlConnection(Fixture.SuperuserConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(
            "insert into probe.tenant_probe_children (id, centre_id, probe_id, label) values (@id, @centre_id, @probe_id, @label)",
            connection);
        command.Parameters.AddWithValue("id", Guid.CreateVersion7());
        command.Parameters.AddWithValue("centre_id", NileCentreId);
        command.Parameters.AddWithValue("probe_id", maadiProbeId);
        command.Parameters.AddWithValue("label", "cross-centre");

        var exception = await Assert.ThrowsAsync<PostgresException>(() => command.ExecuteNonQueryAsync());
        Assert.Equal("23503", exception.SqlState);
    }
}
