# `frontend/src/main.tsx`

## Purpose

Declares `Register`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **source**. [Open current file](../../../../frontend/src/main.tsx). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 34 lines and 1,079 bytes. Source hash: `a1ae3c7218e8c12fafeca18de913d6860f221b9ba80ffa537cc5498125f83b99`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { StrictMode } from "react";`
- Line 2: `import { createRoot } from "react-dom/client";`
- Line 3: `import { QueryClientProvider } from "@tanstack/react-query";`
- Line 4: `import { RouterProvider, createRouter } from "@tanstack/react-router";`
- Line 5: `import { queryClient, setLoginRedirect } from "@/app/queryClient";`
- Line 6: `import { routeTree } from "./routeTree.gen";`
- Line 7: `import "./i18n";`
- Line 8: `import "./index.css";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 18](../../../../frontend/src/main.tsx#L18): `interface Register {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/e2e/students.spec.ts:76](../../../../frontend/e2e/students.spec.ts#L76)
- [frontend/src/features/dashboard/DashboardOverview.test.tsx:52](../../../../frontend/src/features/dashboard/DashboardOverview.test.tsx#L52)
- [frontend/src/routes/_authenticated/students.index.test.tsx:80](../../../../frontend/src/routes/_authenticated/students.index.test.tsx#L80)
- [frontend/src/routes/_authenticated/students.new.test.tsx:71](../../../../frontend/src/routes/_authenticated/students.new.test.tsx#L71)
- [src/TutoringCentre.Application/Academics/Students/Commands/RegisterStudent/RegisterStudentHandler.cs:71](../../../../src/TutoringCentre.Application/Academics/Students/Commands/RegisterStudent/RegisterStudentHandler.cs#L71)
- [src/TutoringCentre.Domain/Academics/Student.cs:58](../../../../src/TutoringCentre.Domain/Academics/Student.cs#L58)
- [tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs:487](../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L487)
- [tests/TutoringCentre.Application.Tests/Academics/ArchiveRestoreStudentHandlerTests.cs:17](../../../../tests/TutoringCentre.Application.Tests/Academics/ArchiveRestoreStudentHandlerTests.cs#L17)
- [tests/TutoringCentre.Application.Tests/Academics/EnrollStudentHandlerTests.cs:124](../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollStudentHandlerTests.cs#L124)
- [tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs:139](../../../../tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs#L139)
- [tests/TutoringCentre.Application.Tests/Academics/ParentLinkHandlerTests.cs:19](../../../../tests/TutoringCentre.Application.Tests/Academics/ParentLinkHandlerTests.cs#L19)
- [tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs:157](../../../../tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs#L157)
- [tests/TutoringCentre.Application.Tests/Academics/UpdateStudentHandlerTests.cs:16](../../../../tests/TutoringCentre.Application.Tests/Academics/UpdateStudentHandlerTests.cs#L16)
- [tests/TutoringCentre.Domain.Tests/Academics/StudentTests.cs:21](../../../../tests/TutoringCentre.Domain.Tests/Academics/StudentTests.cs#L21)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentTestRequests.cs:89](../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentTestRequests.cs#L89)
- [tests/TutoringCentre.Infrastructure.Tests/Audit/AuditPolicyTests.cs:135](../../../../tests/TutoringCentre.Infrastructure.Tests/Audit/AuditPolicyTests.cs#L135)
- [tests/TutoringCentre.Infrastructure.Tests/Persistence/TransactionBoundaryTests.cs:18](../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/TransactionBoundaryTests.cs#L18)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { QueryClientProvider } from "@tanstack/react-query";
import { RouterProvider, createRouter } from "@tanstack/react-router";
import { queryClient, setLoginRedirect } from "@/app/queryClient";
import { routeTree } from "./routeTree.gen";
import "./i18n";
import "./index.css";

const router = createRouter({ routeTree, context: { queryClient } });

setLoginRedirect((redirectTarget) => {
  void router.navigate({ to: "/login", search: { redirect: redirectTarget } });
});

// Registers the router type so links and navigation are type-checked everywhere.
declare module "@tanstack/react-router" {
  interface Register {
    router: typeof router;
  }
}

const rootElement = document.getElementById("root");
if (rootElement === null) {
  throw new Error("Root element #root was not found in index.html.");
}

createRoot(rootElement).render(
  <StrictMode>
    <QueryClientProvider client={queryClient}>
      <RouterProvider router={router} />
    </QueryClientProvider>
  </StrictMode>,
);
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
