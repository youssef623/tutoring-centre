using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Api.Tests.Fixtures;

internal sealed record ConventionCommand(string Kind) : ICommand<string>;

internal sealed record TestNameCommand(string Name) : ICommand<string>;

internal sealed record TestNameBody(string Name);

internal sealed record SensitiveProbe(string Email, string Password, string PhoneNumber);

/// <summary>Returns the outcome named by the request: one handler, every error kind, plus a throwing case.</summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class ConventionCommandHandler : ICommandHandler<ConventionCommand, string>
{
    public Task<Result<string>> HandleAsync(ConventionCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var result = command.Kind switch
        {
            "ok" => Result<string>.Success("ok"),
            "validation" => Result<string>.Failure(Error.Validation(
                "test.invalid",
                "Invalid.",
                new Dictionary<string, string[]> { ["name"] = ["Name is required."] })),
            "notfound" => Result<string>.Failure(Error.NotFound("test.not_found", "Missing.")),
            "conflict" => Result<string>.Failure(Error.Conflict("test.conflict", "Conflict.")),
            "rule" => Result<string>.Failure(Error.Rule("test.rule", "Rule broken.")),
            "forbidden" => Result<string>.Failure(Error.Forbidden("test.forbidden", "Not allowed.")),
            "throws" => throw new InvalidOperationException("Secret internal detail: Host=db;Password=hunter2"),
            _ => throw new ArgumentOutOfRangeException(nameof(command), command.Kind, "Unknown convention kind."),
        };

        return Task.FromResult(result);
    }
}

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class TestNameHandler : ICommandHandler<TestNameCommand, string>
{
    public Task<Result<string>> HandleAsync(TestNameCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        return Task.FromResult(Result<string>.Success($"Hello {command.Name}"));
    }
}

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class TestNameValidator : AbstractValidator<TestNameCommand>
{
    public TestNameValidator() => RuleFor(command => command.Name).NotEmpty();
}
