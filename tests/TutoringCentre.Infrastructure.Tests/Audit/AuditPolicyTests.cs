using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Audit;
using TutoringCentre.Infrastructure.Identity;

namespace TutoringCentre.Infrastructure.Tests.Audit;

/// <summary>Task 31.4: the allow-list is asserted literally, never by reflecting over the audited entities —
/// a property the reflection would find is exactly what this policy must be free to leave unaudited.</summary>
public sealed class AuditPolicyTests
{
    [Fact]
    public void Entries_HasExactlyTheThreeAuditedTypes()
    {
        Assert.Equal(3, AuditPolicy.Entries.Count);
        Assert.Contains(typeof(Subject), AuditPolicy.Entries.Keys);
        Assert.Contains(typeof(Membership), AuditPolicy.Entries.Keys);
        Assert.Contains(typeof(Centre), AuditPolicy.Entries.Keys);
    }

    [Fact]
    public void Subject_IsAuditedAsSubject_WithNameAndStatusOnly()
    {
        var entry = AuditPolicy.Entries[typeof(Subject)];

        Assert.Equal(AuditEntityType.Subject, entry.EntityType);
        Assert.Equal(new HashSet<string> { "Name", "Status" }, entry.AllowedFields);
    }

    [Fact]
    public void Membership_IsAuditedAsMembership_WithUserIdRoleAndStatusOnly()
    {
        var entry = AuditPolicy.Entries[typeof(Membership)];

        Assert.Equal(AuditEntityType.Membership, entry.EntityType);
        Assert.Equal(new HashSet<string> { "UserId", "Role", "Status" }, entry.AllowedFields);
    }

    [Fact]
    public void Centre_IsAuditedAsCentre_WithNameAndDefaultLocaleOnly()
    {
        var entry = AuditPolicy.Entries[typeof(Centre)];

        Assert.Equal(AuditEntityType.Centre, entry.EntityType);
        Assert.Equal(new HashSet<string> { "Name", "DefaultLocale" }, entry.AllowedFields);
    }

    [Fact]
    public void Subject_ResolvesItsCentreId_FromItsOwnCentreIdProperty()
    {
        var subject = Subject.Create(Guid.CreateVersion7(), "Maths").Value;
        var entry = AuditPolicy.Entries[typeof(Subject)];

        Assert.Equal(subject.CentreId, entry.ResolveCentreId(subject));
    }

    [Fact]
    public void Membership_ResolvesItsCentreId_FromItsOwnCentreIdProperty()
    {
        var membership = Membership.Create(Guid.CreateVersion7(), Guid.CreateVersion7(), StaffRole.Teacher).Value;
        var entry = AuditPolicy.Entries[typeof(Membership)];

        Assert.Equal(membership.CentreId, entry.ResolveCentreId(membership));
    }

    [Fact]
    public void Centre_ResolvesItsCentreId_FromItsOwnId()
    {
        var centre = Centre.Create("Nile Tutoring Centre", "nile-centre", "Africa/Cairo", SupportedLocale.En).Value;
        var entry = AuditPolicy.Entries[typeof(Centre)];

        Assert.Equal(centre.Id, entry.ResolveCentreId(centre));
    }

    [Fact]
    public void Entries_NeverIncludesAuditEntriesOrIdentityUsers()
    {
        Assert.DoesNotContain(typeof(AuditEntry), AuditPolicy.Entries.Keys);
        Assert.DoesNotContain(typeof(ApplicationUser), AuditPolicy.Entries.Keys);
    }
}
