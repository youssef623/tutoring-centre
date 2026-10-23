using FluentValidation;

namespace TutoringCentre.Application.Academics.Groups.Commands.ArchiveGroup;

internal sealed class ArchiveGroupValidator : AbstractValidator<ArchiveGroupCommand>
{
    public ArchiveGroupValidator()
    {
        RuleFor(command => command.GroupId).NotEmpty();
    }
}
