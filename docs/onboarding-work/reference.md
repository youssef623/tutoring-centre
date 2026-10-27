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

Logout still calls `SignOutAsync` and clears the browser's copy. It does not rotate the security stamp. A separately copied valid ticket is therefore not invalidated solely by that logout operation. This is a consequence of the self-contained ticket design and the current logout code, not a claim that the application has observed a stolen-cookie incident. Absolute logout revocation would need a separate policy and server-side invalidation mechanism.

## 7.4 Permission and ownership decisions

The fallback HTTP policy requires an authenticated user unless an endpoint explicitly allows anonymous access. Application requests implementing `IRequirePermission` additionally pass through `Dispatcher.CheckPermission`. A system actor is allowed; a staff actor must carry a role that holds the capability. Unknown roles and keys deny through the catalogue/mapping. The catalogue has sixteen keys, with separate view/manage capabilities for the academic areas and owner-only capabilities for staff, audit and settings.

Teacher scope is a further restriction. The Teacher role holds the group, enrollment and schedule read capabilities, but the corresponding read services narrow records to the teacher linked to the current staff user in the active centre. The same generic permission key does not grant access to all centre groups. Tenant scope, capability and record ownership are separate decisions and need separate test coverage.

The frontend mirrors permission names and consumes the permissions returned with the current session. `Can`, `useCan` and `RouteGuard` shape navigation and control visibility. `RouteGuard` prevents its protected child component from mounting when permission is absent, so the child's data hooks do not fire. This is useful feedback and avoids needless requests. It is not enforcement against a client that manually sends HTTP; the dispatcher remains authoritative.

# 8. Routing and Middleware

## 8.1 Actual request chain

The explicit order in `Api/Program.cs:166` onward is trusted forwarding, Production HSTS, security headers, correlation, Serilog request logging, exception handling, status-code pages, SPA static files, routing, authentication, actor translation, authorization and rate limiting. Endpoint execution follows; API endpoint execution includes the group's antiforgery filter.

| Stage | Responsibility | Why the relative order matters |
| --- | --- | --- |
| Trusted forwarding | Accept forwarded scheme/address only from configured networks | Authentication flags, proxy-aware rate partitions and security behaviour must see the validated external request context. |
| HSTS/security headers | Set browser security policy | Apply before static and dynamic responses so serving assets does not omit the policy. |
| Correlation | Choose/request identifier and establish scoped/log context | Later logs, errors and audit rows consume the identifier. |
| Request log | Measure and record final status | Surround the handled response and inherit correlation context. |
| Exception boundary | Translate malformed requests and unexpected faults | Must surround binding, filters and endpoint operations that can throw. |
| Status pages | Enrich otherwise empty error statuses | Surround bare authentication and routing failures. |
| Static files | Serve actual built browser assets | Run before routing selects a fallback for that same path. |
| Explicit routing | Choose endpoint and metadata | Authentication/authorization/rate limiting need the selected endpoint's rules. |
| Authentication | Read/decrypt ticket and revalidate it | Actor construction and authorization depend on the resulting principal. |
| Actor middleware | Translate claims into Application identity | Handlers consume `ICurrentActor`, not HTTP claims. |
| Authorization | Apply endpoint metadata or fail-closed fallback | Stop an unsigned request before executing protected work. |
| Rate limiter | Enforce endpoint policy | Refuse excess attempts before the endpoint's business operation. |
| Antiforgery filter | Validate unsafe API requests | Runs as endpoint execution; absence of a valid token prevents side effects. |

## 8.2 Fallbacks and static hosting

API fallbacks return `route.not_found` in the common Problem Details representation and are excluded from OpenAPI. They are anonymous so a signed-out caller receives the same unknown-route result. A built SPA can receive deep-link GET requests through `MapSpaFallback`. The last fallback also matches file-looking paths that the SPA's nonfile constraint deliberately excludes. This prevents a missing `/assets/...js` file from producing an authentication challenge or an HTML shell disguised as JavaScript.

Explicit `UseRouting` is a substantive change from the older host. Without it, minimal hosting can insert routing before the application's middleware sequence. Selection of a fallback endpoint and the existence of a physical asset are independent facts. The current order gives static files first opportunity to answer before endpoint selection. Do not “simplify” the explicit routing call without preserving that behaviour and its regression tests.

## 8.3 Three failure paths

