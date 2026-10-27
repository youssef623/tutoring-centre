# `docs/onboarding-work/opening.md`

## Purpose

Documentation covering Tutoring Centre Manager — Current Repository Re-onboarding; 1. Project Overview; 1.1 Product purpose and users; 1.2 Current implementation state; 1.3 Major user journeys. Treat historical claims according to the revision described in the file.

## Where it fits

Category: **documentation**. [Open current file](../../../onboarding-work/opening.md). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 244 lines and 29,474 bytes. Source hash: `a0c500887f8fd42255890249dab49f88b3695e257667e404d7cc4a53c5a7ec7a`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 17](../../../onboarding-work/opening.md#L17): `A **centre** is the tenant: the customer whose records must remain isolated. A staff account can have memberships in multiple centres. Each membership carries its own role. The same person can therefore have different access depending on the centre selected for the session. The staff roles are Owner, Secretary and Teacher; the UI can call the Secretary an Assistant. The role names, permission keys and mapping are defined in src/TutoringCentre.Domain/Identity/StaffRole.cs, Permissions.cs and RolePermissions.cs. A parent contact record is academic data, not a parent login. These are separate concepts even when the word “parent” appears in the product vision.`
- [Line 51](../../../onboarding-work/opening.md#L51): `**Set up a centre.** An owner creates subjects and teacher records, optionally links teachers to staff accounts, and creates groups referring to those records. Students are registered with parent contacts. Enrollment puts students into groups. Weekly rules generate dated sessions, and the timetable presents those sessions in the centre's time zone. These steps cross different aggregates because the invariant for one record is not necessarily the invariant for a whole academic workflow. A group's capacity is enforced by a lock-and-count operation, while schedule overlap is enforced by database exclusion constraints.`
- [Line 113](../../../onboarding-work/opening.md#L113): `The backend is one deployable process with four source projects. Domain owns business state and invariant methods. Application owns requests, handlers and ports. Infrastructure implements persistence, credential and other technical ports. Api is the HTTP edge and composition root. An interface being declared in Application allows a handler to call an Infrastructure object at runtime without compiling against that implementation.`
- [Line 162](../../../onboarding-work/opening.md#L162): `The selected centre is not accepted as an arbitrary trusted field from every request. Authentication supplies a staff actor, and centre selection checks the membership before issuing the session claim. Tenant-scoped requests require a centre before a transaction opens. EF adds centre predicates to mapped tenant entities. The save guard checks tracked writes. The transaction sets app.current_centre, and PostgreSQL policies enforce the same boundary even for allow-listed raw SQL. Composite foreign keys prevent a record in one centre from referring to a record in another centre.`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```text
# Tutoring Centre Manager — Current Repository Re-onboarding

This is a fresh account of the repository at the starting revision `ef486f9`, prepared on 27 October 2026. Source files, configuration, migrations, tests and documentation are evidence; old overview statements are not evidence of current behaviour. An implemented feature is not automatically a deployed or independently exercised feature. Test outcomes below distinguish those meanings.

The intended reader knows basic programming and needs to understand why one user action crosses several files. Read the execution examples with the corresponding source open. A path and line number identify the implementation, while the discussion explains ownership, mechanisms and consequences. The [file index](file-explanations/INDEX.md) covers the complete application inventory and links to per-file guides. The historical overview documents remain unchanged.

Contents: [Project](#1-project-overview) · [Stack](#2-technology-stack) · [Architecture](#3-architecture) · [File map](#4-folder-and-file-architecture) · [Startup](#5-startup--bootstrapping) · [Concepts](#6-concepts-and-patterns) · [Authentication](#7-authentication-and-authorization) · [Routing](#8-routing-and-middleware) · [Data](#9-data-layer) · [Use cases](#10-business-logic-and-services) · [Frontend](#11-frontend) · [Tests](#12-testing) · [Infrastructure](#13-infrastructure) · [Findings](#14-code-quality-findings) · [Learning path](#15-recommended-learning-path) · [Changes](#16-changes-since-v2) · [Glossary](#17-glossary-and-open-questions).

When this document discusses a language or framework mechanism, it explains the mechanism required to read the implementation. That is distinct from observing every internal instruction of the framework at runtime. Where the repository does not state a design motivation, the following convention applies: **The repository does not explicitly state why this approach was chosen. The following is an interpretation based on the implementation.** Interpretations must not be read as historical decisions by the author.

# 1. Project Overview

## 1.1 Product purpose and users

The README describes a management platform for Egyptian tutoring centres. Its intended scope includes students, parents, groups, schedules, attendance, fees, payments and a WhatsApp assistant. The implementation now supports substantial administrative and academic workflows. Attendance, payments and a WhatsApp delivery channel must not be inferred from that product description. The implementation reference is the registered endpoint groups in `src/TutoringCentre.Api/Program.cs`, the use cases in Application, and the rendered frontend routes.

A **centre** is the tenant: the customer whose records must remain isolated. A staff account can have memberships in multiple centres. Each membership carries its own role. The same person can therefore have different access depending on the centre selected for the session. The staff roles are Owner, Secretary and Teacher; the UI can call the Secretary an Assistant. The role names, permission keys and mapping are defined in `src/TutoringCentre.Domain/Identity/StaffRole.cs`, `Permissions.cs` and `RolePermissions.cs`. A parent contact record is academic data, not a parent login. These are separate concepts even when the word “parent” appears in the product vision.

The operator is another actor. Operator actions such as migrations, seed data and resetting a password run through process commands. They do not become public HTTP endpoints merely because they are implemented. The CLI branches in `Program.cs` separate `migrate`, `seed` and `reset-password` from starting the web server. Trusted process execution and staff HTTP authorization therefore have different entry conditions.

## 1.2 Current implementation state

The states below concern repository implementation. “Complete” means the described slice has implementation and supporting checks, not that the entire eventual product is finished. Actual execution results appear in the testing section.

```

The excerpt stops at line 24; use the original file for the remaining 220 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
