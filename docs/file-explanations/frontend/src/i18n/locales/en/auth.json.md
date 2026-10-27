# `frontend/src/i18n/locales/en/auth.json`

## Purpose

JSON data with top-level keys `login`, `panel`, `validation`, `changePassword`, `errors`, `selectCentre`, `roles`. Its consumers determine whether this is build configuration, a contract or a language resource.

## Where it fits

Category: **configuration**. [Open current file](../../../../../../../frontend/src/i18n/locales/en/auth.json). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 68 lines and 2,937 bytes. Source hash: `c5e167ccb82aea107356e0b6b57e44fe478d64ff56b7c9276d480fef54cd993b`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

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
  "login": {
    "brand": "Hessa",
    "eyebrow": "Welcome back",
    "title": "Sign in",
    "subtitle": "Enter your details to manage your tutoring centre.",
    "emailLabel": "Email",
    "passwordLabel": "Password",
    "submit": "Sign in",
    "submitting": "Signing in…"
  },
  "panel": {
    "tagline": "Run your tutoring centre with confidence.",
    "description": "Students, groups, attendance, and fees — all in one calm workspace.",
    "features": {
      "attendance": { "title": "Attendance", "description": "Mark by hand or scan QR codes in seconds." },
      "groups": { "title": "Groups & schedules", "description": "Organise classes, teachers, and timetables." },
      "fees": { "title": "Fees & payments", "description": "Track dues and settle balances clearly." },
      "assistant": { "title": "WhatsApp assistant", "description": "Parents get answers in Arabic and English." }
    }
  },
  "validation": {
    "emailRequired": "Email is required.",
    "emailInvalid": "Enter a valid email address.",
    "passwordRequired": "Password is required.",
    "currentPasswordRequired": "Enter your current password.",
    "newPasswordTooShort": "The new password must be at least 10 characters.",
    "newPasswordSameAsCurrent": "The new password must be different from your current password.",
    "confirmPasswordRequired": "Confirm the new password.",
    "confirmPasswordMismatch": "The passwords don't match."
  },
  "changePassword": {
    "title": "Change password",
    "subtitle": "Choose a new password for your account.",
    "firstLoginSubtitle": "You're signing in with a temporary password. Choose a new password to continue.",
    "currentPasswordLabel": "Current password",
    "newPasswordLabel": "New password",
    "confirmPasswordLabel": "Confirm new password",
    "submit": "Change password",
    "submitting": "Changing password…",
    "toastSuccess": "Password changed."
  },
  "errors": {
    "invalidCredentials": "Incorrect email or password.",
    "rateLimited": "Too many attempts. Please wait a minute and try again."
  },
  "selectCentre": {
    "title": "Choose a centre",
    "subtitle": "{count, plural, one {You work in one centre.} other {You work in # centres.}} What you can see and do depends on your role in each.",
    "signedInAs": "Signed in as",
    "logout": "Log out",
    "missingCentre": "Missing a centre? Ask its owner to invite you with this email address.",
    "noActiveCentre": {
      "title": "No active centre",
      "description": "Your account isn't linked to any active centre right now. Either no centre has invited you yet, or your access was deactivated.",
      "inviteLabel": "Ask the centre owner to invite this email",
      "copy": "Copy",
      "copied": "Copied!",
      "checkAgain": "Check again",
      "checking": "Checking…"
    }
  },
  "roles": {
    "owner": "Owner",
    "teacher": "Teacher",
    "secretary": "Assistant"
  }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