An unknown API URL reaches the anonymous API fallback and returns a standard 404. A protected URL with no valid ticket fails authorization and returns a 401 that status pages can enrich. A validly authenticated request with insufficient capability passes the HTTP identity check but fails the dispatcher permission step before a transaction begins. These are not interchangeable errors; they answer different questions about routing, identity and access.

A malformed body can fail even before a handler's own validation. Strict JSON conversion throws a bad-request exception, and the global exception handler maps it to `request.malformed`. A valid DTO with a domain conflict instead returns a failed `Result`, which maps through the common result-to-HTTP conversion. The endpoint should remain bind, dispatch and map rather than recreating those categories with local catches.

# 9. Data Layer

## 9.1 Mapping, state and database guarantees

`AppDbContext` remains the shared persistence boundary across modules. It now inherits Identity's user context and implements the Data Protection key interface. It applies all mapping configurations, allows the narrowly defined test-model extension and applies the tenant convention last. `CurrentCentreId` reads the current actor live rather than capturing the actor's centre at context construction. That matters because login can construct the context before replacing the actor.

An entity passed to a repository's `Add` becomes tracked; the database write happens later at save. A repository loading a versioned entity assigns the expected client version to EF's original concurrency value. Saving then detects a stale update when its generated predicate affects no matching row. The context's in-memory state, the caller's expected version and the database's current row version are different pieces of state.

The database adds independent guarantees: required and bounded columns, check constraints, unique indexes, composite tenant foreign keys, row-level security and exclusion constraints. EF interceptors cannot replace those guarantees for a raw SQL writer. Conversely, database constraints do not supply user-facing permission checks or decide which translated message a form should show.

## 9.2 Enrollment as a relational example

`EnrollmentConfiguration.cs:26` maps `academics.enrollments`. Its student and group references use `(centre_id, id)` composite keys. That makes a cross-centre reference invalid even if the supplied UUID exists elsewhere. Both relationships restrict deletion. A partial unique index on centre, student and group applies only when status is active, allowing an ended relationship to remain as history and a later enrollment to be created.

The table checks constrain the status and end reason and keep the ended flag consistent with end time/reason. They also prevent an end timestamp preceding enrollment. These are row-level truths. Capacity is a truth about a count of other rows, so it is enforced by the locked group load and domain decision rather than by pretending a per-row check can count concurrent enrollments safely.

## 9.3 Transactions, audit and reconciliation

Commands begin read-write transactions, run handler reads inside the transaction, save once and commit. Queries begin read-only transactions, set PostgreSQL transaction mode before other SQL and never call save. Both set the transaction-local tenant variable, using a parameterized call and an empty value when no centre exists. Transaction-local scope prevents one pooled connection from carrying a prior request's tenant into the next transaction.

Before saving, the unit of work reconciles removed student-parent links. The student's domain collection can omit a link while the required foreign key still exists in EF tracking. The reconciliation temporarily disables automatic change detection, compares the current collection with tracked links and marks removed links deleted. It restores the previous detection setting in a `finally` block. This is a concrete persistence concern, not a reason to weaken the domain's aggregate API or change database deletion rules silently.

The save pipeline stamps timestamps, builds audited changes and checks tenant writes. Audit rows share the same context and transaction as business changes. The guard also rejects modifications or deletions of tracked audit entries. Database grants/policies provide the corresponding persistence boundary. Auditability is therefore an atomic consistency property, not a later best-effort log message.

## 9.4 Migration and seed ownership

The runtime connection belongs to a restricted application role. Migration contexts use the separately configured owning connection. `DatabaseOptions` requires the runtime string, while the migration runner checks its own optional-at-web-startup migration string when an operator actually asks to migrate. The web process should not need schema ownership just to serve records.

The migration catalogue below names every current migration class and its changed operations. Generated designers and the model snapshot are explicitly inventoried rather than treated as hand-authored domain logic. Their purpose is reproducing and comparing schema state. The seed command has grown beyond two centres: its academic fixtures include subjects, parent/student records, teacher/group relationships, enrollments and schedules. It is gated by environment/demo intent and remains a repeatable development/demo operation, not a public account-creation endpoint.

# 10. Business Logic and Services

## 10.1 Where decisions belong

