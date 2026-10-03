# Directory.Build.props

## Purpose
MSBuild properties that apply automatically to **every** `.csproj` under the repo root. They set the target framework and strictness once, so individual projects can't drift.

## Where it fits
Root build config. MSBuild imports it implicitly into all 9 projects (4 `src`, 5 `tests`). Its sibling `Directory.Packages.props` controls package versions.

## Walkthrough
| Line | Property | Effect |
| --- | --- | --- |
| 4 | `TargetFramework=net10.0` | every project targets .NET 10 |
| 5 | `Nullable=enable` | nullable reference types; null-safety warnings |
| 6 | `ImplicitUsings=enable` | common `using`s (System, System.Linq, System.Threading.Tasks…) are added automatically |
| 7 | `TreatWarningsAsErrors=true` | any compiler or analyzer warning fails the build |
| 8 | `AnalysisLevel=latest-recommended` | enables the "recommended" .NET analyzer set (e.g. CA1707, CA1716, CA1848, CA1861, CA1000) |

Because of lines 7–8, the code has several `[SuppressMessage]` attributes, each with a justification (e.g. `Dispatcher.cs:145-152`, `Result.cs:39-42`, `Error.cs:10-13`, `IUnitOfWork.cs:12-15`).

## Concepts used
- **Directory.Build.props:** MSBuild's hierarchical, automatically imported property file.
- **Roslyn analyzers:** code-quality rules that run during compilation.

## Data and control flow
Not applicable.

## Configuration and environment
None.

## Gotchas and issues
- Individual `.csproj` files deliberately omit these properties. Adding them per project would silently override this file (as the old `src/.semantic.md:62` notes).
- The build is verified clean: 0 warnings in Release on SDK 10.0.112.

## Related files
- [Directory.Packages.props](Directory.Packages.props.md)
- [.editorconfig](.editorconfig.md)
- [global.json](global.json.md)
