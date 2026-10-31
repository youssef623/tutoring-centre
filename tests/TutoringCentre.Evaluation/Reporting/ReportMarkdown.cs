using System.Globalization;
using System.Text;
namespace TutoringCentre.Evaluation.Reporting;

public static class ReportMarkdown
{
    public static string Render(EvalReport report)
    {
        var b = new StringBuilder($"# Evaluation report\n\nMode: {report.Mode}; prompt: {report.PromptVersion}; model: {report.Model}; date: {report.Date:O}; cases: {report.CaseCount}/{report.DatasetCaseCount}.\n\nScope: {report.Scope}.\n\n");
        if (report.Mode == "replay") b.AppendLine("replay tests the system around the model, not the model\n");
        b.AppendLine("## Security gates\n\n| Gate | Applicable | Failed cases |\n|---|---:|---|");
        foreach (var g in report.Gates) b.AppendLine(CultureInfo.InvariantCulture, $"| {g.Gate} | {g.Applicable} | {string.Join(", ", g.FailedCases)} |");
        void Table(string title, RateRow[] rows) { b.AppendLine(CultureInfo.InvariantCulture, $"\n## {title}\n\n| Group | Cases | Applicable | Passed | Quality rate |\n|---|---:|---:|---:|---:|"); foreach (var r in rows) b.AppendLine(CultureInfo.InvariantCulture, $"| {r.Key} | {r.Cases} | {r.Applicable} | {r.Passed} | {(r.Rate is double rate ? rate.ToString("P1", CultureInfo.InvariantCulture) : "N/A")} |"); }
        Table("Quality overall", [report.Overall]); Table("Language style", report.LanguageStyles); Table("Category", report.Categories);
        b.AppendLine("\n## Cases\n\n| Case | Passed | Gate failed | Failures |\n|---|---|---|---|");
        foreach (var c in report.Cases) b.AppendLine(CultureInfo.InvariantCulture, $"| {c.CaseId} | {c.Passed} | {c.GateFailed} | {string.Join(", ", c.Results.Where(r => r.State == Scoring.ScoreState.Fail).Select(r => r.Scorer + ":" + r.Code))} |");
        return b.ToString();
    }
}
