# `frontend/src/i18n/locales/ar/groups.json`

## Purpose

JSON data with top-level keys `page`, `filters`, `stats`, `columns`, `actions`, `loading`, `error`, `detailError`, `notFound`, `emptyState`, `detail`, `overview`. Its consumers determine whether this is build configuration, a contract or a language resource.

## Where it fits

Category: **configuration**. [Open current file](../../../../../../../frontend/src/i18n/locales/ar/groups.json). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 136 lines and 6,215 bytes. Source hash: `5b2f4e9d6e8ddd75f0f1df88856bcfb8b361076f6e3d232154340f85d0eb8391`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

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
    "title": "المجموعات",
    "subtitle": "الفصول التي يلتحق بها الطلاب: مادة واحدة ومعلم واحد وعدد محدد من المقاعد."
  },
  "filters": {
    "label": "تصفية المجموعات",
    "subject": "المادة",
    "teacher": "المعلم",
    "allSubjects": "كل المواد",
    "allTeachers": "كل المعلمين",
    "showArchived": "إظهار المؤرشفة"
  },
  "stats": {
    "activeGroups": "المجموعات النشطة",
    "openSeats": "المقاعد المتاحة",
    "fullGroups": "المجموعات المكتملة"
  },
  "columns": {
    "name": "الاسم",
    "subject": "المادة",
    "teacher": "المعلم",
    "seats": "المقاعد",
    "status": "الحالة"
  },
  "actions": {
    "create": "إنشاء مجموعة",
    "rename": "إعادة التسمية",
    "changeCapacity": "تغيير السعة",
    "changeTeacher": "تغيير المعلم",
    "archive": "أرشفة",
    "restore": "استعادة"
  },
  "loading": {
    "label": "جاري تحميل المجموعات",
    "detailLabel": "جاري تحميل المجموعة"
  },
  "error": {
    "title": "تعذر تحميل المجموعات",
    "reference": "المرجع:"
  },
  "detailError": {
    "title": "تعذر تحميل هذه المجموعة"
  },
  "notFound": {
    "title": "المجموعة غير موجودة",
    "description": "هذه المجموعة غير موجودة، أو ليس لديك صلاحية الوصول إليها.",
    "backToList": "العودة إلى المجموعات"
  },
  "emptyState": {
    "title": "لا توجد مجموعات حتى الآن",
    "description": "أنشئ مجموعة لمادة وأسند إليها معلمًا لتبدأ تسجيل الطلاب.",
    "assignedTitle": "لا توجد مجموعات مسندة إليك",
    "assignedDescription": "ستظهر هنا المجموعات التي تدرّسها عندما يسندها المركز إليك.",
    "noMatchTitle": "لا توجد مجموعات مطابقة لهذه التصفية",
    "noMatchDescription": "جرّب مادة أو معلمًا آخر، أو امسح التصفية."
  },
  "detail": {
    "backToList": "كل المجموعات"
  },
  "overview": {
    "seats": "المقاعد",
    "subject": "المادة",
    "teacher": "المعلم",
    "capacity": "السعة",
    "status": "الحالة"
  },
  "tabs": {
    "label": "أقسام المجموعة",
    "overview": "نظرة عامة",
    "roster": "قائمة الطلاب",
    "schedule": "الجدول"
  },
  "archivedBanner": {
    "title": "هذه المجموعة مؤرشفة",
    "description": "المجموعة المؤرشفة للقراءة فقط. استعدها لإعادة تسميتها أو تغيير سعتها أو تغيير معلمها."
  },
  "dialogs": {
    "createTitle": "إنشاء مجموعة",
    "createDescription": "اختر المادة والمعلم. لا يمكن تغيير المادة لاحقًا.",
    "renameTitle": "إعادة تسمية المجموعة",
    "renameDescription": "أسماء المجموعات فريدة داخل المركز.",
    "capacityTitle": "تغيير السعة",
    "capacityDescription": "بين 1 و200 مقعد. الملتحقون حاليًا: {enrolled}.",
    "teacherTitle": "تغيير المعلم",
    "teacherDescription": "سيرى حساب المعلم الجديد هذه المجموعة، ولن يراها حساب المعلم السابق."
  },
  "form": {
    "nameLabel": "الاسم",
    "namePlaceholder": "مثال: فيزياء – الصف الثاني الثانوي أ",
```

The excerpt stops at line 90; use the original file for the remaining 46 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
