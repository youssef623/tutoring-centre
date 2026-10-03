using Serilog;
using Serilog.Events;
using TutoringCentre.Api.Logging;

namespace TutoringCentre.Api.Tests.Logging;

public sealed class SensitiveDataDestructuringPolicyTests
{
    private sealed record Credentials(string Email, string Password, string ApiToken, string PhoneNumber);

    [Fact]
    public void Destructure_MasksPasswordTokenAndPhoneProperties()
    {
        var logEvent = CaptureDestructured(new Credentials("sara@example.test", "hunter2", "abc-secret", "+201001234567"));

        var structure = Assert.IsType<StructureValue>(logEvent.Properties["Request"]);
        Assert.Equal("***", Scalar(structure, "Password"));
        Assert.Equal("***", Scalar(structure, "ApiToken"));
        Assert.Equal("***", Scalar(structure, "PhoneNumber"));
    }

    [Fact]
    public void Destructure_LeavesNonSensitivePropertiesIntact()
    {
        var logEvent = CaptureDestructured(new Credentials("sara@example.test", "hunter2", "abc-secret", "+201001234567"));

        var structure = Assert.IsType<StructureValue>(logEvent.Properties["Request"]);
        Assert.Equal("sara@example.test", Scalar(structure, "Email"));
    }

    [Fact]
    public void Destructure_ObjectWithNoSensitiveProperties_IsUnaffected()
    {
        var logEvent = CaptureDestructured(new { Name = "Sara", Age = 30 });

        var structure = Assert.IsType<StructureValue>(logEvent.Properties["Request"]);
        Assert.Equal("Sara", Scalar(structure, "Name"));
    }

    private static LogEvent CaptureDestructured(object value)
    {
        var events = new List<LogEvent>();
        using var logger = new LoggerConfiguration()
            .Destructure.With<SensitiveDataDestructuringPolicy>()
            .WriteTo.Sink(new DelegateSink(events.Add))
            .CreateLogger();

        logger.Information("Probe {@Request}", value);

        return events.Single();
    }

    private static string? Scalar(StructureValue structure, string propertyName) =>
        structure.Properties.Single(p => p.Name == propertyName).Value is ScalarValue { Value: string text } ? text : null;

    private sealed class DelegateSink(Action<LogEvent> onEmit) : Serilog.Core.ILogEventSink
    {
        public void Emit(LogEvent logEvent) => onEmit(logEvent);
    }
}
