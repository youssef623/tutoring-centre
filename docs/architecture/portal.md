# Portal identity

Portal accounts reuse the existing login and per-centre membership. `PortalRole` limits the portal factory to parent and student. The existing `StaffRole` wire and persistence discriminator now carries all five role keys for compatibility with session claims and the single permission lookup. Staff factories, validators and staff queries must explicitly restrict it to owner, secretary and teacher.

Portal roles hold only `attendance.view` and `portal.chat`. Permissions grant capabilities; the immutable `PortalLink` identifies whose records the account may access. Links duplicate no names, email addresses or other personal details.

`identity.portal_links` has forced tenant RLS and restrict composite foreign keys to its parent or student. A role-bearing foreign key to `(centre_id, user_id, role)` in memberships prevents promotion while linked. The target is a unique index, deliberately not an EF alternate key: EF treats alternate-key properties as immutable, which would also prevent legitimate staff role changes. The FK is maintained explicitly in migration SQL because the domain link exposes a string role key and the membership uses the existing converted enum discriminator.

The application role has only SELECT and INSERT on links. Moving an account to another subject requires a separately designed administrative lifecycle; current use cases never update or delete links. Link creation is audited with only role, parent ID and student ID.

A second FK to the stable membership identity `(centre_id, user_id)` gives EF an insert dependency so membership and link can be saved together. Its EF alternate key is separate from the existing unique index: earlier migrations already reference that index, so replacing it would break existing foreign keys. Neither stable identity property can change; the staff role remains mutable.

## Account use cases and sessions

Create, deactivate, reactivate and list requests require `portal.accounts.manage` and a selected centre. Creation checks an active subject through the Academics contract before ensuring a login, then creates the membership and link in the dispatcher's transaction. Any later failure rolls back even the user saved by Identity. Subject and login uniqueness races return the same portal conflict codes as pre-checks; shared membership constraints retain their existing staff codes for staff creation.

Session validation returns validity plus an optional portal subject. The boolean conversion preserves existing validation consumers. A portal session is valid only with an active, matching membership and link. Before actor construction, the session read temporarily binds forced RLS to the centre established by that user's active membership, reads the link with explicit centre/user/role predicates, then restores the ambient setting. This is a narrowly allow-listed raw read, not an RLS exemption.

The existing session cache stores validity and the subject for its configured revalidation window. The middleware constructs `ParentActor` or `StudentActor` only from that validated result, including cache hits. The cookie still contains only user, security stamp, centre and role; no subject or student set is added. Missing links, inactive memberships and changed role claims fail the next revalidation. Child membership changes are visible on the next request because student sets are never cached across requests.

Account deactivation changes only membership status and uses its row version. It preserves the link, login and other centres' memberships.

## HTTP and browser boundary

The four `/api/portal-accounts` routes dispatch the account use cases. They accept no centre, role or creator identifiers. Creation returns a one-time credential only in its 201 body, with `Cache-Control: no-store`; list responses contain status, email and version but never credentials. Unsafe routes use the shared antiforgery filter and strict JSON binding.

Parents and students land at `/portal`, with a full-viewport shell using the staff product's Hessa branding, fonts and teal controls. A faint grid distinguishes the portal without introducing extra capabilities or outer page margins. Chat and Attendance remain disabled until their later slices. Guards redirect portal users away from staff pages and staff users away from the portal; API permissions remain the security boundary. The two staff-specific attendance reads also reject portal actors despite the shared attendance permission.

Account creation calls the generated API function directly, outside the mutation cache. Its password lives only in mounted component state, is cleared when the dialog closes, and never enters browser storage or query results. Development seeds create `parent@nile.test` and `student@nile.test` through the same command as staff, as a system actor attributed to the existing owner. Testing retains the established seed fixtures. Production never receives demo credentials.

Password recovery is not implemented. A forgotten portal password currently requires a separately designed recovery workflow; account deactivation and reactivation do not reset credentials.
# Month 5 checkpoint

Portal finance remains unavailable: Parent/Student do not hold billing.view or billing.generate, and receive 403 from the staff charges endpoint. Parent chat and attendance still resolve only the validated portal subject's family. Deactivation/session revocation and first-login password change remain the existing Identity pipeline; no second authentication implementation is added. See the fresh-local runbook and Month 5 checkpoint for executed evidence and any unavailable live-provider checks.
