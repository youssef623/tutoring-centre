namespace TutoringCentre.Api.Cli;

/// <summary>
/// Whether this deployment is a demo instance (Task 34.7). Defaults to false: a Production container is safe by
/// default, and only an operator who deliberately wants demo data on a throwaway instance opts in. Changes
/// nothing except the seed command's gate — the demo banner and reporting it in system info are Day 35.
/// </summary>
public sealed class DemoOptions
{
    public bool Enabled { get; set; }
}
