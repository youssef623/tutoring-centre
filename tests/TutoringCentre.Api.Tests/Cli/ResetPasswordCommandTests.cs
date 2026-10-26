using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;
using TutoringCentre.Infrastructure.Identity;

namespace TutoringCentre.Api.Tests.Cli;

[Collection(ApiCollection.Name)]
public sealed class ResetPasswordCommandTests(ApiFactory factory) : IAsyncLifetime
{
    public Task InitializeAsync() => factory.ResetAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task RunAsync_ExistingUser_OldPasswordStopsWorkingAndNewPasswordWorks()
    {
        await SeedCommand.RunAsync(factory.Services);

        var exitCode = await ResetPasswordCommand.RunAsync(factory.Services, "owner@nile.test", "a-new-rotated-password-1!");

        // A fresh scope, not the one seed used: CheckPasswordAsync checks the passed-in instance's own
        // PasswordHash, so a stale, pre-rotation copy would wrongly still say the old password works.
        using var scope = factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var user = await userManager.FindByEmailAsync("owner@nile.test");

        Assert.Equal(0, exitCode);
        Assert.False(await userManager.CheckPasswordAsync(user!, ApiFactory.TestSeedPassword));
        Assert.True(await userManager.CheckPasswordAsync(user!, "a-new-rotated-password-1!"));
    }

    [Fact]
    public async Task RunAsync_UnknownEmail_ReturnsNonZeroWithoutThrowing()
    {
        var exitCode = await ResetPasswordCommand.RunAsync(factory.Services, "nobody@nowhere.test", "whatever-password-1!");

        Assert.Equal(1, exitCode);
    }
}
