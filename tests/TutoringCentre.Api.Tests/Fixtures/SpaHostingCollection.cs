using System.Diagnostics.CodeAnalysis;

namespace TutoringCentre.Api.Tests.Fixtures;

/// <summary>SPA hosting tests share one factory/container; its web root is fixed for the whole class.</summary>
[SuppressMessage(
    "Naming",
    "CA1711:Identifiers should not have incorrect suffix",
    Justification = "xUnit collection-definition convention; 'Collection' here names an xUnit concept, not .NET's ICollection.")]
[CollectionDefinition(Name)]
public sealed class SpaHostingCollection : ICollectionFixture<SpaHostingFactory>
{
    public const string Name = "spa-hosting";
}
