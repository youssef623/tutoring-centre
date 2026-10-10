using TutoringCentre.Application.Audit;
using TutoringCentre.Application.Audit.Queries.GetAuditLog;

namespace TutoringCentre.Application.Tests.Audit;

public sealed class GetAuditLogValidatorTests
{
    private readonly GetAuditLogValidator _validator = new();

    [Fact]
    public void Validate_WithEveryFilterOmitted_Succeeds()
    {
        var result = _validator.Validate(new GetAuditLogQuery(null, null, null, null, null, null, 50));

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    public void Validate_WithPageSizeOutOfRange_Fails(int pageSize)
    {
        var result = _validator.Validate(new GetAuditLogQuery(null, null, null, null, null, null, pageSize));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, failure => failure.PropertyName == nameof(GetAuditLogQuery.PageSize));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    public void Validate_WithPageSizeAtBoundary_Succeeds(int pageSize)
    {
        var result = _validator.Validate(new GetAuditLogQuery(null, null, null, null, null, null, pageSize));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithFromAfterTo_Fails()
    {
        var now = DateTimeOffset.UtcNow;
        var result = _validator.Validate(new GetAuditLogQuery(null, null, null, now, now.AddDays(-1), null, 50));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, failure => failure.PropertyName == nameof(GetAuditLogQuery.From));
    }

    [Fact]
    public void Validate_WithFromEqualToTo_Succeeds()
    {
        var now = DateTimeOffset.UtcNow;
        var result = _validator.Validate(new GetAuditLogQuery(null, null, null, now, now, null, 50));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithUnknownEntityType_Fails()
    {
        var result = _validator.Validate(new GetAuditLogQuery("not-a-real-type", null, null, null, null, null, 50));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, failure => failure.PropertyName == nameof(GetAuditLogQuery.EntityType));
    }

    [Theory]
    [InlineData("subject")]
    [InlineData("membership")]
    [InlineData("centre")]
    public void Validate_WithKnownEntityType_Succeeds(string entityType)
    {
        var result = _validator.Validate(new GetAuditLogQuery(entityType, null, null, null, null, null, 50));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithEntityIdButNoEntityType_Fails()
    {
        var result = _validator.Validate(new GetAuditLogQuery(null, Guid.CreateVersion7(), null, null, null, null, 50));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, failure => failure.PropertyName == nameof(GetAuditLogQuery.EntityType));
    }

    [Fact]
    public void Validate_WithEntityIdAndEntityType_Succeeds()
    {
        var result = _validator.Validate(new GetAuditLogQuery("subject", Guid.CreateVersion7(), null, null, null, null, 50));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithGarbageCursor_Fails()
    {
        var result = _validator.Validate(new GetAuditLogQuery(null, null, null, null, null, "not-a-valid-cursor!!", 50));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, failure => failure.PropertyName == nameof(GetAuditLogQuery.Cursor));
    }

    [Fact]
    public void Validate_WithWellFormedCursor_Succeeds()
    {
        var cursor = AuditCursor.Encode(DateTimeOffset.UtcNow, Guid.CreateVersion7());
        var result = _validator.Validate(new GetAuditLogQuery(null, null, null, null, null, cursor, 50));

        Assert.True(result.IsValid);
    }
}
