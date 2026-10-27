# `frontend/src/i18n/locales/en/status.json`

## Purpose

JSON data with top-level keys `readiness`, `systemInfo`. Its consumers determine whether this is build configuration, a contract or a language resource.

## Where it fits

Category: **configuration**. [Open current file](../../../../../../../frontend/src/i18n/locales/en/status.json). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 23 lines and 902 bytes. Source hash: `5450badb1c106679f0562dd671106f434f7ef3c4aacb646e53da533f81982419`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

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
  "readiness": {
    "checking": "Checking API status",
    "unreachableTitle": "Cannot reach the API",
    "unreachableBody": "The API did not respond. Check that it is running, then try again.",
    "databaseUnavailableTitle": "API is running but the database is unavailable",
    "databaseUnavailableBody": "Start PostgreSQL, then try again.",
    "healthyTitle": "Healthy",
    "healthyDescription": "API and database are reachable",
    "lastCheckedAt": "Last checked at {time}"
  },
  "systemInfo": {
    "loading": "Loading system information",
    "unavailableTitle": "System information is unavailable",
    "reference": "Reference: {id}",
    "title": "System information",
    "schemaUpToDate": "Database schema up to date",
    "migrationsPending": "Migrations pending",
    "applicationVersion": "Application version",
    "latestMigration": "Latest migration",
    "none": "none"
  }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
