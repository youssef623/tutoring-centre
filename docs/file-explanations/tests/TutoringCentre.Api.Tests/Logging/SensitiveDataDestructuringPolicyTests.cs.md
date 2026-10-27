# `tests/TutoringCentre.Api.Tests/Logging/SensitiveDataDestructuringPolicyTests.cs`

## Purpose

Declares `SensitiveDataDestructuringPolicyTests`, `Credentials`, `DelegateSink`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Logging/SensitiveDataDestructuringPolicyTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 60 lines and 2,255 bytes. Source hash: `2e1c88494f3bb28bf973c4e1df0fea454b1f9904ad93daa4ab62a43b8423c732`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Serilog;`
- Line 2: `using Serilog.Events;`
- Line 3: `using TutoringCentre.Api.Logging;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../tests/TutoringCentre.Api.Tests/Logging/SensitiveDataDestructuringPolicyTests.cs#L7): `public sealed class SensitiveDataDestructuringPolicyTests`
- [Line 9](../../../../../tests/TutoringCentre.Api.Tests/Logging/SensitiveDataDestructuringPolicyTests.cs#L9): `private sealed record Credentials(string Email, string Password, string ApiToken, string PhoneNumber);`
- [Line 12](../../../../../tests/TutoringCentre.Api.Tests/Logging/SensitiveDataDestructuringPolicyTests.cs#L12): `public void Destructure_MasksPasswordTokenAndPhoneProperties()`
- [Line 23](../../../../../tests/TutoringCentre.Api.Tests/Logging/SensitiveDataDestructuringPolicyTests.cs#L23): `public void Destructure_LeavesNonSensitivePropertiesIntact()`
- [Line 32](../../../../../tests/TutoringCentre.Api.Tests/Logging/SensitiveDataDestructuringPolicyTests.cs#L32): `public void Destructure_ObjectWithNoSensitiveProperties_IsUnaffected()`
- [Line 40](../../../../../tests/TutoringCentre.Api.Tests/Logging/SensitiveDataDestructuringPolicyTests.cs#L40): `private static LogEvent CaptureDestructured(object value)`
- [Line 53](../../../../../tests/TutoringCentre.Api.Tests/Logging/SensitiveDataDestructuringPolicyTests.cs#L53): `private static string? Scalar(StructureValue structure, string propertyName) =>`
- [Line 56](../../../../../tests/TutoringCentre.Api.Tests/Logging/SensitiveDataDestructuringPolicyTests.cs#L56): `private sealed class DelegateSink(Action<LogEvent> onEmit) : Serilog.Core.ILogEventSink`
- [Line 58](../../../../../tests/TutoringCentre.Api.Tests/Logging/SensitiveDataDestructuringPolicyTests.cs#L58): `public void Emit(LogEvent logEvent) => onEmit(logEvent);`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
