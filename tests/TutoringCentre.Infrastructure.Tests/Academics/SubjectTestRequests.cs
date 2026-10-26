using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Task 23.6: test-only requests that exercise Subject through the real AppDbContext, interceptors and unit of
/// work — standing in for the repository and read service that arrive on Day 25. The centre is taken as an
/// explicit parameter, not the acting actor's own centre, only so <c>AddSubjectCommand</c> can also play the
/// "constructed with another centre" scenario (test 8); every other test passes the actor's own centre.
/// </summary>
internal sealed record AddSubjectCommand(Guid CentreId, string Name) : ICommand<Guid>, ITenantScoped;

internal sealed record ListSubjectNamesQuery : IQuery<List<string>>, ITenantScoped;

internal sealed record RenameSubjectCommand(Guid SubjectId, string NewName) : ICommand<Unit>, ITenantScoped;

/// <summary>Task 31.7: renames a subject, then always fails — proves a rolled-back change leaves no audit row,
/// before a production command that fails after changing a subject exists to test instead.</summary>
internal sealed record RenameSubjectThenFailCommand(Guid SubjectId, string NewName) : ICommand<Unit>, ITenantScoped;

internal sealed class AddSubjectCommandHandler(AppDbContext db) : ICommandHandler<AddSubjectCommand, Guid>
{
    public Task<Result<Guid>> HandleAsync(AddSubjectCommand command, CancellationToken cancellationToken)
    {
        var created = Subject.Create(command.CentreId, command.Name);
        if (created.IsFailure)
        {
            return Task.FromResult(Result<Guid>.Failure(created.Error!));
        }

        db.Set<Subject>().Add(created.Value);
        return Task.FromResult(Result<Guid>.Success(created.Value.Id));
    }
}

internal sealed class ListSubjectNamesQueryHandler(AppDbContext db) : IQueryHandler<ListSubjectNamesQuery, List<string>>
{
    public async Task<Result<List<string>>> HandleAsync(ListSubjectNamesQuery query, CancellationToken cancellationToken)
    {
        var names = await db.Set<Subject>().Select(subject => subject.Name).ToListAsync(cancellationToken);
        return Result<List<string>>.Success(names);
    }
}

internal sealed class RenameSubjectCommandHandler(AppDbContext db) : ICommandHandler<RenameSubjectCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(RenameSubjectCommand command, CancellationToken cancellationToken)
    {
        var subject = await db.Set<Subject>().SingleAsync(s => s.Id == command.SubjectId, cancellationToken);
        var renamed = subject.Rename(command.NewName);
        return renamed.IsFailure ? Result<Unit>.Failure(renamed.Error!) : Result<Unit>.Success(Unit.Value);
    }
}

internal sealed class RenameSubjectThenFailCommandHandler(AppDbContext db) : ICommandHandler<RenameSubjectThenFailCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(RenameSubjectThenFailCommand command, CancellationToken cancellationToken)
    {
        var subject = await db.Set<Subject>().SingleAsync(s => s.Id == command.SubjectId, cancellationToken);
        var renamed = subject.Rename(command.NewName);
        if (renamed.IsFailure)
        {
            return Result<Unit>.Failure(renamed.Error!);
        }

        return Result<Unit>.Failure(Error.Rule("test.deliberate_failure", "Always fails, after the subject was renamed."));
    }
}
