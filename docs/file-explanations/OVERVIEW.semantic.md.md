# OVERVIEW.semantic.md

## Purpose
A generated (semantic-git) project overview **as of commit 42bdfc1 (Day 2)**: what the project is, who it is for, getting started, the structure tree, a reading guide, and "key things to know".

## Where it fits
Documentation, indexed by `.semantic-manifest.json`. It points to `ARCHITECTURE.semantic.md`, `src/.semantic.md`, `tests/.semantic.md` and the ADRs.

## Walkthrough
- **Line 5:** portfolio project structured around Clean Architecture.
- **Line 11:** audience is the developer, demonstrating skills for .NET interviews.
- **Lines 15–20:** getting-started steps (same as README).
- **Lines 24–39:** folder tree with per-folder status.
- **Lines 45–51:** reading guide.
- **Lines 55–58:** key facts (no business domain yet; architecture enforced; CPM; secrets never in git).

## Concepts used
Same as `ARCHITECTURE.semantic.md`.

## Data and control flow
Not applicable.

## Configuration and environment
Getting-started commands only.

## Gotchas and issues
- **Outdated.** Line 7 says "No business features ... exist yet" and line 55 says "Domain and Application are intentionally empty skeletons". Line 37 says the frontend is "not scaffolded yet". Lines 31–33 say the Domain/Application/Infrastructure test projects are empty. All are false at HEAD: the frontend exists, and those test projects hold 19, 12 and 3 tests.
- "Do not edit manually". Regenerate or delete it.

## Related files
- [.semantic-manifest.json](.semantic-manifest.json.md)
- [ARCHITECTURE.semantic.md](ARCHITECTURE.semantic.md.md)
- [README.md](README.md.md)
