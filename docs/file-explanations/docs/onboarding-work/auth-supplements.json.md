# `docs/onboarding-work/auth-supplements.json`

## Purpose

JSON data with top-level keys `auth-cookie`, `auth-csrf`, `auth-rate`, `auth-revalidation`, `auth-identity`, `auth-authorization`. Its consumers determine whether this is build configuration, a contract or a language resource.

## Where it fits

Category: **configuration**. [Open current file](../../../onboarding-work/auth-supplements.json). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 8 lines and 5,574 bytes. Source hash: `f30cdf9031896729b572e4c9371157a59028bed657311d7d887833af7a92802f`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

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
  "auth-cookie": "##### Ticket storage does not make the profile static\n\nThe principal factory and the profile query deliberately have different outputs. The cookie can remain the same while a profile query returns a changed display name or a different active membership list. Conversely, receiving a new profile DTO does not update the cookie's centre and role unless the server performs the session transition. This distinction matters during debugging: compare the selected membership returned by the fresh query with the claims used by actor construction instead of assuming both copies changed together.\n\nFor the same reason, restart verification must exercise an already-issued ticket. Merely signing in again after a restart proves the new host can issue a ticket; it does not prove that the old ticket survived. A meaningful persistence test obtains the cookie from one host and presents that same cookie to another compatible host using the database key ring. The test's claim is cross-host ticket acceptance, not successful cloud failover or permanent key availability. Those stronger operational conclusions need their own evidence and retention policy.",
  "auth-csrf": "##### Promise caching is part of the token lifecycle\n\n`csrf.ts:5` caches a `Promise<string>`, not only the eventual string. Concurrent callers can therefore share the in-flight token acquisition instead of each issuing a separate initial request. `fetchToken` clears that cached reference when acquisition fails, so a rejected promise does not poison every later action forever. `refreshCsrfToken` replaces it with a fresh acquisition. These details explain why tests should distinguish initial concurrent acquisition, failed acquisition, a stale-token retry and a session-changing refresh. They are different states even though each eventually needs a token header.",
  "auth-rate": "##### The rejection body belongs to the common error contract\n\n`RateLimitRejection.cs:17` obtains retry metadata from the lease, falls back to one minute when absent, formats seconds with invariant culture and writes `Retry-After`. It then uses `ProblemResult` with status429 and code `auth.rate_limited`. This preserves trace/correlation enrichment instead of returning an unrelated plaintext error. The same machine code covers both named policies, so a consumer should not assume every429 means exactly ten login attempts. The endpoint and policy identify the operation; the shared body identifies the broad refusal.",
  "auth-revalidation": "##### Source comments and the executed predicate need separate review\n\nThe callback's summary retains an older cache-key description that omits role. Its actual tuple and query now include role. When adding another freshness input, inspect executable construction and comparison rather than extending only the summary. The state service's no-centre branch is equally important: it reports null role, which must match an unselected ticket. Accidentally assigning a default role there could either reject every unselected session or make a partial identity appear selected to later checks. Test the intermediate state explicitly instead of exercising only fully selected owners.",
  "auth-identity": "##### Relationship integrity is more than an in-memory list\n\n`MembershipConfiguration.cs:64` creates the unique centre/user index. Its user and centre foreign keys restrict deletion, and its role/status converters correspond to check constraints. The row-version mapping adds stale-update detection to role and status transitions. Each mechanism answers a different question: existence of referenced records, uniqueness of the relationship, permitted stored values and whether the caller edited the version it last read. An entity factory cannot prove all of them without current storage state.\n\n`StaffAccountService` also distinguishes finding an existing account from creating one. Existing identity returns no temporary password. A new identity receives a cryptographically generated sixteen-character password and the pending-change flag. That result then participates in staff-use-case orchestration. Never show or reconstruct a temporary credential for an already-existing account merely because a second membership was added; the service explicitly reports whether creation occurred.",
  "auth-authorization": "##### Permission lists describe capability, not ownership evidence\n\nThe current role map gives an owner all sixteen catalogue capabilities. A secretary receives academic management except teacher management and does not receive owner staff/settings/audit capabilities. A teacher receives subject, group, enrollment and schedule views without manage capabilities. These lists explain why some screens disappear while some rows within a visible screen remain restricted. A frontend array containing `groups.view` is not proof that a particular group belongs to the linked teacher. The read service must establish that relationship for the requested record.\n\nAuthorization tests therefore need both broad and narrow negatives. A secretary attempting staff management demonstrates capability denial. A teacher requesting another teacher's group demonstrates ownership restriction despite possessing the general view key. A user requesting another centre's identifier demonstrates tenant isolation. Combining them into one test with a caller lacking every prerequisite would let an early denial mask missing checks later in the chain. Independent probes are the reason the test suite exercises these dimensions separately. "
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
