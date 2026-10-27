# `frontend/src/routeTree.gen.ts`

## Purpose

Declares `FileRoutesByFullPath`, `FileRoutesByTo`, `FileRoutesById`, `FileRouteTypes`, `RootRouteChildren`, `FileRoutesByPath`, `AuthenticatedGroupsRouteChildren`, `AuthenticatedStudentsRouteChildren`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **generated**. [Open current file](../../../../frontend/src/routeTree.gen.ts). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 489 lines and 18,267 bytes. Source hash: `1f5f43ee1946a28e5c5c5232a3de9db12c929fb30aedb2d00c2db67b191a7c9b`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 11: `import { Route as rootRouteImport } from './routes/__root'`
- Line 12: `import { Route as AuthenticatedRouteImport } from './routes/_authenticated'`
- Line 13: `import { Route as ChangePasswordRouteImport } from './routes/change-password'`
- Line 14: `import { Route as LoginRouteImport } from './routes/login'`
- Line 15: `import { Route as SelectCentreRouteImport } from './routes/select-centre'`
- Line 16: `import { Route as StatusRouteImport } from './routes/status'`
- Line 17: `import { Route as AuthenticatedIndexRouteImport } from './routes/_authenticated/index'`
- Line 18: `import { Route as AuthenticatedAuditRouteImport } from './routes/_authenticated/audit'`
- Line 19: `import { Route as AuthenticatedGroupsRouteImport } from './routes/_authenticated/groups'`
- Line 20: `import { Route as AuthenticatedSettingsRouteImport } from './routes/_authenticated/settings'`
- Line 21: `import { Route as AuthenticatedStaffRouteImport } from './routes/_authenticated/staff'`
- Line 22: `import { Route as AuthenticatedStudentsRouteImport } from './routes/_authenticated/students'`
- Line 23: `import { Route as AuthenticatedSubjectsRouteImport } from './routes/_authenticated/subjects'`
- Line 24: `import { Route as AuthenticatedTeachersRouteImport } from './routes/_authenticated/teachers'`
- Line 25: `import { Route as AuthenticatedTimetableRouteImport } from './routes/_authenticated/timetable'`
- Line 26: `import { Route as AuthenticatedGroupsIndexRouteImport } from './routes/_authenticated/groups.index'`
- Line 27: `import { Route as AuthenticatedGroupsGroupIdRouteImport } from './routes/_authenticated/groups.$groupId'`
- Line 28: `import { Route as AuthenticatedParentsParentIdRouteImport } from './routes/_authenticated/parents.$parentId'`
- Line 29: `import { Route as AuthenticatedStudentsIndexRouteImport } from './routes/_authenticated/students.index'`
- Line 30: `import { Route as AuthenticatedStudentsStudentIdRouteImport } from './routes/_authenticated/students.$studentId'`
- Line 31: `import { Route as AuthenticatedStudentsNewRouteImport } from './routes/_authenticated/students.new'`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 139](../../../../frontend/src/routeTree.gen.ts#L139): `export interface FileRoutesByFullPath {`
- [Line 160](../../../../frontend/src/routeTree.gen.ts#L160): `export interface FileRoutesByTo {`
- [Line 179](../../../../frontend/src/routeTree.gen.ts#L179): `export interface FileRoutesById {`
- [Line 202](../../../../frontend/src/routeTree.gen.ts#L202): `export interface FileRouteTypes {`
- [Line 267](../../../../frontend/src/routeTree.gen.ts#L267): `export interface RootRouteChildren {`
- [Line 276](../../../../frontend/src/routeTree.gen.ts#L276): `interface FileRoutesByPath {`
- [Line 420](../../../../frontend/src/routeTree.gen.ts#L420): `interface AuthenticatedGroupsRouteChildren {`
- [Line 433](../../../../frontend/src/routeTree.gen.ts#L433): `interface AuthenticatedStudentsRouteChildren {`
- [Line 450](../../../../frontend/src/routeTree.gen.ts#L450): `interface AuthenticatedRouteChildren {`
- [Line 487](../../../../frontend/src/routeTree.gen.ts#L487): `export const routeTree = rootRouteImport`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/main.tsx:6](../../../../frontend/src/main.tsx#L6)
- [frontend/src/test/renderRouter.tsx:5](../../../../frontend/src/test/renderRouter.tsx#L5)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
/* eslint-disable */

// @ts-nocheck

// noinspection JSUnusedGlobalSymbols

// This file was automatically generated by TanStack Router.
// You should NOT make any changes in this file as it will be overwritten.
// Additionally, you should also exclude this file from your linter and/or formatter to prevent it from being checked or modified.

import { Route as rootRouteImport } from './routes/__root'
import { Route as AuthenticatedRouteImport } from './routes/_authenticated'
import { Route as ChangePasswordRouteImport } from './routes/change-password'
import { Route as LoginRouteImport } from './routes/login'
import { Route as SelectCentreRouteImport } from './routes/select-centre'
import { Route as StatusRouteImport } from './routes/status'
import { Route as AuthenticatedIndexRouteImport } from './routes/_authenticated/index'
import { Route as AuthenticatedAuditRouteImport } from './routes/_authenticated/audit'
import { Route as AuthenticatedGroupsRouteImport } from './routes/_authenticated/groups'
import { Route as AuthenticatedSettingsRouteImport } from './routes/_authenticated/settings'
import { Route as AuthenticatedStaffRouteImport } from './routes/_authenticated/staff'
import { Route as AuthenticatedStudentsRouteImport } from './routes/_authenticated/students'
import { Route as AuthenticatedSubjectsRouteImport } from './routes/_authenticated/subjects'
import { Route as AuthenticatedTeachersRouteImport } from './routes/_authenticated/teachers'
```

The excerpt stops at line 24; use the original file for the remaining 465 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
