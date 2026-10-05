using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Antiforgery;
using TutoringCentre.Api.Http;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Api.Auth;

/// <summary>
/// Applied once to the whole /api group: every request whose method is not GET, HEAD or OPTIONS must carry a
/// valid antiforgery token, login included — this is what stops login CSRF (an attacker signing a victim into
/// the attacker's own account). Minimal APIs only validate antiforgery automatically for form-bound endpoints;
/// this JSON API needs the explicit check.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the endpoint filter pipeline via DI.")]
internal sealed class AntiforgeryEndpointFilter(IAntiforgery antiforgery) : IEndpointFilter
{
    private static readonly HashSet<string> SafeMethods = new(StringComparer.OrdinalIgnoreCase) { "GET", "HEAD", "OPTIONS" };

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(next);

        var httpContext = context.HttpContext;
        if (SafeMethods.Contains(httpContext.Request.Method))
        {
            return await next(context);
        }

        try
        {
            await antiforgery.ValidateRequestAsync(httpContext);
        }
        catch (AntiforgeryValidationException)
        {
            // No side effects: the endpoint handler never runs.
            return Error.Forbidden("auth.csrf_invalid", "A valid antiforgery token is required for this request.").ToProblemResult();
        }

        return await next(context);
    }
}
