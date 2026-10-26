using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Students.Commands.UpdateStudent;

/// <summary>
/// Orchestrates: parse the phone → scoped lookup → Domain invariants. A student in another centre and one
/// that does not exist are indistinguishable: both student.not_found. Parent links are never touched here.
/// The handler never compares versions itself — the save detects a stale one.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class UpdateStudentHandler(IStudentRepository students) : ICommandHandler<UpdateStudentCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(UpdateStudentCommand command, CancellationToken cancellationToken)
    {
        PhoneNumber? phone = null;
        if (!string.IsNullOrWhiteSpace(command.Phone))
        {
            var parsedPhone = PhoneNumber.Create(command.Phone);
            if (parsedPhone.IsFailure)
            {
                return Result<Unit>.Failure(StudentErrors.PhoneInvalid(parsedPhone.Error!));
            }

            phone = parsedPhone.Value;
        }

        var student = await students.GetByIdAsync(command.StudentId, command.Version, cancellationToken);
        if (student is null)
        {
            return Result<Unit>.Failure(StudentErrors.NotFound());
        }

        var updated = student.UpdateDetails(command.FullName, phone, command.SchoolYear);
        return updated.IsFailure ? Result<Unit>.Failure(updated.Error!) : Result<Unit>.Success(Unit.Value);
    }
}
