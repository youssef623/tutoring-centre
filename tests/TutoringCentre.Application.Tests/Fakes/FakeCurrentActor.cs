using TutoringCentre.Application.Common.Security;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>A settable stand-in for <see cref="ICurrentActor"/>; starts anonymous like the real context does.</summary>
public sealed class FakeCurrentActor : ICurrentActor
{
    public Actor Actor { get; set; } = new AnonymousActor();
}
