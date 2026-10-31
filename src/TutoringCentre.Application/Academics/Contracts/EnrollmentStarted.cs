using TutoringCentre.Application.Common.Messaging;
namespace TutoringCentre.Application.Academics.Contracts;
public sealed record EnrollmentStarted(Guid EnrollmentId) : IOutboxMessage
{
    public static string TypeName => "academics.enrollment-started.v1";
}
