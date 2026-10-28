using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Academics.Contracts;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Attendance.Queries.GetAttendance;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI.")]
internal sealed class GetAttendanceHandler(ITeachingScope teachingScope, IAttendanceReadService reads)
    : IQueryHandler<GetAttendanceQuery, AttendancePageDto>
{
    public async Task<Result<AttendancePageDto>> HandleAsync(GetAttendanceQuery query, CancellationToken cancellationToken)
    {
        var scope = await teachingScope.GetAsync(cancellationToken);
        var access = scope.Kind == TeachingScopeKind.All ? AttendanceAccess.All
            : scope.Kind == TeachingScopeKind.Own && scope.TeacherId is { } id ? AttendanceAccess.OwnSessions(id)
            : AttendanceAccess.None;
        return Result<AttendancePageDto>.Success(await reads.ListAsync(new(query.StudentId, query.GroupId, query.From, query.To),
            access, query.Cursor, query.PageSize, cancellationToken));
    }
}
