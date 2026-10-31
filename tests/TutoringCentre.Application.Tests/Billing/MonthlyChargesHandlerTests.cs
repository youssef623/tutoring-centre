using TutoringCentre.Domain.Common;
using TutoringCentre.Application.Academics.Contracts;
using TutoringCentre.Application.Billing;
using TutoringCentre.Application.Billing.Commands.CreateMonthlyCharges;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Billing;
namespace TutoringCentre.Application.Tests.Billing;
public sealed class MonthlyChargesHandlerTests
{
    private readonly Guid _centre = Guid.NewGuid();
    [Fact]
    public async Task NullMonthUsesCairoAndSnapshotsFacts()
    {
        var roster = new Roster(); var charges = new Charges();
        var handler = Handler(roster, charges); var result = await handler.HandleAsync(new(), CancellationToken.None);
        Assert.True(result.IsSuccess); Assert.Equal(new(1, 0), result.Value); Assert.Equal("2026-11", roster.Month.ToString());
        var row = Assert.Single(charges.Items); Assert.Equal("300.00", row.Amount.ToString()); Assert.Equal(_centre, row.CentreId); Assert.Null(row.CreatedBy);
    }
    [Fact]
    public async Task KnownAndRacedDuplicatesAreSkipped()
    {
        var roster = new Roster(); var charges = new Charges { Existing = true };
        Assert.Equal(new(0, 1), (await Handler(roster, charges).HandleAsync(new(), CancellationToken.None)).Value); Assert.Empty(charges.Items);
        charges.Existing = false; charges.Raced = true;
        Assert.Equal(new(0, 1), (await Handler(roster, charges).HandleAsync(new(), CancellationToken.None)).Value);
    }
    [Fact]
    public async Task UnrelatedErrorsAreNotSwallowed()
    {
        var charges = new Charges { Throw = true };
        await Assert.ThrowsAsync<InvalidOperationException>(() => Handler(new(), charges).HandleAsync(new(), CancellationToken.None));
    }
    private CreateMonthlyChargesHandler Handler(Roster roster, Charges charges) => new(new FakeCurrentActor { Actor = new SystemActor(_centre) },
        new FakeClock { UtcNow = new(2026, 10, 31, 22, 30, 0, TimeSpan.Zero) }, new FakeCentreTimeZoneProvider(), roster, charges);
    private sealed class Roster : IBillingRosterSource
    {
        public BillingMonth Month { get; private set; }
        public Task<IReadOnlyList<ChargeableEnrollment>> ListChargeableAsync(BillingMonth month, CancellationToken ct)
        { Month = month; return Task.FromResult<IReadOnlyList<ChargeableEnrollment>>([new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Money.Create(300).Value)]); }
        public Task<ChargeableEnrollment?> GetChargeableAsync(Guid id, BillingMonth month, CancellationToken ct) => throw new NotSupportedException();
        public Task<bool> StudentExistsAsync(Guid id, CancellationToken ct) => throw new NotSupportedException();
        public Task<IReadOnlyDictionary<Guid, BillingNames>> GetGroupNamesAsync(IReadOnlyCollection<Guid> ids, CancellationToken ct) => throw new NotSupportedException();
    }
    private sealed class Charges : IChargeRepository
    {
        public bool Existing { get; set; } public bool Raced { get; set; } public bool Throw { get; set; }
        public List<Charge> Items { get; } = [];
        public Task<bool> ExistsForMonthAsync(Guid studentId, Guid subjectId, BillingMonth month, CancellationToken ct) => Task.FromResult(Existing);
        public void Add(Charge charge) => Items.Add(charge);
        public Task<bool> TryAddMonthlyAsync(Charge charge, CancellationToken ct) { if (Throw) throw new InvalidOperationException("probe"); if (!Raced) Items.Add(charge); return Task.FromResult(!Raced); }
    }
}
