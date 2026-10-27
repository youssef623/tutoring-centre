# `frontend/src/i18n/locales/ar/enrollments.json`

## Purpose

JSON data with top-level keys `seats`, `roster`, `actions`, `enrollDisabled`, `form`, `enrollDialog`, `picker`, `outcomes`, `toasts`, `withdrawDialog`, `transferDialog`, `studentCard`. Its consumers determine whether this is build configuration, a contract or a language resource.

## Where it fits

Category: **configuration**. [Open current file](../../../../../../../frontend/src/i18n/locales/ar/enrollments.json). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 100 lines and 4,590 bytes. Source hash: `ede934cd0724c4b0b071d009fc85fef186dd1959e893cfecff6616511248e1b4`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

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
    "label": "المشغول {enrolledText} من {capacity, plural, zero {{capacityText} مقعد} one {مقعد واحد} two {مقعدين} few {{capacityText} مقاعد} many {{capacityText} مقعدًا} other {{capacityText} مقعد}}",
    "full": "مكتملة",
    "almostFull": "شبه مكتملة"
  },
  "roster": {
    "loadingLabel": "جارٍ تحميل قائمة الطلاب",
    "columns": {
      "student": "الطالب",
      "enrolledAt": "تاريخ الالتحاق",
      "actions": "الإجراءات"
    },
    "emptyTitle": "لا يوجد طلاب ملتحقون بعد",
    "emptyDescription": "سيظهر هنا الطلاب الملتحقون بهذه المجموعة.",
    "errorTitle": "تعذّر تحميل قائمة الطلاب",
    "errorReference": "المرجع: {correlationId}"
  },
  "actions": {
    "enroll": "إلحاق طالب",
    "openMenu": "إجراءات {name}",
    "transfer": "النقل إلى مجموعة أخرى",
    "withdraw": "سحب"
  },
  "enrollDisabled": {
    "full": "هذه المجموعة مكتملة. اسحب طالبًا أو زِد السعة أولًا.",
    "archived": "هذه المجموعة مؤرشفة. استعدها لإلحاق الطلاب."
  },
  "form": {
    "cancel": "إلغاء"
  },
  "enrollDialog": {
    "title": "إلحاق طالب",
    "description": "ابحث عن طالب نشط لإضافته إلى {group}.",
    "submit": "إلحاق"
  },
  "picker": {
    "searchLabel": "البحث عن الطلاب",
    "listLabel": "الطلاب",
    "loadingLabel": "جارٍ تحميل الطلاب",
    "empty": "لا يوجد طالب نشط يطابق هذا البحث.",
    "alreadyInGroup": "موجود بالفعل في هذه المجموعة"
  },
  "outcomes": {
    "groupFull": "هذه المجموعة مكتملة",
    "groupArchived": "هذه المجموعة مؤرشفة ولا تقبل التحاقات.",
    "alreadyEnrolled": "موجود بالفعل في هذه المجموعة",
    "studentArchived": "هذا الطالب مؤرشف ولا يمكن إلحاقه.",
    "alreadyEnded": "انتهى هذا الالتحاق بالفعل.",
    "sameGroup": "الطالب موجود بالفعل في هذه المجموعة. اختر مجموعة أخرى.",
    "subjectMismatch": "لا يمكن نقل الطالب إلا إلى مجموعة من المادة نفسها.",
    "stale": "غيّر شخص آخر هذا الالتحاق. تم تحديث القائمة.",
    "studentHasActiveEnrollments": "اسحب هذا الطالب من مجموعاته أولًا."
  },
  "toasts": {
    "enrolled": "تم إلحاق {student} بـ{group}.",
    "withdrawn": "تم سحب {student} من {group}.",
    "transferred": "تم نقل {student} إلى {group}."
  },
  "withdrawDialog": {
    "title": "سحب {student} من {group}؟",
    "body": "سيغادر {student} قائمة طلاب هذه المجموعة، ويبقى الالتحاق في سجله.",
    "confirm": "سحب"
  },
  "transferDialog": {
    "title": "نقل {student}",
    "description": "اختر مجموعة أخرى من المادة نفسها. يغادر الطالب {group} في الخطوة نفسها.",
    "groupsLabel": "المجموعة الجديدة",
    "loadingLabel": "جارٍ تحميل المجموعات",
    "empty": "لا توجد مجموعة أخرى لهذه المادة",
    "submit": "نقل"
  },
  "studentCard": {
    "title": "المجموعات",
    "enroll": "الإلحاق بمجموعة",
    "loadingLabel": "جارٍ تحميل المجموعات",
    "errorTitle": "تعذّر تحميل المجموعات",
    "empty": "غير ملتحق بأي مجموعة بعد.",
    "current": "المجموعات الحالية",
    "past": "المجموعات السابقة",
    "since": "منذ {date}",
    "ended": "انتهى في {date} · {reason}"
  },
  "groupPicker": {
    "title": "الإلحاق بمجموعة",
    "description": "اختر مادة، ثم إحدى مجموعاتها لـ{student}.",
    "subjectLabel": "المادة",
    "groupsLabel": "المجموعة",
    "loadingLabel": "جارٍ تحميل المجموعات",
    "empty": "لا توجد مجموعة نشطة لهذه المادة.",
```

The excerpt stops at line 90; use the original file for the remaining 10 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
