namespace TutoringCentre.Application.Academics.Enrollments.Commands.TransferEnrollment;

/// <summary>The new enrollment in the target group; the old one has ended.</summary>
public sealed record TransferEnrollmentResult(Guid EnrollmentId);
