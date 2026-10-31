using System.Globalization;
using TutoringCentre.Api.Tests.Fixtures;
namespace TutoringCentre.Evaluation.Runner;
// Reuses the shared structured sink, including exception/structured-property rendering.
public static class LogCapture
{
    public static string[] Read(InMemoryLogSink sink) => sink.Snapshot().Select(entry => entry.RenderMessage(CultureInfo.InvariantCulture) + entry.ToString()).ToArray();
}
