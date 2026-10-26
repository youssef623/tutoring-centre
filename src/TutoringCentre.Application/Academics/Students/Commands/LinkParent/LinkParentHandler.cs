using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Academics.Parents;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Students.Commands.LinkParent;

/// <summary>
/// Orchestrates: scoped lookup of both aggregates → the Domain invariant (at most 4 parents, never the same
/// parent twice). A parent in another centre and one that does not exist are indistinguishable: both
/// parent.not_found — <see cref="IParentRepository.FindAsync"/> is itself tenant-scoped.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class LinkParentHandler(IStudentRepository students, IParentRepository parents) : ICommandHandler<LinkParentCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(LinkParentCommand command, CancellationToken cancellationToken)
    {
        var student = await students.GetByIdAsync(command.StudentId, command.Version, cancellationToken);
        if (student is null)
        {
            return Result<Unit>.Failure(StudentErrors.NotFound());
        }

        var parent = await parents.FindAsync(command.ParentId, cancellationToken);
        if (parent is null)
        {
            return Result<Unit>.Failure(ParentErrors.NotFound());
        }

        var added = student.AddParent(parent.Id, command.Relationship);
        return added.IsFailure ? Result<Unit>.Failure(added.Error!) : Result<Unit>.Success(Unit.Value);
    }
}
