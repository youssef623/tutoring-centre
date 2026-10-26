using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Groups.Commands.RenameGroup;

/// <summary>
/// Orchestrates: scoped lookup → Domain invariants → set-based uniqueness, excluding the group itself. The same
/// name is a success with no change. The save, not the handler, detects a stale version.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class RenameGroupHandler(IGroupRepository groups) : ICommandHandler<RenameGroupCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(RenameGroupCommand command, CancellationToken cancellationToken)
    {
        var group = await groups.GetByIdAsync(command.GroupId, command.Version, cancellationToken);
        if (group is null)
        {
            return Result<Unit>.Failure(GroupErrors.NotFound());
        }

        var previousNormalizedName = group.NormalizedName;

        var renamed = group.Rename(command.Name);
        if (renamed.IsFailure)
        {
            return Result<Unit>.Failure(renamed.Error!);
        }

        if (group.NormalizedName != previousNormalizedName
            && await groups.NameExistsAsync(group.NormalizedName, group.Id, cancellationToken))
        {
            return Result<Unit>.Failure(GroupErrors.NameTaken());
        }

        return Result<Unit>.Success(Unit.Value);
    }
}
