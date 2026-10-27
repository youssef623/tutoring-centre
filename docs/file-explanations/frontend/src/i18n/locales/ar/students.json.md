# `frontend/src/i18n/locales/ar/students.json`

## Purpose

JSON data with top-level keys `page`, `columns`, `actions`, `loading`, `error`, `detailError`, `notFound`, `emptyState`, `detail`, `parentsCard`, `addParent`, `archivedBanner`. Its consumers determine whether this is build configuration, a contract or a language resource.

## Where it fits

Category: **configuration**. [Open current file](../../../../../../../frontend/src/i18n/locales/ar/students.json). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 119 lines and 4,992 bytes. Source hash: `9d84af87c56a6d01f32e7cbdb280638d9a3b56fb6bf5f805927582b21f488aa6`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

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
    "title": "الطلاب",
    "searchLabel": "البحث عن الطلاب",
    "showArchived": "إظهار المؤرشفين",
    "subtitle": "كل الطلاب المسجلين في مركزك مع ولي الأمر الأساسي.",
    "filtersLabel": "تصفية الطلاب"
  },
  "columns": {
    "name": "الاسم",
    "schoolYear": "الصف الدراسي",
    "primaryParent": "ولي الأمر الأساسي",
    "parentPhone": "هاتف ولي الأمر",
    "status": "الحالة"
  },
  "actions": {
    "register": "تسجيل طالب",
    "edit": "تعديل",
    "archive": "أرشفة",
    "restore": "استعادة",
    "addParent": "إضافة ولي أمر",
    "makePrimary": "جعله الأساسي",
    "unlink": "إلغاء الربط"
  },
  "loading": {
    "label": "جاري تحميل الطلاب",
    "detailLabel": "جاري تحميل بيانات الطالب"
  },
  "error": {
    "title": "تعذر تحميل الطلاب",
    "reference": "المرجع: {correlationId}"
  },
  "detailError": {
    "title": "تعذر تحميل بيانات هذا الطالب"
  },
  "notFound": {
    "title": "الطالب غير موجود",
    "description": "هذا الطالب غير موجود، أو ليس لديك صلاحية الوصول إليه.",
    "backToList": "العودة إلى الطلاب"
  },
  "emptyState": {
    "title": "لا يوجد طلاب حتى الآن",
    "description": "سجّل أول طالب للبدء.",
    "noMatchTitle": "لا يوجد طلاب مطابقون لـ \"{search}\"."
  },
  "detail": {
    "phoneLabel": "الهاتف"
  },
  "parentsCard": {
    "title": "أولياء الأمور",
    "primaryBadge": "جهة الاتصال الأساسية",
    "openMenu": "فتح قائمة الإجراءات"
  },
  "addParent": {
    "link": "ربط",
    "retryLink": "إعادة محاولة الربط",
    "linkFailedNotice": "تم إنشاء ولي الأمر لكن تعذّر ربطه بعد. حاول الربط مرة أخرى."
  },
  "archivedBanner": {
    "title": "هذا الطالب مؤرشف",
    "description": "استعد الطالب لتعديل بياناته أو إدارة أولياء أموره."
  },
  "form": {
    "studentSectionTitle": "الطالب",
    "parentSectionTitle": "ولي الأمر",
    "fullNameLabel": "الاسم الكامل",
    "fullNamePlaceholder": "مثال: أحمد محمود",
    "phoneLabel": "الهاتف (اختياري)",
    "schoolYearLabel": "الصف الدراسي (اختياري)",
    "schoolYearPlaceholder": "مثال: الصف الخامس",
    "parentPhoneLabel": "هاتف ولي الأمر",
    "parentFullNameLabel": "اسم ولي الأمر الكامل",
    "parentFullNamePlaceholder": "مثال: محمود حسن",
    "relationshipLabel": "صلة القرابة",
    "save": "حفظ",
    "cancel": "إلغاء"
  },
  "dialogs": {
    "editTitle": "تعديل بيانات الطالب",
    "addParentTitle": "إضافة ولي أمر"
  },
  "confirmations": {
    "archiveTitle": "أرشفة \"{name}\"؟",
    "archiveBody": "سيصبح للقراءة فقط حتى تتم استعادته، وستُخفى إجراءات أولياء الأمور الخاصة به.",
    "archiveConfirm": "أرشفة الطالب",
    "unlinkTitle": "إلغاء ربط {name}؟",
    "unlinkBody": "لن يعود مرتبطًا بهذا الطالب. هذا لا يحذف سجله.",
    "unlinkConfirm": "إلغاء الربط"
  },
  "lookup": {
```

The excerpt stops at line 90; use the original file for the remaining 29 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
