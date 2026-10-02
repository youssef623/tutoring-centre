using System.Diagnostics.CodeAnalysis;

namespace TutoringCentre.Infrastructure.Tests.Fixtures;

/// <summary>All real-database tests share one container (started once per run); tests in a collection run sequentially.</summary>
[SuppressMessage(
    "Naming",
    "CA1711:Identifiers should not have incorrect suffix",
    Justification = "xUnit collection-definition convention; 'Collection' here names an xUnit concept, not .NET's ICollection.")]
[CollectionDefinition(Name)]
public sealed class PostgresCollection : ICollectionFixture<PostgresFixture>
{
    public const string Name = "postgres";
}
