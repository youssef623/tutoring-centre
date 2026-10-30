# ADR 0017: Application model port and provider adapter

Status: Accepted

## Context

The assistant depends on an unreliable external model. Application handlers must remain deterministic in tests and must not depend on vendor message types, exceptions, credentials, or SDKs. Tool declarations and tool call requests are needed by the forthcoming agent loop.

## Decision

Application owns `ILanguageModel.GenerateAsync(ModelRequest, CancellationToken)` and provider-neutral messages, tools, responses, usage, and stop reasons. A response is a `Result<ModelResponse>`: a failed call has an error and cannot expose a successful response. Cancellation returns `assistant.model_cancelled`; the per-call deadline returns `assistant.model_timeout`; other provider and translation failures return `assistant.model_error`.

Infrastructure uses Microsoft.Extensions.AI 10.11.0 and Microsoft.Extensions.AI.OpenAI 10.11.0, backed by the official OpenAI SDK. OpenAI is the single live provider. `IChatClient` is an adapter detail, not the application port. Tool declarations are sent without automatic execution; the application agent loop will authorize and execute requested tools. Tool results and preceding assistant tool calls round-trip through the same port.

Configure `Assistant:Provider` as `live` or `scripted`. No model is called until the assistant consumer is introduced on Day 78. Leaving the provider unset permits existing deployments and administrative CLI commands to start; resolving the port without selecting a provider fails explicitly. An explicit invalid provider fails startup validation.

Live configuration requires `Assistant:Model` and `Assistant:ApiKey`. Supply the key through user-secrets (`Assistant:ApiKey`) or the environment (`Assistant__ApiKey`), never committed settings. The model identifier remains deployment configuration so the adapter does not silently choose or change a billable model. An explicit live configuration without a key fails startup.

`Assistant:ScriptPath` names an existing JSON array of model responses. Scripted responses advance once per call, do not consume a step for pre-cancelled calls, and fail explicitly when exhausted. The scripted adapter is accepted only in Development and Testing; Production and every other environment reject it even when the file exists.

Server defaults: MaxSteps 4, MaxTotalTokens 6000, CallTimeout 20 seconds, MaxReplyCharacters 800, HistoryMessages 10. Startup rejects nonpositive limits and unreasonable upper bounds. The adapter caps output tokens by MaxTotalTokens and enforces CallTimeout with a linked token and `WaitAsync`, including clients that ignore cancellation. Day 78 owns aggregate step/token budgets, history selection and reply length enforcement.

Logs contain only configured model name, input/output token counts, duration and outcome. They contain no prompt, response, tool arguments, API key or exception objects. Do not add SDK request-body logging or sensitive telemetry middleware.

## Alternatives

Direct OpenAI types in Application would couple handlers and tests to a vendor. Exposing `IChatClient` as the port would move a larger third-party API across the boundary. Azure OpenAI would add deployment and authentication configuration; local Ollama would require a separately operated model service. Neither is necessary for this slice. The small owned port allows either adapter later without changing handlers.

## Verification

Fake IChatClient tests cover translation, provider exceptions, timeout, and cancellation. Options tests cover missing keys, invalid limits and Production refusal. Architecture tests prohibit Microsoft.Extensions.AI, OpenAI and Azure.AI dependencies in Application. PostgreSQL tests cover merged chat timelines and ownership; HTTP tests cover portal permissions and rate limiting.

A real-key smoke call is a separate local verification gate. Never substitute a dummy key or claim live verification from fake-client tests. No key or model identifier is committed here.

References: [IChatClient documentation](https://learn.microsoft.com/dotnet/ai/ichatclient), [OpenAI adapter package](https://www.nuget.org/packages/Microsoft.Extensions.AI.OpenAI/10.11.0).
