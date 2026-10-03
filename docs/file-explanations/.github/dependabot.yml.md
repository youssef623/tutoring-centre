# .github/dependabot.yml

## Purpose
Configures GitHub Dependabot to propose dependency updates **weekly** for three ecosystems. Minor and patch updates are grouped into one PR per ecosystem to cut PR noise.

## Where it fits
CI/CD. It updates `Directory.Packages.props` (nuget at `/`), `frontend/package.json` and `frontend/package-lock.json` (npm at `/frontend`), and the action versions in `.github/workflows/*.yml`.

## Walkthrough
| Lines | Ecosystem | Directory | Group |
| --- | --- | --- | --- |
| 3–10 | nuget | `/` | `nuget-minor-and-patch`: all packages, minor and patch |
| 12–19 | npm | `/frontend` | `npm-minor-and-patch` |
| 21–28 | github-actions | `/` | `actions-minor-and-patch` |

Major updates are not grouped, so each arrives as its own PR (default Dependabot behaviour).

## Concepts used
- **Dependabot version updates and groups.**

## Data and control flow
Not applicable.

## Configuration and environment
None.

## Gotchas and issues
None found.

## Related files
- [Directory.Packages.props](../Directory.Packages.props.md)
- [frontend/package.json](../frontend/package.json.md)
- [ci.yml](workflows/ci.yml.md)
