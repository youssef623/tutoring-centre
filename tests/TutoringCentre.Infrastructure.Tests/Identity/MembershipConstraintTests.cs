using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using TutoringCentre.Application.Centres.Commands.CreateCentre;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Identity;
using TutoringCentre.Infrastructure.Persistence;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Identity;

/// <summary>Proves the database itself — not just application code — prevents duplicate, orphaned or cascaded identity data.</summary>
public sealed class MembershipConstraintTests(PostgresFixture fixture) : PostgresTestBase(fixture)
{
    [Fact]
    public async Task Migrations_AppliedToEmptyDatabase_CreateExactlyTheFiveIdentityTablesAndNoRoleTables()
    {
        await using var connection = new NpgsqlConnection(Fixture.ConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(
            "select table_name from information_schema.tables where table_schema = 'identity' order by table_name",
            connection);
        await using var reader = await command.ExecuteReaderAsync();

        var tables = new List<string>();
        while (await reader.ReadAsync())
        {
            tables.Add(reader.GetString(0));
        }

        Assert.Equal(["memberships", "user_claims", "user_logins", "user_tokens", "users"], tables);
    }

    [Fact]
    public async Task SaveChanges_SecondMembershipForSameCentreAndUser_ViolatesUniqueIndex()
    {
        using var scope = Fixture.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var centreId = await CreateCentreAsync();
        var userId = await CreateUserAsync(context);

        context.Add(Membership.Create(userId, centreId, StaffRole.Teacher).Value);
        await context.SaveChangesAsync();

        context.Add(Membership.Create(userId, centreId, StaffRole.Secretary).Value);
        var exception = await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());

        Assert.IsType<PostgresException>(exception.InnerException);
        Assert.Equal("23505", ((PostgresException)exception.InnerException).SqlState);
    }

    [Fact]
    public async Task SaveChanges_MembershipForNonExistentCentre_ViolatesForeignKey()
    {
        using var scope = Fixture.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userId = await CreateUserAsync(context);

        context.Add(Membership.Create(userId, Guid.CreateVersion7(), StaffRole.Owner).Value);
        var exception = await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());

        Assert.IsType<PostgresException>(exception.InnerException);
        Assert.Equal("23503", ((PostgresException)exception.InnerException).SqlState);
    }

    [Fact]
    public async Task Delete_CentreWithAMembership_IsRejected()
    {
        using var scope = Fixture.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var centreId = await CreateCentreAsync();
        var userId = await CreateUserAsync(context);
        context.Add(Membership.Create(userId, centreId, StaffRole.Owner).Value);
        await context.SaveChangesAsync();

        await using var connection = new NpgsqlConnection(Fixture.ConnectionString);
        await connection.OpenAsync();
        await using var deleteCommand = new NpgsqlCommand("delete from platform.centres where id = @id", connection);
        deleteCommand.Parameters.AddWithValue("id", centreId);

        var exception = await Assert.ThrowsAsync<PostgresException>(() => deleteCommand.ExecuteNonQueryAsync());
        Assert.Equal("23503", exception.SqlState);
    }

    private async Task<Guid> CreateCentreAsync()
    {
        var command = new CreateCentreCommand("Nile Tutoring Centre", "nile-centre", "Africa/Cairo", SupportedLocale.Ar);
        var result = await Fixture.SendAsAsync<CreateCentreCommand, CreateCentreResult>(new SystemActor(null), command);
        return result.Value.CentreId;
    }

    private static async Task<Guid> CreateUserAsync(AppDbContext context)
    {
        var user = new ApplicationUser
        {
            UserName = "owner@nile.test",
            NormalizedUserName = "OWNER@NILE.TEST",
            Email = "owner@nile.test",
            NormalizedEmail = "OWNER@NILE.TEST",
            DisplayName = "Nile Owner",
            PreferredLocale = "ar",
        };
        context.Add(user);
        await context.SaveChangesAsync();
        return user.Id;
    }
}
