using TutoringCentre.Application.Common.Cqrs;

namespace TutoringCentre.Application.Academics.Subjects.Commands.RenameSubject;

/// <summary>Intent to change a subject's name, guarded by the version the client last read. No centre: scope comes from the actor.</summary>
public sealed record RenameSubjectCommand(Guid SubjectId, string Name, uint Version) : ICommand<Unit>, ITenantScoped;
