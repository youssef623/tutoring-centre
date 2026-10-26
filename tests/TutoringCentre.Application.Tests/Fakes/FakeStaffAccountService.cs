using TutoringCentre.Application.Staff;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>In-memory stand-in for account creation. <see cref="ExistingAccounts"/> seeds accounts that already exist (normalized email → user ID); any other email is "created" with a fake temporary password.</summary>
public sealed class FakeStaffAccountService : IStaffAccountService
{
    public Dictionary<string, Guid> ExistingAccounts { get; } = new(StringComparer.OrdinalIgnoreCase);

    public List<Guid> CreatedUserIds { get; } = [];

    public Task<Result<EnsureStaffAccountResult>> EnsureAccountAsync(string email, string displayName, string preferredLocale, CancellationToken ct)
    {
        if (ExistingAccounts.TryGetValue(email, out var existingUserId))
        {
            return Task.FromResult(Result<EnsureStaffAccountResult>.Success(
                new EnsureStaffAccountResult(existingUserId, Created: false, TemporaryPassword: null)));
        }

        var newUserId = Guid.CreateVersion7();
        ExistingAccounts[email] = newUserId;
        CreatedUserIds.Add(newUserId);
        return Task.FromResult(Result<EnsureStaffAccountResult>.Success(
            new EnsureStaffAccountResult(newUserId, Created: true, TemporaryPassword: "fake-temporary-password-1")));
    }
}
