using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Academics.Subjects;
using TutoringCentre.Application.Academics.Teachers;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Groups.Commands.CreateGroup;

/// <summary>
/// Orchestrates: resolve the subject, then the teacher, inside the acting centre → Domain invariants →
/// set-based name uniqueness → add. Another centre's subject or teacher is simply not found here, so the
/// composite foreign keys are a backstop and never the source of the answer.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class CreateGroupHandler(
    ICurrentActor currentActor,
    IGroupRepository groups,
    ISubjectRepository subjects,
    ITeacherRepository teachers)
    : ICommandHandler<CreateGroupCommand, CreateGroupResult>
{
    public async Task<Result<CreateGroupResult>> HandleAsync(CreateGroupCommand command, CancellationToken cancellationToken)
    {
        // ITenantScoped guarantees the dispatcher already refused any actor without a centre.
        var centreId = currentActor.Actor.CentreId!.Value;

        var subject = await subjects.FindAsync(command.SubjectId, cancellationToken);
        if (subject is null)
        {
            return Result<CreateGroupResult>.Failure(GroupErrors.SubjectNotFound());
        }

        if (subject.Status == SubjectStatus.Archived)
        {
            return Result<CreateGroupResult>.Failure(GroupErrors.SubjectArchived());
        }

        var teacher = await teachers.FindAsync(command.TeacherId, cancellationToken);
        if (teacher is null)
        {
            return Result<CreateGroupResult>.Failure(GroupErrors.TeacherNotFound());
        }

        if (teacher.Status == TeacherStatus.Archived)
        {
            return Result<CreateGroupResult>.Failure(GroupErrors.TeacherArchived());
        }

        var created = Group.Create(centreId, command.Name, subject.Id, teacher.Id, command.Capacity);
        if (created.IsFailure)
        {
            return Result<CreateGroupResult>.Failure(created.Error!);
        }

        if (await groups.NameExistsAsync(created.Value.NormalizedName, excludingId: null, cancellationToken))
        {
            return Result<CreateGroupResult>.Failure(GroupErrors.NameTaken());
        }

        groups.Add(created.Value);
        return Result<CreateGroupResult>.Success(new CreateGroupResult(created.Value.Id));
    }
}
