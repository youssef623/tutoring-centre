using System.Diagnostics.CodeAnalysis;

namespace TutoringCentre.Api.Tests.Fixtures;

/// <summary>Forwarded-header and forwarded-IP-rate-limit tests share one factory/container; they run sequentially.</summary>
[SuppressMessage(
    "Naming",
    "CA1711:Identifiers should not have incorrect suffix",
    Justification = "xUnit collection-definition convention; 'Collection' here names an xUnit concept, not .NET's ICollection.")]
[CollectionDefinition(Name)]
public sealed class ForwardedHeadersCollection : ICollectionFixture<ForwardedHeadersFactory>
{
    public const string Name = "forwarded-headers";
}
