using Npgsql;
using TutoringCentre.Domain.Attendance;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Attendance;

public sealed class StudentCardRowLevelSecurityTests(PostgresFixture fixture) : AttendanceTestBase(fixture)
{
    private async Task<string> CardInsert(Guid centre, Guid id)
    {
        var student = await Fixture.SeedStudentAsync(centre, "Card Student");
        var user = await Fixture.SeedUserAsync();
        var hash = Convert.ToHexString(CardToken.Generate().Hash());
        return $"""
            insert into attendance.student_cards (id, centre_id, student_id, token_hash, status, issued_at, issued_by_user_id, created_at)
            values ('{id}', '{centre}', '{student}', decode('{hash}', 'hex'), 'active', '2027-03-01T08:00Z', '{user}', now())
            """;
    }

    private async Task<Guid> SeedCard(Guid centre)
    {
        var id = Guid.CreateVersion7();
        await ExecuteAsync(await CardInsert(centre, id));
        return id;
    }

    [Fact]
    public async Task NoSetting_ReturnsNoRows()
    {
        await SeedCard(NileCentreId);
        Assert.Equal(0, await CardCount(Fixture.AppConnectionString));
    }

    [Fact]
    public async Task OwnCentre_OnlyOwnRows()
    {
        await SeedCard(NileCentreId);
        await SeedCard(MaadiCentreId);
        Assert.Equal(1, await CardCount(Fixture.AppConnectionString, NileCentreId));
        Assert.Equal(1, await CardCount(Fixture.AppConnectionString, MaadiCentreId));
    }

    [Fact]
    public async Task ForeignInsert_Denied()
    {
        var sql = await CardInsert(MaadiCentreId, Guid.CreateVersion7());
        var exception = await Assert.ThrowsAsync<PostgresException>(() => ExecuteAsync(Fixture.AppConnectionString, sql, NileCentreId));
        Assert.Equal("42501", exception.SqlState);
    }

    [Fact]
    public async Task ForeignUpdate_NoRowsChanged()
    {
        var id = await SeedCard(MaadiCentreId);
        Assert.Equal(0, await ExecuteAsync(Fixture.AppConnectionString,
            $"update attendance.student_cards set updated_at = now() where id = '{id}'", NileCentreId));
        Assert.Equal(1, await Fixture.ScalarAsync<long>($"select count(*) from attendance.student_cards where id = '{id}' and updated_at is null"));
    }

    [Fact]
    public async Task Delete_DeniedEvenForOwnCentre()
    {
        var id = await SeedCard(NileCentreId);
        var exception = await Assert.ThrowsAsync<PostgresException>(() => ExecuteAsync(Fixture.AppConnectionString,
            $"delete from attendance.student_cards where id = '{id}'", NileCentreId));
        Assert.Equal("42501", exception.SqlState);
    }

    [Fact]
    public async Task Owner_RlsForced()
    {
        await SeedCard(NileCentreId);
        await SeedCard(MaadiCentreId);
        Assert.Equal(0, await CardCount(Fixture.OwnerConnectionString));
        Assert.Equal(1, await CardCount(Fixture.OwnerConnectionString, NileCentreId));
    }

    private static async Task<long> CardCount(string connectionString, Guid? centre = null)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        await using var setting = new NpgsqlCommand("select set_config('app.current_centre', @centre, true)", connection, transaction);
        setting.Parameters.AddWithValue("centre", centre?.ToString() ?? "");
        await setting.ExecuteScalarAsync();
        await using var command = new NpgsqlCommand("select count(*) from attendance.student_cards", connection, transaction);
        return (long)(await command.ExecuteScalarAsync())!;
    }
}
