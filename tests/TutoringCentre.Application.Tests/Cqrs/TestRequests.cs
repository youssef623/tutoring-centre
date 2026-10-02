using FluentValidation;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Cqrs;

public enum HandlerMode
{
    Succeed,
    Fail,
    Throw,
}

/// <summary>Tells the test handlers how to behave in the current test.</summary>
public sealed class HandlerBehaviour
{
    public HandlerMode Mode { get; set; }
}

public static class TestErrors
{
    public static readonly Error HandlerFailure = Error.Rule("test.handler_failed", "The handler refused.");
}

public sealed record TestCommand(string Name, string Slug) : ICommand<string>;

public sealed record TestQuery(string Name) : IQuery<string>;

public sealed class TestCommandValidator : AbstractValidator<TestCommand>
{
    public TestCommandValidator()
    {
        RuleFor(command => command.Name).NotEmpty().WithMessage("Name is required.");
        RuleFor(command => command.Name).MinimumLength(3).WithMessage("Name is too short.");
        RuleFor(command => command.Slug).MaximumLength(5).WithMessage("Slug is too long.");
    }
}

public sealed class TestQueryValidator : AbstractValidator<TestQuery>
{
    public TestQueryValidator()
    {
        RuleFor(query => query.Name).NotEmpty();
    }
}

public sealed class TestCommandHandler(FakeUnitOfWork unitOfWork, HandlerBehaviour behaviour)
    : ICommandHandler<TestCommand, string>
{
    public Task<Result<string>> HandleAsync(TestCommand command, CancellationToken cancellationToken)
    {
        unitOfWork.Calls.Add("Handle");
        return behaviour.Mode switch
        {
            HandlerMode.Throw => throw new InvalidOperationException("The handler blew up."),
            HandlerMode.Fail => Task.FromResult(Result<string>.Failure(TestErrors.HandlerFailure)),
            _ => Task.FromResult(Result<string>.Success(command.Name)),
        };
    }
}

public sealed class TestQueryHandler(FakeUnitOfWork unitOfWork, HandlerBehaviour behaviour)
    : IQueryHandler<TestQuery, string>
{
    public Task<Result<string>> HandleAsync(TestQuery query, CancellationToken cancellationToken)
    {
        unitOfWork.Calls.Add("Handle");
        return behaviour.Mode == HandlerMode.Throw
            ? throw new InvalidOperationException("The query handler blew up.")
            : Task.FromResult(Result<string>.Success(query.Name));
    }
}
