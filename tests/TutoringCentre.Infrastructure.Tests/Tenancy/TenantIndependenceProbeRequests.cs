using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.Tests.Tenancy;

/// <summary>
/// Task 21.5: test-only requests that each depend on exactly one isolation layer, so a layer's own test can be
/// proven to hold even when the other layer is bypassed or absent.
/// </summary>

/// <summary>Bypasses the EF query filter (Task 20.2) on purpose — only row-level security (Task 21.3) can still narrow this.</summary>
internal sealed record ListProbeCentresIgnoringEfFilterQuery : IQuery<List<Guid>>;

/// <summary>A normal, filtered EF query — relies only on the Task 20.2 filter, not on row-level security.</summary>
internal sealed record ListProbeCentresQuery : IQuery<List<Guid>>;

/// <summary>Raw SQL: never goes through the EF filter at all (it can't) — only row-level security narrows this.</summary>
internal sealed record ListProbeCentresByRawSqlQuery : IQuery<List<Guid>>;

internal sealed class ListProbeCentresIgnoringEfFilterQueryHandler(AppDbContext db)
    : IQueryHandler<ListProbeCentresIgnoringEfFilterQuery, List<Guid>>
{
    public async Task<Result<List<Guid>>> HandleAsync(ListProbeCentresIgnoringEfFilterQuery query, CancellationToken cancellationToken)
    {
        var centreIds = await db.Set<TenantProbe>().IgnoreQueryFilters().Select(probe => probe.CentreId).ToListAsync(cancellationToken);
        return Result<List<Guid>>.Success(centreIds);
    }
}

internal sealed class ListProbeCentresQueryHandler(AppDbContext db) : IQueryHandler<ListProbeCentresQuery, List<Guid>>
{
    public async Task<Result<List<Guid>>> HandleAsync(ListProbeCentresQuery query, CancellationToken cancellationToken)
    {
        var centreIds = await db.Set<TenantProbe>().Select(probe => probe.CentreId).ToListAsync(cancellationToken);
        return Result<List<Guid>>.Success(centreIds);
    }
}

internal sealed class ListProbeCentresByRawSqlQueryHandler(AppDbContext db) : IQueryHandler<ListProbeCentresByRawSqlQuery, List<Guid>>
{
    public async Task<Result<List<Guid>>> HandleAsync(ListProbeCentresByRawSqlQuery query, CancellationToken cancellationToken)
    {
        var centreIds = await db.Database
            .SqlQueryRaw<Guid>("select centre_id from probe.tenant_probes")
            .ToListAsync(cancellationToken);
        return Result<List<Guid>>.Success(centreIds);
    }
}
