using System.Globalization;
using System.Net;
using System.Text.Json;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Authorization;

/// <summary>
/// Task 30.8 (extended Task 32.11): one table of role x endpoint -> expected status, executed over real HTTP
/// against Nile's three roles. 42 literal cases (six subject endpoints, five staff endpoints, one audit
/// endpoint, two centre-settings endpoints, three roles). Allowed cases assert a non-403 success status;
/// denied cases assert 403 auth.permission_denied and an unchanged database, compared against the exact
/// pre-request state, not a snapshot taken after the request already ran. Values are written literally here,
/// never derived from <c>RolePermissions</c> — that would prove the matrix agrees with itself, not with the
/// contract.
/// </summary>
[Collection(ApiCollection.Name)]
public sealed class PermissionMatrixTests(ApiFactory factory) : IAsyncLifetime
{
    /// <summary>The fourteen routes this matrix classifies, in the exact raw pattern <see cref="EndpointInventoryTests"/> reads off the running app.</summary>
    internal static readonly string[] MatrixRoutes =
    [
        "GET /api/subjects",
        "GET /api/subjects/{id:guid}",
        "POST /api/subjects",
        "PUT /api/subjects/{id:guid}",
        "POST /api/subjects/{id:guid}/archive",
        "POST /api/subjects/{id:guid}/restore",
        "GET /api/staff",
        "POST /api/staff",
        "PUT /api/staff/{membershipId:guid}/role",
        "POST /api/staff/{membershipId:guid}/deactivate",
        "POST /api/staff/{membershipId:guid}/reactivate",
        "GET /api/audit",
        "GET /api/centre/settings",
        "PUT /api/centre/settings",
        "POST /api/parents",
        "GET /api/parents/{id:guid}",
        "PUT /api/parents/{id:guid}",
        "POST /api/parents/lookup",
        "GET /api/students",
        "GET /api/students/{id:guid}",
        "POST /api/students",
        "PUT /api/students/{id:guid}",
        "POST /api/students/{id:guid}/archive",
        "POST /api/students/{id:guid}/restore",
        "POST /api/students/{id:guid}/parents",
        "POST /api/students/{id:guid}/parents/{parentId:guid}/unlink",
        "POST /api/students/{id:guid}/parents/{parentId:guid}/make-primary",
    ];

    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.OK)]
    [InlineData("secretary@nile.test", HttpStatusCode.OK)]
    [InlineData("teacher", HttpStatusCode.OK)]
    public async Task SubjectsList(string role, HttpStatusCode expected)
    {
        using var signedIn = await SignInAsync(role);

        using var response = await signedIn.Client.GetAsync(new Uri("/api/subjects", UriKind.Relative));

        Assert.Equal(expected, response.StatusCode);
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.OK)]
    [InlineData("secretary@nile.test", HttpStatusCode.OK)]
    [InlineData("teacher", HttpStatusCode.OK)]
    public async Task SubjectsGetById(string role, HttpStatusCode expected)
    {
        var (id, _) = await CreateSubjectAsOwnerAsync($"Matrix GetById {Guid.NewGuid():N}");
        using var signedIn = await SignInAsync(role);

        using var response = await signedIn.Client.GetAsync(new Uri($"/api/subjects/{id}", UriKind.Relative));

        Assert.Equal(expected, response.StatusCode);
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.Created)]
    [InlineData("secretary@nile.test", HttpStatusCode.Created)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    public async Task SubjectsCreate(string role, HttpStatusCode expected)
    {
        var countBefore = await CountSubjectsAsync();
        using var signedIn = await SignInAsync(role);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            signedIn.Client, new Uri("/api/subjects", UriKind.Relative), new { name = $"Matrix Create {Guid.NewGuid():N}" });

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
            Assert.Equal(countBefore, await CountSubjectsAsync());
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.NoContent)]
    [InlineData("secretary@nile.test", HttpStatusCode.NoContent)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    public async Task SubjectsRename(string role, HttpStatusCode expected)
    {
        var (id, version) = await CreateSubjectAsOwnerAsync($"Matrix Rename {Guid.NewGuid():N}");
        using var signedIn = await SignInAsync(role);

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            signedIn.Client, HttpMethod.Put, new Uri($"/api/subjects/{id}", UriKind.Relative), new { name = "Renamed By Attempt", version });

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
            Assert.Equal($"active|{version}", await SnapshotSubjectAsync(id));
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.NoContent)]
    [InlineData("secretary@nile.test", HttpStatusCode.NoContent)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    public async Task SubjectsArchive(string role, HttpStatusCode expected)
    {
        var (id, version) = await CreateSubjectAsOwnerAsync($"Matrix Archive {Guid.NewGuid():N}");
        using var signedIn = await SignInAsync(role);

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            signedIn.Client, HttpMethod.Post, new Uri($"/api/subjects/{id}/archive", UriKind.Relative), new { version });

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
            Assert.Equal($"active|{version}", await SnapshotSubjectAsync(id));
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.NoContent)]
    [InlineData("secretary@nile.test", HttpStatusCode.NoContent)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    public async Task SubjectsRestore(string role, HttpStatusCode expected)
    {
        var (id, archivedVersion) = await CreateArchivedSubjectAsOwnerAsync($"Matrix Restore {Guid.NewGuid():N}");
        using var signedIn = await SignInAsync(role);

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            signedIn.Client, HttpMethod.Post, new Uri($"/api/subjects/{id}/restore", UriKind.Relative), new { version = archivedVersion });

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
            Assert.Equal($"archived|{archivedVersion}", await SnapshotSubjectAsync(id));
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.OK)]
    [InlineData("secretary@nile.test", HttpStatusCode.Forbidden)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    public async Task StaffList(string role, HttpStatusCode expected)
    {
        using var signedIn = await SignInAsync(role);

        using var response = await signedIn.Client.GetAsync(new Uri("/api/staff", UriKind.Relative));

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.Created)]
    [InlineData("secretary@nile.test", HttpStatusCode.Forbidden)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    public async Task StaffCreate(string role, HttpStatusCode expected)
    {
        var usersBefore = await factory.ScalarAsync<long>("select count(*) from identity.users");
        var membershipsBefore = await factory.ScalarAsync<long>("select count(*) from identity.memberships");
        using var signedIn = await SignInAsync(role);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            signedIn.Client, new Uri("/api/staff", UriKind.Relative),
            new { email = $"matrix-create-{Guid.NewGuid():N}@nile.test", displayName = "Matrix Create", role = "teacher", preferredLocale = "en" });

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
            Assert.Equal(usersBefore, await factory.ScalarAsync<long>("select count(*) from identity.users"));
            Assert.Equal(membershipsBefore, await factory.ScalarAsync<long>("select count(*) from identity.memberships"));
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.NoContent)]
    [InlineData("secretary@nile.test", HttpStatusCode.Forbidden)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    public async Task StaffChangeRole(string role, HttpStatusCode expected)
    {
        var (membershipId, version) = await CreateStaffMemberAsOwnerAsync($"matrix-role-{Guid.NewGuid():N}@nile.test");
        using var signedIn = await SignInAsync(role);

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            signedIn.Client, HttpMethod.Put, new Uri($"/api/staff/{membershipId}/role", UriKind.Relative), new { role = "secretary", version });

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
            Assert.Equal($"teacher|active|{version}", await SnapshotMembershipAsync(membershipId));
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.NoContent)]
    [InlineData("secretary@nile.test", HttpStatusCode.Forbidden)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    public async Task StaffDeactivate(string role, HttpStatusCode expected)
    {
        var (membershipId, version) = await CreateStaffMemberAsOwnerAsync($"matrix-deact-{Guid.NewGuid():N}@nile.test");
        using var signedIn = await SignInAsync(role);

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            signedIn.Client, HttpMethod.Post, new Uri($"/api/staff/{membershipId}/deactivate", UriKind.Relative), new { version });

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
            Assert.Equal($"teacher|active|{version}", await SnapshotMembershipAsync(membershipId));
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.NoContent)]
    [InlineData("secretary@nile.test", HttpStatusCode.Forbidden)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    public async Task StaffReactivate(string role, HttpStatusCode expected)
    {
        var (membershipId, deactivatedVersion) = await CreateDeactivatedStaffMemberAsOwnerAsync($"matrix-react-{Guid.NewGuid():N}@nile.test");
        using var signedIn = await SignInAsync(role);

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            signedIn.Client, HttpMethod.Post, new Uri($"/api/staff/{membershipId}/reactivate", UriKind.Relative), new { version = deactivatedVersion });

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
            Assert.Equal($"teacher|inactive|{deactivatedVersion}", await SnapshotMembershipAsync(membershipId));
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.OK)]
    [InlineData("secretary@nile.test", HttpStatusCode.Forbidden)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    public async Task AuditList(string role, HttpStatusCode expected)
    {
        using var signedIn = await SignInAsync(role);

        using var response = await signedIn.Client.GetAsync(new Uri("/api/audit", UriKind.Relative));

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.OK)]
    [InlineData("secretary@nile.test", HttpStatusCode.Forbidden)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    public async Task CentreSettingsGet(string role, HttpStatusCode expected)
    {
        using var signedIn = await SignInAsync(role);

        using var response = await signedIn.Client.GetAsync(new Uri("/api/centre/settings", UriKind.Relative));

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.NoContent)]
    [InlineData("secretary@nile.test", HttpStatusCode.Forbidden)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    public async Task CentreSettingsUpdate(string role, HttpStatusCode expected)
    {
        var before = await SnapshotNileCentreAsync();
        var version = await GetNileCentreVersionAsync();
        using var signedIn = await SignInAsync(role);

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            signedIn.Client, HttpMethod.Put, new Uri("/api/centre/settings", UriKind.Relative),
            new { name = "Matrix Attempt", defaultLocale = "en", version });

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
            Assert.Equal(before, await SnapshotNileCentreAsync());
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.Created)]
    [InlineData("secretary@nile.test", HttpStatusCode.Created)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    public async Task ParentsCreate(string role, HttpStatusCode expected)
    {
        var countBefore = await CountParentsAsync();
        using var signedIn = await SignInAsync(role);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            signedIn.Client, new Uri("/api/parents", UriKind.Relative),
            new { fullName = $"Matrix Create {Guid.NewGuid():N}", phone = $"015{Random.Shared.Next(10000000, 99999999)}" });

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
            Assert.Equal(countBefore, await CountParentsAsync());
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.OK)]
    [InlineData("secretary@nile.test", HttpStatusCode.OK)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    public async Task ParentsGetById(string role, HttpStatusCode expected)
    {
        var (id, _) = await CreateParentAsOwnerAsync($"Matrix GetById {Guid.NewGuid():N}");
        using var signedIn = await SignInAsync(role);

        using var response = await signedIn.Client.GetAsync(new Uri($"/api/parents/{id}", UriKind.Relative));

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.NoContent)]
    [InlineData("secretary@nile.test", HttpStatusCode.NoContent)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    public async Task ParentsUpdate(string role, HttpStatusCode expected)
    {
        var (id, version) = await CreateParentAsOwnerAsync($"Matrix Update {Guid.NewGuid():N}");
        var snapshotBefore = await SnapshotParentAsync(id);
        using var signedIn = await SignInAsync(role);

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            signedIn.Client, HttpMethod.Put, new Uri($"/api/parents/{id}", UriKind.Relative),
            new { fullName = "Renamed By Attempt", phone = "01012340000", version });

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
            Assert.Equal(snapshotBefore, await SnapshotParentAsync(id));
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.OK)]
    [InlineData("secretary@nile.test", HttpStatusCode.OK)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    public async Task ParentsLookup(string role, HttpStatusCode expected)
    {
        using var signedIn = await SignInAsync(role);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            signedIn.Client, new Uri("/api/parents/lookup", UriKind.Relative), new { phone = "01012340001" });

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.OK)]
    [InlineData("secretary@nile.test", HttpStatusCode.OK)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    public async Task StudentsList(string role, HttpStatusCode expected)
    {
        using var signedIn = await SignInAsync(role);

        using var response = await signedIn.Client.GetAsync(new Uri("/api/students", UriKind.Relative));

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.OK)]
    [InlineData("secretary@nile.test", HttpStatusCode.OK)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    public async Task StudentsGetById(string role, HttpStatusCode expected)
    {
        var (id, _, _) = await RegisterStudentAsOwnerAsync($"Matrix GetById {Guid.NewGuid():N}");
        using var signedIn = await SignInAsync(role);

        using var response = await signedIn.Client.GetAsync(new Uri($"/api/students/{id}", UriKind.Relative));

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.Created)]
    [InlineData("secretary@nile.test", HttpStatusCode.Created)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    public async Task StudentsRegister(string role, HttpStatusCode expected)
    {
        var countBefore = await CountStudentsAsync();
        using var signedIn = await SignInAsync(role);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            signedIn.Client, new Uri("/api/students", UriKind.Relative),
            new
            {
                fullName = $"Matrix Register {Guid.NewGuid():N}", phone = (string?)null, schoolYear = (string?)null,
                parent = new { fullName = "Matrix Parent", phone = UniquePhone("012") }, relationship = "father",
            });

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
            Assert.Equal(countBefore, await CountStudentsAsync());
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.NoContent)]
    [InlineData("secretary@nile.test", HttpStatusCode.NoContent)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    public async Task StudentsUpdate(string role, HttpStatusCode expected)
    {
        var (id, _, version) = await RegisterStudentAsOwnerAsync($"Matrix Update {Guid.NewGuid():N}");
        var snapshotBefore = await SnapshotStudentAsync(id);
        using var signedIn = await SignInAsync(role);

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            signedIn.Client, HttpMethod.Put, new Uri($"/api/students/{id}", UriKind.Relative),
            new { fullName = "Renamed By Attempt", phone = (string?)null, schoolYear = (string?)null, version });

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
            Assert.Equal(snapshotBefore, await SnapshotStudentAsync(id));
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.NoContent)]
    [InlineData("secretary@nile.test", HttpStatusCode.NoContent)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    public async Task StudentsArchive(string role, HttpStatusCode expected)
    {
        var (id, _, version) = await RegisterStudentAsOwnerAsync($"Matrix Archive {Guid.NewGuid():N}");
        using var signedIn = await SignInAsync(role);

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            signedIn.Client, HttpMethod.Post, new Uri($"/api/students/{id}/archive", UriKind.Relative), new { version });

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
            Assert.Equal($"active|{version}", await SnapshotStudentAsync(id));
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.NoContent)]
    [InlineData("secretary@nile.test", HttpStatusCode.NoContent)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    public async Task StudentsRestore(string role, HttpStatusCode expected)
    {
        var (id, _, archivedVersion) = await RegisterArchivedStudentAsOwnerAsync($"Matrix Restore {Guid.NewGuid():N}");
        using var signedIn = await SignInAsync(role);

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            signedIn.Client, HttpMethod.Post, new Uri($"/api/students/{id}/restore", UriKind.Relative), new { version = archivedVersion });

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
            Assert.Equal($"archived|{archivedVersion}", await SnapshotStudentAsync(id));
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.NoContent)]
    [InlineData("secretary@nile.test", HttpStatusCode.NoContent)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    public async Task StudentsLinkParent(string role, HttpStatusCode expected)
    {
        var (id, _, version) = await RegisterStudentAsOwnerAsync($"Matrix Link {Guid.NewGuid():N}");
        var secondParentId = await RegisterSecondParentAsOwnerAsync();
        using var signedIn = await SignInAsync(role);

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            signedIn.Client, HttpMethod.Post, new Uri($"/api/students/{id}/parents", UriKind.Relative),
            new { parentId = secondParentId, relationship = "mother", version });

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
            Assert.Equal($"active|{version}", await SnapshotStudentAsync(id));
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.NoContent)]
    [InlineData("secretary@nile.test", HttpStatusCode.NoContent)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    public async Task StudentsUnlinkParent(string role, HttpStatusCode expected)
    {
        var (id, firstParentId, secondParentId, version) = await RegisterStudentWithTwoParentsAsOwnerAsync($"Matrix Unlink {Guid.NewGuid():N}");
        using var signedIn = await SignInAsync(role);

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            signedIn.Client, HttpMethod.Post, new Uri($"/api/students/{id}/parents/{secondParentId}/unlink", UriKind.Relative), new { version });

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
            Assert.Equal($"active|{version}", await SnapshotStudentAsync(id));
            Assert.True(firstParentId != Guid.Empty);
        }
    }

    [Theory]
    [InlineData("owner@nile.test", HttpStatusCode.NoContent)]
    [InlineData("secretary@nile.test", HttpStatusCode.NoContent)]
    [InlineData("teacher", HttpStatusCode.Forbidden)]
    public async Task StudentsMakePrimary(string role, HttpStatusCode expected)
    {
        var (id, _, secondParentId, version) = await RegisterStudentWithTwoParentsAsOwnerAsync($"Matrix MakePrimary {Guid.NewGuid():N}");
        using var signedIn = await SignInAsync(role);

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            signedIn.Client, HttpMethod.Post, new Uri($"/api/students/{id}/parents/{secondParentId}/make-primary", UriKind.Relative), new { version });

        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertPermissionDeniedAsync(response);
            Assert.Equal($"active|{version}", await SnapshotStudentAsync(id));
        }
    }

    // Random.Shared.Next(10000000, 99999999) collides often enough across this many call sites in one run to
    // occasionally produce a 409 parent.phone_taken where a test expects 201/204 — an interlocked counter
    // never repeats within a run, unlike eight random digits.
    private static int _nextPhoneSuffix = 10000000;

    private static string UniquePhone(string prefix) => prefix + Interlocked.Increment(ref _nextPhoneSuffix).ToString(CultureInfo.InvariantCulture);

    private Task<long> CountStudentsAsync() =>
        factory.ScalarAsync<long>("select count(*) from academics.students where centre_id = (select id from platform.centres where slug = 'nile-centre')");

    private async Task<(Guid Id, Guid ParentId, uint Version)> RegisterStudentAsOwnerAsync(string fullName)
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        using var createResponse = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, new Uri("/api/students", UriKind.Relative),
            new { fullName, phone = (string?)null, schoolYear = (string?)null, parent = new { fullName = "Matrix Target Parent", phone = UniquePhone("012") }, relationship = "father" });
        var createBody = await ReadJsonAsync(createResponse);
        var id = createBody.RootElement.GetProperty("id").GetGuid();
        var parentId = createBody.RootElement.GetProperty("parentId").GetGuid();
        using var getResponse = await owner.Client.GetAsync(new Uri($"/api/students/{id}", UriKind.Relative));
        var body = await ReadJsonAsync(getResponse);
        return (id, parentId, body.RootElement.GetProperty("version").GetUInt32());
    }

    private async Task<(Guid Id, Guid ParentId, uint ArchivedVersion)> RegisterArchivedStudentAsOwnerAsync(string fullName)
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (id, parentId, version) = await RegisterStudentAsOwnerAsync(fullName);
        using var archiveResponse = await AntiforgeryTestHelper.SendAsJsonAsync(
            owner.Client, HttpMethod.Post, new Uri($"/api/students/{id}/archive", UriKind.Relative), new { version });
        Assert.Equal(HttpStatusCode.NoContent, archiveResponse.StatusCode);
        using var getResponse = await owner.Client.GetAsync(new Uri($"/api/students/{id}", UriKind.Relative));
        var body = await ReadJsonAsync(getResponse);
        return (id, parentId, body.RootElement.GetProperty("version").GetUInt32());
    }

    private async Task<Guid> RegisterSecondParentAsOwnerAsync()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        using var createResponse = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, new Uri("/api/students", UriKind.Relative),
            new
            {
                fullName = $"Matrix Temp Holder {Guid.NewGuid():N}", phone = (string?)null, schoolYear = (string?)null,
                parent = new { fullName = "Matrix Second Parent", phone = UniquePhone("012") }, relationship = "mother",
            });
        var body = await ReadJsonAsync(createResponse);
        return body.RootElement.GetProperty("parentId").GetGuid();
    }

    private async Task<(Guid Id, Guid FirstParentId, Guid SecondParentId, uint Version)> RegisterStudentWithTwoParentsAsOwnerAsync(string fullName)
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (id, firstParentId, version) = await RegisterStudentAsOwnerAsync(fullName);
        var secondParentId = await RegisterSecondParentAsOwnerAsync();
        using var linkResponse = await AntiforgeryTestHelper.SendAsJsonAsync(
            owner.Client, HttpMethod.Post, new Uri($"/api/students/{id}/parents", UriKind.Relative),
            new { parentId = secondParentId, relationship = "mother", version });
        Assert.Equal(HttpStatusCode.NoContent, linkResponse.StatusCode);
        using var getResponse = await owner.Client.GetAsync(new Uri($"/api/students/{id}", UriKind.Relative));
        var body = await ReadJsonAsync(getResponse);
        return (id, firstParentId, secondParentId, body.RootElement.GetProperty("version").GetUInt32());
    }

    private Task<string> SnapshotStudentAsync(Guid id) =>
        factory.ScalarAsync<string>($"select status || '|' || xmin from academics.students where id = '{id}'");

    private Task<long> CountParentsAsync() =>
        factory.ScalarAsync<long>("select count(*) from academics.parents where centre_id = (select id from platform.centres where slug = 'nile-centre')");

    private async Task<(Guid Id, uint Version)> CreateParentAsOwnerAsync(string fullName)
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        using var createResponse = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, new Uri("/api/parents", UriKind.Relative),
            new { fullName, phone = $"011{Random.Shared.Next(10000000, 99999999)}" });
        var location = createResponse.Headers.Location!;
        using var getResponse = await owner.Client.GetAsync(location);
        var body = await ReadJsonAsync(getResponse);
        return (body.RootElement.GetProperty("id").GetGuid(), body.RootElement.GetProperty("version").GetUInt32());
    }

    private Task<string> SnapshotParentAsync(Guid id) =>
        factory.ScalarAsync<string>($"select full_name || '|' || phone || '|' || xmin from academics.parents where id = '{id}'");

    private Task<string> SnapshotNileCentreAsync() =>
        factory.ScalarAsync<string>("select name || '|' || default_locale || '|' || xmin from platform.centres where slug = 'nile-centre'");

    private async Task<uint> GetNileCentreVersionAsync()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        using var response = await owner.Client.GetAsync(new Uri("/api/centre/settings", UriKind.Relative));
        var body = await ReadJsonAsync(response);
        return body.RootElement.GetProperty("version").GetUInt32();
    }

    private Task<SignedInClient> SignInAsync(string role) =>
        role == "teacher"
            ? SignInHelper.SignInAsTwoCentreTeacherAsync(factory, "nile-centre")
            : SignInHelper.SignInAsync(factory, role);

    private static async Task AssertPermissionDeniedAsync(HttpResponseMessage response)
    {
        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("auth.permission_denied", body.RootElement.GetProperty("code").GetString());
    }

    private async Task<(Guid Id, uint Version)> CreateSubjectAsOwnerAsync(string name)
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        using var createResponse = await AntiforgeryTestHelper.PostAsJsonAsync(owner.Client, new Uri("/api/subjects", UriKind.Relative), new { name });
        var location = createResponse.Headers.Location!;
        using var getResponse = await owner.Client.GetAsync(location);
        var body = await ReadJsonAsync(getResponse);
        return (body.RootElement.GetProperty("id").GetGuid(), body.RootElement.GetProperty("version").GetUInt32());
    }

    private async Task<(Guid Id, uint ArchivedVersion)> CreateArchivedSubjectAsOwnerAsync(string name)
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (id, version) = await CreateSubjectAsOwnerAsync(name);
        using var archiveResponse = await AntiforgeryTestHelper.SendAsJsonAsync(
            owner.Client, HttpMethod.Post, new Uri($"/api/subjects/{id}/archive", UriKind.Relative), new { version });
        Assert.Equal(HttpStatusCode.NoContent, archiveResponse.StatusCode);
        using var getResponse = await owner.Client.GetAsync(new Uri($"/api/subjects/{id}", UriKind.Relative));
        var body = await ReadJsonAsync(getResponse);
        return (id, body.RootElement.GetProperty("version").GetUInt32());
    }

    private async Task<(Guid MembershipId, uint Version)> CreateStaffMemberAsOwnerAsync(string email)
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        using var createResponse = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, new Uri("/api/staff", UriKind.Relative), new { email, displayName = "Matrix Target", role = "teacher", preferredLocale = "en" });
        var body = await ReadJsonAsync(createResponse);
        var membershipId = body.RootElement.GetProperty("membershipId").GetGuid();
        using var listResponse = await owner.Client.GetAsync(new Uri("/api/staff", UriKind.Relative));
        var listBody = await ReadJsonAsync(listResponse);
        var version = listBody.RootElement.GetProperty("items").EnumerateArray()
            .Single(item => item.GetProperty("membershipId").GetGuid() == membershipId).GetProperty("version").GetUInt32();
        return (membershipId, version);
    }

    private async Task<(Guid MembershipId, uint DeactivatedVersion)> CreateDeactivatedStaffMemberAsOwnerAsync(string email)
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (membershipId, version) = await CreateStaffMemberAsOwnerAsync(email);
        using var deactivateResponse = await AntiforgeryTestHelper.SendAsJsonAsync(
            owner.Client, HttpMethod.Post, new Uri($"/api/staff/{membershipId}/deactivate", UriKind.Relative), new { version });
        Assert.Equal(HttpStatusCode.NoContent, deactivateResponse.StatusCode);
        using var listResponse = await owner.Client.GetAsync(new Uri("/api/staff", UriKind.Relative));
        var listBody = await ReadJsonAsync(listResponse);
        var deactivatedVersion = listBody.RootElement.GetProperty("items").EnumerateArray()
            .Single(item => item.GetProperty("membershipId").GetGuid() == membershipId).GetProperty("version").GetUInt32();
        return (membershipId, deactivatedVersion);
    }

    private Task<string> SnapshotSubjectAsync(Guid id) =>
        factory.ScalarAsync<string>($"select status || '|' || xmin from academics.subjects where id = '{id}'");

    private Task<string> SnapshotMembershipAsync(Guid membershipId) =>
        factory.ScalarAsync<string>($"select role || '|' || status || '|' || xmin from identity.memberships where id = '{membershipId}'");

    private async Task<int> CountSubjectsAsync()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        using var response = await owner.Client.GetAsync(new Uri("/api/subjects?includeArchived=true", UriKind.Relative));
        var body = await ReadJsonAsync(response);
        return body.RootElement.GetProperty("items").GetArrayLength();
    }

    private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)
    {
        var text = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(text);
    }
}
