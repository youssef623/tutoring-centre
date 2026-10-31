using TutoringCentre.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;
using TutoringCentre.Application.Billing;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Billing;
using TutoringCentre.Infrastructure.Audit;
using TutoringCentre.Infrastructure.Persistence;
namespace TutoringCentre.Infrastructure.Repositories;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI.")]
internal sealed class ChargeRepository(AppDbContext db, ICurrentActor actor, IClock clock, ICorrelationContext correlation) : IChargeRepository
{
    public Task<bool> ExistsForMonthAsync(Guid studentId, Guid subjectId, BillingMonth month, CancellationToken ct) =>
        db.Set<Charge>().AnyAsync(x => x.CentreId == actor.Actor.CentreId && x.StudentId == studentId && x.SubjectId == subjectId && x.BillingMonth == month && x.Kind == ChargeKind.Monthly, ct);
    public void Add(Charge charge) => db.Add(charge);
    public async Task<bool> TryAddMonthlyAsync(Charge charge, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(charge);
        if (actor.Actor.CentreId != charge.CentreId || charge.Kind != ChargeKind.Monthly) throw new InvalidOperationException("A monthly charge in the current centre is required.");
        var tx = db.Database.CurrentTransaction ?? throw new InvalidOperationException("A dispatcher transaction is required.");
        await using var command = new NpgsqlCommand("""
            INSERT INTO billing.charges (centre_id,id,student_id,enrollment_id,group_id,subject_id,teacher_id,kind,billing_month,amount,due_date,created_at,created_by)
            VALUES (@centre,@id,@student,@enrollment,@group,@subject,@teacher,'monthly',@month,@amount,@due,@now,@by)
            ON CONFLICT (centre_id,student_id,subject_id,billing_month) WHERE kind='monthly' DO NOTHING RETURNING id
            """, (NpgsqlConnection)db.Database.GetDbConnection(), (NpgsqlTransaction)tx.GetDbTransaction());
        command.Parameters.AddWithValue("centre", charge.CentreId); command.Parameters.AddWithValue("id", charge.Id);
        command.Parameters.AddWithValue("student", charge.StudentId); command.Parameters.AddWithValue("enrollment", charge.EnrollmentId);
        command.Parameters.AddWithValue("group", charge.GroupId); command.Parameters.AddWithValue("subject", charge.SubjectId);
        command.Parameters.AddWithValue("teacher", charge.TeacherId); command.Parameters.AddWithValue("month", charge.BillingMonth.FirstDay);
        command.Parameters.AddWithValue("amount", charge.Amount.Amount); command.Parameters.AddWithValue("due", charge.DueDate);
        command.Parameters.AddWithValue("now", clock.UtcNow);
        command.Parameters.Add(new NpgsqlParameter("by", NpgsqlTypes.NpgsqlDbType.Uuid) { Value = (object?)charge.CreatedBy ?? DBNull.Value });
        if (await command.ExecuteScalarAsync(ct) is not Guid) return false;
        // ON CONFLICT avoids aborting the ambient transaction; audit only the successful insert in that same transaction.
        var policy = AuditPolicy.Entries[typeof(Charge)];
        var changes = policy.AllowedFields.Select(field => new AuditPropertyChange(field, null, typeof(Charge).GetProperty(field)!.GetValue(charge), false)).ToArray();
        db.Add(new AuditEntry
        {
            Id = Guid.CreateVersion7(), CentreId = charge.CentreId, OccurredAt = clock.UtcNow,
            ActorType = actor.Actor is SystemActor ? AuditActorType.System : AuditActorType.Staff,
            ActorUserId = (actor.Actor as IUserActor)?.UserId, Action = AuditAction.Created, EntityType = AuditEntityType.Charge,
            EntityId = charge.Id, Changes = AuditDiffBuilder.Build(AuditAction.Created, policy.AllowedFields, changes)!, CorrelationId = correlation.CorrelationId
        });
        return true;
    }
}
