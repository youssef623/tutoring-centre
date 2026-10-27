# `tests/TutoringCentre.Application.Tests/Centres/CentreSettingsDispatcherAuthorizationTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Through the real dispatcher: Secretary and Teacher are refused both centre-settings requests (auth.permission_denied), and a centre-less actor is refused before permission is even considered (tenant.not_selected) — the same two pipeline guarantees proved for Audit, Staff and Subjects.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Centres/CentreSettingsDispatcherAuthorizationTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 114 lines and 4,684 bytes. Source hash: `2500cbd339c97dfbb73318835d10a685d60fe1201f2f9fe8cc5bc64c4becc1ec`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using FluentValidation;`
- Line 2: `using Microsoft.Extensions.DependencyInjection;`
- Line 3: `using Microsoft.Extensions.Logging.Abstractions;`
- Line 4: `using TutoringCentre.Application.Centres;`
- Line 5: `using TutoringCentre.Application.Centres.Commands.UpdateCentreSettings;`
- Line 6: `using TutoringCentre.Application.Centres.Queries.GetCentreSettings;`
- Line 7: `using TutoringCentre.Application.Common.Cqrs;`
- Line 8: `using TutoringCentre.Application.Common.Security;`
- Line 9: `using TutoringCentre.Application.Tests.Fakes;`
- Line 10: `using TutoringCentre.Domain.Centres;`
- Line 11: `using TutoringCentre.Domain.Common;`
- Line 12: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 21](../../../../../tests/TutoringCentre.Application.Tests/Centres/CentreSettingsDispatcherAuthorizationTests.cs#L21): `public sealed class CentreSettingsDispatcherAuthorizationTests`
- [Line 28](../../../../../tests/TutoringCentre.Application.Tests/Centres/CentreSettingsDispatcherAuthorizationTests.cs#L28): `public async Task QueryAsync_GetCentreSettings_ReturnsPermissionDeniedForSecretaryAndTeacher(StaffRole role)`
- [Line 41](../../../../../tests/TutoringCentre.Application.Tests/Centres/CentreSettingsDispatcherAuthorizationTests.cs#L41): `public async Task SendAsync_UpdateCentreSettings_ReturnsPermissionDeniedForSecretaryAndTeacher(StaffRole role)`
- [Line 52](../../../../../tests/TutoringCentre.Application.Tests/Centres/CentreSettingsDispatcherAuthorizationTests.cs#L52): `public async Task QueryAsync_GetCentreSettings_CentrelessActor_ReturnsTenantNotSelected()`
- [Line 63](../../../../../tests/TutoringCentre.Application.Tests/Centres/CentreSettingsDispatcherAuthorizationTests.cs#L63): `public async Task SendAsync_UpdateCentreSettings_CentrelessActor_ReturnsTenantNotSelected()`
- [Line 73](../../../../../tests/TutoringCentre.Application.Tests/Centres/CentreSettingsDispatcherAuthorizationTests.cs#L73): `private static void AssertPermissionDenied<T>(Result<T> result)`
- [Line 80](../../../../../tests/TutoringCentre.Application.Tests/Centres/CentreSettingsDispatcherAuthorizationTests.cs#L80): `private static void AssertTenantNotSelected<T>(Result<T> result)`
- [Line 87](../../../../../tests/TutoringCentre.Application.Tests/Centres/CentreSettingsDispatcherAuthorizationTests.cs#L87): `private static Fixture CreateSut(StaffRole? role)`
- [Line 108](../../../../../tests/TutoringCentre.Application.Tests/Centres/CentreSettingsDispatcherAuthorizationTests.cs#L108): `private sealed class Fixture(Dispatcher dispatcher, ServiceProvider provider) : IDisposable`
- [Line 112](../../../../../tests/TutoringCentre.Application.Tests/Centres/CentreSettingsDispatcherAuthorizationTests.cs#L112): `public void Dispose() => provider.Dispose();`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using TutoringCentre.Application.Centres;
using TutoringCentre.Application.Centres.Commands.UpdateCentreSettings;
using TutoringCentre.Application.Centres.Queries.GetCentreSettings;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Centres;

/// <summary>
/// Through the real dispatcher: Secretary and Teacher are refused both centre-settings requests
/// (auth.permission_denied), and a centre-less actor is refused before permission is even considered
/// (tenant.not_selected) — the same two pipeline guarantees proved for Audit, Staff and Subjects.
/// </summary>
public sealed class CentreSettingsDispatcherAuthorizationTests
{
    private static readonly Guid CentreId = Guid.CreateVersion7();

    [Theory]
    [InlineData(StaffRole.Secretary)]
    [InlineData(StaffRole.Teacher)]
    public async Task QueryAsync_GetCentreSettings_ReturnsPermissionDeniedForSecretaryAndTeacher(StaffRole role)
    {
        using var fixture = CreateSut(role);

        var result = await fixture.Dispatcher.QueryAsync<GetCentreSettingsQuery, CentreSettingsDto>(
            new GetCentreSettingsQuery(), CancellationToken.None);

        AssertPermissionDenied(result);
    }

    [Theory]
    [InlineData(StaffRole.Secretary)]
    [InlineData(StaffRole.Teacher)]
    public async Task SendAsync_UpdateCentreSettings_ReturnsPermissionDeniedForSecretaryAndTeacher(StaffRole role)
    {
        using var fixture = CreateSut(role);

        var result = await fixture.Dispatcher.SendAsync<UpdateCentreSettingsCommand, Unit>(
            new UpdateCentreSettingsCommand("New Name", SupportedLocale.En, 0), CancellationToken.None);

        AssertPermissionDenied(result);
    }

    [Fact]
    public async Task QueryAsync_GetCentreSettings_CentrelessActor_ReturnsTenantNotSelected()
    {
        using var fixture = CreateSut(role: null);

        var result = await fixture.Dispatcher.QueryAsync<GetCentreSettingsQuery, CentreSettingsDto>(
            new GetCentreSettingsQuery(), CancellationToken.None);

        AssertTenantNotSelected(result);
    }

    [Fact]
    public async Task SendAsync_UpdateCentreSettings_CentrelessActor_ReturnsTenantNotSelected()
    {
        using var fixture = CreateSut(role: null);

        var result = await fixture.Dispatcher.SendAsync<UpdateCentreSettingsCommand, Unit>(
            new UpdateCentreSettingsCommand("New Name", SupportedLocale.En, 0), CancellationToken.None);

        AssertTenantNotSelected(result);
    }

    private static void AssertPermissionDenied<T>(Result<T> result)
    {
        Assert.True(result.IsFailure);
        Assert.Equal("auth.permission_denied", result.Error!.Code);
        Assert.Equal(ErrorKind.Forbidden, result.Error.Kind);
    }

    private static void AssertTenantNotSelected<T>(Result<T> result)
    {
        Assert.True(result.IsFailure);
        Assert.Equal("tenant.not_selected", result.Error!.Code);
        Assert.Equal(ErrorKind.Forbidden, result.Error.Kind);
    }

    private static Fixture CreateSut(StaffRole? role)
    {
        var actor = role is null
            ? new StaffActor(Guid.NewGuid(), CentreId: null, Role: null)
```

The excerpt stops at line 90; use the original file for the remaining 24 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
