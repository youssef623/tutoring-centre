using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Billing;

public sealed class Charge : Entity, ITenantOwned
{
    private Charge() { }
    public Guid CentreId { get; private set; }
    public Guid StudentId { get; private set; }
    public Guid EnrollmentId { get; private set; }
    public Guid GroupId { get; private set; }
    public Guid SubjectId { get; private set; }
    public Guid TeacherId { get; private set; }
    public ChargeKind Kind { get; private set; }
    public BillingMonth BillingMonth { get; private set; }
    public Money Amount { get; private set; }
    public DateOnly DueDate { get; private set; }
    public string? Reason { get; private set; }
    public Guid? CreatedBy { get; private set; }

    public static Result<Charge> CreateMonthly(Guid centreId, Guid studentId, Guid enrollmentId, Guid groupId,
        Guid subjectId, Guid teacherId, BillingMonth month, Money fee, Guid? createdBy = null)
    {
        if (centreId == Guid.Empty || studentId == Guid.Empty || enrollmentId == Guid.Empty || groupId == Guid.Empty || subjectId == Guid.Empty || teacherId == Guid.Empty)
            return Result<Charge>.Failure(Error.Validation("billing.identifiers_required", "Charge identifiers are required."));
        if (fee.Amount <= 0)
            return Result<Charge>.Failure(Error.Validation("billing.charge_amount_invalid", "A monthly charge must be positive."));
        return Result<Charge>.Success(new()
        {
            CentreId = centreId, StudentId = studentId, EnrollmentId = enrollmentId, GroupId = groupId,
            SubjectId = subjectId, TeacherId = teacherId, Kind = ChargeKind.Monthly, BillingMonth = month,
            Amount = fee, DueDate = BillingCalendar.DueDate(month), CreatedBy = createdBy
        });
    }
}
