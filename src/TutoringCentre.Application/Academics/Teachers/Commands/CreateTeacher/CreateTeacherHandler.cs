using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Teachers.Commands.CreateTeacher;

/// <summary>Orchestrates: parse the phone → Domain invariants → add. Teacher names are not unique, so there is no set-based rule here.</summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class CreateTeacherHandler(ICurrentActor currentActor, ITeacherRepository teachers)
    : ICommandHandler<CreateTeacherCommand, CreateTeacherResult>
{
    public Task<Result<CreateTeacherResult>> HandleAsync(CreateTeacherCommand command, CancellationToken cancellationToken)
    {
        // ITenantScoped guarantees the dispatcher already refused any actor without a centre.
        var centreId = currentActor.Actor.CentreId!.Value;

        PhoneNumber? phone = null;
        if (!string.IsNullOrWhiteSpace(command.Phone))
        {
            var parsedPhone = PhoneNumber.Create(command.Phone);
            if (parsedPhone.IsFailure)
            {
                return Task.FromResult(Result<CreateTeacherResult>.Failure(TeacherErrors.PhoneInvalid(parsedPhone.Error!)));
            }

            phone = parsedPhone.Value;
        }

        var created = Teacher.Create(centreId, command.FullName, phone);
        if (created.IsFailure)
        {
            return Task.FromResult(Result<CreateTeacherResult>.Failure(created.Error!));
        }

        teachers.Add(created.Value);
        return Task.FromResult(Result<CreateTeacherResult>.Success(new CreateTeacherResult(created.Value.Id)));
    }
}
