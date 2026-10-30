using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
namespace TutoringCentre.Application.Assistant.Tools;
public sealed class ToolRegistry
{
    private readonly IReadOnlyDictionary<string, ITool> _tools;
    public ToolRegistry(IEnumerable<ITool> tools)
    {
        ArgumentNullException.ThrowIfNull(tools);
        var map = new Dictionary<string, ITool>(StringComparer.Ordinal);
        foreach (var tool in tools)
        {
            if (!Regex.IsMatch(tool.Descriptor.Name, "^[a-z][a-z_]{2,39}$", RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(100)))
                throw new ArgumentException("Invalid tool name.");
            ToolArguments.ValidateSchema(tool.Descriptor.Schema);
            if (!map.TryAdd(tool.Descriptor.Name, tool)) throw new ArgumentException("Duplicate tool name.");
        }
        _tools = map;
    }
    public IReadOnlyList<ToolDescriptor> Descriptors => _tools.Values.Select(x => x.Descriptor).ToArray();
    public ITool? Find(string name) => _tools.GetValueOrDefault(name);
}
public static class ToolRegistration
{
    public static IServiceCollection AddTool<T>(this IServiceCollection services) where T : class, ITool
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddScoped<ITool, T>();
        return services;
    }
}
