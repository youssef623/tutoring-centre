using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Mono.Cecil;
using TutoringCentre.Application;
using TutoringCentre.Application.Assistant.Tools;
namespace TutoringCentre.Architecture.Tests;

public sealed class ToolRuleTests
{
    private static IEnumerable<TypeDefinition> AllTypes(ModuleDefinition module) => module.Types.SelectMany(Flatten);
    private static IEnumerable<TypeDefinition> Flatten(TypeDefinition type) => new[] { type }.Concat(type.NestedTypes.SelectMany(Flatten));
    internal static bool ForbiddenExecution(TypeDefinition caller, MethodReference method) =>
        method.Name == nameof(ITool.ExecuteAsync) && (method.DeclaringType.FullName == typeof(ITool).FullName
            || SourceAssemblies.All.SelectMany(a=>a.GetTypes()).Any(t=>typeof(ITool).IsAssignableFrom(t) && t.FullName?.Replace('+','/')==method.DeclaringType.FullName))
        && !Root(caller).FullName.Equals(typeof(ToolGateway).FullName, StringComparison.Ordinal);
    private static TypeDefinition Root(TypeDefinition type) => type.DeclaringType is null ? type : Root(type.DeclaringType);

    [Fact] public void OnlyGatewayCallsExecuteAsync()
    {
        foreach(var assembly in SourceAssemblies.All)
        {
            using var module=ModuleDefinition.ReadModule(assembly.Location);
            Assert.Empty(AllTypes(module).SelectMany(t=>t.Methods.Where(m=>m.HasBody).SelectMany(m=>m.Body.Instructions)
                .Where(i=>i.Operand is MethodReference method && ForbiddenExecution(t,method)).Select(_=>t.FullName)));
        }
    }
    internal static bool AllowedToolDependency(Type type) => !type.Name.EndsWith("Repository",StringComparison.Ordinal)
        && !type.Name.EndsWith("ReadService",StringComparison.Ordinal) && (type.Namespace is null
        || !type.Namespace.StartsWith("TutoringCentre.",StringComparison.Ordinal)
        || type == typeof(TutoringCentre.Application.Common.Cqrs.Dispatcher)
        || type == typeof(ITool) || type == typeof(ToolDescriptor) || type == typeof(ToolContext)
        || type == typeof(ToolArguments) || type == typeof(ToolResult)
        || type == typeof(TutoringCentre.Application.Assistant.Handles.HandleTable)
        || type == typeof(TutoringCentre.Application.Assistant.Handles.HandleError)
        || type.Namespace.Contains(".Contracts",StringComparison.Ordinal)
        || type.Namespace.StartsWith("TutoringCentre.Domain.Common",StringComparison.Ordinal)
        || (type.Namespace.StartsWith("TutoringCentre.Application.",StringComparison.Ordinal)
            && !type.GetInterfaces().Any(i=>i.IsGenericType && i.GetGenericTypeDefinition()==typeof(TutoringCentre.Application.Common.Cqrs.ICommand<>))
            && (type.GetMethod("<Clone>$") is not null || type.GetInterfaces().Any(i=>i.IsGenericType && i.GetGenericTypeDefinition()==typeof(TutoringCentre.Application.Common.Cqrs.IQuery<>)))));

