# global.json

## Purpose
Pins the .NET SDK used to build the repo: version `10.0.401` with `rollForward: latestFeature`.

## Where it fits
Root. The `dotnet` host reads it on every command. CI installs exactly this SDK with `actions/setup-dotnet@v5` and `global-json-file: global.json` (`ci.yml:24-27`, `codeql.yml:33-37`).

## Walkthrough
- `"version": "10.0.401"`: the minimum SDK.
- `"rollForward": "latestFeature"`: any 10.0.4xx patch, or a higher feature band (10.0.5xx …), is accepted. A lower band such as 10.0.1xx is **not**.

## Concepts used
- **SDK pinning / roll-forward policy:** gives reproducible builds across machines.

## Data and control flow
Not applicable.

## Configuration and environment
None.

## Gotchas and issues
- Verified while preparing these docs: with only SDK 10.0.112 installed, every `dotnet` command fails with "A compatible .NET SDK was not found. Requested SDK version: 10.0.401". Contributors need a 10.0.4xx or newer SDK.

## Related files
- [Directory.Build.props](Directory.Build.props.md)
- [ci.yml](.github/workflows/ci.yml.md)
