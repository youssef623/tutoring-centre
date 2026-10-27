# `frontend/src/i18n/locales/ar/auth.json`

## Purpose

JSON data with top-level keys `login`, `panel`, `validation`, `changePassword`, `errors`, `selectCentre`, `roles`. Its consumers determine whether this is build configuration, a contract or a language resource.

## Where it fits

Category: **configuration**. [Open current file](../../../../../../../frontend/src/i18n/locales/ar/auth.json). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 68 lines and 4,242 bytes. Source hash: `4a617999e2fade01c2211b9b098f70aeadac3862978bba544e1b5fa30dc7a2da`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

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
    "brand": "حصة",
    "eyebrow": "مرحبًا بعودتك",
    "title": "تسجيل الدخول",
    "subtitle": "أدخل بياناتك لإدارة مركز الدروس الخاص بك.",
    "emailLabel": "البريد الإلكتروني",
    "passwordLabel": "كلمة المرور",
    "submit": "تسجيل الدخول",
    "submitting": "جارٍ تسجيل الدخول…"
  },
  "panel": {
    "tagline": "أدِر مركز دروسك بثقة.",
    "description": "الطلاب والمجموعات والحضور والرسوم — كلها في مساحة عمل واحدة مريحة.",
    "features": {
      "attendance": { "title": "الحضور", "description": "سجّل يدويًا أو امسح رمز QR في ثوانٍ." },
      "groups": { "title": "المجموعات والجداول", "description": "نظّم الحصص والمعلمين والمواعيد." },
      "fees": { "title": "الرسوم والمدفوعات", "description": "تابع المستحقات وسوِّ الأرصدة بوضوح." },
      "assistant": { "title": "مساعد واتساب", "description": "يحصل أولياء الأمور على إجابات بالعربية والإنجليزية." }
    }
  },
  "validation": {
    "emailRequired": "البريد الإلكتروني مطلوب.",
    "emailInvalid": "أدخل بريدًا إلكترونيًا صحيحًا.",
    "passwordRequired": "كلمة المرور مطلوبة.",
    "currentPasswordRequired": "أدخل كلمة المرور الحالية.",
    "newPasswordTooShort": "يجب أن تتكون كلمة المرور الجديدة من 10 أحرف على الأقل.",
    "newPasswordSameAsCurrent": "يجب أن تختلف كلمة المرور الجديدة عن كلمة المرور الحالية.",
    "confirmPasswordRequired": "أكّد كلمة المرور الجديدة.",
    "confirmPasswordMismatch": "كلمتا المرور غير متطابقتين."
  },
  "changePassword": {
    "title": "تغيير كلمة المرور",
    "subtitle": "اختر كلمة مرور جديدة لحسابك.",
    "firstLoginSubtitle": "أنت تسجل الدخول بكلمة مرور مؤقتة. اختر كلمة مرور جديدة للمتابعة.",
    "currentPasswordLabel": "كلمة المرور الحالية",
    "newPasswordLabel": "كلمة المرور الجديدة",
    "confirmPasswordLabel": "تأكيد كلمة المرور الجديدة",
    "submit": "تغيير كلمة المرور",
    "submitting": "جارٍ تغيير كلمة المرور…",
    "toastSuccess": "تم تغيير كلمة المرور."
  },
  "errors": {
    "invalidCredentials": "البريد الإلكتروني أو كلمة المرور غير صحيحة.",
    "rateLimited": "محاولات كثيرة جدًا. يرجى الانتظار دقيقة والمحاولة مرة أخرى."
  },
  "selectCentre": {
    "title": "اختر مركزًا",
    "subtitle": "{count, plural, zero {أنت لا تعمل في أي مركز.} one {أنت تعمل في مركز واحد.} two {أنت تعمل في مركزين.} few {أنت تعمل في # مراكز.} many {أنت تعمل في # مركزًا.} other {أنت تعمل في # مركز.}} ما تراه وما يمكنك فعله يعتمد على دورك في كل مركز.",
    "signedInAs": "تم الدخول باسم",
    "logout": "تسجيل الخروج",
    "missingCentre": "لا تجد مركزك؟ اطلب من صاحب المركز دعوتك بهذا البريد الإلكتروني.",
    "noActiveCentre": {
      "title": "لا يوجد مركز نشط",
      "description": "حسابك غير مرتبط بأي مركز نشط حاليًا. إما لم يدعُك أي مركز بعد، أو تم إيقاف صلاحيتك.",
      "inviteLabel": "اطلب من صاحب المركز دعوة هذا البريد الإلكتروني",
      "copy": "نسخ",
      "copied": "تم النسخ!",
      "checkAgain": "تحقق مرة أخرى",
      "checking": "جارٍ التحقق…"
    }
  },
  "roles": {
    "owner": "مالك",
    "teacher": "معلم",
    "secretary": "سكرتارية"
  }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
