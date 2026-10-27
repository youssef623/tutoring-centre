# `tests/TutoringCentre.Api.Tests/Fixtures/ConventionsFactory.cs`

## Purpose

Source summary (intent, not independent execution proof): The real API plus test-only endpoints/handlers and an in-memory log sink. Own container, own collection.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ConventionsFactory.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 29 lines and 1,221 bytes. Source hash: `7559ef33178d94dcedcb74b92e7ee359a8f9807dc7c1821d45941eb72e87329d`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using FluentValidation;`
- Line 2: `using Microsoft.AspNetCore.Hosting;`
- Line 3: `using Microsoft.AspNetCore.TestHost;`
- Line 4: `using Microsoft.Extensions.DependencyInjection;`
- Line 5: `using Serilog.Core;`
- Line 6: `using TutoringCentre.Application.Common.Cqrs;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 11](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ConventionsFactory.cs#L11): `public sealed class ConventionsFactory : ApiFactory`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Api.Tests/Auth/SecurityTests.cs:24](../../../../../tests/TutoringCentre.Api.Tests/Auth/SecurityTests.cs#L24)
- [tests/TutoringCentre.Api.Tests/Fixtures/ConventionsCollection.cs:11](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/ConventionsCollection.cs#L11)
- [tests/TutoringCentre.Api.Tests/Http/CorrelationIdTests.cs:7](../../../../../tests/TutoringCentre.Api.Tests/Http/CorrelationIdTests.cs#L7)
- [tests/TutoringCentre.Api.Tests/Http/LoggingConventionTests.cs:7](../../../../../tests/TutoringCentre.Api.Tests/Http/LoggingConventionTests.cs#L7)
- [tests/TutoringCentre.Api.Tests/Http/ProblemDetailsTests.cs:9](../../../../../tests/TutoringCentre.Api.Tests/Http/ProblemDetailsTests.cs#L9)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Core;
using TutoringCentre.Application.Common.Cqrs;

namespace TutoringCentre.Api.Tests.Fixtures;

/// <summary>The real API plus test-only endpoints/handlers and an in-memory log sink. Own container, own collection.</summary>
public sealed class ConventionsFactory : ApiFactory
{
    public InMemoryLogSink Logs { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureTestServices(services =>
        {
            services.AddScoped<ICommandHandler<ConventionCommand, string>, ConventionCommandHandler>();
            services.AddScoped<ICommandHandler<TestNameCommand, string>, TestNameHandler>();
            services.AddScoped<IValidator<TestNameCommand>, TestNameValidator>();
            services.AddScoped<IQueryHandler<CurrentActorQuery, CurrentActorDto>, CurrentActorQueryHandler>();
            services.AddSingleton<ILogEventSink>(Logs);
            services.AddSingleton<IStartupFilter, ConventionEndpointsStartupFilter>();
        });
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
