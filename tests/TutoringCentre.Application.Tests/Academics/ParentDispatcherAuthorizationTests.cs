using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using TutoringCentre.Application.Academics.Parents;
using TutoringCentre.Application.Academics.Parents.Commands.CreateParent;
using TutoringCentre.Application.Academics.Parents.Commands.UpdateParent;
using TutoringCentre.Application.Academics.Parents.Queries.GetParent;
using TutoringCentre.Application.Academics.Parents.Queries.LookupParentByPhone;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Academics;

/// <summary>
/// Through the real dispatcher: Teacher is refused students.manage/students.view for all four parent
/// requests, and a centre-less actor is refused all four before any handler runs — layer 1 (tenant scope) and
/// the permission step, both proven before any transaction, mirroring Subject's Day 24/28 proofs.
/// </summary>
public sealed class ParentDispatcherAuthorizationTests
{
    private static readonly Guid CentreId = Guid.CreateVersion7();
    private static readonly StaffActor CentrelessActor = new(Guid.NewGuid(), CentreId: null, Role: null);

    [Fact]
    public async Task SendAsync_CreateParent_ReturnsPermissionDeniedForTeacher()
    {
        using var fixture = CreateSut(StaffRole.Teacher);

        var result = await fixture.Dispatcher.SendAsync<CreateParentCommand, CreateParentResult>(
            new CreateParentCommand("Omar Hassan", "01012345678"), CancellationToken.None);

        AssertPermissionDenied(result, fixture);
        Assert.Empty(fixture.ParentRepository.Added);
    }

    [Fact]
    public async Task SendAsync_UpdateParent_ReturnsPermissionDeniedForTeacher()
    {
        using var fixture = CreateSut(StaffRole.Teacher);

        var result = await fixture.Dispatcher.SendAsync<UpdateParentCommand, Unit>(
            new UpdateParentCommand(fixture.ExistingParent.Id, "Omar Hassan Ali", "01012345678", 0), CancellationToken.None);

        AssertPermissionDenied(result, fixture);
    }

    [Fact]
    public async Task QueryAsync_GetParent_ReturnsPermissionDeniedForTeacher()
    {
        using var fixture = CreateSut(StaffRole.Teacher);

        var result = await fixture.Dispatcher.QueryAsync<GetParentQuery, ParentDetailDto>(
            new GetParentQuery(fixture.ExistingParent.Id), CancellationToken.None);

        AssertPermissionDenied(result, fixture);
    }

    [Fact]
    public async Task QueryAsync_LookupParentByPhone_ReturnsPermissionDeniedForTeacher()
    {
        using var fixture = CreateSut(StaffRole.Teacher);

        var result = await fixture.Dispatcher.QueryAsync<LookupParentByPhoneQuery, ParentLookupDto>(
            new LookupParentByPhoneQuery("01012345678"), CancellationToken.None);

        AssertPermissionDenied(result, fixture);
    }

