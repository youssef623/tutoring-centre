using TutoringCentre.Infrastructure.Persistence.Migrations;

namespace TutoringCentre.Infrastructure.Tests.Persistence;

/// <summary>Task 21.3: pure SQL-generation tests for the RLS migration helper — no database involved.</summary>
public sealed class TenantRowLevelSecurityTests
{
    private const string Expression = "centre_id = nullif(current_setting('app.current_centre', true), '')::uuid";

    [Fact]
    public void BuildEnableStatements_ForATable_IssuesEnableForceAndOnePolicyWithBothClausesAndNoToClause()
    {
        var statements = TenantRowLevelSecurity.BuildEnableStatements("probe", "tenant_probes");

        Assert.Equal(
            [
                "alter table probe.tenant_probes enable row level security;",
                "alter table probe.tenant_probes force row level security;",
                $"create policy tenant_isolation on probe.tenant_probes for all using ({Expression}) with check ({Expression});",
            ],
            statements);
    }

    [Fact]
    public void BuildDisableStatements_ForATable_DropsThePolicyThenUnforcesThenDisables()
    {
        var statements = TenantRowLevelSecurity.BuildDisableStatements("probe", "tenant_probes");

        Assert.Equal(
            [
                "drop policy tenant_isolation on probe.tenant_probes;",
                "alter table probe.tenant_probes no force row level security;",
                "alter table probe.tenant_probes disable row level security;",
            ],
            statements);
    }

    [Theory]
    [InlineData("probe'; drop table platform.centres; --")]
    [InlineData("probe; select 1;")]
    [InlineData("")]
    public void BuildEnableStatements_UnsafeSchemaName_Throws(string schema)
    {
        Assert.Throws<ArgumentException>(() => TenantRowLevelSecurity.BuildEnableStatements(schema, "tenant_probes"));
    }

    [Theory]
    [InlineData("tenant_probes'; drop table platform.centres; --")]
    [InlineData("tenant_probes; select 1;")]
    [InlineData("")]
    public void BuildEnableStatements_UnsafeTableName_Throws(string table)
    {
        Assert.Throws<ArgumentException>(() => TenantRowLevelSecurity.BuildEnableStatements("probe", table));
    }
}
