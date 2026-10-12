using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Academics.Parents;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Students.Commands.RegisterStudent;

/// <summary>
/// Orchestrates one transaction across two aggregates: resolve or create the parent, then register the
/// student against it. Nothing is added before every check that can fail has passed, and the handler never
/// dispatches a nested command — the nested-transaction guard would refuse it, and it would be two
/// transactions instead of one. The dispatcher saves once: both aggregates and the link, or neither.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class RegisterStudentHandler(ICurrentActor currentActor, IStudentRepository students, IParentRepository parents)
    : ICommandHandler<RegisterStudentCommand, RegisterStudentResult>
{
    public async Task<Result<RegisterStudentResult>> HandleAsync(RegisterStudentCommand command, CancellationToken cancellationToken)
    {
        // ITenantScoped guarantees the dispatcher already refused any actor without a centre (Day 22).
        var centreId = currentActor.Actor.CentreId!.Value;

        PhoneNumber? studentPhone = null;
        if (!string.IsNullOrWhiteSpace(command.Phone))
        {
            var parsedStudentPhone = PhoneNumber.Create(command.Phone);
            if (parsedStudentPhone.IsFailure)
            {
                return Result<RegisterStudentResult>.Failure(StudentErrors.PhoneInvalid(parsedStudentPhone.Error!));
            }

            studentPhone = parsedStudentPhone.Value;
        }

        Guid parentId;
        if (command.Parent.ExistingParentId is { } existingParentId)
        {
            var existingParent = await parents.FindAsync(existingParentId, cancellationToken);
            if (existingParent is null)
            {
                return Result<RegisterStudentResult>.Failure(ParentErrors.NotFound());
            }

            parentId = existingParent.Id;
        }
        else
        {
            var parentPhone = PhoneNumber.Create(command.Parent.Phone!);
            if (parentPhone.IsFailure)
            {
                return Result<RegisterStudentResult>.Failure(StudentErrors.ParentPhoneInvalid(parentPhone.Error!));
            }

            if (await parents.PhoneExistsAsync(parentPhone.Value, excludingId: null, cancellationToken))
            {
                return Result<RegisterStudentResult>.Failure(StudentErrors.ParentPhoneTaken());
            }

            var createdParent = Parent.Create(centreId, command.Parent.FullName!, parentPhone.Value);
            if (createdParent.IsFailure)
            {
                return Result<RegisterStudentResult>.Failure(createdParent.Error!);
            }

            parents.Add(createdParent.Value);
            parentId = createdParent.Value.Id;
        }

        var registered = Student.Register(centreId, command.FullName, studentPhone, command.SchoolYear, parentId, command.Relationship);
        if (registered.IsFailure)
        {
            return Result<RegisterStudentResult>.Failure(registered.Error!);
        }

        students.Add(registered.Value);
        return Result<RegisterStudentResult>.Success(new RegisterStudentResult(registered.Value.Id, parentId));
    }
}
