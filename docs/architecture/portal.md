# Portal identity

Portal accounts reuse the existing login and per-centre membership. `PortalRole` limits the portal factory to parent and student. The existing `StaffRole` wire and persistence discriminator now carries all five role keys for compatibility with session claims and the single permission lookup. Staff factories, validators and staff queries must explicitly restrict it to owner, secretary and teacher.

Portal roles hold only `attendance.view` and `portal.chat`. Permissions grant capabilities; the immutable `PortalLink` identifies whose records the account may access. Links duplicate no names, email addresses or other personal details.

`identity.portal_links` has forced tenant RLS and restrict composite foreign keys to its parent or student. A role-bearing foreign key to `(centre_id, user_id, role)` in memberships prevents promotion while linked. The target is a unique index, deliberately not an EF alternate key: EF treats alternate-key properties as immutable, which would also prevent legitimate staff role changes. The FK is maintained explicitly in migration SQL because the domain link exposes a string role key and the membership uses the existing converted enum discriminator.

The application role has only SELECT and INSERT on links. Moving an account to another subject requires a separately designed administrative lifecycle; current use cases never update or delete links. Link creation is audited with only role, parent ID and student ID.

A second FK to the stable membership identity `(centre_id, user_id)` gives EF an insert dependency so membership and link can be saved together. Its EF alternate key is separate from the existing unique index: earlier migrations already reference that index, so replacing it would break existing foreign keys. Neither stable identity property can change; the staff role remains mutable.
