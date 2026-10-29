using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TutoringCentre.Application.Identity;
using TutoringCentre.Infrastructure.Jobs;

namespace TutoringCentre.Infrastructure.Identity;

internal sealed class OperatorDirectory(UserManager<ApplicationUser> users, IOptions<JobsOptions> options) : IOperatorDirectory
{
    public async Task<bool> IsOperatorAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = await users.Users.Where(user => user.Id == userId)
            .Select(user => user.NormalizedEmail).SingleOrDefaultAsync(cancellationToken);
        return normalizedEmail is not null && options.Value.OperatorEmails.Any(email =>
            string.Equals(users.NormalizeEmail(email), normalizedEmail, StringComparison.Ordinal));
    }
}
