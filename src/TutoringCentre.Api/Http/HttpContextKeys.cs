namespace TutoringCentre.Api.Http;

/// <summary>Keys for per-request values stored in <see cref="HttpContext.Items"/>.</summary>
internal static class HttpContextKeys
{
    public const string CorrelationId = "CorrelationId";
}
