namespace TutoringCentre.Application.Platform;

/// <summary>
/// Whether this deployment is a demo instance (Task 35.7). A port: the Application layer reads this flag
/// without knowing it ultimately comes from configuration (<c>Demo:Enabled</c>) — the same options the Api's
/// seed gate (Task 34.7) already reads, implemented in the Api project because that is where options binding
/// lives, never by the Application layer reading configuration types itself.
/// </summary>
public interface IDemoModeProvider
{
    bool IsEnabled { get; }
}
