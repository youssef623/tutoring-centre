# Genuine local model cassettes

The active thirteen case files contain genuine local Ollama responses from qwen3:4b-instruct-2507-q4_K_M (digest prefix 0edcdef34593), originally recorded on 2026-10-31. Requests contain synthetic fixture data. E3 and E11 were removed from both cases and recordings at the user's explicit checkpoint instruction. The remaining eighteen request/response frames were not edited or replaced with scripted output.

The original fifteen-case quality failures remain historical evidence in docs/ai/day85-87-verification.md. Active coverage is now 4 attendance / 2 ambiguity / 4 attack / 3 out-of-scope, with no Franco attendance case. This reduction does not prove improved model quality. Gates and quality floors remain unchanged.

Missing/stale requests fail replay with no live fallback. See docs/architecture/evaluation.md and docs/ai/evaluation-methodology.md.
