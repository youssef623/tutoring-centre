using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Students.Commands.UnlinkParent;

/// <summary>
/// Orchestrates: scoped lookup → the Domain invariant (never below one parent, never the primary parent
/// without first making another one primary). Removing the link's row once the aggregate agrees is
/// Infrastructure's job (UnitOfWork.SaveChangesAsync), not this handler's — it only ever touches the aggregate.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class UnlinkParentHandler(IStudentRepository students) : ICommandHandler<UnlinkParentCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(UnlinkParentCommand command, CancellationToken cancellationToken)
    {
        var student = await students.GetByIdAsync(command.StudentId, command.Version, cancellationToken);
        if (student is null)
        {
            return Result<Unit>.Failure(StudentErrors.NotFound());
        }

        var removed = student.RemoveParent(command.ParentId);
        return removed.IsFailure ? Result<Unit>.Failure(removed.Error!) : Result<Unit>.Success(Unit.Value);
    }
}
