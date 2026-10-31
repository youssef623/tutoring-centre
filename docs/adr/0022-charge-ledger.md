# ADR 0022: Immutable monthly charge obligations

Status: accepted design; execution evidence is in the Month 5 checkpoint.

## Decision

Use an insert-only billing.charges ledger. A monthly row snapshots the enrollment, student, subject, group, teacher, exact EGP amount and centre-local billing month. There is no stored balance. Only SELECT/INSERT are granted to tutoring_app, and forced tenant RLS plus composite foreign keys also constrain direct SQL. The three-column enrollment foreign key proves the charged student belongs to that enrollment.

A partial unique key on (centre_id,student_id,subject_id,billing_month) WHERE kind='monthly' is the exactly-once obligation boundary. A targeted ON CONFLICT insert returns Skipped for that key alone without aborting PostgreSQL's transaction. Other errors propagate and roll back all inserts/audits in the centre command. A successful raw insertion adds the same policy-controlled created audit entry to that transaction. Ordered batch keys avoid opposite-order unique-index lock acquisition.

Money is a signed decimal representation with no more than two fractional places and numeric(10,2) range validation. Fees are nonnegative and monthly obligations strictly positive at their own boundaries. Adjustment is reserved in the schema with nonzero amount/reason rules; Month 5 has no adjustment creation.

Money and BillingMonth live in Domain.Common because both Academics pricing and Billing use these value primitives. Charge/ChargeKind/BillingCalendar remain in Domain.Billing. This follows the existing shared-kernel boundary rather than adding a type exception or permitting Academics to depend on Billing aggregates. The no-floating-point architecture rule explicitly checks the shared financial primitives as well as Billing types.

The approved fee semantics use a small effective-month schedule owned by Academics. Initial fees apply in the creation month; edits apply from the next centre-local month. Re-editing that future month replaces its proposed fee. Missing historical rows mean the pre-billing zero fee. Historical generation selects the latest effective fee no later than the requested month. The group's monthly_fee is the latest advertised fee, not an instruction to rewrite current or historical obligations. Existing charges are never repriced. This is a price schedule, with no proration or adjustment workflow.

Monthly jobs fan out IDs through the existing system scope runner; enrollment/transfer enqueues an identifier-only message inside its own transaction. The eventual consumer re-resolves the winning chargeable enrollment. For overlaps within a subject, Academics chooses the latest enrolled_at, then greatest enrollment ID, before excluding an inactive/free group. List and single lookup share that selection.

## Alternatives and trade-offs

- A mutable balance or regenerated charge row loses the original obligation and creates drift; rejected. Future payments/allocations will derive balances from separate ledgers.
- A nonpartial unique key prevents future adjustments coexisting with monthly obligations; rejected.
- Catching any unique/database exception hides unrelated failures and leaves PostgreSQL aborted; rejected.
- Calling Billing inside enrollment couples modules and makes billing availability block enrollment; rejected. Atomic outbox enqueue keeps enrollment committed while retrying the consumer.
- Editing only a bare group fee makes “applies next month” false for an uncharged current enrollment. A current/pending pair cannot answer older backfills reliably. The user approved the minimal effective-month schedule instead of either shortcut.

The ledger snapshots IDs, not display names: authorized staff reads obtain current group/subject labels through an Academics contract. Teacher ownership at generation is snapshotted; no historical teacher-assignment subsystem is introduced. Currency and due day remain EGP/day 5 constants. Residual risks and deferred work are recorded in later.md.
