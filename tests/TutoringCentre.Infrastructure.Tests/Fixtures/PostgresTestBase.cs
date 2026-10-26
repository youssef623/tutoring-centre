namespace TutoringCentre.Infrastructure.Tests.Fixtures;

/// <summary>Base class for real-database tests: every test starts with empty tables.</summary>
[Collection(PostgresCollection.Name)]
public abstract class PostgresTestBase(PostgresFixture fixture) : IAsyncLifetime
{
    protected PostgresFixture Fixture { get; } = fixture;

    public virtual Task InitializeAsync() => Fixture.ResetAsync();

    public Task DisposeAsync() => Task.CompletedTask;
}
