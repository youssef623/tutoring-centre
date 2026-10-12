using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Students.Commands.RegisterStudent;

/// <summary>
/// Intent to create a student with their first parent, in the acting actor's own centre, in one transaction.
/// <see cref="Parent"/> carries exactly one of two forms: an existing parent's id, or a new parent's name and
/// phone. The phone fields are raw input here; PhoneNumber.Create parses each once, in the handler.
/// </summary>
public sealed record RegisterStudentCommand(
    string FullName, string? Phone, string? SchoolYear, RegisterStudentParent Parent, ParentRelationship Relationship)
    : ICommand<RegisterStudentResult>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.StudentsManage;
}

/// <summary>Exactly one of <see cref="ExistingParentId"/> alone, or <see cref="FullName"/> and <see cref="Phone"/> together.</summary>
public sealed record RegisterStudentParent(Guid? ExistingParentId, string? FullName, string? Phone);
