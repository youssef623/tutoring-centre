using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Centres.Commands.CreateCentre;

/// <summary>Creates a centre (tenant). Orchestrates: authorize → uniqueness → Domain rules → add. The dispatcher saves and commits.</summary>
internal sealed class CreateCentreHandler(ICurrentActor currentActor, ICentreRepository centres)
    : ICommandHandler<CreateCentreCommand, CreateCentreResult>
{
    public async Task<Result<CreateCentreResult>> HandleAsync(
        CreateCentreCommand command,
        CancellationToken cancellationToken)
    {
        // 1. Authorize BEFORE any database read: creating tenants is a platform operation, not a web-user operation.
        if (currentActor.Actor is not SystemActor)
        {
            return Result<CreateCentreResult>.Failure(
                Error.Forbidden("centre.create_forbidden", "Only the platform can create centres."));
        }

        // 2. Uniqueness (the unique index is the real guarantee under concurrency; see Day 10's race test).
        if (await centres.ExistsBySlugAsync(command.Slug, cancellationToken))
        {
            return Result<CreateCentreResult>.Failure(
                Error.Conflict("centre.slug_taken", "A centre with this slug already exists."));
        }

        // 3. The Domain decides validity.
        var created = Centre.Create(command.Name, command.Slug, command.TimeZoneId, command.DefaultLocale);
        if (created.IsFailure)
        {
            return Result<CreateCentreResult>.Failure(created.Error!);
        }

        // 4. Track the new entity; NO SaveChanges here — the dispatcher saves once and commits.
        centres.Add(created.Value);

        return Result<CreateCentreResult>.Success(new CreateCentreResult(created.Value.Id, created.Value.Slug));
    }
}
