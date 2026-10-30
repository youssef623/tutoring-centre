using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data.Common;
using TutoringCentre.Application.Identity;
using TutoringCentre.Application.Identity.Portal;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Identity;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.ReadServices;

/// <summary>
/// Read side of a staff member's own profile, memberships and session state. Non-tracking projections only;
/// every query filters by the given user ID, and profile/active-membership results include only active memberships.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class MembershipReadService(AppDbContext db) : IMembershipReadService
{
    public async Task<StaffProfileDto?> GetProfileAsync(Guid userId, CancellationToken ct)
    {
        var user = await db.Set<ApplicationUser>()
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => new { u.Id, u.DisplayName, u.Email, u.PreferredLocale, u.MustChangePassword })
            .SingleOrDefaultAsync(ct);

        if (user is null)
        {
            return null;
        }

        var memberships = await (
            from membership in db.Set<Membership>().AsNoTracking()
            where membership.UserId == userId && membership.Status == MembershipStatus.Active
            join centre in db.Set<Centre>().AsNoTracking() on membership.CentreId equals centre.Id
            select new MembershipDto(centre.Id, centre.Name, centre.Slug, membership.Role, centre.TimeZoneId))
            .ToListAsync(ct);

        return new StaffProfileDto(user.Id, user.DisplayName, user.Email ?? string.Empty, user.PreferredLocale, memberships, user.MustChangePassword);
    }

    public async Task<ActiveMembershipDto?> GetActiveMembershipAsync(Guid userId, Guid centreId, CancellationToken ct) =>
        await (
            from membership in db.Set<Membership>().AsNoTracking()
            where membership.UserId == userId && membership.CentreId == centreId && membership.Status == MembershipStatus.Active
            join centre in db.Set<Centre>().AsNoTracking() on membership.CentreId equals centre.Id
            select new ActiveMembershipDto(centre.Id, centre.Name, membership.Role))
            .SingleOrDefaultAsync(ct);

    public async Task<SessionStateDto?> GetSessionStateAsync(Guid userId, Guid? centreId, CancellationToken ct)
    {
        var stamp = await db.Set<ApplicationUser>()
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => u.SecurityStamp)
            .SingleOrDefaultAsync(ct);

        if (stamp is null)
        {
            return null;
        }

        if (centreId is null)
        {
            return new SessionStateDto(stamp, MembershipActive: true, CurrentRole: null);
        }

        var membership = await db.Set<Membership>()
            .AsNoTracking()
            .Where(m => m.UserId == userId && m.CentreId == centreId)
            .Select(m => new { m.Status, m.Role })
            .SingleOrDefaultAsync(ct);

        var membershipActive = membership is not null && membership.Status == MembershipStatus.Active;
        var subject = membershipActive && RoleScopes.IsPortalRole(membership!.Role)
            ? await ReadPortalSubjectAsync(userId, centreId.Value, membership.Role, ct) : null;
        return new SessionStateDto(stamp, membershipActive, membership?.Role, subject);
    }

    private async Task<PortalSubject?> ReadPortalSubjectAsync(Guid userId, Guid centreId, StaffRole role, CancellationToken ct)
    {
        // Authentication runs before an actor exists. Only after finding this user's active membership do
        // we bind RLS to that membership's centre. Keep an explicit centre/user/role predicate as well.
        // A direct adapter caller gets a short-lived transaction; the dispatcher normally supplies one.
        await using var ownedTransaction = db.Database.CurrentTransaction is null
            ? await db.Database.BeginTransactionAsync(ct) : null;
        var previousCentre = db.CurrentCentreId?.ToString() ?? string.Empty;
        try
        {
            await db.Database.ExecuteSqlRawAsync("SELECT set_config('app.current_centre', {0}, true)", [centreId.ToString()], ct);
            await using var command = db.Database.GetDbConnection().CreateCommand();
            command.Transaction = db.Database.CurrentTransaction!.GetDbTransaction();
            command.CommandText = """
                SELECT parent_id, student_id FROM identity.portal_links
                WHERE centre_id = @centre AND user_id = @user AND role_key = @role
                """;
            AddParameter(command, "centre", centreId);
            AddParameter(command, "user", userId);
            AddParameter(command, "role", role.ToString().ToLowerInvariant());
            PortalSubject? subject;
            await using (var reader = await command.ExecuteReaderAsync(ct))
            {
                subject = !await reader.ReadAsync(ct) ? null : role == StaffRole.Parent
                    ? new PortalSubject(PortalSubjectKind.Parent, reader.GetGuid(0))
                    : new PortalSubject(PortalSubjectKind.Student, reader.GetGuid(1));
            }
            if (ownedTransaction is not null)
                await ownedTransaction.CommitAsync(ct);
            return subject;
        }
        finally
        {
            if (ownedTransaction is null)
                await db.Database.ExecuteSqlRawAsync("SELECT set_config('app.current_centre', {0}, true)", [previousCentre], CancellationToken.None);
        }
    }

    private static void AddParameter(DbCommand command, string name, object value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value;
        command.Parameters.Add(parameter);
    }

    public Task<bool> MustChangePasswordAsync(Guid userId, CancellationToken ct) =>
        db.Set<ApplicationUser>().AsNoTracking().Where(u => u.Id == userId).Select(u => u.MustChangePassword).SingleOrDefaultAsync(ct);
}
