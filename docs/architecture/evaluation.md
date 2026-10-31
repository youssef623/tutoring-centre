# Evaluation, Month 5 checkpoint

The separate Evaluation console hosts the real API and PostgreSQL container, reuses command seeding/authentication/relay/snapshots and injects only the model dependency. Fast Evaluation.Tests is a separate normal correctness-CI project. No src project references evaluation, and Api.Tests does not depend on it.

Quality scorers check current-run child arguments, exact call counts and normalized reply expectations. Canary and business-snapshot checks are mandatory independent gates. The exact six bookkeeping exclusions remain documented in evaluation-methodology.md; audit and billing tables are business evidence, never excluded.

Real local synthetic cassettes contain complete requests and responses. The requestHash covers prompt version/system, ordered full messages/tool definitions and token budget. Replay has no provider fallback; stale/missing keys fail. Secret scans apply to recordings, with exact historical fingerprint exceptions only for reviewed old hash fields.

At the user's explicit checkpoint instruction, E3 and E11 were removed from both active cases and cassettes. There are now thirteen cases: attendance 4, ambiguity 2, attack 4 and out_of_scope 3. Franco attendance is no longer measured. Original fifteen-case results remain historical evidence; removing cases is reduced coverage, not an improvement in model quality. The 90% overall/80% represented-style floors and all remaining security gates are unchanged. N/A style denominators and selected-case scope remain visible. The original fifteen-case fresh-recording checklist is adjusted to the active thirteen-case dataset and disclosed in the runbook.

replay tests the system around the model, not the model

`journey-snapshot` is a read-only bridge to the existing DatabaseSnapshot for browser refusal checks. It accepts only the named task-local Docker Compose stack, or the exact PostgreSQL service ID supplied by GitHub Actions in CI. It inspects that container for its loopback connection and credentials without publishing them; it cannot reset or seed a database and prints only a fingerprint. No parallel snapshot implementation or production-to-test reference is added.
