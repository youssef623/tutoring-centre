using TutoringCentre.Application.Academics.Parents;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>In-memory stand-in for parent reads.</summary>
public sealed class FakeParentReadService : IParentReadService
{
    public List<ParentDetailDto> Details { get; } = [];

    public List<ParentDto> ByPhone { get; } = [];

    public Task<ParentDetailDto?> GetDetailAsync(Guid id, CancellationToken ct) =>
        Task.FromResult(Details.FirstOrDefault(parent => parent.Id == id));

    public Task<ParentDto?> FindByPhoneAsync(PhoneNumber phone, CancellationToken ct) =>
        Task.FromResult(ByPhone.FirstOrDefault(parent => parent.Phone == phone.Value));
}
