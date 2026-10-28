using System.Globalization;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Academics.Contracts;
using TutoringCentre.Application.Academics.Groups;
using TutoringCentre.Application.Academics.Groups.Queries.ListGroups;
using TutoringCentre.Application.Academics.Scheduling;
using TutoringCentre.Application.Academics.Scheduling.Commands.AddScheduleRule;
using TutoringCentre.Application.Academics.Scheduling.Commands.EndScheduleRule;
using TutoringCentre.Application.Academics.Scheduling.Queries.ListGroupScheduleRules;
using TutoringCentre.Application.Centres.Contracts;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Api.Cli;

public static class DemoSessionCommand
{
    public static async Task<int> RunAsync(IServiceProvider services, IHostEnvironment environment, string[] args, TextWriter output)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(environment);
        ArgumentNullException.ThrowIfNull(args);
        ArgumentNullException.ThrowIfNull(output);
        if (!environment.IsDevelopment())
        {
            await output.WriteLineAsync("Refused: demo-session only runs in Development.");
            return 1;
        }

        var options = new Dictionary<string, string>(StringComparer.Ordinal);
        for (var index = 0; index < args.Length; index += 2)
        {
            if (index + 1 >= args.Length || args[index] is not ("--centre" or "--group" or "--in-minutes")
                || !options.TryAdd(args[index], args[index + 1])) { return await Fail(output, "Usage: demo-session --centre <slug> --group <name> [--in-minutes 10]"); }
        }
        var minutes = 10;
        if (!options.TryGetValue("--centre", out var slug) || !options.TryGetValue("--group", out var name)
            || (options.TryGetValue("--in-minutes", out var text) && !int.TryParse(text, CultureInfo.InvariantCulture, out minutes))
            || minutes is < 0 or > 1440) { return await Fail(output, "Provide centre, group, and in-minutes between 0 and 1440."); }

        Guid centreId;
        await using (var lookup = services.CreateAsyncScope())
        {
            var centre = await lookup.ServiceProvider.GetRequiredService<AppDbContext>().Set<Centre>().AsNoTracking()
                .SingleOrDefaultAsync(centre => centre.Slug == slug);
            if (centre is null) { return await Fail(output, "Centre not found."); }
            centreId = centre.Id;
        }

        await using var scope = services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<CurrentActorContext>().Set(new SystemActor(centreId));
        var dispatcher = scope.ServiceProvider.GetRequiredService<Dispatcher>();
        var zone = await scope.ServiceProvider.GetRequiredService<ICentreTimeZoneProvider>().GetForCurrentCentreAsync(CancellationToken.None);
        var now = TimeZoneInfo.ConvertTime(scope.ServiceProvider.GetRequiredService<IClock>().UtcNow, zone);
        if (now.Hour >= 23) { return await Fail(output, "Refused: demo-session cannot run between 23:00 and midnight in the centre's zone."); }
        var today = DateOnly.FromDateTime(now.DateTime);
        var groups = await dispatcher.QueryAsync<ListGroupsQuery, IReadOnlyList<GroupDto>>(new(null, null, false), CancellationToken.None);
        if (groups.IsFailure) { return await Fail(output, groups.Error!.Code); }
        var group = groups.Value.SingleOrDefault(group => group.Name == name);
        if (group is null) { return await Fail(output, "Group not found."); }
        var raw = now.DateTime.AddMinutes(minutes);
        var rounded = new DateTime(((raw.Ticks + TimeSpan.TicksPerMinute * 5 - 1) / (TimeSpan.TicksPerMinute * 5)) * (TimeSpan.TicksPerMinute * 5), DateTimeKind.Unspecified);
        for (var attempt = 0; attempt < 5; attempt++)
        {
            var start = rounded.AddMinutes(attempt * 30);
            if (DateOnly.FromDateTime(start.AddMinutes(60)) != today) { return await Fail(output, "No 60-minute slot remains today."); }
            // The existing command generates its normal horizon. Ending the rule through the public command
            // retains today and cancels later sessions without changing scheduling rules or inserting rows.
            var added = await dispatcher.SendAsync<AddScheduleRuleCommand, AddScheduleRuleResult>(
                new(group.Id, today.DayOfWeek, TimeOnly.FromDateTime(start), 60, null, today), CancellationToken.None);
            if (added.IsFailure)
            {
                if (added.Error!.Code == "schedule.teacher_double_booked") { continue; }
                return await Fail(output, added.Error.Code);
            }
            var rules = await dispatcher.QueryAsync<ListGroupScheduleRulesQuery, IReadOnlyList<ScheduleRuleDto>>(new(group.Id), CancellationToken.None);
            if (rules.IsFailure) { return await Fail(output, rules.Error!.Code); }
            var rule = rules.Value.Single(rule => rule.Id == added.Value.ScheduleRuleId);
            var ended = await dispatcher.SendAsync<EndScheduleRuleCommand, Unit>(new(rule.Id, today, rule.Version), CancellationToken.None);
            if (ended.IsFailure) { return await Fail(output, ended.Error!.Code); }
            var unit = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            await unit.BeginAsync(readOnly: true, CancellationToken.None);
            var sessions = await scope.ServiceProvider.GetRequiredService<IAttendanceRosterSource>().ListSessionsOnDateAsync(today, null, CancellationToken.None);
            await unit.CommitAsync(CancellationToken.None);
            var session = sessions.Single(session => session.GroupId == group.Id && TimeZoneInfo.ConvertTime(session.StartsAt, zone).DateTime == start);
            await output.WriteLineAsync($"Session: {session.SessionId}");
            await output.WriteLineAsync($"/api/sessions/{session.SessionId}/attendance");
            return 0;
        }
        return await Fail(output, "No free teacher slot after four retries.");
    }

    private static async Task<int> Fail(TextWriter output, string message)
    {
        await output.WriteLineAsync(message);
        return 1;
    }
}
