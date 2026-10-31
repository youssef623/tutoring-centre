# ADR 0021: Isolated evaluation with exact-request cassettes

Status: accepted for harness design; real recording evidence is tracked separately.

## Decision

Keep quality evaluation in its own console project, with fast deterministic scorer tests in Evaluation.Tests and a separate container-backed replay CI tier. Use real API/auth/outbox/gateway/guards/PostgreSQL around recorded model responses. Keep quality floors independent of mandatory no-canary/no-business-mutation gates.

Record synthetic real-model requests/responses locally using environment credentials. Replay uses a hash of prompt version, full system text, ordered messages and full ordered tool definitions plus token budget. Missing/stale requests fail closed; there is no network fallback. Publish safe report codes rather than transcripts. Both live and record refuse CI=true.

## Alternatives

A trait in Api.Tests would mix measurement with correctness timing/pipeline and provide no dedicated report. An external non-.NET script could not directly host WebApplicationFactory. Live models in CI add cost, secrets, fork risks, provider availability and nondeterminism. LLM-as-judge adds another untrusted dependency to a mechanical gate. No evaluation loses style/category measurements. These alternatives are rejected for this slice.

## Trade-offs

Two additional evaluation projects and one shared-support project cost builds/maintenance. Cassettes test surrounding-system regressions rather than current model quality. Prompt/context changes require deliberate new real recordings. Fifteen cases and lexical checks offer limited semantic coverage; no variance claims. The private identity mapping stays in memory and only the exact six bookkeeping tables are excluded from content snapshots. See evaluation-methodology.md for decisions, commands and denominators.
