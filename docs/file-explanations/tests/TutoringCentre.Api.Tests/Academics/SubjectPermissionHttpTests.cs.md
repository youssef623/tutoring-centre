# `tests/TutoringCentre.Api.Tests/Academics/SubjectPermissionHttpTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 28.10: the Day 28 permission matrix proved through HTTP for the six Subject endpoints, over Nile's three roles (the Day 26 harness). 18 literal cases (6 endpoints x 3 roles) plus one extra: permission is evaluated before existence, so a forbidden request against an unknown ID is still 403, never 404.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectPermissionHttpTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 204 lines and 9,082 bytes. Source hash: `29408a0963cd0a856e3e6975e7bab7d33ec4755af859b1479c6f68aa306190fc`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Net;`
- Line 2: `using System.Text.Json;`
- Line 3: `using TutoringCentre.Api.Cli;`
- Line 4: `using TutoringCentre.Api.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 14](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectPermissionHttpTests.cs#L14): `public sealed class SubjectPermissionHttpTests(ApiFactory factory) : IAsyncLifetime`
- [Line 18](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectPermissionHttpTests.cs#L18): `public async Task InitializeAsync()`
- [Line 24](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectPermissionHttpTests.cs#L24): `public Task DisposeAsync() => Task.CompletedTask;`
- [Line 27](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectPermissionHttpTests.cs#L27): `public async Task Teacher_List_Returns200()`
- [Line 37](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectPermissionHttpTests.cs#L37): `public async Task Teacher_GetById_Returns200()`
- [Line 49](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectPermissionHttpTests.cs#L49): `public async Task Teacher_Create_IsForbiddenAndCreatesNoRow()`
- [Line 62](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectPermissionHttpTests.cs#L62): `public async Task Teacher_Rename_IsForbiddenAndLeavesTheRowUnchanged()`
- [Line 76](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectPermissionHttpTests.cs#L76): `public async Task Teacher_Archive_IsForbiddenAndLeavesTheRowUnchanged()`
- [Line 89](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectPermissionHttpTests.cs#L89): `public async Task Teacher_Restore_IsForbiddenAndLeavesTheRowUnchanged()`
- [Line 104](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectPermissionHttpTests.cs#L104): `public async Task OwnerAndSecretary_CanListGetCreateRenameArchiveAndRestore(string email)`
- [Line 146](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectPermissionHttpTests.cs#L146): `public async Task Teacher_ForbiddenRenameAgainstAnUnknownId_Returns403NotFound_PermissionBeforeExistence()`
- [Line 157](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectPermissionHttpTests.cs#L157): `private static async Task AssertPermissionDeniedAsync(Task<HttpResponseMessage> responseTask)`
- [Line 178](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectPermissionHttpTests.cs#L178): `private static async Task<string> GetNameAsync(HttpClient client, Guid id)`
- [Line 185](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectPermissionHttpTests.cs#L185): `private static async Task<uint> GetVersionAsync(HttpClient client, Guid id)`
- [Line 192](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectPermissionHttpTests.cs#L192): `private static async Task<int> CountSubjectsAsync(HttpClient client)`
- [Line 199](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectPermissionHttpTests.cs#L199): `private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Net;
using System.Text.Json;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Academics;

/// <summary>
/// Task 28.10: the Day 28 permission matrix proved through HTTP for the six Subject endpoints, over Nile's
/// three roles (the Day 26 harness). 18 literal cases (6 endpoints x 3 roles) plus one extra: permission is
/// evaluated before existence, so a forbidden request against an unknown ID is still 403, never 404.
/// </summary>
[Collection(ApiCollection.Name)]
public sealed class SubjectPermissionHttpTests(ApiFactory factory) : IAsyncLifetime
{
    private static readonly Uri SubjectsUri = new("/api/subjects", UriKind.Relative);

    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Teacher_List_Returns200()
    {
        using var teacher = await SignInHelper.SignInAsTwoCentreTeacherAsync(factory, "nile-centre");

        using var response = await teacher.Client.GetAsync(SubjectsUri);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Teacher_GetById_Returns200()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (id, _) = await CreateSubjectAsync(owner.Client, "Teacher Get Target");
        using var teacher = await SignInHelper.SignInAsTwoCentreTeacherAsync(factory, "nile-centre");

        using var response = await teacher.Client.GetAsync(new Uri($"/api/subjects/{id}", UriKind.Relative));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Teacher_Create_IsForbiddenAndCreatesNoRow()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var countBefore = await CountSubjectsAsync(owner.Client);
        using var teacher = await SignInHelper.SignInAsTwoCentreTeacherAsync(factory, "nile-centre");

        await AssertPermissionDeniedAsync(AntiforgeryTestHelper.PostAsJsonAsync(
            teacher.Client, SubjectsUri, new { name = "Teacher Create Attempt" }));

        Assert.Equal(countBefore, await CountSubjectsAsync(owner.Client));
    }

    [Fact]
    public async Task Teacher_Rename_IsForbiddenAndLeavesTheRowUnchanged()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (id, version) = await CreateSubjectAsync(owner.Client, "Teacher Rename Target");
        using var teacher = await SignInHelper.SignInAsTwoCentreTeacherAsync(factory, "nile-centre");

        await AssertPermissionDeniedAsync(AntiforgeryTestHelper.SendAsJsonAsync(
            teacher.Client, HttpMethod.Put, new Uri($"/api/subjects/{id}", UriKind.Relative), new { name = "Teacher Rename Attempt", version }));

        Assert.Equal("Teacher Rename Target", await GetNameAsync(owner.Client, id));
        Assert.Equal(version, await GetVersionAsync(owner.Client, id));
    }

    [Fact]
    public async Task Teacher_Archive_IsForbiddenAndLeavesTheRowUnchanged()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (id, version) = await CreateSubjectAsync(owner.Client, "Teacher Archive Target");
        using var teacher = await SignInHelper.SignInAsTwoCentreTeacherAsync(factory, "nile-centre");

        await AssertPermissionDeniedAsync(AntiforgeryTestHelper.SendAsJsonAsync(
            teacher.Client, HttpMethod.Post, new Uri($"/api/subjects/{id}/archive", UriKind.Relative), new { version }));

        Assert.Equal(version, await GetVersionAsync(owner.Client, id));
    }

    [Fact]
    public async Task Teacher_Restore_IsForbiddenAndLeavesTheRowUnchanged()
    {
```

The excerpt stops at line 90; use the original file for the remaining 114 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
