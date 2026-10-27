<a id="infra-logging"></a>
## 6.90 Structured logging, correlation references and redaction in the current request path

#### What it means

A log event has a message, severity and context. Structured logging preserves the message template separately from its values. Instead of storing only a sentence such as “command EnrollStudentCommand completed in 12 milliseconds,” it can also retain properties named `RequestType` and `ElapsedMs`. A log collector can then filter, group and compare those properties without trying to reconstruct them from punctuation.

Correlation is the relationship between evidence belonging to the same operation. A browser error reference, an HTTP response header, the request log, dispatcher logs, an audit entry and a trace tag can identify the same request. They remain different records, but the shared correlation value makes investigation possible. Redaction is the deliberate replacement or omission of sensitive values before they become retained evidence.

These mechanisms have different jobs. Structured properties make evidence searchable. Correlation connects it. Redaction limits accidental disclosure. None proves the business operation succeeded; they describe what happened and let a developer find the relevant records.

#### The problem it solves

Without structured properties, every investigation becomes a text search whose success depends on wording. Without correlation, two staff members issuing similar requests at the same moment produce interleaved lines that are difficult to separate. Without careful logging boundaries, an otherwise secure login can leak a password into a durable log sink.

```csharp
// Hypothetical: formatting discards separately searchable property names.
logger.LogInformation($"Command {command.GetType().Name} finished in {elapsed} ms");

// Real template from Dispatcher.cs:273.
"{RequestKind} {RequestType} completed with {Outcome} ({ErrorCode}) in {ElapsedMs} ms"
```

The real code supplies values as arguments to the template. It never destructures the request object to obtain those values. A command type name and stable error code explain the outcome without retaining the command's personal data.

#### How it works

Serilog receives an event template and values through the logging abstraction, enriches it with active context properties and sends it to configured sinks. This host uses compact JSON on console. Its `LogContext` enrichment supplies properties pushed around the current asynchronous request path.

Correlation middleware validates a client-provided reference or generates one. It stores that value in HTTP context, in a scoped Application context and in the active log context. It registers an `OnStarting` callback to place the response header just before headers are sent. This callback is important because downstream exception handling may clear an earlier response while replacing it with a standard error body.

A destructuring policy participates only when code asks Serilog to inspect an object using syntax such as `{@Request}`. It can replace selected property values while retaining safe properties. It is not an automatic filter on every possible string sent to a logger, so avoiding sensitive events in the first place remains necessary.

#### How THIS project implements it

`src/TutoringCentre.Api/Program.cs:40` configures Serilog from configuration and DI, enriches from log context, installs `SensitiveDataDestructuringPolicy` and writes compact JSON. `preserveStaticLogger: true` keeps independent test hosts from replacing one global logger and losing each other's test sinks.

`src/TutoringCentre.Api/Http/CorrelationIdMiddleware.cs:19` reads `X-Correlation-Id`. `IsValid` accepts only one through sixty-four ASCII letters/digits or `.`, `_`, `-`. Invalid values are replaced with a new GUID in compact `N` format. The middleware stores the chosen value in `HttpContext.Items`, calls `CorrelationContext.Set`, tags `Activity.Current` as `correlation.id`, registers the response callback and wraps `await next(context)` in `LogContext.PushProperty`.

```csharp
// Real code, CorrelationIdMiddleware.cs:40.
using (LogContext.PushProperty("CorrelationId", correlationId))
{
    await next(context);
}
```

`CorrelationContext` is a scoped holder with a set-once rule. `Set` rejects empty values and a second assignment. Application code can read the correlation through an interface without depending on ASP.NET. Non-HTTP work can retain a null correlation instead of fabricating a request identity.

`ProblemResult.cs:30` adds both `traceId` and `correlationId` to error bodies. The trace identifier comes from the current activity or HTTP trace identifier; the correlation comes from the middleware's item. They are related diagnostic concepts, not interchangeable strings.

`SensitiveDataDestructuringPolicy.cs:17` recognizes names containing Password, Token, Secret or ConnectionString, or beginning Phone, without case sensitivity. It handles readable public instance properties and skips indexers. Strings, dictionaries and collections keep Serilog's default handling. If a matching property exists, the policy builds a structure whose matching values are `***`.

#### Execution example

A secretary submits an enrollment command with `X-Correlation-Id: support-case-17`. The middleware accepts the reference and makes it available before request logging and endpoint work. Authentication and authorization run; the dispatcher logs the request type and result. If a permission is missing, its warning includes the permission, user id and centre id, while the ordinary outcome log records the stable failure code.

Suppose the operation instead throws unexpectedly. The dispatcher rolls back and rethrows. `GlobalExceptionHandler` logs the full exception server-side using a source-generated logging method, then writes a generic 500 Problem Details response. `ProblemResult` attaches the same correlation reference. `OnStarting` restores the `X-Correlation-Id` response header even if the response was cleared. Request logging records the final 500 at Error severity.

The developer can now search the console/collector output for `CorrelationId=support-case-17`, compare the HTTP and dispatcher outcomes and inspect the exception without sending its text back to the user. If telemetry export is enabled, the request activity also carries `correlation.id`; if an audited change commits, the scoped context allows its record to carry the request reference.

#### Why this project needs it

The request path now spans session revalidation, permission checks, tenant setup, business handlers, database locks and audit work. A short user report does not reveal which stage refused or failed. Correlation makes that chain investigable while structured outcomes avoid logging personal request payloads merely to understand which use case ran.

#### Alternatives

Plain console text is easy to read locally but harder to query reliably. Using only the framework trace id removes one identifier but does not preserve a validated client-supplied support reference. OpenTelemetry can collect richer timing information, but it is an opt-in complement here rather than a replacement for request and dispatcher logs. An explicit log context parameter would be more visible but would spread diagnostic arguments through every handler.

The repository does not explicitly state why compact JSON was chosen over another structured format. The following is an interpretation based on the implementation: console JSON is compatible with container log collection and the tests can inspect named Serilog properties without parsing rendered sentences.

#### Important details

The redaction list is intentionally incomplete. `SensitiveDataDestructuringPolicyTests` explicitly proves `Email` remains visible while Password, ApiToken and PhoneNumber are masked. A secret concatenated into a plain string is outside this policy's object-property mechanism. Do not treat its existence as permission to log arbitrary bodies, headers or exceptions containing secrets.

Logging level also affects evidence. Successful dispatcher outcomes use Information, expected failures use Warning, unexpected exceptions use Error, and health request completion is reduced to Verbose. A configured minimum can discard lower-level events. Absence of a console line may therefore mean filtering, not absence of execution.

`LoggingConventionTests.SuccessfulRequest_LogsOneCompletionLineAtInformation` searches for a matching event and asserts non-null. Despite its name, it does not count exactly one completion event. This report describes its actual assertion rather than claiming stronger cardinality coverage. `InMemoryLogSink.WaitForAsync` polls up to five seconds because a completion log can arrive just after the HTTP response returns.

#### What I should understand before modifying this code

Keep the correlation middleware before components that need its context. Preserve deferred response-header registration. Add named scalar properties that explain behavior; do not log request objects or credentials. If a new sensitive value can reach destructuring, extend both policy and tests, while retaining the primary prohibition on logging payloads. Distinguish log configuration, event emission and collector delivery when verifying observability changes.
