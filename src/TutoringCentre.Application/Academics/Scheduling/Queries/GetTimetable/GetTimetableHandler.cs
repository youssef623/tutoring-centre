using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Centres.Contracts;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Scheduling.Queries.GetTimetable;

/// <summary>
/// The week is selected by local date, and the instants go out as UTC with the centre's zone ID beside them: the
/// client formats them in the centre's zone, never the browser's, and the server never turns them into local
/// strings.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class GetTimetableHandler(
    ITeacherScopeProvider scopeProvider,
    ICentreTimeZoneProvider timeZones,
    IScheduleReadService readService)
    : IQueryHandler<GetTimetableQuery, TimetableDto>
{
    public async Task<Result<TimetableDto>> HandleAsync(GetTimetableQuery query, CancellationToken cancellationToken)
    {
        var scope = await scopeProvider.GetAsync(cancellationToken);
        var zone = await timeZones.GetForCurrentCentreAsync(cancellationToken);
        var sessions = await readService.GetTimetableAsync(query.WeekStart, query.GroupId, query.TeacherId, scope, cancellationToken);
        return Result<TimetableDto>.Success(new TimetableDto(zone.Id, query.WeekStart, sessions));
    }
}
