using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Application.Centres.Commands.CreateCentre;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Identity;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Identity;
using TutoringCentre.Infrastructure.Persistence;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Identity;

/// <summary>Proves the read side against real PostgreSQL, using the Day 13 staff set.</summary>
public sealed class MembershipReadServiceTests(PostgresFixture fixture) : PostgresTestBase(fixture)
{
    [Fact]
    public async Task GetProfileAsync_TwoCentreTeacher_SeesBothCentres()
    {
        var staff = await SeedStaffAsync();
        using var scope = Fixture.CreateScope();
        var readService = scope.ServiceProvider.GetRequiredService<IMembershipReadService>();

        var profile = await readService.GetProfileAsync(staff.TeacherId, CancellationToken.None);

        Assert.NotNull(profile);
        Assert.Equal(2, profile.Memberships.Count);
    }

    [Fact]
    public async Task GetProfileAsync_Secretary_SeesOneCentre()
    {
        var staff = await SeedStaffAsync();
        using var scope = Fixture.CreateScope();
        var readService = scope.ServiceProvider.GetRequiredService<IMembershipReadService>();

        var profile = await readService.GetProfileAsync(staff.SecretaryId, CancellationToken.None);

        Assert.NotNull(profile);
        Assert.Single(profile.Memberships);
    }

    [Fact]
    public async Task GetProfileAsync_InactiveMember_SeesNoCentres()
    {
        var staff = await SeedStaffAsync();
        using var scope = Fixture.CreateScope();
        var readService = scope.ServiceProvider.GetRequiredService<IMembershipReadService>();

        var profile = await readService.GetProfileAsync(staff.InactiveId, CancellationToken.None);

        Assert.NotNull(profile);
        Assert.Empty(profile.Memberships);
    }

    [Fact]
    public async Task GetActiveMembershipAsync_OwnerAskingForAForeignCentre_ReturnsNull()
    {
        var staff = await SeedStaffAsync();
        using var scope = Fixture.CreateScope();
        var readService = scope.ServiceProvider.GetRequiredService<IMembershipReadService>();

        var membership = await readService.GetActiveMembershipAsync(staff.NileOwnerId, staff.MaadiCentreId, CancellationToken.None);

        Assert.Null(membership);
    }

    [Fact]
    public async Task GetProfileAsync_NeverIncludesAnotherUsersMemberships()
    {
        var staff = await SeedStaffAsync();
        using var scope = Fixture.CreateScope();
        var readService = scope.ServiceProvider.GetRequiredService<IMembershipReadService>();

        var profile = await readService.GetProfileAsync(staff.NileOwnerId, CancellationToken.None);

        Assert.NotNull(profile);
        Assert.Single(profile.Memberships);
        Assert.Equal(staff.NileCentreId, profile.Memberships[0].CentreId);
    }

    private async Task<StaffIds> SeedStaffAsync()
    {
        var nileCentreId = await CreateCentreAsync("Nile Tutoring Centre", "nile-centre");
        var maadiCentreId = await CreateCentreAsync("Maadi Learning Hub", "maadi-hub");

        using var scope = Fixture.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var nileOwnerId = await CreateUserAsync(db, "owner@nile.test", "Nile Owner", "ar");
        var teacherId = await CreateUserAsync(db, "teacher@both.test", "Two-Centre Teacher", "en");
        var secretaryId = await CreateUserAsync(db, "secretary@nile.test", "Nile Secretary", "ar");
        var inactiveId = await CreateUserAsync(db, "inactive@nile.test", "Former Secretary", "ar");

        db.Add(Membership.Create(nileOwnerId, nileCentreId, StaffRole.Owner).Value);
        db.Add(Membership.Create(teacherId, nileCentreId, StaffRole.Teacher).Value);
        db.Add(Membership.Create(teacherId, maadiCentreId, StaffRole.Teacher).Value);
        db.Add(Membership.Create(secretaryId, nileCentreId, StaffRole.Secretary).Value);

        var inactiveMembership = Membership.Create(inactiveId, nileCentreId, StaffRole.Secretary).Value;
        inactiveMembership.Deactivate();
        db.Add(inactiveMembership);

        await db.SaveChangesAsync(CancellationToken.None);

        return new StaffIds(nileCentreId, maadiCentreId, nileOwnerId, teacherId, secretaryId, inactiveId);
    }

    private async Task<Guid> CreateCentreAsync(string name, string slug)
    {
        var command = new CreateCentreCommand(name, slug, "Africa/Cairo", SupportedLocale.Ar);
        var result = await Fixture.SendAsAsync<CreateCentreCommand, CreateCentreResult>(new SystemActor(null), command);
        return result.Value.CentreId;
    }

    private static async Task<Guid> CreateUserAsync(AppDbContext db, string email, string displayName, string locale)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            NormalizedUserName = email.ToUpperInvariant(),
            Email = email,
            NormalizedEmail = email.ToUpperInvariant(),
            DisplayName = displayName,
            PreferredLocale = locale,
        };
        db.Add(user);
        await db.SaveChangesAsync(CancellationToken.None);
        return user.Id;
    }

    private sealed record StaffIds(
        Guid NileCentreId,
        Guid MaadiCentreId,
        Guid NileOwnerId,
        Guid TeacherId,
        Guid SecretaryId,
        Guid InactiveId);
}