Endpoints translate HTTP values into application requests. Handlers orchestrate operations that need multiple collaborators or state-dependent checks. Domain entities/value objects enforce invariants that should remain true regardless of delivery mechanism. Infrastructure decides how to load, lock, project and persist. That division explains why enrollment capacity is split across a repository operation and a domain method: the repository establishes a trustworthy count under a lock; the group decides whether the count permits enrollment.

Some services represent technical capabilities, such as credentials or the clock. Others are published feature contracts, such as a staff directory or centre time-zone lookup. They exist to expose a narrow need across feature boundaries. Returning a DTO instead of an EF query object prevents the caller from silently adding persistence-specific operations outside the intended adapter.

## 10.2 Trace: enroll one student

1. The roster dialog selects a student and sends the generated enrollment mutation. Its input contains the student identifier; the group comes from the endpoint route and the tenant comes from the actor.
2. The API's unsafe-request filter validates the antiforgery pair. Binding and shape validation refuse malformed or missing fields.
3. `EnrollmentEndpoints.EnrollStudentAsync` constructs `EnrollStudentCommand` and sends it through the dispatcher.
4. Validation, tenant scope and permission checks run before opening the transaction.
5. The unit of work begins and sets the tenant setting. `EnrollStudentHandler` asks for the locking group load.
6. `GroupRepository.GetForEnrollmentAsync` selects the group with `FOR UPDATE`. After the lock is granted, it counts active enrollments.
7. The handler resolves the student inside the same tenant scope, refuses an archived student, checks an existing active enrollment and calls the group's capacity rule.
8. `Enrollment.Start` creates the domain relationship at `IClock.UtcNow`. The repository tracks it.
9. Save creates persistence timestamps and applicable audit history; database constraints remain the final defense against duplicates and invalid relationships.
10. Commit releases the group lock. HTTP mapping returns 201 with the new identifier and a location pointing to the group roster.
11. The client invalidates relevant reads and presents the server's new roster/seat state. A concurrent refusal stays a normal business result, not an optimistic assumption that the seat was obtained.

The order of group lookup before student lookup is intentional in the handler comments: a foreign group identifier produces the scoped group-not-found outcome before disclosing student information. The lock covers the decision and the write. Merely putting the eventual insert inside a transaction would not serialize the earlier count.

## 10.3 Trace: transfer without losing history

`TransferEnrollmentHandler` loads the current enrollment with the caller's expected version. It refuses an already-ended relationship or the same target group. It locks only the target group and counts target seats. The source is read without a lock because removing a student cannot exceed the source's capacity. Subject matching and duplicate-target checks run before changing state. The handler ends the old enrollment with reason Transferred and starts a new one, using one captured time value. Both changes are saved in one transaction.

The old relationship's group identifier is never rewritten. That preserves where the student previously studied. Locking only one group also avoids an opposite-transfer lock cycle: A-to-B and B-to-A do not each hold one lock while asking for the other's second lock. This is the implemented decision, which supersedes the earlier draft expectation of two ordered group locks.

## 10.4 Trace: recurring wall time to dated class session

`SessionGenerator.Generate` receives a schedule rule, teacher identifier, explicit time zone and inclusive date range. It visits dates in order, selects dates on which the rule occurs, constructs that date's local start and converts it into an instant. It calculates the end by adding elapsed duration to the start instant. It does not generate all future sessions by adding seven UTC days to the first occurrence; that would shift local class time across daylight-saving changes.

An invalid local time in a forward clock gap is shifted by the gap. An ambiguous repeated time uses the larger offset, selecting the first occurrence. These policies are implemented once in the generator's conversion method. The caller controls what “today” means, which dates need generation and which sessions already exist. Persistence separately arbitrates collisions through exclusion constraints. Pure generation, orchestration and concurrent persistence are separate jobs.

# 11. Frontend

The browser is a client-rendered React application. File routes determine navigation, feature components own workflows, generated functions provide typed HTTP operations and TanStack Query owns reusable server data. Local state owns transient UI such as an open dialog or a currently selected form target. Keeping those owners distinct prevents a stale component copy from silently becoming the authoritative version of a database row.

Protected layout logic checks the session and the pending password-change state before rendering the shell. Permission-aware navigation exposes appropriate pages, and route-level guards prevent missing-capability screens from issuing their protected data requests. The backend remains authoritative for tenant, permission and teacher ownership checks. A stale UI permission can cause a refused request; it must never be accepted as evidence of authority by the endpoint.

