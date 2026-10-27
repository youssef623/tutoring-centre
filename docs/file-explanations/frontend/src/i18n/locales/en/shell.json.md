# `frontend/src/i18n/locales/en/shell.json`

## Purpose

JSON data with top-level keys `nav`, `header`, `userMenu`, `noAccess`, `dashboard`. Its consumers determine whether this is build configuration, a contract or a language resource.

## Where it fits

Category: **configuration**. [Open current file](../../../../../../../frontend/src/i18n/locales/en/shell.json). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 53 lines and 1,526 bytes. Source hash: `523fa2184e6fc98c1a4695bc8c8c2e8ce354046971a3679bfb326286cc5ec621`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

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
  "nav": {
    "dashboard": "Dashboard",
    "subjects": "Subjects",
    "students": "Students",
    "teachers": "Teachers",
    "groups": "Groups",
    "timetable": "Timetable",
    "staff": "Staff",
    "settings": "Settings",
    "audit": "Audit log",
    "openMenu": "Open menu",
    "closeMenu": "Close menu",
    "home": "Hessa home"
  },
  "header": {
    "activeCentre": "Active centre"
  },
  "userMenu": {
    "switchCentre": "Switch centre",
    "changePassword": "Change password",
    "logout": "Log out"
  },
  "noAccess": {
    "title": "You don't have access to this page",
    "description": "Your role doesn't include this permission. If you think this is a mistake, ask your centre's owner.",
    "backToDashboard": "Back to dashboard"
  },
  "dashboard": {
    "welcome": "Welcome, {name}",
    "activeCentre": "You are working in {centreName}.",
    "actions": {
      "registerStudent": "Register student",
      "students": "Students",
      "groups": "Groups"
    },
    "loadingLabel": "Loading the overview",
    "errorTitle": "The overview could not be loaded",
    "stats": {
      "activeGroups": "Active groups",
      "seatsTaken": "Seats taken",
      "openSeats": "Open seats",
      "fullGroups": "Full groups"
    },
    "fillingUp": {
      "title": "Groups filling up",
      "description": "Active groups with 80% or more of their seats taken.",
      "viewAll": "All groups",
      "empty": "No group is close to full.",
      "noGroups": "There are no active groups yet."
    }
  }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
