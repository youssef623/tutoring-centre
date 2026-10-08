using TutoringCentre.Application.Common.Cqrs;

namespace TutoringCentre.Application.Academics.Subjects.Commands.RestoreSubject;

/// <summary>Intent to bring an archived subject back into use, guarded by the version the client last read.</summary>
public sealed record RestoreSubjectCommand(Guid SubjectId, uint Version) : ICommand<Unit>, ITenantScoped;