Forms combine React Hook Form and Zod. Translation keys are used for client validation copy; the shared API layer handles Problem Details and known machine codes. A new field must remain consistent across DTO, validator, generated contract, form schema, field-error mapping and paired language resources. A generated request type does not replace a runtime input check, and a client-side check does not replace the domain rule.

The academic interface includes students and parents, subject/teacher management, groups with roster and schedule tabs, enrollment actions and a weekly timetable. Administrative pages include staff, centre settings and audit history. The dashboard summarizes current centre data rather than remaining the old placeholder. The detailed frontend concepts above trace each area's cache keys, mutations, error recovery and component boundaries from current files.

Dates shown in the timetable are formatted using the centre's time zone. Phone fields also distinguish editable input from a formatted identifier display, and bidirectional isolation protects technical identifiers inside Arabic text. These details matter because translating labels alone does not establish correct internationalization. Direction, number/phone presentation, local-date calculations and user-provided text are separate concerns.

# 12. Testing

## 12.1 Evidence hierarchy

The five backend test projects separate domain rules, application orchestration, compiled architecture, persistence and HTTP. Pure logic tests use collaborators that can be supplied without a real database. Integration tests use PostgreSQL because RLS, transaction-local settings, exclusion constraints, `FOR UPDATE`, system row versions and provider SQL behaviour cannot be proved by a dictionary fake.

Frontend component tests use a real component tree with controlled HTTP responses. The MSW boundary prevents unrecognized network requests from being silently ignored. Router tests exercise the route tree and guards. Playwright journeys exercise the actual browser/API seams that mocked responses cannot establish, including cookies, antiforgery, proxy forwarding and complete workflows. Smoke tests target the production-style serving topology rather than the Vite development environment.

## 12.2 Actual checks from this session

<!-- SESSION_RESULTS -->

The source catalogue names tests and fixtures separately from execution totals. A `[Theory]` declaration can produce several runtime cases; adding declaration counts is not the same as counting executed cases. Likewise, a file containing a test proves an intended assertion exists, not that its environment was available or that its latest execution passed.

## 12.3 Coverage boundaries

The integration suite contains explicit independence probes: database RLS can be exercised through raw SQL while the application query filter is bypassed or absent, and EF query filters can be checked through an elevated connection where RLS is not doing the work. Controls matter because otherwise two protections can mask a missing third. Capacity races and exclusion conflicts likewise require overlapping operations and assertions about the resulting invariant rather than a favorable single-threaded sequence.

No test can prove all possible concurrent executions or every future use case. The lock protocol is only as strong as the paths that use it, and the architectural naming rules are only as strong as their scope and selectors. Treat regression tests as concrete executable claims. Read the setup, the intentional bypass, the asserted failure and the success control before deciding what the test guarantees.

# 13. Infrastructure

The repository contains an actual production application Dockerfile, a local production Compose overlay, a TLS-terminating Caddy configuration, a migration CLI and smoke tooling. It also contains cloud deployment decisions and operational documents. The README explicitly states that no live instance has yet been deployed. Building a local image and documenting Azure Container Apps do not prove cloud resources or a public URL exist.

The production-style application receives the restricted runtime database connection. One-shot migration and seed containers receive the configuration needed for their operator work. The app starts only after seed completes, which in turn waits for migration and database health. The proxy network is fixed so `Proxy:KnownNetworks` can name the actual source network. Trusting arbitrary forwarded headers would make the external scheme and client address attacker-controlled; the configured trust list is part of the security boundary.

Configuration values have separate owners. Compose reads `.env` substitution values; ASP.NET reads its configuration providers and uses double underscores for hierarchical environment keys. A local `.env` is not automatically loaded into the .NET process. Secrets are not reproduced in this document or the file index. The tracked template shows expected key names, while the actual development values remain local metadata only.

OpenTelemetry registration supplies optional observability export. Structured logs, correlation identifiers and database traces answer related but different questions. A trace can relate timed operations, a log records an event and its structured properties, and an audit row records an authorized business change under transactional ownership. Do not substitute one for another: a successful request log is not immutable audit history, and an audit row is not a complete performance trace.

GitHub Actions defines backend/frontend gates, browser journeys and production-container verification alongside security analysis and secret scanning. The workflow files establish what is configured to run. Whether branch protection requires a particular check is a remote repository setting and remains unverified by these files. No pull request or remote CI wait is part of this documentation task.

