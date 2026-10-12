using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Students.Commands.SetPrimaryParent;

/// <summary>Orchestrates: scoped lookup → the Domain invariant (exactly one primary, and only among already-linked parents).</summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class SetPrimaryParentHandler(IStudentRepository students) : ICommandHandler<SetPrimaryParentCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(SetPrimaryParentCommand command, CancellationToken cancellationToken)
    {
        var student = await students.GetByIdAsync(command.StudentId, command.Version, cancellationToken);
        if (student is null)
        {
            return Result<Unit>.Failure(StudentErrors.NotFound());
        }

        var updated = student.SetPrimaryParent(command.ParentId);
        return updated.IsFailure ? Result<Unit>.Failure(updated.Error!) : Result<Unit>.Success(Unit.Value);
    }
}
