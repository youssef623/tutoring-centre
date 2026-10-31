using System.Text.Json;
using TutoringCentre.Evaluation.Cases;
namespace TutoringCentre.Evaluation.Reporting;

public static class ReportJson { public static Task WriteAsync(EvalReport report, string path) => File.WriteAllTextAsync(path, JsonSerializer.Serialize(report, CaseLoader.Json)); }
