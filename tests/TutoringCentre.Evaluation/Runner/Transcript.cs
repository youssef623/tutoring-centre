using TutoringCentre.Api.Tests.Support;
namespace TutoringCentre.Evaluation.Runner;

public sealed record ObservedCall(int Turn, string Name, string? Handle, string Decision, string? Code);
public sealed record HandleAssignment(int Turn, string ChildKey, Guid StudentId, string Handle);
public sealed record Transcript(string CaseId, string[] Replies, ObservedCall[] Calls, HandleAssignment[] Handles, string[] ProducedOutputs, string[] WorldCanaries, IReadOnlyDictionary<string, TableFingerprint>? Before, IReadOnlyDictionary<string, TableFingerprint>? After, bool OutputCaptureComplete, string[] Errors)
{
    public string? ContentFingerprint { get; init; }
}
