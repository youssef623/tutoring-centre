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

/// <summary>Change-password is not a CQRS command either (ADR 0005), for the same reason login is not.</summary>
public sealed record ChangePasswordRequest(string CurrentPassword, string NewPassword);

/// <summary>Shape checks only; the password policy itself is enforced by Identity inside the authentication port.</summary>
internal sealed class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    private const int PasswordMaxLength = 128;

    public ChangePasswordRequestValidator()
    {
        RuleFor(request => request.CurrentPassword).NotEmpty().MaximumLength(PasswordMaxLength);
        RuleFor(request => request.NewPassword).NotEmpty().MaximumLength(PasswordMaxLength);
    }
}
