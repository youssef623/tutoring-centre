using System.Net;
using System.Text.Json;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Academics;

/// <summary>Task 41.5: proves the nine student endpoints' behaviour through the full HTTP pipeline, against the Day 41 contract's outcome table.</summary>
[Collection(ApiCollection.Name)]
public sealed class StudentEndpointTests(ApiFactory factory) : IAsyncLifetime
{
    private static readonly Uri StudentsUri = new("/api/students", UriKind.Relative);

    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Register_NewParent_ThenGet_ThenUpdate_HappyPath()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var createResponse = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, StudentsUri,
            new { fullName = "أحمد محمود", phone = (string?)null, schoolYear = "الصف الثالث الثانوي", parent = new { fullName = "محمود حسن", phone = "01012345678" }, relationship = "father" });
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var location = createResponse.Headers.Location;
        Assert.NotNull(location);
        var createBody = await ReadJsonAsync(createResponse);
        var studentId = createBody.RootElement.GetProperty("id").GetGuid();
        var parentId = createBody.RootElement.GetProperty("parentId").GetGuid();
        Assert.NotEqual(Guid.Empty, parentId);

        using var getResponse = await owner.Client.GetAsync(location);
        var getBody = await ReadJsonAsync(getResponse);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.Equal("أحمد محمود", getBody.RootElement.GetProperty("fullName").GetString());
        Assert.Equal("active", getBody.RootElement.GetProperty("status").GetString());
        var parent = Assert.Single(getBody.RootElement.GetProperty("parents").EnumerateArray());
        Assert.True(parent.GetProperty("isPrimary").GetBoolean());
        Assert.Equal("father", parent.GetProperty("relationship").GetString());
        var version = getBody.RootElement.GetProperty("version").GetUInt32();

        using var updateResponse = await AntiforgeryTestHelper.SendAsJsonAsync(
            owner.Client, HttpMethod.Put, location,
            new { fullName = "أحمد محمود حسن", phone = (string?)null, schoolYear = "الصف الثالث الثانوي", version });
        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

        using var getAfterUpdate = await owner.Client.GetAsync(location);
        var afterBody = await ReadJsonAsync(getAfterUpdate);
        Assert.Equal("أحمد محمود حسن", afterBody.RootElement.GetProperty("fullName").GetString());
    }

    [Fact]
    public async Task Register_ExistingParentForSibling_SharesTheSameParentId()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (_, parentId, _) = await RegisterStudentAsync(owner.Client, "أحمد محمود", "محمود حسن", "01012345671");

        using var siblingResponse = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, StudentsUri,
            new { fullName = "محمد محمود", phone = (string?)null, schoolYear = (string?)null, parent = new { id = parentId }, relationship = "father" });

        Assert.Equal(HttpStatusCode.Created, siblingResponse.StatusCode);
        var body = await ReadJsonAsync(siblingResponse);
        Assert.Equal(parentId, body.RootElement.GetProperty("parentId").GetGuid());
    }

    [Fact]
    public async Task Register_DuplicateParentPhone_Returns409WithNestedParentPhoneField()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        await RegisterStudentAsync(owner.Client, "أحمد محمود", "محمود حسن", "01012345672");

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, StudentsUri,
            new { fullName = "Another Student", phone = (string?)null, schoolYear = (string?)null, parent = new { fullName = "Another Parent", phone = "0101 234 5672" }, relationship = "father" });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("parent.phone_taken", body.RootElement.GetProperty("code").GetString());
        Assert.True(body.RootElement.GetProperty("errors").GetProperty("parent.phone").GetArrayLength() > 0);
    }

    [Fact]
    public async Task Register_BothParentFormsGiven_Returns400WithErrorsParentAndCreatesNoRow()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var countBefore = await factory.ScalarAsync<long>("select count(*) from academics.students");

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, StudentsUri,
            new { fullName = "Student", phone = (string?)null, schoolYear = (string?)null, parent = new { id = Guid.NewGuid(), fullName = "Parent", phone = "01099990003" }, relationship = "father" });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.True(body.RootElement.GetProperty("errors").GetProperty("parent").GetArrayLength() > 0);
        Assert.Equal(countBefore, await factory.ScalarAsync<long>("select count(*) from academics.students"));
    }

    [Fact]
    public async Task Register_NeitherParentFormGiven_Returns400WithErrorsParent()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, StudentsUri,
            new { fullName = "Student", phone = (string?)null, schoolYear = (string?)null, parent = new { }, relationship = "father" });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.True(body.RootElement.GetProperty("errors").GetProperty("parent").GetArrayLength() > 0);
    }

    [Fact]
    public async Task Register_InvalidNestedParentPhone_Returns400PhoneInvalidWithNestedField()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, StudentsUri,
            new { fullName = "Student", phone = (string?)null, schoolYear = (string?)null, parent = new { fullName = "Parent", phone = "123" }, relationship = "father" });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("phone.invalid", body.RootElement.GetProperty("code").GetString());
        Assert.True(body.RootElement.GetProperty("errors").GetProperty("parent.phone").GetArrayLength() > 0);
    }

    [Fact]
    public async Task Register_InvalidStudentOwnPhone_Returns400PhoneInvalidWithTopLevelField()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, StudentsUri,
            new { fullName = "Student", phone = "123", schoolYear = (string?)null, parent = new { fullName = "Parent", phone = "01099990004" }, relationship = "father" });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("phone.invalid", body.RootElement.GetProperty("code").GetString());
        Assert.True(body.RootElement.GetProperty("errors").GetProperty("phone").GetArrayLength() > 0);
    }

    [Fact]
    public async Task Register_OneCharacterName_Returns400WithErrorsFullName()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, StudentsUri,
            new { fullName = "X", phone = (string?)null, schoolYear = (string?)null, parent = new { fullName = "Parent", phone = "01099990005" }, relationship = "father" });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.True(body.RootElement.GetProperty("errors").GetProperty("fullName").GetArrayLength() > 0);
    }

    [Fact]
    public async Task Register_UnknownFieldAtTopLevel_Returns400RequestMalformedAndCreatesNoRow()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var countBefore = await factory.ScalarAsync<long>("select count(*) from academics.students");

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, StudentsUri,
            new { fullName = "Student", phone = (string?)null, schoolYear = (string?)null, parent = new { fullName = "Parent", phone = "01099990006" }, relationship = "father", centreId = Guid.NewGuid() });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("request.malformed", body.RootElement.GetProperty("code").GetString());
        Assert.Equal(countBefore, await factory.ScalarAsync<long>("select count(*) from academics.students"));
    }

    [Fact]
    public async Task Register_UnknownFieldInsideNestedParent_Returns400RequestMalformedAndCreatesNoRow()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var countBefore = await factory.ScalarAsync<long>("select count(*) from academics.students");

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, StudentsUri,
            new { fullName = "Student", phone = (string?)null, schoolYear = (string?)null, parent = new { fullName = "Parent", phone = "01099990007", centreId = Guid.NewGuid() }, relationship = "father" });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("request.malformed", body.RootElement.GetProperty("code").GetString());
        Assert.Equal(countBefore, await factory.ScalarAsync<long>("select count(*) from academics.students"));
    }

    [Fact]
    public async Task List_EmptySearch_ReturnsActiveStudentsInNormalizedOrder()
    {
        // Distinctively named so these two can never collide with the seed's own demo students (Day 41.7) that
        // the same database already carries — the point here is relative order, not an exhaustive list.
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        await RegisterStudentAsync(owner.Client, "زينب اختبار", "ولي أمر 1", "01099990010");
        await RegisterStudentAsync(owner.Client, "أحمد اختبار", "ولي أمر 2", "01099990011");

        using var response = await owner.Client.GetAsync(new Uri("/api/students?pageSize=100", UriKind.Relative));

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var names = body.RootElement.GetProperty("items").EnumerateArray().Select(item => item.GetProperty("fullName").GetString()).ToList();
        Assert.Contains("أحمد اختبار", names);
        Assert.Contains("زينب اختبار", names);
        Assert.True(names.IndexOf("أحمد اختبار") < names.IndexOf("زينب اختبار"), "Expected normalized order: أحمد اختبار before زينب اختبار.");
    }

    [Fact]
    public async Task List_HamzaInsensitiveSearch_FindsStudentRegisteredWithHamza()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        await RegisterStudentAsync(owner.Client, "أحمد محمود", "ولي أمر", "01099990012");

        using var response = await owner.Client.GetAsync(new Uri("/api/students?search=" + Uri.EscapeDataString("احمد"), UriKind.Relative));

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains(body.RootElement.GetProperty("items").EnumerateArray(), item => item.GetProperty("fullName").GetString() == "أحمد محمود");
    }

    [Fact]
    public async Task List_PageSizeTooLarge_Returns400WithErrorsPageSize()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await owner.Client.GetAsync(new Uri("/api/students?pageSize=101", UriKind.Relative));

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.True(body.RootElement.GetProperty("errors").GetProperty("pageSize").GetArrayLength() > 0);
    }

    [Fact]
    public async Task List_MalformedCursor_Returns400WithErrorsCursor()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await owner.Client.GetAsync(new Uri("/api/students?cursor=not-a-real-cursor", UriKind.Relative));

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.True(body.RootElement.GetProperty("errors").GetProperty("cursor").GetArrayLength() > 0);
    }

    [Fact]
    public async Task List_NextCursorRoundTrips_SecondPageHasNoOverlap()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        await RegisterStudentAsync(owner.Client, "Student A", "Parent A", "01099990020");
        await RegisterStudentAsync(owner.Client, "Student B", "Parent B", "01099990021");
        await RegisterStudentAsync(owner.Client, "Student C", "Parent C", "01099990022");

        using var firstResponse = await owner.Client.GetAsync(new Uri("/api/students?pageSize=2", UriKind.Relative));
        var firstBody = await ReadJsonAsync(firstResponse);
        var firstIds = firstBody.RootElement.GetProperty("items").EnumerateArray().Select(item => item.GetProperty("id").GetGuid()).ToList();
        var cursor = firstBody.RootElement.GetProperty("nextCursor").GetString();
        Assert.NotNull(cursor);

        using var secondResponse = await owner.Client.GetAsync(new Uri($"/api/students?pageSize=2&cursor={Uri.EscapeDataString(cursor)}", UriKind.Relative));
        var secondBody = await ReadJsonAsync(secondResponse);
        var secondIds = secondBody.RootElement.GetProperty("items").EnumerateArray().Select(item => item.GetProperty("id").GetGuid()).ToList();

        Assert.Empty(firstIds.Intersect(secondIds));
    }

    [Fact]
    public async Task Update_StaleVersion_Returns409ConcurrencyStale()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (id, _, version) = await RegisterStudentAsync(owner.Client, "Student", "Parent", "01099990030");

        using var firstUpdate = await AntiforgeryTestHelper.SendAsJsonAsync(
            owner.Client, HttpMethod.Put, new Uri($"/api/students/{id}", UriKind.Relative),
            new { fullName = "Student Updated", phone = (string?)null, schoolYear = (string?)null, version });
        Assert.Equal(HttpStatusCode.NoContent, firstUpdate.StatusCode);

        using var staleUpdate = await AntiforgeryTestHelper.SendAsJsonAsync(
            owner.Client, HttpMethod.Put, new Uri($"/api/students/{id}", UriKind.Relative),
            new { fullName = "Another Name", phone = (string?)null, schoolYear = (string?)null, version });

        var body = await ReadJsonAsync(staleUpdate);
        Assert.Equal(HttpStatusCode.Conflict, staleUpdate.StatusCode);
        Assert.Equal("concurrency.stale", body.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task Update_UnknownId_Returns404StudentNotFound()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            owner.Client, HttpMethod.Put, new Uri($"/api/students/{Guid.NewGuid()}", UriKind.Relative),
            new { fullName = "Student", phone = (string?)null, schoolYear = (string?)null, version = 0 });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("student.not_found", body.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task Get_UnknownId_Returns404StudentNotFound()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await owner.Client.GetAsync(new Uri($"/api/students/{Guid.NewGuid()}", UriKind.Relative));

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("student.not_found", body.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task ArchiveThenArchiveAgain_SecondReturns422AlreadyArchived()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (id, _, version) = await RegisterStudentAsync(owner.Client, "Student", "Parent", "01099990031");

        using var firstArchive = await AntiforgeryTestHelper.SendAsJsonAsync(
            owner.Client, HttpMethod.Post, new Uri($"/api/students/{id}/archive", UriKind.Relative), new { version });
        Assert.Equal(HttpStatusCode.NoContent, firstArchive.StatusCode);
        var archivedVersion = await GetVersionAsync(owner.Client, id);

        using var secondArchive = await AntiforgeryTestHelper.SendAsJsonAsync(
            owner.Client, HttpMethod.Post, new Uri($"/api/students/{id}/archive", UriKind.Relative), new { version = archivedVersion });

        var body = await ReadJsonAsync(secondArchive);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, secondArchive.StatusCode);
        Assert.Equal("student.already_archived", body.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task Archive_WithAnActiveEnrollment_Returns422HasActiveEnrollmentsAndStudentStaysActive()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (id, _, version) = await RegisterStudentAsync(owner.Client, "Enrolled Student", "Parent", "01099990048");
        var groupId = await CreateGroupAsync(owner.Client, "Archive Student Group");
        using (var enrolled = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, new Uri($"/api/groups/{groupId}/enrollments", UriKind.Relative), new { studentId = id }))
        {
            Assert.Equal(HttpStatusCode.Created, enrolled.StatusCode);
        }

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            owner.Client, HttpMethod.Post, new Uri($"/api/students/{id}/archive", UriKind.Relative), new { version });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.Equal("student.has_active_enrollments", body.RootElement.GetProperty("code").GetString());
        Assert.Equal(version, await GetVersionAsync(owner.Client, id));
    }

    [Fact]
    public async Task RestoreAnActiveStudent_Returns422NotArchived()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (id, _, version) = await RegisterStudentAsync(owner.Client, "Student", "Parent", "01099990032");

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            owner.Client, HttpMethod.Post, new Uri($"/api/students/{id}/restore", UriKind.Relative), new { version });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.Equal("student.not_archived", body.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task UpdateAnArchivedStudent_Returns422ArchivedReadOnly()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (id, _, version) = await RegisterStudentAsync(owner.Client, "Student", "Parent", "01099990033");
        using var archive = await AntiforgeryTestHelper.SendAsJsonAsync(
            owner.Client, HttpMethod.Post, new Uri($"/api/students/{id}/archive", UriKind.Relative), new { version });
        Assert.Equal(HttpStatusCode.NoContent, archive.StatusCode);
        var archivedVersion = await GetVersionAsync(owner.Client, id);

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            owner.Client, HttpMethod.Put, new Uri($"/api/students/{id}", UriKind.Relative),
            new { fullName = "New Name", phone = (string?)null, schoolYear = (string?)null, version = archivedVersion });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.Equal("student.archived_read_only", body.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task LinkParent_UnknownParentId_Returns404ParentNotFound()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (id, _, version) = await RegisterStudentAsync(owner.Client, "Student", "Parent", "01099990034");

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            owner.Client, HttpMethod.Post, new Uri($"/api/students/{id}/parents", UriKind.Relative),
            new { parentId = Guid.NewGuid(), relationship = "mother", version });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("parent.not_found", body.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task LinkParent_AlreadyLinked_Returns409AlreadyLinked()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (id, parentId, version) = await RegisterStudentAsync(owner.Client, "Student", "Parent", "01099990035");

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            owner.Client, HttpMethod.Post, new Uri($"/api/students/{id}/parents", UriKind.Relative),
            new { parentId, relationship = "mother", version });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("student.parent_already_linked", body.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task FullLinkSequence_MakePrimaryThenUnlinkThenLastParentRefused()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (id, firstParentId, version) = await RegisterStudentAsync(owner.Client, "Student", "First Parent", "01099990036");
        var secondParentId = await RegisterSecondParentAsync(owner.Client, "Second Parent", "01099990037");

        using var link = await AntiforgeryTestHelper.SendAsJsonAsync(
            owner.Client, HttpMethod.Post, new Uri($"/api/students/{id}/parents", UriKind.Relative),
            new { parentId = secondParentId, relationship = "mother", version });
        Assert.Equal(HttpStatusCode.NoContent, link.StatusCode);
        var afterLinkVersion = await GetVersionAsync(owner.Client, id);

        using var makePrimary = await AntiforgeryTestHelper.SendAsJsonAsync(
            owner.Client, HttpMethod.Post, new Uri($"/api/students/{id}/parents/{secondParentId}/make-primary", UriKind.Relative),
            new { version = afterLinkVersion });
        Assert.Equal(HttpStatusCode.NoContent, makePrimary.StatusCode);
        var afterPrimaryVersion = await GetVersionAsync(owner.Client, id);

        using var unlinkFirst = await AntiforgeryTestHelper.SendAsJsonAsync(
            owner.Client, HttpMethod.Post, new Uri($"/api/students/{id}/parents/{firstParentId}/unlink", UriKind.Relative),
            new { version = afterPrimaryVersion });
        Assert.Equal(HttpStatusCode.NoContent, unlinkFirst.StatusCode);
        var afterUnlinkVersion = await GetVersionAsync(owner.Client, id);

        using var unlinkLast = await AntiforgeryTestHelper.SendAsJsonAsync(
            owner.Client, HttpMethod.Post, new Uri($"/api/students/{id}/parents/{secondParentId}/unlink", UriKind.Relative),
            new { version = afterUnlinkVersion });
        var body = await ReadJsonAsync(unlinkLast);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, unlinkLast.StatusCode);
        Assert.Equal("student.last_parent", body.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task UnlinkParent_NotLinked_Returns404StudentParentNotLinked()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (id, _, version) = await RegisterStudentAsync(owner.Client, "Student", "Parent", "01099990038");

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            owner.Client, HttpMethod.Post, new Uri($"/api/students/{id}/parents/{Guid.NewGuid()}/unlink", UriKind.Relative),
            new { version });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("student.parent_not_linked", body.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task MakePrimary_AlreadyPrimary_Returns422AlreadyPrimary()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (id, parentId, version) = await RegisterStudentAsync(owner.Client, "Student", "Parent", "01099990039");

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            owner.Client, HttpMethod.Post, new Uri($"/api/students/{id}/parents/{parentId}/make-primary", UriKind.Relative),
            new { version });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.Equal("student.already_primary", body.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task LinkParent_FifthParent_Returns422TooManyParents()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (id, _, initialVersion) = await RegisterStudentAsync(owner.Client, "Student", "Parent One", "01099990040");
        var version = initialVersion;

        for (var i = 0; i < 3; i++)
        {
            var extraParentId = await RegisterSecondParentAsync(owner.Client, $"Extra Parent {i}", $"010999900{41 + i}");
            using var link = await AntiforgeryTestHelper.SendAsJsonAsync(
                owner.Client, HttpMethod.Post, new Uri($"/api/students/{id}/parents", UriKind.Relative),
                new { parentId = extraParentId, relationship = "guardian", version });
            Assert.Equal(HttpStatusCode.NoContent, link.StatusCode);
            version = await GetVersionAsync(owner.Client, id);
        }

        var fifthParentId = await RegisterSecondParentAsync(owner.Client, "Fifth Parent", "01099990044");
        using var fifthLink = await AntiforgeryTestHelper.SendAsJsonAsync(
            owner.Client, HttpMethod.Post, new Uri($"/api/students/{id}/parents", UriKind.Relative),
            new { parentId = fifthParentId, relationship = "guardian", version });

        var body = await ReadJsonAsync(fifthLink);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, fifthLink.StatusCode);
        Assert.Equal("student.too_many_parents", body.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task Get_NoCookie_Returns401()
    {
        using var client = factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
        });

        using var response = await client.GetAsync(new Uri($"/api/students/{Guid.NewGuid()}", UriKind.Relative));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Register_WithoutTheAntiforgeryHeader_Returns403CsrfInvalid()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await owner.Client.PostAsJsonAsync(
            StudentsUri, new { fullName = "Student", phone = (string?)null, schoolYear = (string?)null, parent = new { fullName = "Parent", phone = "01099990045" }, relationship = "father" });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("auth.csrf_invalid", body.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task Register_TwoCentreTeacherBeforeSelectingACentre_Returns403TenantNotSelected()
    {
        using var teacher = await SignInHelper.SignInAsync(factory, "teacher@both.test");
        Assert.Null(teacher.CentreId);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            teacher.Client, StudentsUri,
            new { fullName = "Student", phone = (string?)null, schoolYear = (string?)null, parent = new { fullName = "Parent", phone = "01099990046" }, relationship = "father" });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("tenant.not_selected", body.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task Register_AsTeacherWithoutPermission_Returns403PermissionDenied()
    {
        using var teacher = await SignInHelper.SignInAsTwoCentreTeacherAsync(factory, "nile-centre");

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            teacher.Client, StudentsUri,
            new { fullName = "Student", phone = (string?)null, schoolYear = (string?)null, parent = new { fullName = "Parent", phone = "01099990047" }, relationship = "father" });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("auth.permission_denied", body.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task ErrorBody_HasCodeTraceIdAndCorrelationIdAndNoStackTrace()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await owner.Client.GetAsync(new Uri($"/api/students/{Guid.NewGuid()}", UriKind.Relative));
        var text = await response.Content.ReadAsStringAsync();
        var body = JsonDocument.Parse(text);

        Assert.True(body.RootElement.TryGetProperty("code", out _));
        Assert.True(body.RootElement.TryGetProperty("traceId", out _));
        Assert.True(body.RootElement.TryGetProperty("correlationId", out _));
        Assert.DoesNotContain("StackTrace", text, StringComparison.Ordinal);
        Assert.DoesNotContain(" at ", text, StringComparison.Ordinal);
    }

    private static async Task<(Guid StudentId, Guid ParentId, uint Version)> RegisterStudentAsync(
        HttpClient client, string fullName, string parentFullName, string parentPhone)
    {
        using var createResponse = await AntiforgeryTestHelper.PostAsJsonAsync(
            client, StudentsUri,
            new { fullName, phone = (string?)null, schoolYear = (string?)null, parent = new { fullName = parentFullName, phone = parentPhone }, relationship = "father" });
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var body = await ReadJsonAsync(createResponse);
        var studentId = body.RootElement.GetProperty("id").GetGuid();
        var parentId = body.RootElement.GetProperty("parentId").GetGuid();
        var version = await GetVersionAsync(client, studentId);
        return (studentId, parentId, version);
    }

    private static async Task<Guid> RegisterSecondParentAsync(HttpClient client, string parentFullName, string parentPhone)
    {
        using var createResponse = await AntiforgeryTestHelper.PostAsJsonAsync(
            client, StudentsUri,
            new { fullName = $"Temp Holder {Guid.NewGuid():N}", phone = (string?)null, schoolYear = (string?)null, parent = new { fullName = parentFullName, phone = parentPhone }, relationship = "mother" });
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var body = await ReadJsonAsync(createResponse);
        return body.RootElement.GetProperty("parentId").GetGuid();
    }

    private static async Task<Guid> CreateGroupAsync(HttpClient client, string name)
    {
        var subjectId = await CreateAsync(client, "/api/subjects", new { name = $"{name} Subject" });
        var teacherId = await CreateAsync(client, "/api/teachers", new { fullName = $"{name} Teacher", phone = (string?)null });
        return await CreateAsync(client, "/api/groups", new { name, subjectId, teacherId, capacity = 20 });
    }

    private static async Task<Guid> CreateAsync(HttpClient client, string url, object body)
    {
        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(client, new Uri(url, UriKind.Relative), body);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await ReadJsonAsync(response)).RootElement.GetProperty("id").GetGuid();
    }

    private static async Task<uint> GetVersionAsync(HttpClient client, Guid studentId)
    {
        using var response = await client.GetAsync(new Uri($"/api/students/{studentId}", UriKind.Relative));
        var body = await ReadJsonAsync(response);
        return body.RootElement.GetProperty("version").GetUInt32();
    }

    private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)
    {
        var text = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(text);
    }
}
