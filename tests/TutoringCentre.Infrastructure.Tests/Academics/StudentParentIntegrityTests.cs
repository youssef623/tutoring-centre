using Npgsql;
using TutoringCentre.Infrastructure.Tests.Fixtures;
using TutoringCentre.Infrastructure.Tests.Tenancy;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Task 38.4: proves the composite foreign keys and unique constraints hold even for the superuser, who
/// bypasses row-level security entirely — the guarantee RLS alone cannot give (docs/architecture/tenancy.md,
/// Layer 4). Rows are seeded and the violating statement run over the owner connection directly, never through
/// the application.
/// </summary>
public sealed class StudentParentIntegrityTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    [Fact]
    public async Task LinkWithNileCentreButMaadiParent_Throws23503()
    {
        var nileStudent = await Fixture.SeedStudentAsync(NileCentreId, "Youssef Ali");
        var maadiParent = await Fixture.SeedParentAsync(MaadiCentreId, "Laila Said", "+201112345678");

        await using var connection = new NpgsqlConnection(Fixture.SuperuserConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(
            """
            insert into academics.student_parents (id, centre_id, student_id, parent_id, relationship, is_primary, created_at)
            values (@id, @centre_id, @student_id, @parent_id, 'father', true, now())
            """,
            connection);
        command.Parameters.AddWithValue("id", Guid.CreateVersion7());
        command.Parameters.AddWithValue("centre_id", NileCentreId);
        command.Parameters.AddWithValue("student_id", nileStudent);
        command.Parameters.AddWithValue("parent_id", maadiParent);

        var exception = await Assert.ThrowsAsync<PostgresException>(() => command.ExecuteNonQueryAsync());
        Assert.Equal("23503", exception.SqlState);
        Assert.Equal("fk_student_parents_parents", exception.ConstraintName);
    }

    [Fact]
    public async Task SamePhoneTwiceInNile_Throws23505OnUxParentsCentrePhone()
    {
        await Fixture.SeedParentAsync(NileCentreId, "Omar Hassan", "+201012345678");

        await using var connection = new NpgsqlConnection(Fixture.SuperuserConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(
            """
            insert into academics.parents (id, centre_id, full_name, search_name, phone, created_at)
            values (@id, @centre_id, 'Another Parent', 'ANOTHER PARENT', '+201012345678', now())
            """,
            connection);
        command.Parameters.AddWithValue("id", Guid.CreateVersion7());
        command.Parameters.AddWithValue("centre_id", NileCentreId);

        var exception = await Assert.ThrowsAsync<PostgresException>(() => command.ExecuteNonQueryAsync());
        Assert.Equal("23505", exception.SqlState);
        Assert.Equal("ux_parents_centre_phone", exception.ConstraintName);
    }

    [Fact]
    public async Task SamePhoneOnceInEachCentre_Succeeds()
    {
        await Fixture.SeedParentAsync(NileCentreId, "Omar Hassan", "+201012345678");

        var maadiParentId = await Fixture.SeedParentAsync(MaadiCentreId, "Omar Hassan (Maadi)", "+201012345678");

        Assert.Equal(
            1,
            await Fixture.ScalarAsync<long>($"select count(*) from academics.parents where id = '{maadiParentId}'"));
    }

    [Fact]
    public async Task TwoPrimaryLinksForOneStudent_Throws23505OnUxStudentParentsOnePrimary()
    {
        var nileStudent = await Fixture.SeedStudentAsync(NileCentreId, "Youssef Ali");
        var firstParent = await Fixture.SeedParentAsync(NileCentreId, "Omar Hassan", "+201012345678");
        var secondParent = await Fixture.SeedParentAsync(NileCentreId, "Amal Fathy", "+201212345678");
        await Fixture.SeedStudentParentLinkAsync(NileCentreId, nileStudent, firstParent, isPrimary: true);

        await using var connection = new NpgsqlConnection(Fixture.SuperuserConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(
            """
            insert into academics.student_parents (id, centre_id, student_id, parent_id, relationship, is_primary, created_at)
            values (@id, @centre_id, @student_id, @parent_id, 'mother', true, now())
            """,
            connection);
        command.Parameters.AddWithValue("id", Guid.CreateVersion7());
        command.Parameters.AddWithValue("centre_id", NileCentreId);
        command.Parameters.AddWithValue("student_id", nileStudent);
        command.Parameters.AddWithValue("parent_id", secondParent);

        var exception = await Assert.ThrowsAsync<PostgresException>(() => command.ExecuteNonQueryAsync());
        Assert.Equal("23505", exception.SqlState);
        Assert.Equal("ux_student_parents_one_primary", exception.ConstraintName);
    }

    [Fact]
    public async Task NonE164Phone_Throws23514()
    {
        await using var connection = new NpgsqlConnection(Fixture.SuperuserConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(
            """
            insert into academics.parents (id, centre_id, full_name, search_name, phone, created_at)
            values (@id, @centre_id, 'Omar Hassan', 'OMAR HASSAN', '01012345678', now())
            """,
            connection);
        command.Parameters.AddWithValue("id", Guid.CreateVersion7());
        command.Parameters.AddWithValue("centre_id", NileCentreId);

        var exception = await Assert.ThrowsAsync<PostgresException>(() => command.ExecuteNonQueryAsync());
        Assert.Equal("23514", exception.SqlState);
        Assert.Equal("ck_parents_phone_format", exception.ConstraintName);
    }
}
