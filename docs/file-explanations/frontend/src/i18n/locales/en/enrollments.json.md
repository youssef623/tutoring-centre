# `frontend/src/i18n/locales/en/enrollments.json`

## Purpose

JSON data with top-level keys `seats`, `roster`, `actions`, `enrollDisabled`, `form`, `enrollDialog`, `picker`, `outcomes`, `toasts`, `withdrawDialog`, `transferDialog`, `studentCard`. Its consumers determine whether this is build configuration, a contract or a language resource.

## Where it fits

Category: **configuration**. [Open current file](../../../../../../../frontend/src/i18n/locales/en/enrollments.json). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 100 lines and 3,572 bytes. Source hash: `f37e0c6639b5769d79b12e2f7f120f908556bb50b41cd4a0751af7c794ed6c97`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

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
  "seats": {
    "label": "{enrolledText} of {capacity, plural, one {{capacityText} seat} other {{capacityText} seats}} taken",
    "full": "Full",
    "almostFull": "Almost full"
  },
  "roster": {
    "loadingLabel": "Loading the roster",
    "columns": {
      "student": "Student",
      "enrolledAt": "Enrolled on",
      "actions": "Actions"
    },
    "emptyTitle": "No students enrolled yet",
    "emptyDescription": "Students enrolled in this group will be listed here.",
    "errorTitle": "The roster could not be loaded",
    "errorReference": "Reference: {correlationId}"
  },
  "actions": {
    "enroll": "Enroll student",
    "openMenu": "Actions for {name}",
    "transfer": "Transfer to another group",
    "withdraw": "Withdraw"
  },
  "enrollDisabled": {
    "full": "This group is full. Withdraw a student or raise the capacity first.",
    "archived": "This group is archived. Restore it to enroll students."
  },
  "form": {
    "cancel": "Cancel"
  },
  "enrollDialog": {
    "title": "Enroll a student",
    "description": "Search for an active student to add to {group}.",
    "submit": "Enroll"
  },
  "picker": {
    "searchLabel": "Search students",
    "listLabel": "Students",
    "loadingLabel": "Loading students",
    "empty": "No active student matches this search.",
    "alreadyInGroup": "Already in this group"
  },
  "outcomes": {
    "groupFull": "This group is full",
    "groupArchived": "This group is archived and accepts no enrollments.",
    "alreadyEnrolled": "Already in this group",
    "studentArchived": "This student is archived and cannot be enrolled.",
    "alreadyEnded": "This enrollment has already ended.",
    "sameGroup": "The student is already in this group. Choose a different group.",
    "subjectMismatch": "A student can only be transferred to a group of the same subject.",
    "stale": "This enrollment was changed by someone else. The list has been refreshed.",
    "studentHasActiveEnrollments": "Withdraw this student from their groups first."
  },
  "toasts": {
    "enrolled": "{student} was enrolled in {group}.",
    "withdrawn": "{student} was withdrawn from {group}.",
    "transferred": "{student} was transferred to {group}."
  },
  "withdrawDialog": {
    "title": "Withdraw {student} from {group}?",
    "body": "{student} will leave this group's roster. The enrollment stays in their history.",
    "confirm": "Withdraw"
  },
  "transferDialog": {
    "title": "Transfer {student}",
    "description": "Choose another group of the same subject. The student leaves {group} in the same step.",
    "groupsLabel": "Target group",
    "loadingLabel": "Loading groups",
    "empty": "No other group for this subject",
    "submit": "Transfer"
  },
  "studentCard": {
    "title": "Groups",
    "enroll": "Enroll in a group",
    "loadingLabel": "Loading groups",
    "errorTitle": "The groups could not be loaded",
    "empty": "Not enrolled in any group yet.",
    "current": "Current groups",
    "past": "Past groups",
    "since": "Since {date}",
    "ended": "Ended {date} · {reason}"
  },
  "groupPicker": {
    "title": "Enroll in a group",
    "description": "Choose a subject, then one of its groups for {student}.",
    "subjectLabel": "Subject",
    "groupsLabel": "Group",
    "loadingLabel": "Loading groups",
    "empty": "This subject has no active group.",
```

The excerpt stops at line 90; use the original file for the remaining 10 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
