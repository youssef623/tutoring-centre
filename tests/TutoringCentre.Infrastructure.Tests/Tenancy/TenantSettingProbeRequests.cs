using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.Tests.Tenancy;

/// <summary>Task 21.2: test-only requests that read app.current_centre back from inside a real dispatched request,
/// proving what UnitOfWork.BeginAsync (Task 21.1) actually set for that transaction — not a stand-in assertion.</summary>
internal sealed record ReadCurrentCentreSettingQuery : IQuery<string>;

internal sealed record ReadCurrentCentreSettingCommand : ICommand<string>;

/// <summary>Always fails, without touching the database: proves a transaction's app.current_centre setting does
/// not survive a rollback (Task 21.2, lifetime test 5).</summary>
internal sealed record AlwaysFailingCommand : ICommand<string>;

internal sealed class ReadCurrentCentreSettingQueryHandler(AppDbContext db) : IQueryHandler<ReadCurrentCentreSettingQuery, string>
{
    public async Task<Result<string>> HandleAsync(ReadCurrentCentreSettingQuery query, CancellationToken cancellationToken) =>
        Result<string>.Success(await TenantSettingReader.ReadAsync(db, cancellationToken));
}

internal sealed class ReadCurrentCentreSettingCommandHandler(AppDbContext db) : ICommandHandler<ReadCurrentCentreSettingCommand, string>
{
    public async Task<Result<string>> HandleAsync(ReadCurrentCentreSettingCommand command, CancellationToken cancellationToken) =>
        Result<string>.Success(await TenantSettingReader.ReadAsync(db, cancellationToken));
}

internal sealed class AlwaysFailingCommandHandler : ICommandHandler<AlwaysFailingCommand, string>
{
    public Task<Result<string>> HandleAsync(AlwaysFailingCommand command, CancellationToken cancellationToken) =>
        Task.FromResult(Result<string>.Failure(Error.Rule("test.always_fails", "Fails on purpose; writes nothing.")));
}

internal static class TenantSettingReader
{
    public static async Task<string> ReadAsync(AppDbContext db, CancellationToken ct)
    {
        var rows = await db.Database
            .SqlQueryRaw<string?>("select current_setting('app.current_centre', true) as value")
            .ToListAsync(ct);
        return rows[0] ?? string.Empty;
    }
}
