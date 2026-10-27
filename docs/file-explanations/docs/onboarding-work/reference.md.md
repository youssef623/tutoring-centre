# `docs/onboarding-work/reference.md`

## Purpose

Documentation covering 7. Authentication and Authorization; 7.1 Credentials, tickets and browser state; 7.2 Login, selection and first-login change; 7.3 Revalidation and revocation; 7.4 Permission and ownership decisions. Treat historical claims according to the revision described in the file.

## Where it fits

Category: **documentation**. [Open current file](../../../onboarding-work/reference.md). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 327 lines and 47,906 bytes. Source hash: `69322dcb6dc6e91fec99ac89b99dfc6f7b9acc66561246700422eed534be7b43`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 31](../../../onboarding-work/reference.md#L31): `Teacher scope is a further restriction. The Teacher role holds the group, enrollment and schedule read capabilities, but the corresponding read services narrow records to the teacher linked to the current staff user in the active centre. The same generic permission key does not grant access to all centre groups. Tenant scope, capability and record ownership are separate decisions and need separate test coverage.`
- [Line 46](../../../onboarding-work/reference.md#L46): `| Request log | Measure and record final status | Surround the handled response and inherit correlation context. |`
- [Line 97](../../../onboarding-work/reference.md#L97): `The migration catalogue below names every current migration class and its changed operations. Generated designers and the model snapshot are explicitly inventoried rather than treated as hand-authored domain logic. Their purpose is reproducing and comparing schema state. The seed command has grown beyond two centres: its academic fixtures include subjects, parent/student records, teacher/group relationships, enrollments and schedules. It is gated by environment/demo intent and remains a repeatable development/demo operation, not a public account-creation endpoint.`
- [Line 129](../../../onboarding-work/reference.md#L129): `## 10.4 Trace: recurring wall time to dated class session`
- [Line 131](../../../onboarding-work/reference.md#L131): `SessionGenerator.Generate receives a schedule rule, teacher identifier, explicit time zone and inclusive date range. It visits dates in order, selects dates on which the rule occurs, constructs that date's local start and converts it into an instant. It calculates the end by adding elapsed duration to the start instant. It does not generate all future sessions by adding seven UTC days to the first occurrence; that would shift local class time across daylight-saving changes.`
- [Line 143](../../../onboarding-work/reference.md#L143): `The academic interface includes students and parents, subject/teacher management, groups with roster and schedule tabs, enrollment actions and a weekly timetable. Administrative pages include staff, centre settings and audit history. The dashboard summarizes current centre data rather than remaining the old placeholder. The detailed frontend concepts above trace each area's cache keys, mutations, error recovery and component boundaries from current files.`
- [Line 312](../../../onboarding-work/reference.md#L312): `| Exclusion constraint | Database rule preventing conflicting ranges such as simultaneous active class sessions. |`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```text
# 7. Authentication and Authorization

## 7.1 Credentials, tickets and browser state

The staff user model belongs to Infrastructure Identity, while centre membership belongs to Domain. The authentication port returns an authenticated user identifier and security stamp rather than exposing `ApplicationUser`. `AuthEndpoints.LoginAsync` validates the request, calls that port, replaces the request actor and dispatches the profile query. It issues the cookie only after the profile read succeeds. This ordering avoids creating a signed-in ticket for a flow whose membership/profile lookup failed.

`AuthenticationSetup.cs:23` names the cookie `__Host-tcm.session`. Its flags are HttpOnly, Secure always, SameSite Lax and path `/`. Its ticket lifetime is eight hours with sliding expiration. The ticket contains claims, not a session-row identifier. The browser stores and sends the cookie; the SPA obtains readable user/session information through `/api/me`. JavaScript cannot read an HttpOnly cookie through `document.cookie`, but script running in the application's origin can still make authenticated requests. HttpOnly limits credential extraction; it does not make XSS harmless.

Data Protection is now registered in `Infrastructure/DependencyInjection.cs:119` with `PersistKeysToDbContext<AppDbContext>()` and application name `TutoringCentre`. The context implements `IDataProtectionKeyContext` and exposes its required key set. This is a platform-level exception to the context's usual lack of named `DbSet` properties. The purpose is persistence across application restarts and shared ticket decryptability where instances use the same key store and application identity. Database availability and access to key material are therefore operational security concerns.

## 7.2 Login, selection and first-login change

The anonymous login route uses its own rate-limit policy and still belongs to the antiforgery-protected API group. Credentials are verified through Identity, which also owns failed-attempt counters and lockout. Those account-security writes are a deliberate path outside the normal command save pipeline. A failed login must still be able to increase the failed-attempt count; wrapping it in a transaction that rolls back every business refusal would defeat lockout.

After authentication the request actor has no selected centre. Login reads `MeDto`. It auto-selects exactly one membership only when `MustChangePassword` is false. It recomputes the permissions for that selected role because the profile query ran before selection. A first-login account receives a signed-in ticket without a selected centre and proceeds to password change. A multi-centre account proceeds to selection. The selector accepts a centre identifier as a request, verifies active membership and then issues a new ticket using the verified membership's centre and role. It carries the user and stamp forward from the existing trusted session, never from the body.

Password change validates current and new credentials, calls the authentication port and reissues a ticket with the new stamp and no centre. Other tickets with the old stamp fail revalidation after their positive-cache window. This session must also choose a centre again. An operator reset is a separate process command. There is no reason to expose the operator's trusted database access level to a web request merely to reuse the credential adapter.

## 7.3 Revalidation and revocation

`SessionRevalidationHandler.cs:20` runs from the cookie handler's validation event. It parses the user, stamp, centre and role claims. The cache key includes all of those facts. On a miss it dispatches `ValidateStaffSessionQuery` before normal actor construction; that query must receive the identity explicitly because `ActorMiddleware` has not populated the Application actor yet. A valid answer can be cached for `SessionValidationOptions.CacheDuration`, whose default is sixty seconds. Invalid answers are not cached. Rejecting a principal also signs out the cookie scheme.

This is not instant distributed revocation. Each process can retain a positive answer until expiration. Role comparison now participates in session validation, correcting the earlier situation where a demoted user could retain an old role claim without detection. Membership deactivation and credential changes now have actual product/operator paths, so revocation is no longer only a capability exercised by direct test SQL.

```

The excerpt stops at line 24; use the original file for the remaining 303 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
