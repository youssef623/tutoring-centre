using System.Reflection;
using TutoringCentre.Application.Common.Messaging;

namespace TutoringCentre.Architecture.Tests;

public sealed class OutboxRuleTests
{
    private static readonly Type[] Allowed = [typeof(Guid), typeof(DateOnly), typeof(DateTimeOffset), typeof(int), typeof(bool)];

    [Fact]
    public void Messages_AreSealedRecords_WithIdentifiersAndSimpleValuesOnly()
    {
        var types = SourceAssemblies.All.Append(typeof(OutboxRuleTests).Assembly).SelectMany(assembly => assembly.GetTypes())
            .Where(type => !type.IsInterface && typeof(IOutboxMessage).IsAssignableFrom(type)).ToList();
        Assert.NotEmpty(types);
        foreach (var type in types)
        {
            Assert.True(type.IsClass && type.IsSealed && type.GetMethod("<Clone>$") is not null, type.FullName);
            Assert.All(type.GetProperties(BindingFlags.Public | BindingFlags.Instance), property =>
                Assert.True(Allowed.Contains(property.PropertyType) || property.PropertyType.IsEnum,
                    $"{type.FullName}.{property.Name}: {property.PropertyType.Name} is not an allowed outbox payload value."));
        }
    }

    private sealed record RuleProbeMessage(Guid Id) : IOutboxMessage
    {
        public static string TypeName => "test.payload-rule";
    }
}
