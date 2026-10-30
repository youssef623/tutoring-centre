using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Portal;

[Collection(ApiCollection.Name)]
public sealed class PortalAccountEndpointTests(ApiFactory factory) : IAsyncLifetime
{
    private static readonly Uri AccountsUri = new("/api/portal-accounts", UriKind.Relative);

    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task CreateListDeactivateReactivate_OneTimeCredentialAndVersionedTransitions()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var subjectId = await ParentAsync("nile-centre");
        using var created = await CreateAsync(owner.Client, subjectId);
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        Assert.Equal("no-store", created.Headers.CacheControl?.ToString());
        Assert.Null(created.Headers.Location);
        using var body = await JsonAsync(created);
        Assert.Equal(["linkId", "membershipId", "temporaryPassword", "userId"], body.RootElement.EnumerateObject().Select(property => property.Name).Order(StringComparer.Ordinal));
        var password = body.RootElement.GetProperty("temporaryPassword").GetString();
        Assert.False(string.IsNullOrWhiteSpace(password));
        Assert.DoesNotContain(password!, string.Join(" ", created.Headers.SelectMany(header => header.Value)), StringComparison.Ordinal);
        var linkId = body.RootElement.GetProperty("linkId").GetGuid();

        var account = await AccountAsync(owner.Client, subjectId);
        Assert.Equal(linkId, account.GetProperty("linkId").GetGuid());
        Assert.Equal("active", account.GetProperty("status").GetString());
        Assert.False(account.TryGetProperty("temporaryPassword", out _));
        Assert.DoesNotContain(password!, account.GetRawText(), StringComparison.Ordinal);

