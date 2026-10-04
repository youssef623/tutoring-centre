using System.Reflection;

namespace TutoringCentre.Architecture.Tests;

internal static class SourceAssemblies
{
    public static Assembly Domain => typeof(TutoringCentre.Domain.AssemblyMarker).Assembly;

    public static Assembly Application => typeof(TutoringCentre.Application.AssemblyMarker).Assembly;

    public static Assembly Infrastructure => typeof(TutoringCentre.Infrastructure.AssemblyMarker).Assembly;

    public static Assembly Api => typeof(Program).Assembly;

    public static IReadOnlyList<Assembly> All => [Domain, Application, Infrastructure, Api];
}
