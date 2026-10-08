using TutoringCentre.Application.Common.Cqrs;

namespace TutoringCentre.Application.Academics.Subjects.Commands.ArchiveSubject;

/// <summary>Intent to take a subject out of use without deleting it, guarded by the version the client last read.</summary>
public sealed record ArchiveSubjectCommand(Guid SubjectId, uint Version) : ICommand<Unit>, ITenantScoped;