    [Theory]
    [InlineData(StaffRole.Secretary)]
    [InlineData(StaffRole.Owner)]
    public async Task SendAsync_CreateParent_SucceedsForSecretaryAndOwner(StaffRole role)
    {
        using var fixture = CreateSut(role);

        var result = await fixture.Dispatcher.SendAsync<CreateParentCommand, CreateParentResult>(
            new CreateParentCommand("Amal Fathy", "01212345678"), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task SendAsync_CreateParent_ReturnsTenantNotSelectedForCentrelessActor()
    {
        using var fixture = CreateSut(CentrelessActor);

        var result = await fixture.Dispatcher.SendAsync<CreateParentCommand, CreateParentResult>(
            new CreateParentCommand("Omar Hassan", "01012345678"), CancellationToken.None);

        AssertTenantNotSelected(result, fixture);
        Assert.Empty(fixture.ParentRepository.Added);
    }

    [Fact]
    public async Task SendAsync_UpdateParent_ReturnsTenantNotSelectedForCentrelessActor()
    {
        using var fixture = CreateSut(CentrelessActor);

        var result = await fixture.Dispatcher.SendAsync<UpdateParentCommand, Unit>(
            new UpdateParentCommand(Guid.CreateVersion7(), "Omar Hassan", "01012345678", 0), CancellationToken.None);

        AssertTenantNotSelected(result, fixture);
    }

    [Fact]
    public async Task QueryAsync_GetParent_ReturnsTenantNotSelectedForCentrelessActor()
    {
        using var fixture = CreateSut(CentrelessActor);

        var result = await fixture.Dispatcher.QueryAsync<GetParentQuery, ParentDetailDto>(
            new GetParentQuery(Guid.CreateVersion7()), CancellationToken.None);

        AssertTenantNotSelected(result, fixture);
    }

    [Fact]
    public async Task QueryAsync_LookupParentByPhone_ReturnsTenantNotSelectedForCentrelessActor()
    {
        using var fixture = CreateSut(CentrelessActor);

        var result = await fixture.Dispatcher.QueryAsync<LookupParentByPhoneQuery, ParentLookupDto>(
            new LookupParentByPhoneQuery("01012345678"), CancellationToken.None);

        AssertTenantNotSelected(result, fixture);
    }

    private static void AssertPermissionDenied<T>(Result<T> result, Fixture fixture)
    {
        Assert.True(result.IsFailure);
        Assert.Equal("auth.permission_denied", result.Error!.Code);
        Assert.Equal(ErrorKind.Forbidden, result.Error.Kind);
        Assert.Empty(fixture.UnitOfWork.Calls);
    }

    private static void AssertTenantNotSelected<T>(Result<T> result, Fixture fixture)
    {
        Assert.True(result.IsFailure);
        Assert.Equal("tenant.not_selected", result.Error!.Code);
        Assert.Equal(ErrorKind.Forbidden, result.Error.Kind);
        Assert.Empty(fixture.UnitOfWork.Calls);
    }

    private static Fixture CreateSut(StaffRole role) => CreateSut(new StaffActor(Guid.NewGuid(), CentreId, role));

    private static Fixture CreateSut(StaffActor actor)
    {
        var unitOfWork = new FakeUnitOfWork();
        var currentActor = new FakeCurrentActor { Actor = actor };
        var parentRepository = new FakeParentRepository();
        var parentReadService = new FakeParentReadService();

        var existingParent = Parent.Create(CentreId, "Existing Parent", PhoneNumber.Create("01512345678").Value).Value;
        parentRepository.Existing.Add(existingParent);
        parentReadService.Details.Add(new ParentDetailDto(existingParent.Id, existingParent.FullName, existingParent.Phone.Value, 0, []));

        var provider = new ServiceCollection()
            .AddSingleton(unitOfWork)
            .AddSingleton<ICurrentActor>(currentActor)
            .AddSingleton<IParentRepository>(parentRepository)
            .AddSingleton<IParentReadService>(parentReadService)
            .AddScoped<ICommandHandler<CreateParentCommand, CreateParentResult>, CreateParentHandler>()
            .AddScoped<ICommandHandler<UpdateParentCommand, Unit>, UpdateParentHandler>()
            .AddScoped<IQueryHandler<GetParentQuery, ParentDetailDto>, GetParentHandler>()
            .AddScoped<IQueryHandler<LookupParentByPhoneQuery, ParentLookupDto>, LookupParentByPhoneHandler>()
            .AddScoped<IValidator<CreateParentCommand>, CreateParentValidator>()
            .AddScoped<IValidator<UpdateParentCommand>, UpdateParentValidator>()
            .AddScoped<IValidator<LookupParentByPhoneQuery>, LookupParentByPhoneValidator>()
            .BuildServiceProvider();

        return new Fixture(
            new Dispatcher(provider, unitOfWork, NullLogger<Dispatcher>.Instance), unitOfWork, parentRepository, existingParent, provider);
    }

    private sealed class Fixture(
        Dispatcher dispatcher, FakeUnitOfWork unitOfWork, FakeParentRepository parentRepository, Parent existingParent, ServiceProvider provider)
        : IDisposable
    {
        public Dispatcher Dispatcher { get; } = dispatcher;

        public FakeUnitOfWork UnitOfWork { get; } = unitOfWork;

        public FakeParentRepository ParentRepository { get; } = parentRepository;

        public Parent ExistingParent { get; } = existingParent;

        public void Dispose() => provider.Dispose();
    }
}
