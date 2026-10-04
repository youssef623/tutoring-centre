using System.Reflection;
using TutoringCentre.Application.Common.Cqrs;

namespace TutoringCentre.Architecture.Tests;

public sealed class CqrsRuleTests
{
    private static readonly Type[] ApplicationTypes = SourceAssemblies.Application.GetTypes();

    [Fact]
    public void EveryCommandAndQuery_HasExactlyOneHandler()
    {
        var requests = ConcreteClasses().Where(type => Implements(type, typeof(ICommand<>)) || Implements(type, typeof(IQuery<>))).ToList();
        var handlersPerRequest = Handlers()
            .SelectMany(HandledRequests)
            .GroupBy(request => request)
            .ToDictionary(group => group.Key, group => group.Count());

        Assert.NotEmpty(requests);
        var violations = requests
            .Where(request => handlersPerRequest.GetValueOrDefault(request) != 1)
            .Select(request => $"{request.FullName}: {handlersPerRequest.GetValueOrDefault(request)} handlers")
            .ToList();
        Assert.Empty(violations);
    }

    [Fact]
    public void Handlers_AreSealedAndNotPublic()
    {
        var handlers = Handlers().ToList();

        Assert.NotEmpty(handlers);
        var violations = handlers
            .Where(handler => !handler.IsSealed || handler.IsPublic || handler.IsNestedPublic)
            .Select(handler => handler.FullName)
            .ToList();
        Assert.Empty(violations);
    }

    [Fact]
    public void QueryHandlers_DoNotDependOnRepositories()
    {
        var queryHandlers = Handlers().Where(handler => Implements(handler, typeof(IQueryHandler<,>))).ToList();

        Assert.NotEmpty(queryHandlers);
        var violations = queryHandlers
            .SelectMany(handler => handler
                .GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .SelectMany(constructor => constructor.GetParameters())
                .Where(parameter => ArchitectureSupport.IsRepositoryInterface(parameter.ParameterType))
                .Select(parameter => $"{handler.FullName} depends on {parameter.ParameterType.Name}"))
            .ToList();
        Assert.Empty(violations);
    }

    private static IEnumerable<Type> ConcreteClasses() =>
        ApplicationTypes.Where(type => type is { IsClass: true, IsAbstract: false });

    private static IEnumerable<Type> Handlers() =>
        ConcreteClasses().Where(type => Implements(type, typeof(ICommandHandler<,>)) || Implements(type, typeof(IQueryHandler<,>)));

    private static IEnumerable<Type> HandledRequests(Type handler) =>
        handler.GetInterfaces()
            .Where(candidate => candidate.IsGenericType
                && (candidate.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)
                    || candidate.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)))
            .Select(candidate => candidate.GetGenericArguments()[0]);

    private static bool Implements(Type type, Type genericDefinition) =>
        type.GetInterfaces().Any(candidate => candidate.IsGenericType && candidate.GetGenericTypeDefinition() == genericDefinition);
}
