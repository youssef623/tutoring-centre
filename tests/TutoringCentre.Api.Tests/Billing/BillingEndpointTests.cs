using System.Net;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Application.Billing.Commands.CreateMonthlyCharges;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;
namespace TutoringCentre.Api.Tests.Billing;

[Collection(ApiCollection.Name)]
public sealed class BillingEndpointTests(ApiFactory factory) : IAsyncLifetime
{
    public async Task InitializeAsync() { await factory.ResetAsync(); Assert.Equal(0, await SeedCommand.RunAsync(factory.Services)); }
    public Task DisposeAsync() { factory.Time.UseRealTime(); return Task.CompletedTask; }
    private static async Task<JsonElement> Json(HttpResponseMessage response)
    { using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync()); return document.RootElement.Clone(); }
    private static async Task<JsonElement> Group(HttpClient client, Guid id)
    { using var response = await client.GetAsync(new Uri($"/api/groups/{id}", UriKind.Relative)); response.EnsureSuccessStatusCode(); return await Json(response); }
    private async Task<Guid> Create(HttpClient client, decimal? fee)
    {
        using var groups = await client.GetAsync(new Uri("/api/groups", UriKind.Relative)); var first = (await Json(groups)).GetProperty("items")[0];
        var teacherId = await factory.ScalarAsync<Guid>("select t.id from academics.teachers t join platform.centres c on c.id=t.centre_id where c.slug='nile-centre' and t.user_id is not null order by t.id limit 1");
        var body = new Dictionary<string, object> { ["name"] = "EVAL Billing Group", ["subjectId"] = first.GetProperty("subjectId").GetGuid(), ["teacherId"] = teacherId, ["capacity"] = 20 };
        if (fee is { } value) body["monthlyFee"] = value;
        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(client, new Uri("/api/groups", UriKind.Relative), body);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode); return (await Json(response)).GetProperty("id").GetGuid();
    }
    [Fact]
    public async Task OmittedCreateFeeIsZeroAndOmittedUpdateFeePreservesValue()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory); var id = await Create(owner.Client, null); var before = await Group(owner.Client, id);
        Assert.Equal("0.00", before.GetProperty("monthlyFee").GetString());
        using var update = await AntiforgeryTestHelper.SendAsJsonAsync(owner.Client, HttpMethod.Put, new Uri($"/api/groups/{id}", UriKind.Relative), new { name = "EVAL Billing Renamed", version = before.GetProperty("version").GetUInt32(), monthlyFee = 300.00m });
        Assert.Equal(HttpStatusCode.NoContent, update.StatusCode); var priced = await Group(owner.Client, id);
        using var rename = await AntiforgeryTestHelper.SendAsJsonAsync(owner.Client, HttpMethod.Put, new Uri($"/api/groups/{id}", UriKind.Relative), new { name = "EVAL Billing Final", version = priced.GetProperty("version").GetUInt32() });
        Assert.Equal(HttpStatusCode.NoContent, rename.StatusCode); Assert.Equal("300.00", (await Group(owner.Client, id)).GetProperty("monthlyFee").GetString());
        Assert.Equal("0.00|300.00", await factory.ScalarAsync<string>($"select (changes->'monthlyFee'->>'before')||'|'||(changes->'monthlyFee'->>'after') from audit.audit_entries where entity_id='{id}' and action='updated' and changes ? 'monthlyFee'"));
    }
    [Theory][InlineData("-5")][InlineData("10.123")][InlineData("100000000")]
    public async Task InvalidFeeIs400WithoutMutation(string rawFee)
    {
        var fee = decimal.Parse(rawFee, System.Globalization.CultureInfo.InvariantCulture);
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory); var id = await Create(owner.Client, 300); var group = await Group(owner.Client, id);
        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(owner.Client, HttpMethod.Put, new Uri($"/api/groups/{id}", UriKind.Relative), new { name = "EVAL Billing Group", version = group.GetProperty("version").GetUInt32(), monthlyFee = fee });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode); Assert.Equal("300.00", (await Group(owner.Client, id)).GetProperty("monthlyFee").GetString());
    }
    [Fact]
    public async Task TeacherSeesNeitherFeePropertyNorEditCapability()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory); var id = await Create(owner.Client, 300);
        using var teacher = await SignInHelper.SignInAsTwoCentreTeacherAsync(factory, "nile-centre"); var group = await Group(teacher.Client, id);
        Assert.False(group.TryGetProperty("monthlyFee", out _));
        using var list = await teacher.Client.GetAsync(new Uri("/api/groups", UriKind.Relative)); Assert.All((await Json(list)).GetProperty("items").EnumerateArray(), row => Assert.False(row.TryGetProperty("monthlyFee", out _)));
        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(teacher.Client, HttpMethod.Put, new Uri($"/api/groups/{id}", UriKind.Relative), new { name = "EVAL Billing Group", version = group.GetProperty("version").GetUInt32(), monthlyFee = 1 });
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode); Assert.Equal("300.00", (await Group(owner.Client, id)).GetProperty("monthlyFee").GetString());
    }
    [Fact]
    public async Task ChargeContractHasTwoPlaceStringsAndCalendarDates()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory); var groupId = await Create(owner.Client, 300);
        var centre = await factory.ScalarAsync<Guid>("select id from platform.centres where slug='nile-centre'");
        var student = await factory.ScalarAsync<Guid>($"select id from academics.students where centre_id='{centre}' order by id limit 1");
        using var enroll = await AntiforgeryTestHelper.PostAsJsonAsync(owner.Client, new Uri($"/api/groups/{groupId}/enrollments", UriKind.Relative), new { studentId = student });
        Assert.Equal(HttpStatusCode.Created, enroll.StatusCode);
        var result = await factory.Services.GetRequiredService<ISystemScopeRunner>().RunAsync(centre, "test:billing", (services, ct) => services.GetRequiredService<Dispatcher>().SendAsync<CreateMonthlyChargesCommand, CreateMonthlyChargesResult>(new(), ct));
        Assert.True(result.IsSuccess, result.Error?.Code);
        using var response = await owner.Client.GetAsync(new Uri($"/api/students/{student}/charges", UriKind.Relative)); Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var row = Assert.Single((await Json(response)).GetProperty("items").EnumerateArray());
        Assert.Equal(JsonValueKind.String, row.GetProperty("amount").ValueKind); Assert.Equal("300.00", row.GetProperty("amount").GetString());
        Assert.Matches("^[0-9]{4}-[0-9]{2}$", row.GetProperty("billingMonth").GetString()!);
        Assert.Matches("^[0-9]{4}-[0-9]{2}-05$", row.GetProperty("dueDate").GetString()!);
        Assert.False(row.TryGetProperty("paid", out _)); Assert.False(row.TryGetProperty("outstanding", out _));
        using var invalid = await owner.Client.GetAsync(new Uri($"/api/students/{student}/charges?cursor=broken", UriKind.Relative)); Assert.Equal(HttpStatusCode.BadRequest, invalid.StatusCode);
    }
    [Theory][InlineData("owner@nile.test", "maadi-hub")][InlineData("owner@maadi.test", "nile-centre")]
    public async Task ForeignAndUnknownStudentsReturnSame404(string acting, string other)
    {
        using var owner = await SignInHelper.SignInAsync(factory, acting);
        var foreignId = await factory.ScalarAsync<Guid>($"select s.id from academics.students s join platform.centres c on c.id=s.centre_id where c.slug='{other}' order by s.id limit 1");
        using var foreign = await owner.Client.GetAsync(new Uri($"/api/students/{foreignId}/charges", UriKind.Relative));
        using var missing = await owner.Client.GetAsync(new Uri($"/api/students/{Guid.NewGuid()}/charges", UriKind.Relative));
        Assert.Equal(HttpStatusCode.NotFound, foreign.StatusCode); Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);
        Assert.Equal((await Json(missing)).GetProperty("code").GetString(), (await Json(foreign)).GetProperty("code").GetString());
    }
}
