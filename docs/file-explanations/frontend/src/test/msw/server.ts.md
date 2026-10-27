# `frontend/src/test/msw/server.ts`

## Purpose

Current test file. The source excerpt and structural map below expose its content without inferring behaviour from its filename.

## Where it fits

Category: **test**. [Open current file](../../../../../../frontend/src/test/msw/server.ts). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 4 lines and 128 bytes. Source hash: `83a51eb356c17ca185e7259ad42122fb6da4af234b226a7d0d7fb5f73e14a093`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { setupServer } from "msw/node";`
- Line 2: `import { handlers } from "./handlers";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 4](../../../../../../frontend/src/test/msw/server.ts#L4): `export const server = setupServer(...handlers);`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/api/apiFetch.test.ts:5](../../../../../../frontend/src/api/apiFetch.test.ts#L5)
- [frontend/src/api/apiFetch.ts:72](../../../../../../frontend/src/api/apiFetch.ts#L72)
- [frontend/src/api/fieldErrors.ts:34](../../../../../../frontend/src/api/fieldErrors.ts#L34)
- [frontend/src/api/showApiError.test.ts:61](../../../../../../frontend/src/api/showApiError.test.ts#L61)
- [frontend/src/api/showApiError.ts:32](../../../../../../frontend/src/api/showApiError.ts#L32)
- [frontend/src/features/dashboard/DashboardOverview.test.tsx:5](../../../../../../frontend/src/features/dashboard/DashboardOverview.test.tsx#L5)
- [frontend/src/features/enrollments/EnrollStudentDialog.test.tsx:7](../../../../../../frontend/src/features/enrollments/EnrollStudentDialog.test.tsx#L7)
- [frontend/src/features/enrollments/GroupRosterTab.test.tsx:7](../../../../../../frontend/src/features/enrollments/GroupRosterTab.test.tsx#L7)
- [frontend/src/features/enrollments/StudentEnrollmentsCard.test.tsx:6](../../../../../../frontend/src/features/enrollments/StudentEnrollmentsCard.test.tsx#L6)
- [frontend/src/features/enrollments/TransferDialog.test.tsx:6](../../../../../../frontend/src/features/enrollments/TransferDialog.test.tsx#L6)
- [frontend/src/features/groups/CreateGroupDialog.tsx:103](../../../../../../frontend/src/features/groups/CreateGroupDialog.tsx#L103)
- [frontend/src/features/groups/GroupEditDialogs.tsx:88](../../../../../../frontend/src/features/groups/GroupEditDialogs.tsx#L88)
- [frontend/src/features/parents/ParentEditDialog.tsx:129](../../../../../../frontend/src/features/parents/ParentEditDialog.tsx#L129)
- [frontend/src/features/parents/updateParentSchema.ts:3](../../../../../../frontend/src/features/parents/updateParentSchema.ts#L3)
- [frontend/src/features/schedule/AddScheduleRuleDialog.test.tsx:7](../../../../../../frontend/src/features/schedule/AddScheduleRuleDialog.test.tsx#L7)
- [frontend/src/features/schedule/AddScheduleRuleDialog.tsx:46](../../../../../../frontend/src/features/schedule/AddScheduleRuleDialog.tsx#L46)
- [frontend/src/features/schedule/ExtendScheduleDialog.test.tsx:7](../../../../../../frontend/src/features/schedule/ExtendScheduleDialog.test.tsx#L7)
- [frontend/src/features/schedule/ExtendScheduleDialog.tsx:21](../../../../../../frontend/src/features/schedule/ExtendScheduleDialog.tsx#L21)
- [frontend/src/features/schedule/GroupScheduleTab.test.tsx:7](../../../../../../frontend/src/features/schedule/GroupScheduleTab.test.tsx#L7)
- [frontend/src/features/schedule/TimetablePage.test.tsx:6](../../../../../../frontend/src/features/schedule/TimetablePage.test.tsx#L6)
- 35 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
import { setupServer } from "msw/node";
import { handlers } from "./handlers";

export const server = setupServer(...handlers);
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
