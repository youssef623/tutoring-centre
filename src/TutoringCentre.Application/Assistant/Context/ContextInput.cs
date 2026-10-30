using TutoringCentre.Application.Assistant.Models;
namespace TutoringCentre.Application.Assistant.Context;
public sealed record ContextInput(string DisplayName, string RoleKind, DateOnly LocalDate, string TimeZone,
    IReadOnlyList<ModelMessage> History, string InboundText, TutoringCentre.Application.Assistant.Handles.HandleTable? Handles = null);
public sealed record ContextOutput(string System, IReadOnlyList<ModelMessage> Messages);
