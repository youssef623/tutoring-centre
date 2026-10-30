using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Application.Assistant.Handles;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Academics.Students.Commands.UnlinkParent;
using TutoringCentre.Domain.Common;
using TutoringCentre.Infrastructure.Tests.Fixtures;
using TutoringCentre.Infrastructure.Tests.Tenancy;
using Xunit.Abstractions;
namespace TutoringCentre.Infrastructure.Tests.Assistant;
public sealed class HandleSetTests(PostgresFixture fixture,ITestOutputHelper output) : TenantProbeTestBase(fixture)
{
    [Fact] public async Task RealAuthorizationSetIsPerFamilyPerTenantAndPerRun()
    {
        var first=await Fixture.SeedParentAsync(NileCentreId,"First","+201000000051");
        var second=await Fixture.SeedParentAsync(NileCentreId,"Second","+201000000052");
        var ahmed=await Fixture.SeedStudentAsync(NileCentreId,"Ahmed");
        var salma=await Fixture.SeedStudentAsync(NileCentreId,"Salma");
        var omar=await Fixture.SeedStudentAsync(NileCentreId,"Omar");
        var inactive=await Fixture.SeedStudentAsync(NileCentreId,"Inactive","archived");
        foreach(var child in new[]{ahmed,salma,inactive}) await Fixture.SeedStudentParentLinkAsync(NileCentreId,child,first,true);
        await Fixture.SeedStudentParentLinkAsync(NileCentreId,omar,second,true);
        var actor=new ParentActor(Guid.NewGuid(),NileCentreId,first);
        var one=await Mint(actor); var two=await Mint(new ParentActor(Guid.NewGuid(),NileCentreId,second));
        Assert.Equal(2,one.Children.Count); Assert.Equal(ahmed,one.Resolve("child_1").Value); Assert.Equal(salma,one.Resolve("child_2").Value);
        Assert.Equal(omar,two.Resolve("child_1").Value); Assert.True(two.Resolve("child_2").IsFailure);
        Assert.DoesNotContain(two.Children.Values,x=>x.Id==ahmed || x.Id==salma);
        Assert.Equal(one.Children,(await Mint(actor)).Children);
        Assert.Equal(ahmed,(await Mint(new StudentActor(Guid.NewGuid(),NileCentreId,ahmed))).Resolve("me").Value);
        Assert.Empty((await Mint(new ParentActor(Guid.NewGuid(),MaadiCentreId,first))).Children);
        var empty=await Fixture.SeedParentAsync(NileCentreId,"Empty","+201000000053");
        Assert.Empty((await Mint(new ParentActor(Guid.NewGuid(),NileCentreId,empty))).Children);
        output.WriteLine(string.Join(", ",one.Children.Select(x=>$"{x.Key}: {x.Value.Name}")));
    }
    [Fact] public async Task MonthThreeUnlinkRemovesHandleOnNextRun()
    {
        var primary=await Fixture.SeedParentAsync(NileCentreId,"Primary","+201000000055");
        var other=await Fixture.SeedParentAsync(NileCentreId,"Other","+201000000056");
        var child=await Fixture.SeedStudentAsync(NileCentreId,"Ahmed");
        await Fixture.SeedStudentParentLinkAsync(NileCentreId,child,primary,true);
        await Fixture.SeedStudentParentLinkAsync(NileCentreId,child,other,false);
        var actor=new ParentActor(Guid.NewGuid(),NileCentreId,other);
        Assert.Single((await Mint(actor)).Children);
        var version=await Fixture.ScalarAsync<uint>($"select xmin from academics.students where id='{child}'");
        var removed=await Fixture.SendAsAsync<UnlinkParentCommand,Unit>(new SystemActor(NileCentreId),new(child,other,version));
        Assert.True(removed.IsSuccess,removed.Error?.Message);
        Assert.Empty((await Mint(actor)).Children);
    }
    private async Task<HandleTable> Mint(Actor actor)
    {
        var result=await Fixture.QueryAsAsync<GetRunHandlesQuery,HandleTable>(actor,new());
        Assert.True(result.IsSuccess,result.Error?.Message); return result.Value;
    }
}
