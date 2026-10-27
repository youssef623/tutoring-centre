# `frontend/src/i18n/locales/ar/errors.json`

## Purpose

JSON data with top-level keys `byCode`, `byKind`, `generic`. Its consumers determine whether this is build configuration, a contract or a language resource.

## Where it fits

Category: **configuration**. [Open current file](../../../../../../../frontend/src/i18n/locales/ar/errors.json). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 153 lines and 10,319 bytes. Source hash: `53808f6761186e88296bfacf1c33e3ac841023213f521a85f89fa275afb8470c`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

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
  "byCode": {
    "validation": {
      "failed": "بعض الحقول غير صالحة. يرجى مراجعة النموذج."
    },
    "centre": {
      "name_required": "اسم المركز مطلوب.",
      "name_too_long": "يجب ألا يزيد اسم المركز عن 120 حرفًا.",
      "name_invalid": "يجب أن يتكون اسم المركز من 1 إلى 120 حرفًا.",
      "slug_invalid": "يجب أن يحتوي العنوان على أحرف إنجليزية صغيرة وأرقام وشرطات مفردة فقط.",
      "time_zone_invalid": "المنطقة الزمنية غير معروفة.",
      "slug_taken": "يوجد مركز بهذا العنوان بالفعل.",
      "create_forbidden": "إنشاء المراكز متاح للمنصة فقط."
    },
    "tenant": {
      "not_selected": "يرجى اختيار مركز للمتابعة."
    },
    "auth": {
      "permission_denied": "ليس لديك صلاحية لتنفيذ هذا الإجراء.",
      "password_change_required": "يرجى تغيير كلمة المرور قبل الاستمرار.",
      "current_password_invalid": "كلمة المرور الحالية التي أدخلتها غير صحيحة.",
      "password_too_weak": "كلمة المرور هذه لا تستوفي الشروط المطلوبة.",
      "rate_limited": "محاولات كثيرة جدًا. يرجى الانتظار دقيقة والمحاولة مرة أخرى.",
      "invalid_credentials": "البريد الإلكتروني أو كلمة المرور غير صحيحة.",
      "not_authenticated": "يرجى تسجيل الدخول للمتابعة."
    },
    "staff": {
      "already_member": "هذا الشخص عضو بالفعل في هذا المركز.",
      "last_owner": "يجب أن يحتفظ هذا المركز بمالك واحد على الأقل.",
      "cannot_change_self": "لا يمكنك تغيير دورك الخاص.",
      "not_found": "تعذر العثور على هذا الموظف.",
      "email_invalid": "أدخل بريدًا إلكترونيًا صحيحًا."
    },
    "membership": {
      "role_unchanged": "الدور هو نفس القيمة الحالية بالفعل.",
      "inactive": "هذه العضوية غير نشطة.",
      "already_active": "هذه العضوية نشطة بالفعل.",
      "already_inactive": "هذه العضوية غير نشطة بالفعل.",
      "centre_required": "المركز مطلوب.",
      "user_required": "المستخدم مطلوب."
    },
    "subject": {
      "not_found": "تعذر العثور على هذه المادة.",
      "name_taken": "توجد مادة بهذا الاسم بالفعل.",
      "name_invalid": "يجب أن يتكون اسم المادة من 1 إلى 80 حرفًا.",
      "already_archived": "هذه المادة مؤرشفة بالفعل.",
      "not_archived": "هذه المادة غير مؤرشفة.",
      "archived_read_only": "لا يمكن إعادة تسمية مادة مؤرشفة حتى تتم استعادتها.",
      "has_active_groups": "لهذه المادة فصول نشطة. أرشفها أو انقلها أولًا."
    },
    "phone": {
      "invalid": "أدخل رقم هاتف صحيحًا."
    },
    "parent": {
      "centre_required": "المركز مطلوب.",
      "name_invalid": "يجب أن يتكون اسم ولي الأمر من 2 إلى 120 حرفًا.",
      "not_found": "تعذر العثور على ولي الأمر.",
      "phone_taken": "يوجد ولي أمر بهذا رقم الهاتف بالفعل."
    },
    "student": {
      "centre_required": "المركز مطلوب.",
      "name_invalid": "يجب أن يتكون اسم الطالب من 2 إلى 120 حرفًا.",
      "school_year_invalid": "يجب ألا يزيد الصف الدراسي عن 40 حرفًا.",
      "parent_required": "ولي الأمر مطلوب.",
      "parent_already_linked": "ولي الأمر هذا مرتبط بالفعل بهذا الطالب.",
      "parent_not_linked": "ولي الأمر هذا غير مرتبط بهذا الطالب.",
      "too_many_parents": "لا يمكن أن يتجاوز عدد أولياء أمور الطالب 4.",
      "last_parent": "يجب أن يحتفظ الطالب بولي أمر واحد على الأقل.",
      "primary_parent_required": "عيّن ولي أمر آخر كجهة اتصال أساسية قبل إزالة جهة الاتصال الأساسية.",
      "already_primary": "ولي الأمر هذا هو جهة الاتصال الأساسية بالفعل.",
      "already_archived": "هذا الطالب مؤرشف بالفعل.",
      "not_archived": "هذا الطالب غير مؤرشف.",
      "archived_read_only": "الطالب المؤرشف للقراءة فقط حتى تتم استعادته.",
      "not_found": "تعذر العثور على هذا الطالب.",
      "has_active_enrollments": "ما زال هذا الطالب مسجلًا في فصل. اسحبه أولًا."
    },
    "teacher": {
      "centre_required": "المركز مطلوب.",
      "name_invalid": "يجب أن يتكون اسم المعلم من 2 إلى 120 حرفًا.",
      "user_required": "المستخدم مطلوب.",
      "already_archived": "هذا المعلم مؤرشف بالفعل.",
      "not_archived": "هذا المعلم غير مؤرشف.",
      "archived_read_only": "المعلم المؤرشف للقراءة فقط حتى تتم استعادته.",
      "not_found": "تعذر العثور على هذا المعلم.",
      "user_not_eligible": "هذا الشخص ليس عضو معلم مؤهل في هذا المركز.",
      "account_taken": "هذا الحساب مرتبط بالفعل بمعلم آخر.",
      "has_active_groups": "لهذا المعلم فصول نشطة. أعد إسنادها أو أرشفها أولًا."
    },
    "group": {
      "centre_required": "المركز مطلوب.",
```

The excerpt stops at line 90; use the original file for the remaining 63 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