    [Fact] public void ToolsDependOnlyOnDispatcherContractsAndModels()
    {
        var tools=SourceAssemblies.All.SelectMany(a=>a.GetTypes()).Where(t=>t.IsClass && typeof(ITool).IsAssignableFrom(t)).ToArray();
        var forbidden=SourceAssemblies.All.SelectMany(a=>a.GetTypes()).Where(t=>!AllowedToolDependency(t)).Select(t=>t.FullName!).ToArray();
        foreach(var tool in tools)
        {
            var check=NetArchTest.Rules.Types.InAssembly(tool.Assembly).That().HaveName(tool.Name).ShouldNot().HaveDependencyOnAny(forbidden).GetResult();
            Assert.True(check.IsSuccessful,ArchitectureSupport.Describe(check));
            var storage=NetArchTest.Rules.Types.InAssembly(tool.Assembly).That().HaveName(tool.Name)
                .ShouldNot().HaveDependencyOnAny("Microsoft.EntityFrameworkCore", "Npgsql", "System.Data").GetResult();
            Assert.True(storage.IsSuccessful,ArchitectureSupport.Describe(storage));
        }
    }
    internal static bool SchemaHasIdentifier(JsonElement schema)
    {
        if(!schema.TryGetProperty("properties",out var properties)) return false;
        return properties.EnumerateObject().Any(p=>p.Name.EndsWith("Id",StringComparison.OrdinalIgnoreCase)
            || p.Name.EndsWith("Ids",StringComparison.OrdinalIgnoreCase) || SchemaHasIdentifier(p.Value));
    }
    [Fact] public async Task RegisteredToolSchemasNeverAcceptIdentifiers()
    {
        var services=new ServiceCollection().AddApplication();
        TutoringCentre.Infrastructure.DependencyInjection.AddInfrastructure(services,new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string,string?> { ["ConnectionStrings:Postgres"]="Host=localhost;Database=architecture;Username=architecture" }).Build());
        await using var provider=services.BuildServiceProvider();
        await using var scope=provider.CreateAsyncScope();
        Assert.All(scope.ServiceProvider.GetRequiredService<ToolRegistry>().Descriptors,d=>Assert.False(SchemaHasIdentifier(d.Schema)));
        foreach(var type in SourceAssemblies.All.SelectMany(a=>a.GetTypes()).Where(t=>t.IsClass && !t.IsAbstract && typeof(ITool).IsAssignableFrom(t)))
            Assert.False(SchemaHasIdentifier(((ITool)ActivatorUtilities.CreateInstance(scope.ServiceProvider,type)).Descriptor.Schema));
    }
    [Fact] public void EveryProductionToolHasExplicitAddToolRegistration()
    {
        var registered=new HashSet<string>(StringComparer.Ordinal);
        foreach(var assembly in SourceAssemblies.All)
        {
            using var module=ModuleDefinition.ReadModule(assembly.Location);
            foreach(var method in AllTypes(module).SelectMany(t=>t.Methods).Where(m=>m.HasBody))
                foreach(var instruction in method.Body.Instructions)
                    if(instruction.Operand is GenericInstanceMethod generic && generic.Name=="AddTool" && generic.DeclaringType.FullName==typeof(ToolRegistration).FullName)
                        registered.Add(generic.GenericArguments[0].FullName);
        }
        Assert.All(SourceAssemblies.All.SelectMany(a=>a.GetTypes()).Where(t=>t.IsClass && !t.IsAbstract && typeof(ITool).IsAssignableFrom(t)),t=>Assert.Contains(t.FullName!.Replace('+','/'),registered));
    }
    [Fact] public void AssistantReachesOtherModulesOnlyThroughContractsOrDispatcherRequests()
    {
        var forbidden=SourceAssemblies.Application.GetTypes().Where(t=>t.Namespace is { } ns
            && ns.StartsWith("TutoringCentre.Application.",StringComparison.Ordinal)
            && !ns.StartsWith("TutoringCentre.Application.Assistant",StringComparison.Ordinal)
            && !ns.StartsWith("TutoringCentre.Application.Common",StringComparison.Ordinal)
            && !ns.Contains(".Contracts",StringComparison.Ordinal)
            && !t.GetInterfaces().Any(i=>i.IsGenericType && i.GetGenericTypeDefinition()==typeof(TutoringCentre.Application.Common.Cqrs.IQuery<>)))
            .Select(t=>t.FullName!).ToArray();
        var result=NetArchTest.Rules.Types.InAssembly(SourceAssemblies.Application).That().ResideInNamespaceStartingWith("TutoringCentre.Application.Assistant").ShouldNot().HaveDependencyOnAny(forbidden).GetResult();
        Assert.True(result.IsSuccessful,ArchitectureSupport.Describe(result));
    }
    [Fact] public void RulePredicatesRejectDeliberateViolations()
    {
        using var module=ModuleDefinition.CreateModule("Violation",ModuleKind.Dll);
        var fake=new TypeDefinition("Unexpected.Namespace","FakeTool",TypeAttributes.Class,module.TypeSystem.Object);
        Assert.True(ForbiddenExecution(fake,module.ImportReference(typeof(ITool).GetMethod("ExecuteAsync")!)));
        Assert.False(AllowedToolDependency(typeof(TutoringCentre.Infrastructure.Persistence.AppDbContext)));
        Assert.False(AllowedToolDependency(typeof(TutoringCentre.Application.Academics.Students.IStudentRepository)));
        Assert.True(SchemaHasIdentifier(JsonDocument.Parse("""{"properties":{"studentId":{"type":"string"}}}""").RootElement));
    }
}
