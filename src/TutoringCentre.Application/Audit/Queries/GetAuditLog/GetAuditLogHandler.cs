using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Audit;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Audit.Queries.GetAuditLog;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class GetAuditLogHandler(IAuditReadService readService) : IQueryHandler<GetAuditLogQuery, AuditPageDto>
{
    public async Task<Result<AuditPageDto>> HandleAsync(GetAuditLogQuery query, CancellationToken cancellationToken)
    {
        var page = await readService.GetPageAsync(
            query.EntityType,
            query.EntityId,
            query.ActorUserId,
            query.From,
            query.To,
            query.Cursor,
            query.PageSize,
            cancellationToken);

        return Result<AuditPageDto>.Success(page);
    }
}
