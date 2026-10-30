using System.ComponentModel.DataAnnotations;

namespace TutoringCentre.Application.Assistant;

public sealed class AssistantOptions
{
    [Range(1, 100)] public int MaxSteps { get; set; } = 4;
    [Range(1, 1000000)] public int MaxTotalTokens { get; set; } = 6000;
    public TimeSpan TotalRunTimeout { get; set; } = TimeSpan.FromSeconds(60);
    public TimeSpan CallTimeout { get; set; } = TimeSpan.FromSeconds(20);
    [Range(1, 1000)] public int MaxReplyCharacters { get; set; } = 800;
    [Range(1, 1000)] public int HistoryMessages { get; set; } = 10;
}
