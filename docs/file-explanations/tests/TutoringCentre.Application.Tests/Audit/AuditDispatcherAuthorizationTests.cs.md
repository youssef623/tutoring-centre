# `tests/TutoringCentre.Application.Tests/Audit/AuditDispatcherAuthorizationTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Through the real dispatcher: Secretary and Teacher are refused (auth.permission_denied), and a centre-less actor is refused before permission is even considered (tenant.not_selected) — the same two pipeline guarantees proved for Staff and Subjects.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Audit/AuditDispatcherAuthorizationTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 76 lines and 3,175 bytes. Source hash: `2099c3bf2e614924a98c440b2018b24500d33e40993979d9421e1945ad090aab`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using FluentValidation;`
- Line 2: `using Microsoft.Extensions.DependencyInjection;`
- Line 3: `using Microsoft.Extensions.Logging.Abstractions;`
- Line 4: `using TutoringCentre.Application.Audit;`
- Line 5: `using TutoringCentre.Application.Audit.Queries.GetAuditLog;`
- Line 6: `using TutoringCentre.Application.Common.Cqrs;`
- Line 7: `using TutoringCentre.Application.Common.Security;`
- Line 8: `using TutoringCentre.Application.Tests.Fakes;`
- Line 9: `using TutoringCentre.Domain.Common;`
- Line 10: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 19](../../../../../tests/TutoringCentre.Application.Tests/Audit/AuditDispatcherAuthorizationTests.cs#L19): `public sealed class AuditDispatcherAuthorizationTests`
- [Line 26](../../../../../tests/TutoringCentre.Application.Tests/Audit/AuditDispatcherAuthorizationTests.cs#L26): `public async Task QueryAsync_GetAuditLog_ReturnsPermissionDeniedForSecretaryAndTeacher(StaffRole role)`
- [Line 39](../../../../../tests/TutoringCentre.Application.Tests/Audit/AuditDispatcherAuthorizationTests.cs#L39): `public async Task QueryAsync_GetAuditLog_CentrelessActor_ReturnsTenantNotSelected()`
- [Line 51](../../../../../tests/TutoringCentre.Application.Tests/Audit/AuditDispatcherAuthorizationTests.cs#L51): `private static Fixture CreateSut(StaffRole? role)`
- [Line 70](../../../../../tests/TutoringCentre.Application.Tests/Audit/AuditDispatcherAuthorizationTests.cs#L70): `private sealed class Fixture(Dispatcher dispatcher, ServiceProvider provider) : IDisposable`
- [Line 74](../../../../../tests/TutoringCentre.Application.Tests/Audit/AuditDispatcherAuthorizationTests.cs#L74): `public void Dispose() => provider.Dispose();`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using TutoringCentre.Application.Audit;
using TutoringCentre.Application.Audit.Queries.GetAuditLog;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Audit;

/// <summary>
/// Through the real dispatcher: Secretary and Teacher are refused (auth.permission_denied), and a centre-less
/// actor is refused before permission is even considered (tenant.not_selected) — the same two pipeline
/// guarantees proved for Staff and Subjects.
/// </summary>
public sealed class AuditDispatcherAuthorizationTests
{
    private static readonly Guid CentreId = Guid.CreateVersion7();

    [Theory]
    [InlineData(StaffRole.Secretary)]
    [InlineData(StaffRole.Teacher)]
    public async Task QueryAsync_GetAuditLog_ReturnsPermissionDeniedForSecretaryAndTeacher(StaffRole role)
    {
        using var fixture = CreateSut(role);

        var result = await fixture.Dispatcher.QueryAsync<GetAuditLogQuery, AuditPageDto>(
            new GetAuditLogQuery(null, null, null, null, null, null, 50), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("auth.permission_denied", result.Error!.Code);
        Assert.Equal(ErrorKind.Forbidden, result.Error.Kind);
    }

    [Fact]
    public async Task QueryAsync_GetAuditLog_CentrelessActor_ReturnsTenantNotSelected()
    {
        using var fixture = CreateSut(role: null);

        var result = await fixture.Dispatcher.QueryAsync<GetAuditLogQuery, AuditPageDto>(
            new GetAuditLogQuery(null, null, null, null, null, null, 50), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("tenant.not_selected", result.Error!.Code);
        Assert.Equal(ErrorKind.Forbidden, result.Error.Kind);
    }

    private static Fixture CreateSut(StaffRole? role)
    {
        var actor = role is null
            ? new StaffActor(Guid.NewGuid(), CentreId: null, Role: null)
            : new StaffActor(Guid.NewGuid(), CentreId, role);
        var currentActor = new FakeCurrentActor { Actor = actor };

        var provider = new ServiceCollection()
            .AddSingleton(new FakeUnitOfWork())
            .AddSingleton<ICurrentActor>(currentActor)
            .AddSingleton<IAuditReadService>(new FakeAuditReadService())
            .AddScoped<IQueryHandler<GetAuditLogQuery, AuditPageDto>, GetAuditLogHandler>()
            .AddScoped<IValidator<GetAuditLogQuery>, GetAuditLogValidator>()
            .BuildServiceProvider();

        var unitOfWork = provider.GetRequiredService<FakeUnitOfWork>();
        return new Fixture(new Dispatcher(provider, unitOfWork, NullLogger<Dispatcher>.Instance), provider);
    }

    private sealed class Fixture(Dispatcher dispatcher, ServiceProvider provider) : IDisposable
    {
        public Dispatcher Dispatcher { get; } = dispatcher;

        public void Dispose() => provider.Dispose();
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
