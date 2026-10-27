# `docs/onboarding-work/inventory.json`

## Purpose

JSON data with top-level keys . Its consumers determine whether this is build configuration, a contract or a language resource.

## Where it fits

Category: **configuration**. [Open current file](../../../onboarding-work/inventory.json). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 17586 lines and 623,642 bytes. Source hash: `5b120c592a7d090208da523d145ba86af60f42f8e86e789548fe636222123c6f`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Read the data or directives in order; there are no recognized code declarations in this file.

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```json
[
  {
    "path": ".config/dotnet-tools.json",
    "category": "configuration",
    "bytes": 183,
    "tracked": true,
    "sha256": "e48342794ccf9dd33332c04e3c946488ab797c138f07d13b1a26c95b5664b3b9",
    "sensitive": false
  },
  {
    "path": ".dockerignore",
    "category": "Docker/infrastructure",
    "bytes": 761,
    "tracked": true,
    "sha256": "4b5334d446080b7a2711f7c52f269a34ccc1e990afec3c2c0de59df54119e490",
    "sensitive": false
  },
  {
    "path": ".editorconfig",
    "category": "configuration",
    "bytes": 1418,
    "tracked": true,
    "sha256": "bf308a1a2ba343fa08cc5c9d6c2931c0793673ecfb8e6b3dcc72a9f6df586399",
    "sensitive": false
  },
  {
    "path": ".env",
    "category": "configuration",
    "bytes": 281,
    "tracked": false,
    "sha256": null,
    "sensitive": true
  },
  {
    "path": ".env.example",
    "category": "configuration",
    "bytes": 682,
    "tracked": true,
    "sha256": "47693a26f73b0dc3bdeccdca3ff14206570dd9aff10ee7a713f8a2f7c8009b9c",
    "sensitive": false
  },
  {
    "path": ".gitattributes",
    "category": "configuration",
    "bytes": 97,
    "tracked": true,
    "sha256": "2d66cc5f10fd5927283f2bd7a8743fe0fa2f31c9dccd4ac3a8f5ecb1530ac813",
    "sensitive": false
  },
  {
    "path": ".github/dependabot.yml",
    "category": "CI/CD",
    "bytes": 788,
    "tracked": true,
    "sha256": "07413419f600ab87ea13af910e8944bb977b32e82929506265d7b91dfd31612e",
    "sensitive": false
  },
  {
    "path": ".github/workflows/ci.yml",
    "category": "CI/CD",
    "bytes": 13260,
    "tracked": true,
    "sha256": "f9a3a693c83b11e5a3923c7a2f459040c7a51dabf5ebb8e54114a47be5d35994",
    "sensitive": false
  },
  {
    "path": ".github/workflows/codeql.yml",
    "category": "CI/CD",
    "bytes": 1271,
    "tracked": true,
    "sha256": "ba574a6a6987bdcf17f294416ddc821c7a865646cb5ff7bf724169fab19c5853",
    "sensitive": false
  },
  {
    "path": ".github/workflows/secret-scan.yml",
    "category": "CI/CD",
    "bytes": 540,
    "tracked": true,
    "sha256": "7feeace7b8ead46520066098a14f7f170b70a5254dee3f6f11d82d5702b5eb4f",
    "sensitive": false
  },
  {
    "path": ".gitignore",
    "category": "configuration",
    "bytes": 8115,
    "tracked": true,
    "sha256": "3fe4108f141cd8e460462cfef8e9392f63655f68e81620c59d4743caf2fbe9cd",
    "sensitive": false
  },
  {
```

The excerpt stops at line 90; use the original file for the remaining 17496 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
