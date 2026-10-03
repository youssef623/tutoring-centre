# .github/workflows/secret-scan.yml

## Purpose
Runs **gitleaks** over the **full git history** on every PR and every push to `main`, to catch committed secrets (passwords, tokens, keys).

## Where it fits
CI/CD security gate (`README.md:98`). It complements the project's secret handling: user-secrets for the connection string and a git-ignored `.env`.

## Walkthrough
- **Lines 3–6:** triggers.
- **Lines 15–17:** `contents: read`, plus `pull-requests: write` so the action can comment on a PR when it finds a leak.
- **Lines 19–22:** checkout with `fetch-depth: 0`, so the whole history is scanned, not just the latest commit.
- **Lines 24–27:** `gitleaks/gitleaks-action@v3` with `GITHUB_TOKEN`.

## Concepts used
- **Secret scanning** across history. A secret removed in a later commit is still in history, and still leaked.

## Data and control flow
Not applicable.

## Configuration and environment
- `GITHUB_TOKEN` (automatic Actions secret).
- No `.gitleaks.toml` is present, so the default rules apply.

## Gotchas and issues
- Unverified: whether this repo's gitleaks-action v3 setup needs a `GITLEAKS_LICENSE` (the action requires one for organisation-owned repos). The repo is owned by a user account, so probably not.

## Related files
- [ci.yml](ci.yml.md)
- [.gitignore](../../.gitignore.md)
