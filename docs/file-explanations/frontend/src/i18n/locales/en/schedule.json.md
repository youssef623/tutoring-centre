# `frontend/src/i18n/locales/en/schedule.json`

## Purpose

JSON data with top-level keys `weekdays`, `durations`, `timeZoneNote`, `rules`, `actions`, `addDisabled`, `addDialog`, `endDialog`, `cancelDialog`, `extendDialog`, `form`, `validation`. Its consumers determine whether this is build configuration, a contract or a language resource.

## Where it fits

Category: **configuration**. [Open current file](../../../../../../../frontend/src/i18n/locales/en/schedule.json). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 146 lines and 6,200 bytes. Source hash: `5d8e76c0737a5cd62130cdf9bc342e061f6539cf2d46bed9950864e69f6b5ace`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

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
  "weekdays": {
    "saturday": "Saturday",
    "sunday": "Sunday",
    "monday": "Monday",
    "tuesday": "Tuesday",
    "wednesday": "Wednesday",
    "thursday": "Thursday",
    "friday": "Friday"
  },
  "durations": {
    "minutes": "{minutes, plural, one {# minute} other {# minutes}}",
    "hoursAndMinutes": "{hours, plural, one {# hour} other {# hours}}{minutes, plural, =0 {} one { # minute} other { # minutes}}"
  },
  "timeZoneNote": "Times are in the centre's time zone ({zone}).",
  "rules": {
    "loadingLabel": "Loading weekly sessions",
    "errorTitle": "Weekly sessions could not be loaded",
    "errorReference": "Reference: {correlationId}",
    "emptyTitle": "No weekly sessions yet",
    "emptyDescription": "A weekly session repeats on the same day and time every week, and fills the timetable four weeks ahead.",
    "emptyDescriptionReadOnly": "When the office adds a weekly session for this group, it appears here and on the timetable.",
    "columns": {
      "day": "Day",
      "time": "Time",
      "room": "Room",
      "effective": "In effect",
      "status": "Status",
      "actions": "Actions"
    },
    "noRoom": "No room",
    "effectiveOpen": "From {from}",
    "effectiveRange": "{from} to {to}",
    "status": {
      "active": "Active",
      "upcoming": "Starts {date}",
      "ending": "Ends {date}",
      "ended": "Ended"
    }
  },
  "actions": {
    "add": "Add weekly session",
    "end": "End",
    "endLabel": "End {day} at {time}",
    "extend": "Extend schedule",
    "openSessionMenu": "Actions for {group} at {time}",
    "cancelSession": "Cancel session"
  },
  "addDisabled": {
    "archived": "This group is archived. Restore it to add weekly sessions."
  },
  "addDialog": {
    "title": "Add weekly session",
    "description": "The group's teacher, {teacher}, teaches every session.",
    "dayLabel": "Day",
    "dayPlaceholder": "Choose a day",
    "timeLabel": "Start time",
    "hourLabel": "Hour",
    "minuteLabel": "Minute",
    "periodLabel": "AM or PM",
    "am": "AM",
    "pm": "PM",
    "durationLabel": "Length",
    "roomLabel": "Room (optional)",
    "roomPlaceholder": "e.g. Room 2",
    "startDateLabel": "Starts on",
    "startDateHint": "Sessions are created from this date for the next four weeks.",
    "submit": "Add weekly session"
  },
  "endDialog": {
    "title": "End {day} at {time}?",
    "body": "Sessions after the last date are cancelled. Earlier sessions stay as they are.",
    "lastDateLabel": "Last date",
    "lastDateHint": "Not before {date}, the day it took effect.",
    "confirm": "End weekly session"
  },
  "cancelDialog": {
    "title": "Cancel {group} on {date} at {time}?",
    "body": "The session stays on the timetable, marked as cancelled.",
    "keep": "Keep session",
    "confirm": "Cancel session"
  },
  "extendDialog": {
    "title": "Extend the schedule",
    "description": "Creates the sessions of every weekly session in effect, up to a date. Sessions that already exist are kept as they are.",
    "throughLabel": "Create sessions through",
    "throughHint": "From today up to {max}.",
    "submit": "Extend"
  },
  "form": {
```

The excerpt stops at line 90; use the original file for the remaining 56 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
