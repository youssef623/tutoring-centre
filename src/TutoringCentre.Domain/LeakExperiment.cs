using Microsoft.AspNetCore.Http;

namespace TutoringCentre.Domain;

internal sealed class LeakExperiment
{
    public HttpContext? Context { get; set; }
}
