# Job conventions

Each job is one Infrastructure class with `ExecuteAsync`, accepting only IDs, dates and a
`CancellationToken`. Never serialize entities, DTOs, names, phones or credentials as arguments.
Hangfire may inject a `PerformContext` parameter from a null placeholder to obtain the current
job ID; this is execution metadata, never caller data. Direct test calls use `job:direct`.
The method calls `ISystemScopeRunner.RunAsync(centreId, "job:<id>", work, cancellationToken)`;
the work dispatches one command through the scoped dispatcher.

The trusted caller supplies the centre ID. The runner creates a new async scope and sets the
system actor and correlation. A failure `Result` is a business outcome: log its code without
arguments and return normally. Let exceptions propagate so Hangfire retries them. Execution
logging includes job ID and job type only, never arguments or exception messages.

The server runs inside the API when enabled. Global retry allows three retries at 10, 20 and
40 seconds. The nightly parent lists centre IDs and enqueues independently transactional
children; it does not dispatch an extension itself. Recurring registration runs only when
the job server is enabled, with the explicit business time zone.
