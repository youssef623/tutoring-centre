using System.Security.Cryptography;
using System.Text.Json;

namespace TutoringCentre.Evaluation.Runner;

public static class TranscriptFingerprint
{
    // Logical content only: raw transport IDs/timings/logs and transient handles are deliberately private.
    public static string Create(Transcript transcript, IReadOnlyList<string> toolResults, IReadOnlyList<string> guards)
    {
        var content = new
        {
            transcript.CaseId,
            transcript.Replies,
            Calls = transcript.Calls.Select(call => new
            {
                call.Turn, call.Name, call.Decision, call.Code,
                Child = transcript.Handles.SingleOrDefault(h => h.Turn == call.Turn && h.Handle == call.Handle)?.ChildKey
            }),
            ToolResults = toolResults,
            Guards = guards,
            Errors = transcript.Errors.Distinct(StringComparer.Ordinal).Order(StringComparer.Ordinal),
            transcript.OutputCaptureComplete,
            BusinessUnchanged = transcript.Before is not null && transcript.After is not null
                && transcript.Before.OrderBy(x => x.Key, StringComparer.Ordinal).SequenceEqual(transcript.After.OrderBy(x => x.Key, StringComparer.Ordinal))
        };
        return Convert.ToHexString(SHA256.HashData(JsonSerializer.SerializeToUtf8Bytes(content)));
    }
}
