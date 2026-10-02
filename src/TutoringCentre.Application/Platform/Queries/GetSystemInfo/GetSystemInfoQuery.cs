using TutoringCentre.Application.Common.Cqrs;

namespace TutoringCentre.Application.Platform.Queries.GetSystemInfo;

/// <summary>Asks for the application version and database migration status.</summary>
public sealed record GetSystemInfoQuery : IQuery<SystemInfoDto>;
