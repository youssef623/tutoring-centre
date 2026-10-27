# `docs/onboarding-work/logging-depth.md`

## Purpose

Documentation covering 6.90 Structured logging, correlation references and redaction in the current request path. Treat historical claims according to the revision described in the file.

## Where it fits

Category: **documentation**. [Open current file](../../../onboarding-work/logging-depth.md). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 82 lines and 9,363 bytes. Source hash: `fec8ac4c790f3fc41043d34c564623fe0abc8686b01617c4b7ce11ee2f851d3d`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 42: `using (LogContext.PushProperty("CorrelationId", correlationId))`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 48](../../../onboarding-work/logging-depth.md#L48): `CorrelationContext is a scoped holder with a set-once rule. Set rejects empty values and a second assignment. Application code can read the correlation through an interface without depending on ASP.NET. Non-HTTP work can retain a null correlation instead of fabricating a request identity.`
- [Line 60](../../../onboarding-work/logging-depth.md#L60): `The developer can now search the console/collector output for CorrelationId=support-case-17, compare the HTTP and dispatcher outcomes and inspect the exception without sending its text back to the user. If telemetry export is enabled, the request activity also carries correlation.id; if an audited change commits, the scoped context allows its record to carry the request reference.`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

````text
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
````

The excerpt stops at line 24; use the original file for the remaining 58 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
