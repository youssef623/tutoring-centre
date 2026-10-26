using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Teachers.Commands.UpdateTeacher;

/// <summary>
/// Orchestrates: parse the phone → scoped lookup → Domain invariants. A teacher in another centre and one
/// that does not exist are indistinguishable: both teacher.not_found. The account link is never touched here.
/// The handler never compares versions itself — the save detects a stale one.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class UpdateTeacherHandler(ITeacherRepository teachers) : ICommandHandler<UpdateTeacherCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(UpdateTeacherCommand command, CancellationToken cancellationToken)
    {
        PhoneNumber? phone = null;
        if (!string.IsNullOrWhiteSpace(command.Phone))
        {
            var parsedPhone = PhoneNumber.Create(command.Phone);
            if (parsedPhone.IsFailure)
            {
                return Result<Unit>.Failure(TeacherErrors.PhoneInvalid(parsedPhone.Error!));
            }

            phone = parsedPhone.Value;
        }

        var teacher = await teachers.GetByIdAsync(command.TeacherId, command.Version, cancellationToken);
        if (teacher is null)
        {
            return Result<Unit>.Failure(TeacherErrors.NotFound());
        }

        var updated = teacher.UpdateDetails(command.FullName, phone);
        return updated.IsFailure ? Result<Unit>.Failure(updated.Error!) : Result<Unit>.Success(Unit.Value);
    }
}