# 14. Code Quality Findings

Only verified current observations appear here. This task documents them and does not change runtime behaviour.

## 14.1 Tenant changes leave unrelated client cache entries intact

`frontend/src/routes/select-centre.tsx:88` posts the selection, refreshes CSRF, invalidates only `meQueryOptions.queryKey` and navigates to the dashboard. `frontend/src/app/queryClient.ts:49` makes normal query data fresh for thirty seconds. `frontend/src/api/generated/tutoring-centre.ts:1401` defines the subject-list key from `/api/subjects` plus list parameters, without a selected-centre identifier. `features/subjects/SubjectsList.tsx:59` uses that generated query directly.

Consequently the same signed-in browser can retain centre A's fresh subject result after changing its session to centre B and reuse that result on revisiting the same list parameters. This is a source-derived risk; a browser reproduction was not executed in this documentation task. Server-side tenant enforcement still controls API reads and writes. Recommended fix: scope tenant query keys to centre identity, or remove/invalidate tenant data as part of centre transition. Add a regression that visits a populated list before switching and verifies no prior-centre rows render afterward.

## 14.2 Local production topology still publishes the database port

`compose.yaml:11` publishes `5432:5432`. `compose.prod.yaml:9` adds networks to the inherited PostgreSQL service without removing that port. The deployment architecture text's “only Caddy publishes a port” description therefore overstates the merged local topology. The application container itself has no published port in the overlay, but PostgreSQL retains the base publication. Recommended fix: update the documentation or explicitly override/remove/bind the database publication for the intended production-style topology.

## 14.3 Missing-database comment contradicts fail-fast validation

`Infrastructure/DependencyInjection.cs:63` says missing configuration must not crash startup and registers an always-unhealthy probe. The same registration configures `DatabaseOptions` with data-annotation validation and `ValidateOnStart`; `Persistence/DatabaseOptions.cs:12` requires a nonempty runtime connection string. A normally started host therefore fails validation instead of serving that degraded readiness state. The code supports fail-fast startup, and the fallback comment still describes a different policy. Recommended fix: remove the obsolete branch/comment or document a deliberately supported host mode where that branch is reachable.

## 14.4 Logout does not revoke an independently copied ticket

`Api/Auth/AuthEndpoints.cs:144` calls `SignOutAsync` and returns no content. It does not change a user stamp or maintain a server session row. Revalidation checks stamp/membership/role; it does not learn that this particular ticket was signed out in another browser. A copy remains subject to normal ticket expiry and revalidation, rather than becoming invalid solely because logout cleared the original browser. Decide whether browser-local sign-out is the intended policy before changing it; rotating a user's stamp would also affect other sessions.

## 14.5 Build-time export can log database key-ring initialization errors

The OpenAPI generation branch supplies a placeholder runtime connection and skips Development migration. Infrastructure nevertheless registers a database-backed Data Protection key ring. The actual Release build completed successfully with zero compiler warnings/errors while its export host logged a password-required error for that placeholder database connection. This is observed build output, not a compiler failure. Recommended fix: isolate key-ring startup side effects in the export host or supply an explicit generation-only key repository without changing normal production registration. Verify that generated contracts remain identical.

## 14.6 Lint signal and resource-sensitive test runs

The frontend lint command exits successfully with zero errors and forty-two fast-refresh warnings. A default-concurrency component run produced timeout failures; a bounded-worker rerun passed all 278 cases. These observations do not prove forty-two application bugs or twenty-four deterministic test defects. They show noisy lint output and an environment-sensitive test workload. Keep the command and worker setting with the recorded result, and avoid claiming that either unsuccessful run proves a product regression without a focused reproduction.

## 14.7 Rejected operator reset can remove the existing password first

`Api/Cli/ResetPasswordCommand.cs:42` calls `RemovePasswordAsync` before `AddPasswordAsync` at line 51. If the replacement is rejected, lines 52–56 log the failure and return exit code 1. The command contains no transaction spanning the two store operations. The source-supported risk is that an invalid replacement leaves the previous password removed rather than preserving it. This was not reproduced against a real account during the documentation task. Validate the replacement before destructive credential mutation or use an atomic reset operation; add a test asserting the old credential remains usable after a rejected replacement.

## 14.8 Self-service password change ignores its second Identity result

