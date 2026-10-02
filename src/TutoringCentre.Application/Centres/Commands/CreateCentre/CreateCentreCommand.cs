using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Centres;

namespace TutoringCentre.Application.Centres.Commands.CreateCentre;

/// <summary>Intent to create a centre (tenant). Only the system actor may do this. Carries exactly the four fields a caller may provide.</summary>
public sealed record CreateCentreCommand(string Name, string Slug, string TimeZoneId, SupportedLocale DefaultLocale)
    : ICommand<CreateCentreResult>;
