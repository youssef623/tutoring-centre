# `frontend/src/i18n/index.ts`

## Purpose

Source comment (intent): The persisted choice, else the browser language, else English. Only the language preference ever goes into localStorage.

## Where it fits

Category: **source**. [Open current file](../../../../../frontend/src/i18n/index.ts). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 131 lines and 4,610 bytes. Source hash: `c7817dbefeec0caff47d3fcf6d533c28683666e48ae713287d94883880973d48`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import i18next from "i18next";`
- Line 2: `import ICU from "i18next-icu";`
- Line 3: `import { initReactI18next } from "react-i18next";`
- Line 4: `import auditAr from "./locales/ar/audit.json";`
- Line 5: `import authAr from "./locales/ar/auth.json";`
- Line 6: `import commonAr from "./locales/ar/common.json";`
- Line 7: `import enrollmentsAr from "./locales/ar/enrollments.json";`
- Line 8: `import errorsAr from "./locales/ar/errors.json";`
- Line 9: `import groupsAr from "./locales/ar/groups.json";`
- Line 10: `import parentsAr from "./locales/ar/parents.json";`
- Line 11: `import scheduleAr from "./locales/ar/schedule.json";`
- Line 12: `import settingsAr from "./locales/ar/settings.json";`
- Line 13: `import shellAr from "./locales/ar/shell.json";`
- Line 14: `import staffAr from "./locales/ar/staff.json";`
- Line 15: `import statusAr from "./locales/ar/status.json";`
- Line 16: `import studentsAr from "./locales/ar/students.json";`
- Line 17: `import subjectsAr from "./locales/ar/subjects.json";`
- Line 18: `import teachersAr from "./locales/ar/teachers.json";`
- Line 19: `import auditEn from "./locales/en/audit.json";`
- Line 20: `import authEn from "./locales/en/auth.json";`
- Line 21: `import commonEn from "./locales/en/common.json";`
- Line 22: `import enrollmentsEn from "./locales/en/enrollments.json";`
- Line 23: `import errorsEn from "./locales/en/errors.json";`
- Line 24: `import groupsEn from "./locales/en/groups.json";`
- Line 25: `import parentsEn from "./locales/en/parents.json";`
- Line 26: `import scheduleEn from "./locales/en/schedule.json";`
- Line 27: `import settingsEn from "./locales/en/settings.json";`
- Line 28: `import shellEn from "./locales/en/shell.json";`
- Line 29: `import staffEn from "./locales/en/staff.json";`
- Line 30: `import statusEn from "./locales/en/status.json";`
- Line 31: `import studentsEn from "./locales/en/students.json";`
- Line 32: `import subjectsEn from "./locales/en/subjects.json";`
- Line 33: `import teachersEn from "./locales/en/teachers.json";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 38](../../../../../frontend/src/i18n/index.ts#L38): `function isSupportedLanguage(value: unknown): value is SupportedLanguage {`
- [Line 43](../../../../../frontend/src/i18n/index.ts#L43): `function detectInitialLanguage(): SupportedLanguage {`
- [Line 58](../../../../../frontend/src/i18n/index.ts#L58): `function applyDocumentDirection(language: string): void {`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
import i18next from "i18next";
import ICU from "i18next-icu";
import { initReactI18next } from "react-i18next";
import auditAr from "./locales/ar/audit.json";
import authAr from "./locales/ar/auth.json";
import commonAr from "./locales/ar/common.json";
import enrollmentsAr from "./locales/ar/enrollments.json";
import errorsAr from "./locales/ar/errors.json";
import groupsAr from "./locales/ar/groups.json";
import parentsAr from "./locales/ar/parents.json";
import scheduleAr from "./locales/ar/schedule.json";
import settingsAr from "./locales/ar/settings.json";
import shellAr from "./locales/ar/shell.json";
import staffAr from "./locales/ar/staff.json";
import statusAr from "./locales/ar/status.json";
import studentsAr from "./locales/ar/students.json";
import subjectsAr from "./locales/ar/subjects.json";
import teachersAr from "./locales/ar/teachers.json";
import auditEn from "./locales/en/audit.json";
import authEn from "./locales/en/auth.json";
import commonEn from "./locales/en/common.json";
import enrollmentsEn from "./locales/en/enrollments.json";
import errorsEn from "./locales/en/errors.json";
import groupsEn from "./locales/en/groups.json";
import parentsEn from "./locales/en/parents.json";
import scheduleEn from "./locales/en/schedule.json";
import settingsEn from "./locales/en/settings.json";
import shellEn from "./locales/en/shell.json";
import staffEn from "./locales/en/staff.json";
import statusEn from "./locales/en/status.json";
import studentsEn from "./locales/en/students.json";
import subjectsEn from "./locales/en/subjects.json";
import teachersEn from "./locales/en/teachers.json";

const StorageKey = "tcm.lang";
type SupportedLanguage = "ar" | "en";

function isSupportedLanguage(value: unknown): value is SupportedLanguage {
  return value === "ar" || value === "en";
}

/** The persisted choice, else the browser language, else English. Only the language preference ever goes into localStorage. */
function detectInitialLanguage(): SupportedLanguage {
  try {
    const stored = localStorage.getItem(StorageKey);
    if (isSupportedLanguage(stored)) {
      return stored;
    }
  } catch {
    // localStorage can throw (private browsing, blocked storage); fall through to browser detection.
  }

  const browserLanguage = typeof navigator === "undefined" ? "en" : navigator.language;
  return browserLanguage.toLowerCase().startsWith("ar") ? "ar" : "en";
}

/** Keeps <html> in sync with the current language: direction is a document-level property driven by language. */
function applyDocumentDirection(language: string): void {
  if (typeof document === "undefined") {
    return;
  }

  document.documentElement.lang = language;
  document.documentElement.dir = language === "ar" ? "rtl" : "ltr";
}

const initialLanguage = detectInitialLanguage();
applyDocumentDirection(initialLanguage);

/**
 * Resources are bundled at build time (no runtime fetch for two small languages). ICU message format
 * (single-brace placeholders, e.g. "{time}") replaces i18next's default interpolation so plural rules —
 * Arabic has six — can be expressed later without a different templating syntax.
 */
void i18next
  .use(ICU)
  .use(initReactI18next)
  .init({
    resources: {
      en: {
        common: commonEn,
        status: statusEn,
        errors: errorsEn,
        auth: authEn,
        shell: shellEn,
        subjects: subjectsEn,
        students: studentsEn,
        parents: parentsEn,
        staff: staffEn,
        settings: settingsEn,
```

The excerpt stops at line 90; use the original file for the remaining 41 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
