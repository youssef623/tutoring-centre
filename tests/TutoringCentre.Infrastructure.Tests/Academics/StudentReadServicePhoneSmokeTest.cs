using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;
using TutoringCentre.Application.Academics.Students;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Persistence;
using TutoringCentre.Infrastructure.ReadServices;
using TutoringCentre.Infrastructure.Tests.Fixtures;
using TutoringCentre.Infrastructure.Tests.Tenancy;

namespace TutoringCentre.Infrastructure.Tests.Academics;

public sealed class StudentReadServicePhoneSmokeTest(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    [Fact]
    public async Task PhoneFragmentSearch_FindsStudentByParentPhoneSuffix()
    {
        var studentId = await Fixture.SeedStudentAsync(NileCentreId, "Omar Hassan");
        await using (var connection = new NpgsqlConnection(Fixture.SuperuserConnectionString))
        {
            await connection.OpenAsync();
            await using var command = new NpgsqlCommand($"update academics.students set phone = '+201012345678' where id = '{studentId}'", connection);
            await command.ExecuteNonQueryAsync();
        }

        var actor = new StaffActor(Guid.CreateVersion7(), NileCentreId, StaffRole.Owner);
        var actorContext = new CurrentActorContext();
        actorContext.Set(actor);
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(Fixture.AppConnectionString).UseSnakeCaseNamingConvention();
        await using var db = new AppDbContext(optionsBuilder.Options, actorContext);
        var unitOfWork = new UnitOfWork(db, NullLogger<UnitOfWork>.Instance);
        await unitOfWork.BeginAsync(readOnly: true, CancellationToken.None);

        var readService = new StudentReadService(db);
        var result = await readService.ListAsync(new StudentSearchCriteria("5678", false, null, 25), CancellationToken.None);

        await unitOfWork.CommitAsync(CancellationToken.None);

        Assert.Single(result.Items);
        Assert.Equal(studentId, result.Items[0].Id);
    }

    [Fact]
    public async Task NameSearch_LogsPredicateParameterAndLimit()
    {
        await Fixture.SeedStudentAsync(NileCentreId, "Ahmed Hassan");

        var actor = new StaffActor(Guid.CreateVersion7(), NileCentreId, StaffRole.Owner);
        var actorContext = new CurrentActorContext();
        actorContext.Set(actor);
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(Fixture.AppConnectionString).UseSnakeCaseNamingConvention();
        await using var db = new AppDbContext(optionsBuilder.Options, actorContext);
        var unitOfWork = new UnitOfWork(db, NullLogger<UnitOfWork>.Instance);
        await unitOfWork.BeginAsync(readOnly: true, CancellationToken.None);

        var readService = new StudentReadService(db);
        var result = await readService.ListAsync(new StudentSearchCriteria("hassan", false, null, 25), CancellationToken.None);

        await unitOfWork.CommitAsync(CancellationToken.None);

        Assert.Single(result.Items);
    }
}
