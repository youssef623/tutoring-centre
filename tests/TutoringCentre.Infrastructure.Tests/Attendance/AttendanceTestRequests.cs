using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Attendance;
using TutoringCentre.Domain.Common;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.Tests.Attendance;

internal sealed record AddAttendanceCommand(AttendanceReferences References) : ICommand<Guid>, ITenantScoped;
internal sealed record GetAttendanceQuery(Guid Id) : IQuery<AttendanceRecord?>, ITenantScoped;
internal sealed record CorrectAttendanceCommand(Guid Id, AttendanceStatus Status, string Reason) : ICommand<Unit>, ITenantScoped;

internal static class AttendanceTestTiming
{
    public static readonly DateTimeOffset Now = new(2026, 12, 5, 16, 1, 0, TimeSpan.Zero);
    public static readonly AttendanceSessionTiming Timing = new(
        new DateTimeOffset(2026, 12, 5, 16, 0, 0, TimeSpan.Zero),
        new DateTimeOffset(2026, 12, 5, 17, 30, 0, TimeSpan.Zero), false);
}

internal sealed class AddAttendanceCommandHandler(AppDbContext db, ICurrentActor actor) : ICommandHandler<AddAttendanceCommand, Guid>
{
    public Task<Result<Guid>> HandleAsync(AddAttendanceCommand command, CancellationToken cancellationToken)
    {
        var references = command.References;
        var result = AttendanceRecord.Create(references.CentreId, references.SessionId, references.StudentId,
            references.EnrollmentId, AttendanceStatus.Present, AttendanceSource.Manual,
            ((StaffActor)actor.Actor).UserId, AttendanceTestTiming.Timing, AttendanceTestTiming.Now);
        if (result.IsFailure)
        {
            return Task.FromResult(Result<Guid>.Failure(result.Error!));
        }

        db.Add(result.Value);
        return Task.FromResult(Result<Guid>.Success(result.Value.Id));
    }
}

internal sealed class GetAttendanceQueryHandler(AppDbContext db) : IQueryHandler<GetAttendanceQuery, AttendanceRecord?>
{
    public async Task<Result<AttendanceRecord?>> HandleAsync(GetAttendanceQuery query, CancellationToken cancellationToken) =>
        Result<AttendanceRecord?>.Success(await db.Set<AttendanceRecord>().AsNoTracking()
            .SingleOrDefaultAsync(record => record.Id == query.Id, cancellationToken));
}

internal sealed class CorrectAttendanceCommandHandler(AppDbContext db, ICurrentActor actor) : ICommandHandler<CorrectAttendanceCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(CorrectAttendanceCommand command, CancellationToken cancellationToken)
    {
        var record = await db.Set<AttendanceRecord>().SingleAsync(record => record.Id == command.Id, cancellationToken);
        var result = record.Correct(command.Status, command.Reason, ((StaffActor)actor.Actor).UserId,
            AttendanceTestTiming.Timing, AttendanceTestTiming.Now.AddMinutes(1));
        return result.IsFailure ? Result<Unit>.Failure(result.Error!) : Result<Unit>.Success(Unit.Value);
    }
}
