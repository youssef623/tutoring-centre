using FluentValidation;

namespace TutoringCentre.Api.Auth;

/// <summary>Login is not a CQRS command: the request model and its validator live with the endpoint, not Application.</summary>
public sealed record LoginRequest(string Email, string Password);

/// <summary>Shape checks only; login accepts whatever password was set, no composition rules here.</summary>
internal sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    private const int EmailMaxLength = 256;
    private const int PasswordMaxLength = 128;

    public LoginRequestValidator()
    {
        RuleFor(request => request.Email).NotEmpty().EmailAddress().MaximumLength(EmailMaxLength);
        RuleFor(request => request.Password).NotEmpty().MaximumLength(PasswordMaxLength);
    }
}
