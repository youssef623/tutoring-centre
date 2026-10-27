# `compose.yaml`

## Purpose

Current Docker/infrastructure file. The source excerpt and structural map below expose its content without inferring behaviour from its filename.

## Where it fits

Category: **Docker/infrastructure**. [Open current file](../../compose.yaml). [Complete file index](INDEX.md). [Current teaching overview](../PROJECT_OVERVIEW3.md).

The reviewed file has 22 lines and 692 bytes. Source hash: `fa4cd9c8824980897e323f51a2a58143ec100e3fe1daee5afab73fc0938b3cb9`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Read the data or directives in order; there are no recognized code declarations in this file.

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```yaml
services:
  postgres:
    image: postgres:17
    environment:
      POSTGRES_DB: ${POSTGRES_DB}
      POSTGRES_USER: ${POSTGRES_USER}
      POSTGRES_PASSWORD: ${POSTGRES_PASSWORD:?set POSTGRES_PASSWORD in .env}
      TUTORING_OWNER_PASSWORD: ${TUTORING_OWNER_PASSWORD:?set TUTORING_OWNER_PASSWORD in .env}
      TUTORING_APP_PASSWORD: ${TUTORING_APP_PASSWORD:?set TUTORING_APP_PASSWORD in .env}
    ports:
      - "5432:5432"
    volumes:
      - pgdata:/var/lib/postgresql/data
      - ./db:/docker-entrypoint-initdb.d:ro
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U ${POSTGRES_USER} -d ${POSTGRES_DB}"]
      interval: 5s
      timeout: 5s
      retries: 5

volumes:
  pgdata:
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