`Infrastructure/Identity/IdentityAuthenticationService.cs:77` awaits `UpdateSecurityStampAsync(user)` after setting `MustChangePassword = false` but does not inspect its `IdentityResult`. The method then returns success. The preceding password-change result is checked correctly; the second persistence outcome is not. A failed second update could therefore be reported as full success without proving the flag/stamp update was saved. Check that result and define recovery for this two-operation boundary. Do not describe these separate calls as one explicit transaction merely because the comment says the tracked entity is saved together.

# 15. Recommended Learning Path

| Step | Learn | Where in this document | Why it matters here | Where to see it in the repo | What to understand before moving on |
| --- | --- | --- | --- | --- | --- |
| 1 | Interfaces and composition | [Interfaces](#foundation-interfaces) | Handlers use ports rather than constructing adapters | Enrollment handler and group repository | Difference between contract type and concrete runtime object |
| 2 | Tasks and cancellation | [Async](#foundation-async) | Every database request and transaction operation composes asynchronous work | Dispatcher and UnitOfWork | Calls start work; awaits observe completion; cleanup still owns resources |
| 3 | Generic types and optional state | [Types](#foundation-types) | One dispatcher supports many typed requests and session states | Handler interfaces and actor records | Why a signed-in user can have no centre or role |
| 4 | Results and exceptions | [Cleanup](#foundation-exceptions) | Expected refusal and unexpected fault share cleanup but different error meaning | Result and persistence translator | When to return failure and when to rethrow |
| 5 | Metadata and generated code | [Metadata](#foundation-metadata) | Registrations, tests, OpenAPI and the frontend contain automated discovery | HandlerRegistration and Orval config | Identify what reads each annotation or input |
| 6 | HTTP and wire shapes | [JSON](#foundation-json) | Strict JSON and scheduling types cross the backend/frontend boundary | Program JSON options and converters | Distinguish malformed input, invalid business input and forbidden actions |
| 7 | Project and request boundaries | [Architecture](#3-architecture) | One operation crosses projects without reversing dependency direction | Four source project files | Trace compile dependency separately from runtime call |
| 8 | Composition and startup | [Startup](#5-startup--bootstrapping) | Web, tooling and operator commands share an entry point | Program and CLI classes | Which branches listen, migrate, seed or exit |
| 9 | Cookies and permission decisions | [Authentication](#7-authentication-and-authorization) | User identity, selected tenant and capability are different facts | AuthEndpoints, RolePermissions and Dispatcher | Why an authenticated user can still be refused |
| 10 | Middleware and endpoint filters | [Routing](#8-routing-and-middleware) | Order controls error mapping, static hosting and security | Program and HTTP setup helpers | Explain a request's inward and outward path |
| 11 | Relational invariants and EF tracking | [Data](#9-data-layer) | Tenant foreign keys, versions and constraints survive adapter mistakes | Entity configurations and migrations | What is checked before SQL versus by PostgreSQL |
| 12 | Locks and concurrent operations | [Feature traces](#10-business-logic-and-services) | Last-seat and transfer correctness depend on ordering | Enrollment handlers and GroupRepository | Why transaction alone does not protect a read/count/insert decision |
| 13 | Browser cache ownership | [Frontend](#11-frontend) | Current UI depends on Query keys, invalidation and session transitions | Generated client and feature mutations | Which state belongs to server, form and URL |
| 14 | Independent regression proof | [Testing](#12-testing) | Filters, RLS and locks need controls that do not mask each other | Infrastructure probes and API matrix | Explain exactly what a test setup bypasses and what its assertion proves |
| 15 | Serving and operations | [Infrastructure](#13-infrastructure) | Production-style hosting differs from Vite development | Dockerfile, Compose overlay and Caddyfile | Which credentials and ports belong to each process |
| 16 | Modification risks | [Findings](#14-code-quality-findings) | Some current boundaries have documented operational or cache limits | Cited source and runtime output | Separate verified observation, interpretation and reproduced failure |

# 16. Changes Since v2

## 16.1 Historical baseline and measured scope

The last commit modifying `PROJECT_OVERVIEW2.md` is `9d638ceb46099a4fd87ba8bc901b10e947c1f737`. The starting HEAD for this audit is `ef486f9`. Git reports 893 changed files, 90,595 insertions and 771 deletions across that interval. This comparison is a repository change history, not an assertion that every insertion is new product logic; migrations, generated contract/client data and tests contribute substantial volume.

The older document has 4,018 lines, 57,697 whitespace-separated words, fifteen numbered top-level sections and thirty-five concept subsections. Its eighteen level-one headings include the document title and two appendices. It has forty-eight labelled fenced blocks and twenty Mermaid diagrams. The benchmark JSON records the exact heading structure and size of every old concept, making comparison reproducible rather than relying on an impression of depth.

## 16.2 Corrected historical claims

| Earlier claim or uncertainty | Current correction | Current evidence |
| --- | --- | --- |
| Product is only a foundation with no students or groups | Academic records and workflows now exist | Academic entities/use cases, endpoint registrations and frontend feature routes |
| Only centre creation writes business data | Academic, staff, settings and schedule commands write data | Dispatcher consumers and repositories |
| Centre creation is the only real use case | Many command/query pairs now implement product workflows | Application feature folders and handler catalogue |
| Roles are carried but never enforced | Code-defined permissions are checked before a transaction | Dispatcher.CheckPermission and RolePermissions |
| No tenant data filtering exists | EF filter convention applies to tenant-owned mapped entities | TenantQueryFilterConvention and AppDbContext |
| `ITenantOwned` has no implementers | Academic entities and audit infrastructure use tenant ownership | Current entity definitions and EF model |
| RLS is a future comment | Forced policies enforce transaction-local tenant setting | TenantRowLevelSecurity and tenant migrations |
| Tenant/permission steps in Dispatcher are placeholders | Both execute after validation and before handler resolution/BEGIN | Current command and query pipelines |
| Staff only exist through development seeding | Staff lifecycle use cases/endpoints and UI now exist | Staff endpoint group and account service |
| `MustChangePassword` is stored but not read | Login and password-change routing enforce the first-login flow | AuthEndpoints and frontend change-password route |
| No password-change path exists | Auth change endpoint and operator reset CLI exist | MapAuthEndpoints and Program CLI patterns |
| No product operation changes revocation inputs | Staff role/status and password operations affect validated facts | Staff commands, authentication adapter and session validation |
| Session validation ignores changed roles | Role is parsed, included in cache key and sent for validation | SessionRevalidationHandler and query handler |
| Key ring persistence is future work | Key ring persists through the database context | Infrastructure DI and IDataProtectionKeyContext |
| No app Dockerfile exists | Multi-stage production build is present | Root Dockerfile |
| Compose runs only PostgreSQL in all scenarios | Production overlay adds operator jobs, app and proxy | compose.prod.yaml |
| No static SPA serving exists | Built assets and deep links are served by API hosting helpers | SpaHostingSetup and Program |
| No forwarded-header handling exists | Known-network validated forwarding runs first | ForwardedHeadersSetup and ProxyOptionsValidator |
| No security headers or HSTS exist | Header setup and Production HSTS are registered | SecurityHeadersSetup and Program |
| Production migration execution is undefined | Dedicated migrate command and one-shot container job exist | MigrateCommand and Compose overlay |
| Dashboard is a placeholder | DashboardOverview composes current data summaries | Frontend dashboard feature |
| Frontend has only four feature areas | Academic and administrative feature areas have been added | Current frontend inventory |
| There are only three E2E journeys | Additional staff, academic, enrollment, schedule and full-setup journeys exist | frontend/e2e files and actual collected test declarations |
| Only two database migrations exist | Later migrations add tenancy, audit, key ring and academic schema | Current migration catalogue |
| No DTO response includes time-zone contract for selected centres | Centre time zone is published and consumed for schedule display | Membership/session DTOs and useCentreTimeZone |
| Slug race is only rethrown pending 409 translation | Recognized persistence conflicts have a shared translation boundary | PersistenceErrorTranslator and save result handling |
| Unit is unused / contradictory usage description | Unit is used as the response type of commands with no body | Enrollment withdrawal endpoint and corresponding command |
| Hand-written dispatcher motivation is unstated | ADR 0003 explicitly discusses ownership/learning, implicit behaviours and changed library licensing | Current ADR 0003 alternatives |
| Missing DB configuration degrades readiness | Required options still cause normal startup validation failure | DatabaseOptions and registration; obsolete comment remains a finding |
| Backend SDK could not be run | SDK exists here; actual build/test outcomes are recorded for this session | Session check output |
| Frontend lint has six warnings | Current lint run reports forty-two warnings | Executed frontend lint |
| Five translation namespaces | Current parity check covers fifteen namespaces | Executed i18n check and bundled resources |
| API contract contains six paths/nine schemas | Contract has grown with all mapped feature groups | Current OpenAPI catalogue below |
| No telemetry export setup exists | Optional OpenTelemetry setup is present | Api/Telemetry/TelemetrySetup.cs |
| No deployed smoke tooling exists | Smoke project and production-style verification job exist | smoke inventory and CI |
| Cloud deployment is not established | Still true: documented design is not a provisioned live instance | Current README and deployment documentation |

Unchanged mechanisms are taught again in this document rather than delegated to the older overview. Cookie flags, Result semantics, DI lifetimes, read-only query transactions, CSRF, log correlation, translation and generated-client principles remain relevant, but their surrounding feature set and execution paths have changed.

## 16.3 Added, removed, renamed and rewritten files

The committed change catalogue records Git's name-status result with rename detection. It is the exact file-level answer rather than an estimate based on folder names. New feature families include tenant isolation and permissions; audit; staff lifecycle and settings; students/parents; teachers/groups; enrollments; weekly scheduling; application hosting and observability. Substantial rewrites include `Program.cs`, dispatcher transaction/security plumbing, the database fixture, seed command, API contract/client and frontend route tree. Generated artifact changes must be reviewed through their source inputs as well as their diffs.

# 17. Glossary and Open Questions

| Term | Meaning in this repository |
| --- | --- |
| Centre / tenant | The customer boundary chosen through an active staff membership. |
| Actor | Application's trusted description of the current caller. |
| Staff membership | A user-centre relationship carrying role and active/inactive status. |
| Permission | A named capability from the closed code-defined catalogue. |
| Teacher scope | Record restriction connecting a staff user to the teacher's groups within one tenant. |
| Port / adapter | Inner-layer capability contract and its outer implementation. |
| Dispatcher | Shared validation, tenant, permission and transaction orchestration. |
| Command / query | A request allowed to change state / a read-only request. |
| Result | Expected success/failure represented as a value. |
| Problem Details | Shared HTTP error representation enriched with stable code and request identifiers. |
| Unit of work | Ownership boundary for tracked changes and a database transaction. |
| RLS | PostgreSQL row-level policies enforcing the tenant predicate. |
| Composite tenant foreign key | A relationship that includes centre identity as well as the related entity identifier. |
| Concurrency version | Expected row version checked during a write to detect stale changes. |
| Pessimistic lock | Lock acquired before reading the state used for a decision, such as the remaining capacity count. |
| Exclusion constraint | Database rule preventing conflicting ranges such as simultaneous active class sessions. |
| Aggregate | Domain boundary protecting related state changes, such as a student's parent links. |
| Value object | Value-defined data with invariants, such as a normalized phone number. |
| Schedule rule | Recurrence described in centre-local calendar/time terms. |
| Class session | One dated occurrence with definite UTC instants. |
| Security stamp | Account fact compared against tickets to reject outdated credentials/sessions. |
| Data Protection key ring | Cryptographic material used to protect session/antiforgery ticket data. |
| Antiforgery pair | Related cookie and request tokens validated together; not two identical plaintext values. |
| Query key | Address used by the browser's server-state cache; must account for relevant data identity. |
| Audit row | Policy-selected business change written atomically with the operation it describes. |
| Read ledger | File paths/hashes proving bytes were opened; not proof every branch executed. |
| Smoke test | Small whole-serving check of a production-style application URL. |

Open questions remain explicit. Remote branch-protection settings are not established by workflow YAML. The cloud subscription, managed resources, external URL and production telemetry collector are not provisioned by the files inspected here. Actual runtime dependency defaults must not be confused with repository configuration. The centre-switch cache finding needs a dedicated browser regression before its exact visible duration is claimed. A full-default-worker timeout pattern needs focused resource/test isolation investigation before being classified as a deterministic defect. Deployment documentation's port statement needs reconciliation with the merged Compose topology.

The final verification report records whether Markdown links, line references and Mermaid diagrams passed their checks. Successful local build/test execution is reported with the exact commands and limits; checks not executed remain unverified. Historical documents remain historical, and no unsupported claim of complete deployed production readiness is made.
