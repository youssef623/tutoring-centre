# `docs/onboarding-work/generated-model-read-ledger.json`

## Purpose

JSON data with top-level keys `method`, `full_packet_inspection_completed`, `latest_designer_and_snapshot_entity_blocks_byte_identical`, `discrepancies`, `files`, `unique_block_count`, `packet_count`, `packet_files`. Its consumers determine whether this is build configuration, a contract or a language resource.

## Where it fits

Category: **configuration**. [Open current file](../../../onboarding-work/generated-model-read-ledger.json). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 6829 lines and 272,528 bytes. Source hash: `3c637e9f5da957e976c81aa39e55ea502c448459c8716fa425b3447accf95fe7`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Read the data or directives in order; there are no recognized code declarations in this file.

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```json
{
  "method": "Every generated file is read in full and partitioned without gaps into exact line ranges. SHA256-identical blocks are inspected once and mapped back to every occurrence; no distinct block content is omitted. Packets contain full distinct block source, not summaries.",
  "full_packet_inspection_completed": true,
  "latest_designer_and_snapshot_entity_blocks_byte_identical": true,
  "discrepancies": [
    "No unexpected generated-model discrepancy found. Historical ProductVersion10.0.12 retained through AddStudentsAndParents; AddTeachers onward and current snapshot10.0.13.",
    "The Teachers centre/user membership FK is intentionally migration-only, absent snapshot/config relationships; TeacherConfiguration explicitly documents this.",
    "RLS policies, grants/revokes, btree_gist extension and schedule exclusion constraints are migration operations outside snapshot model metadata. Model parity does not verify their live presence."
  ],
  "files": [
    {
      "path": "src/TutoringCentre.Infrastructure/Persistence/Migrations/20261002222404_InitialPlatform.Designer.cs",
      "sha256": "cc3c827f73294bab760ed225d86b2bc1c8235dae5ecff5854eb127b0aaea19e7",
      "line_count": 81,
      "product_version": [
        "10.0.12"
      ],
      "segments": [
        {
          "start_line": 1,
          "end_line": 27,
          "sha256": "3741064d960c0af6ef9628830d640b4d161a1ce8b54e83fc912e6e4e07f08547",
          "entity": null
        },
        {
          "start_line": 28,
          "end_line": 81,
          "sha256": "dcef0e5adc59358561f09474d9eea4ea4239122fd426a599a2789f252ef60f42",
          "entity": "TutoringCentre.Domain.Centres.Centre"
        }
      ],
      "observed_model": [
        {
          "entity": "TutoringCentre.Domain.Centres.Centre",
          "start_line": 28,
          "end_line": 81,
          "tables": [
            {
              "table": "centres",
              "schema": "platform"
            }
          ],
          "checks": [
            "\"ck_centres_default_locale\", \"default_locale IN ('ar','en')\")"
          ],
          "keys": [
            "\"Id\")\n                        .HasName(\"pk_centres\")"
          ],
          "indexes": [
            "\"Slug\")\n                        .IsUnique()\n                        .HasDatabaseName(\"ux_centres_slug\")"
          ],
          "foreign_keys": [],
          "concurrency_properties": [],
          "has_xmin": false
        }
      ]
    },
    {
      "path": "src/TutoringCentre.Infrastructure/Persistence/Migrations/20261004112858_AddIdentityAndMemberships.Designer.cs",
      "sha256": "cf6e988ce1631597fb0d2878149bb74b51960d2cf3fa84168e838dd43db14721",
      "line_count": 362,
      "product_version": [
        "10.0.12"
      ],
      "segments": [
        {
          "start_line": 1,
          "end_line": 27,
          "sha256": "3dada278a9743aa1c08e6b9dcba92cc347f55d7f94c69a39ef700ffbfb803a5c",
          "entity": null
        },
        {
          "start_line": 28,
          "end_line": 57,
          "sha256": "b5a41daeb26d0bf50a639ab4b23a48d05c761f75f3e0a3394df96163d2226593",
          "entity": "Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>"
        },
        {
          "start_line": 58,
          "end_line": 84,
          "sha256": "0c32a037a3c28b4860aad71205761de6126d50c7f90c10a35f0c6822143aef97",
          "entity": "Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>"
        },
        {
          "start_line": 85,
          "end_line": 108,
          "sha256": "a035177488c2ac11b2014d6189ba3544f1941a774422b154bd0329e8241ecd57",
          "entity": "Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>"
        },
        {
```

The excerpt stops at line 90; use the original file for the remaining 6739 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
