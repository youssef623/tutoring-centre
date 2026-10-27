# `.github/workflows/secret-scan.yml`

## Purpose

Current CI/CD file. The source excerpt and structural map below expose its content without inferring behaviour from its filename.

## Where it fits

Category: **CI/CD**. [Open current file](../../../../.github/workflows/secret-scan.yml). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 27 lines and 540 bytes. Source hash: `7feeace7b8ead46520066098a14f7f170b70a5254dee3f6f11d82d5702b5eb4f`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Read the data or directives in order; there are no recognized code declarations in this file.

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```yaml
name: Secret scan

on:
  pull_request:
  push:
    branches: [main]

permissions:
  contents: read

jobs:
  gitleaks:
    name: gitleaks
    runs-on: ubuntu-latest
    permissions:
      contents: read
      pull-requests: write # lets the action comment on a PR when it finds a leak
    steps:
      - name: Check out full history
        uses: actions/checkout@v6
        with:
          fetch-depth: 0

      - name: Run gitleaks
        uses: gitleaks/gitleaks-action@v3
        env:
          GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
