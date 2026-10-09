using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Staff;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Identity;

/// <summary>Task 29.4: proves IStaffAccountService over real PostgreSQL, including that it rolls back with the command that calls it.</summary>
public sealed class StaffAccountServiceTests(PostgresFixture fixture) : PostgresTestBase(fixture)
{
    [Fact]
    public async Task EnsureAccountAsync_NewEmail_CreatesOneUserWithAHashedPasswordAndTheMustChangeFlagSet()
    {
        using var scope = Fixture.Services.CreateScope();
        var accountService = scope.ServiceProvider.GetRequiredService<IStaffAccountService>();

        var result = await accountService.EnsureAccountAsync("new-teacher@nile.test", "New Teacher", "en", CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value.Created);
        Assert.False(string.IsNullOrWhiteSpace(result.Value.TemporaryPassword));
        Assert.Equal(
            1,
            await Fixture.ScalarAsync<long>(
                $"""
                select count(*) from identity.users
                where id = '{result.Value.UserId}' and password_hash is not null and must_change_password = true
                """));
    }

    [Fact]
    public async Task EnsureAccountAsync_NewEmail_TheReturnedTemporaryPasswordVerifiesThroughTheAuthenticationPort()
    {
        using var scope = Fixture.Services.CreateScope();
        var accountService = scope.ServiceProvider.GetRequiredService<IStaffAccountService>();
        var authService = scope.ServiceProvider.GetRequiredService<IAuthenticationService>();

        var created = await accountService.EnsureAccountAsync("verify-me@nile.test", "Verify Me", "en", CancellationToken.None);
        var verified = await authService.VerifyCredentialsAsync("verify-me@nile.test", created.Value.TemporaryPassword!, CancellationToken.None);

        Assert.True(verified.IsSuccess);
        Assert.Equal(created.Value.UserId, verified.Value.UserId);
    }

    [Fact]
    public async Task EnsureAccountAsync_ExistingEmail_ReturnsTheSameIdCreatesNothingAndLeavesTheProfileUntouched()
    {
        using var firstScope = Fixture.Services.CreateScope();
        var first = await firstScope.ServiceProvider.GetRequiredService<IStaffAccountService>()
            .EnsureAccountAsync("existing@nile.test", "Original Name", "en", CancellationToken.None);

        using var secondScope = Fixture.Services.CreateScope();
        var second = await secondScope.ServiceProvider.GetRequiredService<IStaffAccountService>()
            .EnsureAccountAsync("existing@nile.test", "Different Name", "ar", CancellationToken.None);

        Assert.True(second.IsSuccess);
        Assert.Equal(first.Value.UserId, second.Value.UserId);
        Assert.False(second.Value.Created);
        Assert.Null(second.Value.TemporaryPassword);
        Assert.Equal(1, await Fixture.ScalarAsync<long>("select count(*) from identity.users"));
        Assert.Equal(
            "Original Name",
            await Fixture.ScalarAsync<string>($"select display_name from identity.users where id = '{first.Value.UserId}'"));
    }

    [Fact]
    public async Task AFailingCommandAfterAccountCreation_LeavesNoUser()
    {
        var result = await Fixture.SendAsAsync<EnsureAccountThenFailCommand, Unit>(
            new SystemActor(null), new EnsureAccountThenFailCommand("rolled-back@nile.test", "Rolled Back", "en"));

        Assert.True(result.IsFailure);
        Assert.Equal("test.deliberate_failure", result.Error!.Code);
        Assert.Equal(0, await Fixture.ScalarAsync<long>("select count(*) from identity.users"));
    }
}
