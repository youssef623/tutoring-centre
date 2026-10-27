# `frontend/src/i18n/locales/ar/audit.json`

## Purpose

JSON data with top-level keys `page`, `filters`, `entityType`, `action`, `field`, `value`, `system`, `referenceLabel`, `copyReference`, `copied`, `loadMore`, `loading`. Its consumers determine whether this is build configuration, a contract or a language resource.

## Where it fits

Category: **configuration**. [Open current file](../../../../../../../frontend/src/i18n/locales/ar/audit.json). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 105 lines and 3,046 bytes. Source hash: `303cb4b1b9186830b6db1bf95cf44571ce5d1a242f5f48029432aae577c6da36`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

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
  "page": {
    "title": "سجل النشاط",
    "subtitle": "كل تغيير تم في هذا المركز، الأحدث أولًا.",
    "filtersLabel": "تصفية سجل النشاط"
  },
  "filters": {
    "entityTypeLabel": "نوع العنصر",
    "entityTypeAll": "الكل",
    "fromLabel": "من",
    "toLabel": "إلى"
  },
  "entityType": {
    "subject": "مادة",
    "membership": "عضوية موظف",
    "centre": "المركز",
    "parent": "ولي أمر",
    "student": "طالب",
    "student_parent": "رابط طالب وولي أمر",
    "teacher": "معلم",
    "group": "فصل",
    "enrollment": "تسجيل",
    "schedule_rule": "قاعدة جدول",
    "class_session": "حصة"
  },
  "action": {
    "created": "تم الإنشاء",
    "updated": "تم التحديث",
    "deleted": "تم الحذف"
  },
  "field": {
    "name": "الاسم",
    "status": "الحالة",
    "role": "الدور",
    "userId": "المستخدم",
    "defaultLocale": "اللغة الافتراضية",
    "fullName": "الاسم الكامل",
    "phone": "الهاتف",
    "schoolYear": "الصف الدراسي",
    "studentId": "الطالب",
    "parentId": "ولي الأمر",
    "relationship": "صلة القرابة",
    "isPrimary": "جهة الاتصال الأساسية",
    "subjectId": "المادة",
    "teacherId": "المعلم",
    "capacity": "السعة",
    "groupId": "الفصل",
    "endReason": "سبب الانتهاء",
    "dayOfWeek": "اليوم",
    "startTime": "موعد البدء",
    "durationMinutes": "المدة (بالدقائق)",
    "room": "القاعة",
    "effectiveFrom": "سارية من",
    "effectiveTo": "سارية حتى"
  },
  "value": {
    "status": {
      "active": "نشط",
      "archived": "مؤرشف",
      "inactive": "غير نشط",
      "ended": "منتهٍ",
      "scheduled": "مجدولة",
      "cancelled": "ملغاة"
    },
    "endReason": {
      "withdrawn": "انسحاب",
      "transferred": "نقل"
    },
    "relationship": {
      "father": "الأب",
      "mother": "الأم",
      "guardian": "ولي الأمر",
      "other": "أخرى"
    },
    "isPrimary": {
      "true": "نعم",
      "false": "لا"
    },
    "dayOfWeek": {
      "saturday": "السبت",
      "sunday": "الأحد",
      "monday": "الاثنين",
      "tuesday": "الثلاثاء",
      "wednesday": "الأربعاء",
      "thursday": "الخميس",
      "friday": "الجمعة"
    }
  },
  "system": "النظام",
  "referenceLabel": "المرجع:",
```

The excerpt stops at line 90; use the original file for the remaining 15 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
