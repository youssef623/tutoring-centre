# `frontend/src/i18n/locales/ar/schedule.json`

## Purpose

JSON data with top-level keys `weekdays`, `durations`, `timeZoneNote`, `rules`, `actions`, `addDisabled`, `addDialog`, `endDialog`, `cancelDialog`, `extendDialog`, `form`, `validation`. Its consumers determine whether this is build configuration, a contract or a language resource.

## Where it fits

Category: **configuration**. [Open current file](../../../../../../../frontend/src/i18n/locales/ar/schedule.json). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 146 lines and 8,365 bytes. Source hash: `3f5b11e90e92b5276eab6e9d237127d5e2faefc52f2dca225f7e78c62f2a7dd8`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

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
    "saturday": "السبت",
    "sunday": "الأحد",
    "monday": "الاثنين",
    "tuesday": "الثلاثاء",
    "wednesday": "الأربعاء",
    "thursday": "الخميس",
    "friday": "الجمعة"
  },
  "durations": {
    "minutes": "{minutes, plural, zero {# دقيقة} one {دقيقة واحدة} two {دقيقتان} few {# دقائق} many {# دقيقة} other {# دقيقة}}",
    "hoursAndMinutes": "{hours, plural, zero {# ساعة} one {ساعة واحدة} two {ساعتان} few {# ساعات} many {# ساعة} other {# ساعة}}{minutes, plural, =0 {} one { ودقيقة واحدة} two { ودقيقتان} few { و# دقائق} many { و# دقيقة} other { و# دقيقة}}"
  },
  "timeZoneNote": "المواعيد بتوقيت المركز ({zone}).",
  "rules": {
    "loadingLabel": "جارٍ تحميل الحصص الأسبوعية",
    "errorTitle": "تعذّر تحميل الحصص الأسبوعية",
    "errorReference": "المرجع: {correlationId}",
    "emptyTitle": "لا توجد حصص أسبوعية بعد",
    "emptyDescription": "تتكرر الحصة الأسبوعية في اليوم والموعد نفسيهما كل أسبوع، وتُملأ بها الجدول لأربعة أسابيع قادمة.",
    "emptyDescriptionReadOnly": "عندما تضيف الإدارة حصة أسبوعية لهذه المجموعة، تظهر هنا وفي الجدول.",
    "columns": {
      "day": "اليوم",
      "time": "الموعد",
      "room": "القاعة",
      "effective": "سارية",
      "status": "الحالة",
      "actions": "الإجراءات"
    },
    "noRoom": "بدون قاعة",
    "effectiveOpen": "من {from}",
    "effectiveRange": "من {from} إلى {to}",
    "status": {
      "active": "سارية",
      "upcoming": "تبدأ {date}",
      "ending": "تنتهي {date}",
      "ended": "انتهت"
    }
  },
  "actions": {
    "add": "إضافة حصة أسبوعية",
    "end": "إنهاء",
    "endLabel": "إنهاء حصة {day} الساعة {time}",
    "extend": "تمديد الجدول",
    "openSessionMenu": "إجراءات {group} الساعة {time}",
    "cancelSession": "إلغاء الحصة"
  },
  "addDisabled": {
    "archived": "هذه المجموعة مؤرشفة. استعدها لإضافة حصص أسبوعية."
  },
  "addDialog": {
    "title": "إضافة حصة أسبوعية",
    "description": "يُدرّس معلم المجموعة، {teacher}، كل الحصص.",
    "dayLabel": "اليوم",
    "dayPlaceholder": "اختر يومًا",
    "timeLabel": "موعد البدء",
    "hourLabel": "الساعة",
    "minuteLabel": "الدقيقة",
    "periodLabel": "صباحًا أو مساءً",
    "am": "ص",
    "pm": "م",
    "durationLabel": "المدة",
    "roomLabel": "القاعة (اختياري)",
    "roomPlaceholder": "مثال: قاعة 2",
    "startDateLabel": "تبدأ في",
    "startDateHint": "تُنشأ الحصص بدءًا من هذا التاريخ للأسابيع الأربعة القادمة.",
    "submit": "إضافة الحصة الأسبوعية"
  },
  "endDialog": {
    "title": "إنهاء حصة {day} الساعة {time}؟",
    "body": "تُلغى الحصص التالية لآخر تاريخ. تبقى الحصص السابقة كما هي.",
    "lastDateLabel": "آخر تاريخ",
    "lastDateHint": "ليس قبل {date}، يوم بدء سريانها.",
    "confirm": "إنهاء الحصة الأسبوعية"
  },
  "cancelDialog": {
    "title": "إلغاء حصة {group} يوم {date} الساعة {time}؟",
    "body": "تبقى الحصة في الجدول مع علامة «ملغاة».",
    "keep": "الإبقاء على الحصة",
    "confirm": "إلغاء الحصة"
  },
  "extendDialog": {
    "title": "تمديد الجدول",
    "description": "يُنشئ حصص كل الحصص الأسبوعية السارية حتى تاريخ محدد. تبقى الحصص الموجودة كما هي.",
    "throughLabel": "إنشاء الحصص حتى",
    "throughHint": "من اليوم حتى {max}.",
    "submit": "تمديد"
  },
  "form": {
```

The excerpt stops at line 90; use the original file for the remaining 56 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
