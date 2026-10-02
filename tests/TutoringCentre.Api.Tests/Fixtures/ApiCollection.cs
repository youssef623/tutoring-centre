using System.Diagnostics.CodeAnalysis;

namespace TutoringCentre.Api.Tests.Fixtures;

/// <summary>All database-backed API tests share one factory/container; they run sequentially.</summary>
[SuppressMessage(
    "Naming",
    "CA1711:Identifiers should not have incorrect suffix",
    Justification = "xUnit collection-definition convention; 'Collection' here names an xUnit concept, not .NET's ICollection.")]
[CollectionDefinition(Name)]
public sealed class ApiCollection : ICollectionFixture<ApiFactory>
{
    public const string Name = "api";
}