        using var deactivated = await AntiforgeryTestHelper.PostAsJsonAsync(owner.Client, TransitionUri(linkId, "deactivate"), new { version = account.GetProperty("version").GetUInt32() });
        Assert.Equal(HttpStatusCode.NoContent, deactivated.StatusCode);
        var inactive = await AccountAsync(owner.Client, subjectId);
        Assert.Equal("inactive", inactive.GetProperty("status").GetString());
        using var stale = await AntiforgeryTestHelper.PostAsJsonAsync(owner.Client, TransitionUri(linkId, "reactivate"), new { version = account.GetProperty("version").GetUInt32() });
        await AssertProblemAsync(stale, HttpStatusCode.Conflict, "concurrency.stale");
        using var reactivated = await AntiforgeryTestHelper.PostAsJsonAsync(owner.Client, TransitionUri(linkId, "reactivate"), new { version = inactive.GetProperty("version").GetUInt32() });
        Assert.Equal(HttpStatusCode.NoContent, reactivated.StatusCode);
        Assert.Equal("active", (await AccountAsync(owner.Client, subjectId)).GetProperty("status").GetString());
    }

    [Fact]
    public async Task DuplicateSubject_ReturnsStableConflictCode()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var subjectId = await ParentAsync("nile-centre");
        using var first = await CreateAsync(owner.Client, subjectId);
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);
        using var duplicate = await CreateAsync(owner.Client, subjectId, "second-portal@nile.test");
        await AssertProblemAsync(duplicate, HttpStatusCode.Conflict, "portal.subject_already_linked");
        Assert.Equal(1L, await factory.ScalarAsync<long>("select count(*) from identity.portal_links"));
        Assert.Equal(0L, await factory.ScalarAsync<long>("select count(*) from identity.users where email='second-portal@nile.test'"));
    }

    [Fact]
    public async Task ExistingUserInCentre_ReturnsStableConflictCode()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        using var response = await CreateAsync(owner.Client, await ParentAsync("nile-centre"), "secretary@nile.test");
        await AssertProblemAsync(response, HttpStatusCode.Conflict, "portal.user_already_linked");
        Assert.Equal(0L, await factory.ScalarAsync<long>("select count(*) from identity.portal_links"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    public async Task InvalidEmail_ReturnsValidationErrorWithoutCreatingAccount(string email)
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        using var response = await CreateAsync(owner.Client, await ParentAsync("nile-centre"), email);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        using var body = await JsonAsync(response);
        Assert.True(body.RootElement.GetProperty("errors").GetProperty("email").GetArrayLength() > 0);
        Assert.Equal(0L, await factory.ScalarAsync<long>("select count(*) from identity.portal_links"));
    }

    [Fact]
    public async Task OverPostedCentreId_IsRejectedByStrictJson()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(owner.Client, AccountsUri,
            new { kind = "parent", subjectId = await ParentAsync("nile-centre"), email = "overposted@nile.test", preferredLocale = "ar", centreId = Guid.NewGuid() });
        await AssertProblemAsync(response, HttpStatusCode.BadRequest, "request.malformed");
        Assert.Equal(0L, await factory.ScalarAsync<long>("select count(*) from identity.portal_links"));
    }

    [Fact]
    public async Task ForeignSubjectAndLink_AreNotFoundAndCannotBeListed()
    {
        using var nile = await SignInHelper.SignInAsNileOwnerAsync(factory);
        using var maadi = await SignInHelper.SignInAsMaadiOwnerAsync(factory);
        var subjectId = await ParentAsync("maadi-hub");
        using var refused = await CreateAsync(nile.Client, subjectId);
        await AssertProblemAsync(refused, HttpStatusCode.NotFound, "portal.subject_not_found");
        using var created = await CreateAsync(maadi.Client, subjectId, "portal@maadi.test");
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        using var body = await JsonAsync(created);
        var linkId = body.RootElement.GetProperty("linkId").GetGuid();
        var account = await AccountAsync(maadi.Client, subjectId);
        using var deactivate = await AntiforgeryTestHelper.PostAsJsonAsync(nile.Client, TransitionUri(linkId, "deactivate"), new { version = account.GetProperty("version").GetUInt32() });
        await AssertProblemAsync(deactivate, HttpStatusCode.NotFound, "portal.not_found");
        using var reactivate = await AntiforgeryTestHelper.PostAsJsonAsync(nile.Client, TransitionUri(linkId, "reactivate"), new { version = account.GetProperty("version").GetUInt32() });
        await AssertProblemAsync(reactivate, HttpStatusCode.NotFound, "portal.not_found");
        using var list = await nile.Client.GetAsync(ListUri(subjectId));
        using var listBody = await JsonAsync(list);
        Assert.Equal(HttpStatusCode.OK, list.StatusCode);
        Assert.Equal(0, listBody.RootElement.GetProperty("items").GetArrayLength());
        Assert.Equal("active", (await AccountAsync(maadi.Client, subjectId)).GetProperty("status").GetString());
    }

    [Fact]
    public async Task CreateWithoutCsrf_IsForbiddenAndLeavesNoAccount()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        using var response = await owner.Client.PostAsJsonAsync(AccountsUri,
            new { kind = "parent", subjectId = await ParentAsync("nile-centre"), email = "csrf@nile.test", preferredLocale = "ar" });
        await AssertProblemAsync(response, HttpStatusCode.Forbidden, "auth.csrf_invalid");
        Assert.Equal(0L, await factory.ScalarAsync<long>("select count(*) from identity.portal_links"));
    }

    [Theory]
    [InlineData("owner@nile.test", "list", HttpStatusCode.OK)]
    [InlineData("secretary@nile.test", "list", HttpStatusCode.OK)]
    [InlineData("teacher", "list", HttpStatusCode.Forbidden)]
    [InlineData("parent", "list", HttpStatusCode.Forbidden)]
    [InlineData("student", "list", HttpStatusCode.Forbidden)]
    [InlineData("owner@nile.test", "create", HttpStatusCode.Created)]
    [InlineData("secretary@nile.test", "create", HttpStatusCode.Created)]
    [InlineData("teacher", "create", HttpStatusCode.Forbidden)]
    [InlineData("parent", "create", HttpStatusCode.Forbidden)]
    [InlineData("student", "create", HttpStatusCode.Forbidden)]
    [InlineData("owner@nile.test", "deactivate", HttpStatusCode.NoContent)]
    [InlineData("secretary@nile.test", "deactivate", HttpStatusCode.NoContent)]
    [InlineData("teacher", "deactivate", HttpStatusCode.Forbidden)]
    [InlineData("parent", "deactivate", HttpStatusCode.Forbidden)]
    [InlineData("student", "deactivate", HttpStatusCode.Forbidden)]
    [InlineData("owner@nile.test", "reactivate", HttpStatusCode.NoContent)]
    [InlineData("secretary@nile.test", "reactivate", HttpStatusCode.NoContent)]
    [InlineData("teacher", "reactivate", HttpStatusCode.Forbidden)]
    [InlineData("parent", "reactivate", HttpStatusCode.Forbidden)]
    [InlineData("student", "reactivate", HttpStatusCode.Forbidden)]
    public async Task FiveRoleMatrix_AllFourRoutesUsePortalAccountsManage(string role, string operation, HttpStatusCode expected)
    {
        var subjectId = await ParentAsync("nile-centre");
        Guid linkId = Guid.Empty;
        uint version = 0;
        if (operation is "deactivate" or "reactivate")
        {
            using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
            using var created = await CreateAsync(owner.Client, subjectId);
            Assert.Equal(HttpStatusCode.Created, created.StatusCode);
            var account = await AccountAsync(owner.Client, subjectId);
            linkId = account.GetProperty("linkId").GetGuid();
            if (operation == "reactivate")
            {
                using var inactive = await AntiforgeryTestHelper.PostAsJsonAsync(owner.Client, TransitionUri(linkId, "deactivate"), new { version = account.GetProperty("version").GetUInt32() });
                Assert.Equal(HttpStatusCode.NoContent, inactive.StatusCode);
                account = await AccountAsync(owner.Client, subjectId);
            }
            version = account.GetProperty("version").GetUInt32();
        }

        using var actor = await SignInAsync(role);
        var before = await MembershipSnapshotAsync();
        var linksBefore = await factory.ScalarAsync<long>("select count(*) from identity.portal_links");
        using var response = operation switch
        {
            "list" => await actor.Client.GetAsync(ListUri(subjectId)),
            "create" => await CreateAsync(actor.Client, subjectId),
            _ => await AntiforgeryTestHelper.PostAsJsonAsync(actor.Client, TransitionUri(linkId, operation), new { version }),
        };
        Assert.Equal(expected, response.StatusCode);
        if (expected == HttpStatusCode.Forbidden)
        {
            await AssertProblemAsync(response, expected, "auth.permission_denied");
            Assert.Equal(before, await MembershipSnapshotAsync());
            Assert.Equal(linksBefore, await factory.ScalarAsync<long>("select count(*) from identity.portal_links"));
        }
    }

    private async Task<SignedInClient> SignInAsync(string role)
    {
        if (role is "parent" or "student")
        {
            var centre = await factory.ScalarAsync<Guid>("select id from platform.centres where slug='nile-centre'");
            var user = await factory.ScalarAsync<Guid>("select id from identity.users where email='secretary@nile.test'");
            var owner = await factory.ScalarAsync<Guid>("select id from identity.users where email='owner@nile.test'");
            var subject = await factory.ScalarAsync<Guid>($"select id from academics.{(role == "parent" ? "parents" : "students")} where centre_id='{centre}' order by id desc limit 1");
            await factory.ExecuteAsync($"""
                update identity.memberships set role='{role}' where centre_id='{centre}' and user_id='{user}';
                insert into identity.portal_links (id,centre_id,user_id,role_key,parent_id,student_id,created_at,created_by)
                values ('{Guid.NewGuid()}','{centre}','{user}','{role}',{(role == "parent" ? $"'{subject}'" : "null")},{(role == "student" ? $"'{subject}'" : "null")},now(),'{owner}');
                """);
            return await SignInHelper.SignInAsync(factory, "secretary@nile.test");
        }
        return role == "teacher" ? await SignInHelper.SignInAsTwoCentreTeacherAsync(factory, "nile-centre") : await SignInHelper.SignInAsync(factory, role);
    }

    private Task<Guid> ParentAsync(string slug) => factory.ScalarAsync<Guid>($"select p.id from academics.parents p join platform.centres c on c.id=p.centre_id where c.slug='{slug}' order by p.id limit 1");
    private Task<string> MembershipSnapshotAsync() => factory.ScalarAsync<string>("select coalesce(jsonb_agg(to_jsonb(m) order by m.id)::text,'[]') from identity.memberships m");
    private static Uri ListUri(Guid subjectId) => new($"/api/portal-accounts?kind=parent&subjectId={subjectId}", UriKind.Relative);
    private static Uri TransitionUri(Guid linkId, string operation) => new($"/api/portal-accounts/{linkId}/{operation}", UriKind.Relative);
    private static Task<HttpResponseMessage> CreateAsync(HttpClient client, Guid subjectId, string email = "portal-contract@nile.test") =>
        AntiforgeryTestHelper.PostAsJsonAsync(client, AccountsUri, new { kind = "parent", subjectId, email, preferredLocale = "ar" });
    private static async Task<JsonDocument> JsonAsync(HttpResponseMessage response) => JsonDocument.Parse(await response.Content.ReadAsStringAsync());
    private static async Task<JsonElement> AccountAsync(HttpClient client, Guid subjectId)
    {
        using var response = await client.GetAsync(ListUri(subjectId));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var body = await JsonAsync(response);
        return Assert.Single(body.RootElement.GetProperty("items").EnumerateArray()).Clone();
    }
    private static async Task AssertProblemAsync(HttpResponseMessage response, HttpStatusCode status, string code)
    {
        Assert.Equal(status, response.StatusCode);
        using var body = await JsonAsync(response);
        Assert.Equal(code, body.RootElement.GetProperty("code").GetString());
    }
}
