namespace TutoringCentre.Api.Tests.Fixtures;

public class ApiFactory : SharedApiFactory, IAsyncLifetime { Task IAsyncLifetime.DisposeAsync() => DisposeAsync().AsTask(); }
