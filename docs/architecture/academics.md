# Academics: students and parents (Day 37)

The Domain model behind registering a student and the parents a centre contacts about them. Written from the
implementation; every name and rule below is checked against the code, not the plan that produced it.
Day 37 is Domain only — nothing here is persisted or exposed over HTTP yet (Days 38–41).

## Entities

| Type | Kind | Tenant-owned | Lives in |
| --- | --- | --- | --- |
| `PhoneNumber` | value object | — | `TutoringCentre.Domain.Common` |
| `SearchText` | pure function | — | `TutoringCentre.Domain.Common` |
| `Parent` | aggregate root | yes | `TutoringCentre.Domain.Academics` |
| `Student` | aggregate root | yes | `TutoringCentre.Domain.Academics` |
| `StudentParentLink` | child entity of `Student` | yes | `TutoringCentre.Domain.Academics` |

`PhoneNumber` and `SearchText` sit in `Common` rather than `Academics` because both are used by more than one
future aggregate (`Teacher`, Day 43) — the same reasoning `docs/architecture/overview.md` gives for the
Domain/Application `Common` namespaces being the shared kernel, not a fifth module.

## Why `Parent` is a separate aggregate, not part of `Student`

A parent is edited on its own (a phone number changes; a name is corrected) and is shared across siblings —
two `Student` rows can name the same parent. Nesting `Parent` inside `Student` would mean one student's save
could touch a row another student's aggregate also claims to own, and editing a parent's phone would require
loading and re-saving every student that references them. `Student` therefore holds only a `ParentId`
(`Guid`), never a `Parent` reference or navigation property — the two aggregates communicate by identity, the
same pattern tenant isolation's Layer 4 relies on for cross-table references (`docs/architecture/tenancy.md`).
Nothing in `Student` or `Parent` knows whether a given `ParentId` actually exists in the centre; that check is
an Application-layer concern (Day 39), not a Domain rule — the Domain only knows the shape of a valid link,
not whether the other side of it is real.

## Why `StudentParentLink` is not a separate aggregate

Unlike `Parent`, a link has no identity or lifecycle of interest outside the one invariant below: it is
created, reassigned as primary, or removed only as a side effect of keeping that invariant true. Letting
something outside `Student` create or mutate a link would mean the invariant could be broken by code that
never consulted it. `StudentParentLink`'s constructor and its two primary-flag mutators
(`MarkPrimary`/`ClearPrimary`) are `internal` — reachable from anywhere in the Domain assembly, which is as
tight as C# access modifiers get without a second assembly, and tight enough that no Application or
Infrastructure code can construct or change one directly.

## The parent-link invariant

A `Student` always has:

- between 1 and 4 parent links (`Student.MinParents`, `Student.MaxParents`);
- never two links to the same `ParentId`;
- exactly one link with `IsPrimary == true`.

Every operation on `Student` — `Register`, `AddParent`, `RemoveParent`, `SetPrimaryParent`, `Archive`,
`Restore`, `UpdateDetails` — either leaves this true or fails and changes nothing; there is no path that
leaves a student with zero parents, two primaries, or a duplicate link. `StudentTests` asserts the invariant
itself, not just each operation's own effect, after every call including every failure.

`LinksRevision` increases by exactly one on every successful `AddParent`, `RemoveParent` and
`SetPrimaryParent`, and never on a failure or on `Archive`/`Restore`/`UpdateDetails`. Its purpose is Day 38's:
a parent link lives in a child table, and the optimistic-concurrency row version Month 2 put on the aggregate
root's own row (`xmin`) has nothing to say about a child row changing underneath it. Bumping a counter on the
root every time a child changes gives the root's row version a reason to change too, so a stale read of the
student (with its cached link count or primary parent) is caught the same way any other stale write already
is.

## The phone format

`PhoneNumber.Create(input)` accepts a wide range of spellings and reduces every one of them to canonical
E.164, or fails with `phone.invalid`. Accepted forms, exactly as pinned in `PhoneNumberTests`:

- a local Egyptian mobile number, any punctuation, Western or Arabic-Indic digits — `01012345678`,
  `010 1234 5678`, `٠١٠١٢٣٤٥٦٧٨` — all become `+201012345678`;
- the same number already in E.164, or with a leading `00` or a bare `20` in place of `+20`;
- an already-E.164 number for another country, e.g. `+966501234567`, unchanged; `00` in front of a country
  code becomes `+`.

Rejected: anything too short or too long, an Egyptian landline (prefix other than `010`/`011`/`012`/`015`),
a value with letters, or a value that does not start with `+` after the transformations above. The rule set
mirrors Egypt's actual mobile-prefix allocation (`010`, `011`, `012`, `015`); a new prefix added later is a
one-line change to the pattern, confirmed by the `015`-prefix mutation check in `PhoneNumberTests`.

`Masked` reveals a leading prefix and the last four digits, nothing more: `+20` for an Egyptian number (since
this type already knows that prefix), or just `+` and the first digit for anything else (since this type has
no database of other countries' country-code lengths to reveal more safely). Everything between becomes one
`•` per hidden digit.

## Search normalization

`SearchText.Normalize(value)` is the one function that turns a name — Arabic or Latin, however it was typed —
into a form used only for matching, never shown to a user. It lower-cases, collapses whitespace, strips
Arabic diacritics and tatweel, folds the four hamza-bearing letters and `ى`/`ة`/`ؤ`/`ئ` to their bare forms,
and maps Arabic-Indic digits to ASCII. `Parent` and `Student` call it from `SetName` whenever `FullName`
changes, storing the result as `SearchName`; Day 40's search query will call the same function on the search
term before comparing, so a search for "احمد" matches a student stored as "أحمد" without either side
special-casing the other. One function, called from both the write side and the read side, is what keeps that
true — two independent implementations could drift silently the first time either one changed.
