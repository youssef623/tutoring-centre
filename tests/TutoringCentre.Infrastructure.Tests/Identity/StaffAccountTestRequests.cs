using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Staff;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Infrastructure.Tests.Identity;

/// <summary>
/// Task 29.4: a test-only command that ensures an account exists, then always fails — standing in for
/// CreateStaffCommand (Day 29.5) so the rollback test can prove a new account does not survive a command that
/// fails afterward, before that command exists.
/// </summary>
internal sealed record EnsureAccountThenFailCommand(string Email, string DisplayName, string PreferredLocale) : ICommand<Unit>;

internal sealed class EnsureAccountThenFailCommandHandler(IStaffAccountService accountService) : ICommandHandler<EnsureAccountThenFailCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(EnsureAccountThenFailCommand command, CancellationToken cancellationToken)
    {
        var accountResult = await accountService.EnsureAccountAsync(command.Email, command.DisplayName, command.PreferredLocale, cancellationToken);
        if (accountResult.IsFailure)
        {
            return Result<Unit>.Failure(accountResult.Error!);
        }

        return Result<Unit>.Failure(Error.Rule("test.deliberate_failure", "Always fails, after the account was ensured."));
    }
}
