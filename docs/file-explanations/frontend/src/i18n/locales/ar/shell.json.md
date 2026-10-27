# `frontend/src/i18n/locales/ar/shell.json`

## Purpose

JSON data with top-level keys `nav`, `header`, `userMenu`, `noAccess`, `dashboard`. Its consumers determine whether this is build configuration, a contract or a language resource.

## Where it fits

Category: **configuration**. [Open current file](../../../../../../../frontend/src/i18n/locales/ar/shell.json). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 53 lines and 2,079 bytes. Source hash: `e12ef84c79de38d9158bf3cc262c5474c175b47f5f5edad9cec570e21875295c`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

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
    "dashboard": "لوحة التحكم",
    "subjects": "المواد",
    "students": "الطلاب",
    "teachers": "المعلمون",
    "groups": "المجموعات",
    "timetable": "جدول الحصص",
    "staff": "الموظفون",
    "settings": "الإعدادات",
    "audit": "سجل النشاط",
    "openMenu": "فتح القائمة",
    "closeMenu": "إغلاق القائمة",
    "home": "الصفحة الرئيسية لحصة"
  },
  "header": {
    "activeCentre": "المركز النشط"
  },
  "userMenu": {
    "switchCentre": "تبديل المركز",
    "changePassword": "تغيير كلمة المرور",
    "logout": "تسجيل الخروج"
  },
  "noAccess": {
    "title": "لا تملك صلاحية الوصول إلى هذه الصفحة",
    "description": "دورك لا يشمل هذه الصلاحية. إذا كنت تعتقد أن هذا خطأ، تواصل مع مالك المركز.",
    "backToDashboard": "العودة إلى لوحة التحكم"
  },
  "dashboard": {
    "welcome": "مرحبًا، {name}",
    "activeCentre": "أنت تعمل في {centreName}.",
    "actions": {
      "registerStudent": "تسجيل طالب",
      "students": "الطلاب",
      "groups": "المجموعات"
    },
    "loadingLabel": "جارٍ تحميل النظرة العامة",
    "errorTitle": "تعذّر تحميل النظرة العامة",
    "stats": {
      "activeGroups": "المجموعات النشطة",
      "seatsTaken": "المقاعد المشغولة",
      "openSeats": "المقاعد المتاحة",
      "fullGroups": "المجموعات المكتملة"
    },
    "fillingUp": {
      "title": "مجموعات تقترب من الاكتمال",
      "description": "المجموعات النشطة التي شُغل 80% أو أكثر من مقاعدها.",
      "viewAll": "كل المجموعات",
      "empty": "لا توجد مجموعة قريبة من الاكتمال.",
      "noGroups": "لا توجد مجموعات نشطة بعد."
    }
  }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
