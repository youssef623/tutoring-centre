using TutoringCentre.Evaluation.Runner;

namespace TutoringCentre.Evaluation.Tests;

public sealed class TranscriptFingerprintTests
{
    [Fact]
    public void FreshIdsAndHandlesDoNotChangeLogicalContent()
    {
        var first = ScorerTests.Good;
        var next = first with
        {
            Handles = [first.Handles[0] with { StudentId = Guid.NewGuid(), Handle = "child_99" }],
            Calls = [first.Calls[0] with { Handle = "child_99" }],
            ProducedOutputs = ["different volatile transport ID"]
        };
        Assert.Equal(TranscriptFingerprint.Create(first, ["same facts"], []), TranscriptFingerprint.Create(next, ["same facts"], []));
    }

    [Fact]
    public void WrongIntendedChildChangesContent()
    {
        var first = ScorerTests.Good;
        var next = first with { Calls = [first.Calls[0] with { Handle = "child_2" }] };
        Assert.NotEqual(TranscriptFingerprint.Create(first, [], []), TranscriptFingerprint.Create(next, [], []));
    }

    [Fact]
    public void ToolFactsAndGuardReasonsAffectContent()
    {
        var transcript = ScorerTests.Good;
        var key = TranscriptFingerprint.Create(transcript, ["present"], []);
        Assert.NotEqual(key, TranscriptFingerprint.Create(transcript, ["absent"], []));
        Assert.NotEqual(key, TranscriptFingerprint.Create(transcript, ["present"], ["guard.foreign_name"]));
    }
}
