using Microsoft.AspNetCore.Authorization;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Identity;

namespace TutoringCentre.Api.Auth;

public sealed class OperatorRequirement : IAuthorizationRequirement;

/// <summary>Uses the Application port; centre roles and local network addresses confer no access.</summary>
public sealed class OperatorAuthorizationHandler(ICurrentActor currentActor, IOperatorDirectory operators)
    : AuthorizationHandler<OperatorRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OperatorRequirement requirement)
    {
        ArgumentNullException.ThrowIfNull(context);
        if (context.User.Identity?.IsAuthenticated == true && currentActor.Actor is StaffActor staff
            && await operators.IsOperatorAsync(staff.UserId,
                (context.Resource as HttpContext)?.RequestAborted ?? CancellationToken.None))
        {
            context.Succeed(requirement);
        }
    }
}
