# `frontend/src/i18n/locales/ar/status.json`

## Purpose

JSON data with top-level keys `readiness`, `systemInfo`. Its consumers determine whether this is build configuration, a contract or a language resource.

## Where it fits

Category: **configuration**. [Open current file](../../../../../../../frontend/src/i18n/locales/ar/status.json). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 23 lines and 1,223 bytes. Source hash: `0e101afa15d8f3c21a739f45988e08828c25be39af23927b3da6ba26d908829c`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

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
  "readiness": {
    "checking": "جارٍ التحقق من حالة واجهة البرمجة",
    "unreachableTitle": "تعذر الوصول إلى واجهة البرمجة",
    "unreachableBody": "لم تستجب واجهة البرمجة. تأكد من أنها تعمل، ثم حاول مرة أخرى.",
    "databaseUnavailableTitle": "واجهة البرمجة تعمل لكن قاعدة البيانات غير متاحة",
    "databaseUnavailableBody": "شغّل PostgreSQL، ثم حاول مرة أخرى.",
    "healthyTitle": "سليم",
    "healthyDescription": "واجهة البرمجة وقاعدة البيانات متاحتان",
    "lastCheckedAt": "آخر فحص في {time}"
  },
  "systemInfo": {
    "loading": "جارٍ تحميل معلومات النظام",
    "unavailableTitle": "معلومات النظام غير متاحة",
    "reference": "المرجع: {id}",
    "title": "معلومات النظام",
    "schemaUpToDate": "مخطط قاعدة البيانات محدّث",
    "migrationsPending": "هناك ترحيلات معلّقة",
    "applicationVersion": "إصدار التطبيق",
    "latestMigration": "آخر ترحيل",
    "none": "لا يوجد"
  }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
