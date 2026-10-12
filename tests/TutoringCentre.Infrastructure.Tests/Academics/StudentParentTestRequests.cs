using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Task 38.5/38.6: test-only requests that exercise the Student and Parent aggregates through the real
/// AppDbContext, interceptors and unit of work — standing in for the repositories and use cases that arrive
/// on Days 39–40.
/// </summary>
internal sealed record AddParentCommand(Guid CentreId, string FullName, string Phone) : ICommand<Guid>, ITenantScoped;

internal sealed record UpdateParentPhoneCommand(Guid ParentId, string Phone) : ICommand<Unit>, ITenantScoped;

internal sealed record RegisterStudentCommand(
    Guid CentreId, string FullName, string? Phone, string? SchoolYear, Guid PrimaryParentId, ParentRelationship Relationship)
    : ICommand<Guid>, ITenantScoped;

internal sealed record AddStudentParentCommand(Guid StudentId, Guid ParentId, ParentRelationship Relationship) : ICommand<Unit>, ITenantScoped;

internal sealed record RemoveStudentParentCommand(Guid StudentId, Guid ParentId) : ICommand<Unit>, ITenantScoped;

internal sealed record SetPrimaryStudentParentCommand(Guid StudentId, Guid ParentId) : ICommand<Unit>, ITenantScoped;

internal sealed record ListStudentNamesQuery : IQuery<List<string>>, ITenantScoped;

internal sealed record GetStudentLinksSnapshotQuery(Guid StudentId) : IQuery<StudentLinksSnapshot>, ITenantScoped;

internal sealed record StudentLinksSnapshot(int LinksRevision, IReadOnlyList<StudentLinkSnapshot> Links);

internal sealed record StudentLinkSnapshot(Guid ParentId, bool IsPrimary);

internal sealed class AddParentCommandHandler(AppDbContext db) : ICommandHandler<AddParentCommand, Guid>
{
    public Task<Result<Guid>> HandleAsync(AddParentCommand command, CancellationToken cancellationToken)
    {
        var phone = PhoneNumber.Create(command.Phone);
        if (phone.IsFailure)
        {
            return Task.FromResult(Result<Guid>.Failure(phone.Error!));
        }

        var created = Parent.Create(command.CentreId, command.FullName, phone.Value);
        if (created.IsFailure)
        {
            return Task.FromResult(Result<Guid>.Failure(created.Error!));
        }

        db.Set<Parent>().Add(created.Value);
        return Task.FromResult(Result<Guid>.Success(created.Value.Id));
    }
}

internal sealed class UpdateParentPhoneCommandHandler(AppDbContext db) : ICommandHandler<UpdateParentPhoneCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(UpdateParentPhoneCommand command, CancellationToken cancellationToken)
    {
        var phone = PhoneNumber.Create(command.Phone);
        if (phone.IsFailure)
        {
            return Result<Unit>.Failure(phone.Error!);
        }

        var parent = await db.Set<Parent>().SingleAsync(p => p.Id == command.ParentId, cancellationToken);
        var updated = parent.Update(parent.FullName, phone.Value);
        return updated.IsFailure ? Result<Unit>.Failure(updated.Error!) : Result<Unit>.Success(Unit.Value);
    }
}

internal sealed class RegisterStudentCommandHandler(AppDbContext db) : ICommandHandler<RegisterStudentCommand, Guid>
{
    public Task<Result<Guid>> HandleAsync(RegisterStudentCommand command, CancellationToken cancellationToken)
    {
        PhoneNumber? phone = null;
        if (command.Phone is not null)
        {
            var created = PhoneNumber.Create(command.Phone);
            if (created.IsFailure)
            {
                return Task.FromResult(Result<Guid>.Failure(created.Error!));
            }

            phone = created.Value;
        }

        var registered = Student.Register(
            command.CentreId, command.FullName, phone, command.SchoolYear, command.PrimaryParentId, command.Relationship);
        if (registered.IsFailure)
        {
            return Task.FromResult(Result<Guid>.Failure(registered.Error!));
        }

        db.Set<Student>().Add(registered.Value);
        return Task.FromResult(Result<Guid>.Success(registered.Value.Id));
    }
}

internal sealed class AddStudentParentCommandHandler(AppDbContext db) : ICommandHandler<AddStudentParentCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(AddStudentParentCommand command, CancellationToken cancellationToken)
    {
        var student = await db.Set<Student>().Include(s => s.Parents).SingleAsync(s => s.Id == command.StudentId, cancellationToken);
        var result = student.AddParent(command.ParentId, command.Relationship);
        return result.IsFailure ? Result<Unit>.Failure(result.Error!) : Result<Unit>.Success(Unit.Value);
    }
}

internal sealed class RemoveStudentParentCommandHandler(AppDbContext db) : ICommandHandler<RemoveStudentParentCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(RemoveStudentParentCommand command, CancellationToken cancellationToken)
    {
        var student = await db.Set<Student>().Include(s => s.Parents).SingleAsync(s => s.Id == command.StudentId, cancellationToken);
        var removedLink = student.Parents.SingleOrDefault(link => link.ParentId == command.ParentId);

        var result = student.RemoveParent(command.ParentId);
        if (result.IsFailure)
        {
            return Result<Unit>.Failure(result.Error!);
        }

        // Student.RemoveParent only takes the link out of its own in-memory collection; with the foreign key
        // kept RESTRICT (Task 38.2 — never cascade at the database level), EF does not infer deletion from a
        // severed required relationship on its own, so the now-detached link is told explicitly to be removed.
        db.Set<StudentParentLink>().Remove(removedLink!);
        return Result<Unit>.Success(Unit.Value);
    }
}

internal sealed class SetPrimaryStudentParentCommandHandler(AppDbContext db) : ICommandHandler<SetPrimaryStudentParentCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(SetPrimaryStudentParentCommand command, CancellationToken cancellationToken)
    {
        var student = await db.Set<Student>().Include(s => s.Parents).SingleAsync(s => s.Id == command.StudentId, cancellationToken);
        var result = student.SetPrimaryParent(command.ParentId);
        return result.IsFailure ? Result<Unit>.Failure(result.Error!) : Result<Unit>.Success(Unit.Value);
    }
}

internal sealed class ListStudentNamesQueryHandler(AppDbContext db) : IQueryHandler<ListStudentNamesQuery, List<string>>
{
    public async Task<Result<List<string>>> HandleAsync(ListStudentNamesQuery query, CancellationToken cancellationToken)
    {
        var names = await db.Set<Student>().Select(student => student.FullName).ToListAsync(cancellationToken);
        return Result<List<string>>.Success(names);
    }
}

internal sealed class GetStudentLinksSnapshotQueryHandler(AppDbContext db) : IQueryHandler<GetStudentLinksSnapshotQuery, StudentLinksSnapshot>
{
    public async Task<Result<StudentLinksSnapshot>> HandleAsync(GetStudentLinksSnapshotQuery query, CancellationToken cancellationToken)
    {
        var student = await db.Set<Student>().Include(s => s.Parents).SingleAsync(s => s.Id == query.StudentId, cancellationToken);
        var links = student.Parents.Select(link => new StudentLinkSnapshot(link.ParentId, link.IsPrimary)).ToList();
        return Result<StudentLinksSnapshot>.Success(new StudentLinksSnapshot(student.LinksRevision, links));
    }
}
