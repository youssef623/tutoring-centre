# Job conventions

Each job is one Infrastructure class with `ExecuteAsync`, accepting only IDs, dates and a
`CancellationToken`. Never serialize entities, DTOs, names, phones or credentials as arguments.
The method calls `ISystemScopeRunner.RunAsync(centreId, "job:<id>", work, cancellationToken)`;
the work dispatches one command through the scoped dispatcher.

The trusted caller supplies the centre ID. The runner creates a new async scope and sets the
system actor and correlation. A failure `Result` is a business outcome: log its code without
arguments and return normally. Let exceptions propagate so Hangfire retries them. Execution
logging includes job ID and job type only, never arguments or exception messages.

The server runs inside the API when enabled. Global retry allows three retries at 10, 20 and
40 seconds. Concrete jobs and recurring schedules belong to Day 68.
