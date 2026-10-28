# ADR 0014: Opaque hashed card tokens

- **Status:** Draft (Day 61; final decision planned for Day 66)
- **Date:** 2026-10-28

## Context

A student's physical card is a bearer credential for attendance. The printed value must not disclose
student identity or centre membership. A database disclosure must not directly expose usable card
credentials. Office staff need to replace and revoke cards without deleting their history.

## Alternatives considered

- **Opaque random token plus SHA-256 (proposed).** Generate 32 bytes with the BCL cryptographic random
  generator and encode them as unpadded base64url after `TCM1.`. Store only the 32-byte SHA-256 digest
  of the UTF-8 token text. A fast digest permits indexed lookup; 256 random bits make guessing
  impractical, so password-style hashing adds cost without a useful guessing defence.
- **Signed tokens.** Can authenticate embedded claims without a database lookup, but readable claims
  disclose information, keys require rotation, and prompt revocation still needs server-side state.
  Even claim-free signed tokens add key management without removing the need for a card record.
- **Clear-text storage.** Makes reprinting easy, but a database reader could immediately copy usable
  bearer credentials. Encryption would add recoverable secrets and key management for reprinting.
- **bcrypt.** Appropriate for low-entropy human passwords. For uniformly random 256-bit credentials,
  its deliberately expensive work adds scan cost and complicates indexed lookup without a practical
  security gain over SHA-256. The token is not a user-chosen password.

## Decision (draft)

Use `CardToken` as the sole owner of generation, parsing, hashing and deliberate reveal. The complete
48-character text is `TCM1.` followed by 43 URL-safe characters. Hash the full normalized UTF-8 text,
including the version prefix, in `CardToken.Hash()` only. `ToString()` returns `TCM1.…` so incidental
interpolation does not expose the body. Reveal is reserved for building the initial issue response.

`StudentCard` holds a defensively copied hash, immutable centre and student references, issue metadata,
and one-way revocation metadata. It never stores a token object or token text. Compare candidate
hashes with the BCL fixed-time comparison. Keep revoked cards so future scans can distinguish a
revoked card within the selected centre from an unrecognised one. Reissuing creates a new card;
revocation never reactivates an old card.

Owner and Secretary hold `attendance.cards.manage`. Teacher retains `attendance.mark` and cannot
mint cards. Attendance lateness is a pure instant comparison: strictly after session start plus
15 minutes is Late; the boundary itself and earlier eligible arrivals are Present. The caller is
responsible for checking the attendance window.

## Consequences

- A card **cannot be reprinted**, only replaced by reissuing: the stored digest cannot recover the
  token. This gives up reprint convenience to avoid storing a usable bearer credential.
- The QR payload is plain opaque text, with no student ID, name, centre, URL or embedded claims.
  Possession still permits copying the card; hashing at rest does not prevent physical sharing.
- Unknown tokens and tokens from another centre must produce the identical `card.not_recognised`
  message. The message never confirms existence or ownership elsewhere.
- Keep revoked records for accountability and replacement guidance. Do not delete on revocation.
- Persistence, the one-active-card constraint, lookup, use cases and scan-path details are deferred
  to Days 62–63. This draft does not add a table, endpoint or handler.

## Evidence

`CardTokenTests` pins the alphabet, length, strict prefix, 1,000 distinct generations, masked display
and an independent Node crypto SHA-256 known answer. A weakened prefix check failed both wrong-prefix
cases and was restored. `StudentCardTests` pins defensive hash copies and irreversible revocation;
`ScanPolicyTests` pins the 15-minute boundary using fixed instants. `RolePermissionsTests` includes
60 literal matrix cells, including Teacher denial for card management.
