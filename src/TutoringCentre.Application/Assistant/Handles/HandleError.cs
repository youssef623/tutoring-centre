using TutoringCentre.Domain.Common;
namespace TutoringCentre.Application.Assistant.Handles;
public static class HandleError
{
    public static Error RawIdRejected => Error.Validation("tool.raw_id_rejected", "Use a listed child handle.");
    public static Error Unknown => Error.Forbidden("tool.handle_unknown", "The child handle is not available in this run.");
}
