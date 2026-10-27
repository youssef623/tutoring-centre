# `tests/TutoringCentre.Architecture.Tests/CqrsRuleTests.cs`

## Purpose

Declares `CqrsRuleTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../tests/TutoringCentre.Architecture.Tests/CqrsRuleTests.cs). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 71 lines and 3,082 bytes. Source hash: `c612794effe935ed95f1c8a44f996b28a0b3e53d25857ee92dc299003e359eb2`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Reflection;`
- Line 2: `using TutoringCentre.Application.Common.Cqrs;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../tests/TutoringCentre.Architecture.Tests/CqrsRuleTests.cs#L6): `public sealed class CqrsRuleTests`
- [Line 11](../../../../tests/TutoringCentre.Architecture.Tests/CqrsRuleTests.cs#L11): `public void EveryCommandAndQuery_HasExactlyOneHandler()`
- [Line 28](../../../../tests/TutoringCentre.Architecture.Tests/CqrsRuleTests.cs#L28): `public void Handlers_AreSealedAndNotPublic()`
- [Line 41](../../../../tests/TutoringCentre.Architecture.Tests/CqrsRuleTests.cs#L41): `public void QueryHandlers_DoNotDependOnRepositories()`
- [Line 56](../../../../tests/TutoringCentre.Architecture.Tests/CqrsRuleTests.cs#L56): `private static IEnumerable<Type> ConcreteClasses() =>`
- [Line 59](../../../../tests/TutoringCentre.Architecture.Tests/CqrsRuleTests.cs#L59): `private static IEnumerable<Type> Handlers() =>`
- [Line 62](../../../../tests/TutoringCentre.Architecture.Tests/CqrsRuleTests.cs#L62): `private static IEnumerable<Type> HandledRequests(Type handler) =>`
- [Line 69](../../../../tests/TutoringCentre.Architecture.Tests/CqrsRuleTests.cs#L69): `private static bool Implements(Type type, Type genericDefinition) =>`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
