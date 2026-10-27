# `frontend/src/i18n/locales/ar/staff.json`

## Purpose

JSON data with top-level keys `page`, `columns`, `status`, `you`, `loading`, `error`, `emptyState`, `actions`, `dialogs`, `form`, `confirmations`, `password`. Its consumers determine whether this is build configuration, a contract or a language resource.

## Where it fits

Category: **configuration**. [Open current file](../../../../../../../frontend/src/i18n/locales/ar/staff.json). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 79 lines and 3,053 bytes. Source hash: `b5fa1f7aa6fb8de55c9fcac768e36f20a15dabb553a852a4ad5fa042bffebe7b`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

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
    "title": "الموظفون",
    "subtitle": "الأشخاص الذين يمكنهم تسجيل الدخول إلى هذا المركز وأدوارهم."
  },
  "columns": {
    "name": "الاسم",
    "email": "البريد الإلكتروني",
    "role": "الدور",
    "status": "الحالة",
    "actions": "الإجراءات"
  },
  "status": {
    "active": "نشط",
    "inactive": "غير نشط"
  },
  "you": "أنت",
  "loading": {
    "label": "جاري تحميل الموظفين"
  },
  "error": {
    "title": "تعذر تحميل بيانات الموظفين",
    "reference": "المرجع: {correlationId}"
  },
  "emptyState": {
    "title": "لا يوجد موظفون بعد",
    "description": "أضف أول موظف لديك للبدء."
  },
  "actions": {
    "create": "إضافة موظف",
    "changeRole": "تغيير الدور",
    "deactivate": "إلغاء التفعيل",
    "reactivate": "إعادة التفعيل",
    "openMenu": "فتح قائمة الإجراءات"
  },
  "dialogs": {
    "createTitle": "إضافة موظف",
    "changeRoleTitle": "تغيير دور {name}"
  },
  "form": {
    "emailLabel": "البريد الإلكتروني",
    "emailPlaceholder": "مثال: teacher@example.com",
    "displayNameLabel": "الاسم",
    "displayNamePlaceholder": "مثال: سارة أحمد",
    "roleLabel": "الدور",
    "preferredLocaleLabel": "اللغة المفضلة",
    "save": "حفظ",
    "cancel": "إلغاء",
    "changeRoleConsequence": "سيتم تسجيل خروجه وسيحصل على الدور الجديد في المرة القادمة التي يسجل فيها الدخول."
  },
  "confirmations": {
    "deactivateTitle": "إلغاء تفعيل {name}؟",
    "deactivateBody": "سيفقد {name} الوصول إلى هذا المركز فقط. حسابه لن يُحذف أبدًا.",
    "deactivateConfirm": "إلغاء التفعيل"
  },
  "password": {
    "title": "كلمة مرور مؤقتة",
    "description": "شارك كلمة المرور هذه مع {name}. لن تظهر مرة أخرى ويجب تغييرها عند أول تسجيل دخول.",
    "copy": "نسخ",
    "copied": "تم النسخ!",
    "done": "تم"
  },
  "existingAccount": {
    "title": "الحساب موجود بالفعل",
    "description": "لدى {name} حساب بالفعل ويمكنه الآن اختيار هذا المركز لتسجيل الدخول."
  },
  "toasts": {
    "created": "تمت إضافة الموظف.",
    "roleChanged": "تم تغيير الدور.",
    "deactivated": "تم إلغاء تفعيل الموظف.",
    "reactivated": "تمت إعادة تفعيل الموظف."
  },
  "validation": {
    "emailRequired": "البريد الإلكتروني مطلوب.",
    "emailInvalid": "أدخل بريدًا إلكترونيًا صالحًا.",
    "displayNameRequired": "الاسم مطلوب.",
    "displayNameTooLong": "يجب ألا يزيد الاسم عن 120 حرفًا."
  }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
