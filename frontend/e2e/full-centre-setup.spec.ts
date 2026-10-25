import { type Locator, type Page, expect, test } from "@playwright/test";

// The seeded dev password is never written here: it is supplied by whoever runs the suite
// (the CI job sets a throwaway, Task 18.5; locally the developer sets their own, Task 18.3).
const password: string = process.env.SEED_PASSWORD ?? "";
if (password === "") {
  throw new Error("SEED_PASSWORD must be set to run the E2E full centre setup journey (see README).");
}

// This journey reads a one-time temporary password off the screen, like first-login.spec.ts: no trace and no
// video, so a retry can never leave that password in a recording kept as a CI artefact (Day 33 rule).
// Arabic throughout: the browser asks for Arabic, so every screen (sign-in included) renders in it.
test.use({ trace: "off", video: "off", locale: "ar-EG" });

// The seed's empty centre (Development and Testing only) and its one owner: nothing else exists there, so this
// journey never depends on, or collides with, the demo data or another spec.
const ownerEmail = "owner@giza.test";
const ownerName = "Giza Owner";
const centreName = "مركز الجيزة الجديد";

/** The first Saturday strictly after today in Cairo, as "YYYY-MM-DD": the week after this one, always fully ahead. */
function nextSaturdayInCairo(): string {
  const parts = new Intl.DateTimeFormat("en-US", { year: "numeric", month: "2-digit", day: "2-digit", timeZone: "Africa/Cairo" }).formatToParts(
    new Date(),
  );
  const part = (type: Intl.DateTimeFormatPartTypes) => Number(parts.find((candidate) => candidate.type === type)?.value);
  const today = new Date(Date.UTC(part("year"), part("month") - 1, part("day")));
  const daysAhead = ((6 - today.getUTCDay() + 7) % 7) || 7;
  today.setUTCDate(today.getUTCDate() + daysAhead);
  return today.toISOString().slice(0, 10);
}

/** A unique, synthetic Egyptian mobile number per run — real-shaped, never a real person's. */
function uniquePhone(): string {
  const digits = Date.now().toString().slice(-8).padStart(8, "0");
  return `+2010${digits}`;
}

async function signIn(page: Page, email: string, withPassword: string): Promise<void> {
  await page.goto("/");
  await expect(page.getByRole("heading", { name: "تسجيل الدخول" })).toBeVisible();
  await page.getByLabel("البريد الإلكتروني").fill(email);
  await page.getByLabel("كلمة المرور").fill(withPassword);
  await page.getByRole("button", { name: "تسجيل الدخول" }).click();
}

/** Picks an option in a Base UI select inside `scope` by the trigger's label. */
async function choose(page: Page, scope: Locator, label: string, option: string | RegExp): Promise<void> {
  await scope.getByRole("combobox", { name: label }).click();
  await page.getByRole("option", typeof option === "string" ? { name: option, exact: true } : { name: option }).click();
}

/** As the signed-in owner: a weekly session at 6 PM for 90 minutes, from today (the dialog's default first date). */
async function addWeeklySession(page: Page, groupPath: string, day: string, ninetyMinutes: string): Promise<void> {
  await page.goto(`${groupPath}?tab=schedule`);
  await page.getByRole("button", { name: "إضافة حصة أسبوعية" }).first().click();
  const dialog = page.getByRole("dialog", { name: "إضافة حصة أسبوعية" });
  await choose(page, dialog, "اليوم", day);
  await choose(page, dialog, "الساعة", "6");
  await choose(page, dialog, "الدقيقة", "00");
  await choose(page, dialog, "صباحًا أو مساءً", "م");
  await choose(page, dialog, "المدة", ninetyMinutes);
  await dialog.getByRole("button", { name: "إضافة الحصة الأسبوعية" }).click();
  await expect(dialog).toBeHidden();
}

test("owner: sets up an empty centre entirely from the UI, down to a weekly timetable its teacher sees", async ({ page, browser }) => {
  test.setTimeout(180_000);

  // Unique per run: the journey runs again and again against the same database and centre.
  const suffix = Date.now().toString();
  const startedAt = Date.now();
  const subjectName = `أحياء ${suffix}`;
  const teacherEmail = `e2e-setup-teacher-${suffix}@giza.test`;
  const teacherName = `ليلى منصور ${suffix}`;
  const groupName = `أحياء - المجموعة أ ${suffix}`;
  const parentName = `ولي أمر ${suffix}`;
  const parentPhone = uniquePhone();
  const firstStudent = `أحمد مصطفى ${suffix}`;
  const secondStudent = `مريم مصطفى ${suffix}`;
  const nextWeek = nextSaturdayInCairo();

  await signIn(page, ownerEmail, password);
  await expect(page.getByRole("heading", { name: `مرحبًا، ${ownerName}` })).toBeVisible();

  // The labels the app itself renders in Arabic, taken from this browser's own Intl rather than guessed.
  const ninetyMinutes = await page.evaluate(() => `ساعة واحدة و${new Intl.NumberFormat("ar").format(30)} دقيقة`);
  const sixToSevenThirty = await page.evaluate(() =>
    new Intl.DateTimeFormat("ar", { hour: "numeric", minute: "2-digit", timeZone: "UTC" }).formatRange(
      new Date(Date.UTC(2000, 0, 1, 18, 0)),
      new Date(Date.UTC(2000, 0, 1, 19, 30)),
    ),
  );

  // 1. A subject (Month 2).
  await page.getByRole("navigation").getByRole("link", { name: "المواد" }).click();
  await expect(page.getByRole("heading", { name: "المواد" })).toBeVisible();
  await page.getByRole("button", { name: "إضافة مادة" }).first().click();
  await page.getByLabel("الاسم", { exact: true }).fill(subjectName);
  await page.getByRole("button", { name: "حفظ" }).click();
  await expect(page.getByText("تمت إضافة المادة.")).toBeVisible();
  await expect(page.getByRole("row", { name: subjectName })).toBeVisible();

  // 2. A staff member, a Teacher by default, who completes the first-login password change in their own context.
  await page.getByRole("navigation").getByRole("link", { name: "الموظفون" }).click();
  await expect(page.getByRole("heading", { name: "الموظفون" })).toBeVisible();
  await page.getByRole("button", { name: "إضافة موظف" }).first().click();
  await page.getByLabel("البريد الإلكتروني").fill(teacherEmail);
  await page.getByLabel("الاسم", { exact: true }).fill(teacherName);
  await page.getByRole("button", { name: "حفظ" }).click();
  await expect(page.getByRole("heading", { name: "كلمة مرور مؤقتة" })).toBeVisible();
  // Read once, straight into the new context below; never logged or kept.
  const temporaryPassword = await page.getByText(/^[A-Za-z0-9]{16}$/).innerText();
  await page.getByRole("button", { name: "تم", exact: true }).click();
  await expect(page.getByRole("row", { name: new RegExp(teacherName) })).toBeVisible();

  const teacherContext = await browser.newContext({ locale: "ar-EG" });
  try {
    const teacherPage = await teacherContext.newPage();
    await signIn(teacherPage, teacherEmail, temporaryPassword);
    await expect(teacherPage.getByRole("heading", { name: "تغيير كلمة المرور" })).toBeVisible();
    const newPassword = "E2E-Replacement-Pass-1";
    await teacherPage.getByLabel("كلمة المرور الحالية").fill(temporaryPassword);
    await teacherPage.getByLabel("كلمة المرور الجديدة", { exact: true }).fill(newPassword);
    await teacherPage.getByLabel("تأكيد كلمة المرور الجديدة").fill(newPassword);
    await teacherPage.getByRole("button", { name: "تغيير كلمة المرور" }).click();
    await expect(teacherPage.getByText("تم تغيير كلمة المرور.")).toBeVisible();
    await expect(teacherPage.getByRole("heading", { name: "اختر مركزًا" })).toBeVisible();
    await teacherPage.getByRole("button", { name: centreName }).click();
    await expect(teacherPage.getByRole("heading", { name: `مرحبًا، ${teacherName}` })).toBeVisible();

    // 3. A teacher record, linked to that account.
    await page.getByRole("navigation").getByRole("link", { name: "المعلمون" }).click();
    await expect(page.getByRole("heading", { name: "المعلمون" })).toBeVisible();
    await page.getByRole("button", { name: "إضافة معلم" }).first().click();
    await page.getByLabel("الاسم الكامل").fill(teacherName);
    await page.getByRole("button", { name: "حفظ" }).click();
    await expect(page.getByText("تمت إضافة المعلم.")).toBeVisible();
    await page.getByRole("button", { name: `إجراءات ${teacherName}` }).click();
    await page.getByRole("menuitem", { name: "ربط حساب" }).click();
    const accountDialog = page.getByRole("dialog", { name: `حساب ${teacherName}` });
    await choose(page, accountDialog, "حساب الموظف", new RegExp(teacherEmail));
    await accountDialog.getByRole("button", { name: "ربط الحساب" }).click();
    await expect(page.getByText("تم ربط الحساب.")).toBeVisible();

    // 4. A two-seat group of the new subject, taught by the new teacher.
    await page.getByRole("navigation").getByRole("link", { name: "المجموعات" }).click();
    await expect(page.getByRole("heading", { name: "المجموعات" })).toBeVisible();
    await page.getByRole("button", { name: "إنشاء مجموعة" }).first().click();
    const groupDialog = page.getByRole("dialog");
    await groupDialog.getByRole("textbox", { name: "الاسم" }).fill(groupName);
    await choose(page, groupDialog, "المادة", subjectName);
    await choose(page, groupDialog, "المعلم", teacherName);
    await groupDialog.getByRole("spinbutton", { name: "السعة" }).fill("2");
    await groupDialog.getByRole("button", { name: "إنشاء مجموعة" }).click();
    await expect(page.getByRole("heading", { name: groupName })).toBeVisible();
    const groupPath = new URL(page.url()).pathname;

    // 5. Two students; the second is a sibling, found through the parent lookup by the same phone.
    await page.getByRole("navigation").getByRole("link", { name: "الطلاب" }).click();
    await expect(page.getByRole("heading", { name: "الطلاب" })).toBeVisible();
    await page.getByRole("link", { name: "تسجيل طالب" }).first().click();
    await page.getByLabel("الاسم الكامل", { exact: true }).fill(firstStudent);
    await page.getByLabel("هاتف ولي الأمر").fill(parentPhone);
    await page.getByLabel("هاتف ولي الأمر").press("Tab");
    await page.getByLabel("اسم ولي الأمر الكامل").fill(parentName);
    await page.getByRole("button", { name: "تسجيل طالب" }).click();
    await expect(page.getByRole("heading", { name: firstStudent })).toBeVisible();

    await page.getByRole("navigation").getByRole("link", { name: "الطلاب" }).click();
    await page.getByRole("link", { name: "تسجيل طالب" }).first().click();
    await page.getByLabel("الاسم الكامل", { exact: true }).fill(secondStudent);
    await page.getByLabel("هاتف ولي الأمر").fill(parentPhone);
    await page.getByLabel("هاتف ولي الأمر").press("Tab");
    await expect(page.getByText(`ولي أمر مسجّل مسبقًا: ${parentName}`)).toBeVisible();
    await expect(page.getByLabel("اسم ولي الأمر الكامل")).not.toBeVisible();
    await page.getByRole("button", { name: "تسجيل طالب" }).click();
    await expect(page.getByRole("heading", { name: secondStudent })).toBeVisible();

    // 6. Both enrolled from the group's roster: the group is full.
    await page.goto(`${groupPath}?tab=roster`);
    await expect(page.getByText("لا يوجد طلاب ملتحقون بعد")).toBeVisible();
    for (const student of [firstStudent, secondStudent]) {
      await page.getByRole("button", { name: "إلحاق طالب" }).first().click();
      const enrollDialog = page.getByRole("dialog", { name: "إلحاق طالب" });
      await enrollDialog.getByRole("searchbox", { name: "البحث عن الطلاب" }).fill(student);
      await enrollDialog.getByRole("option", { name: new RegExp(student) }).click();
      await enrollDialog.getByRole("button", { name: "إلحاق", exact: true }).click();
      await expect(enrollDialog).toBeHidden();
      await expect(page.getByRole("table").getByRole("link", { name: student })).toBeVisible();
    }
    await expect(page.getByText("مكتملة", { exact: true })).toBeVisible();
    await expect(page.getByRole("button", { name: "إلحاق طالب" })).toHaveAttribute("aria-disabled", "true");

    // 7. Saturday and Tuesday at 18:00 for 90 minutes.
    await addWeeklySession(page, groupPath, "السبت", ninetyMinutes);
    await addWeeklySession(page, groupPath, "الثلاثاء", ninetyMinutes);
    await expect(page.getByRole("row", { name: /السبت/ })).toBeVisible();
    await expect(page.getByRole("row", { name: /الثلاثاء/ })).toBeVisible();

    // 8. The timetable: next week (always wholly ahead of today) has both sessions, at 18:00 centre time.
    await page.getByRole("navigation").getByRole("link", { name: "جدول الحصص" }).click();
    await expect(page.getByRole("heading", { name: "جدول الحصص" })).toBeVisible();
    await page.goto(`/timetable?week=${nextWeek}`);
    const ownSession = (column: string) =>
      page.getByRole("region", { name: new RegExp(`^${column}`) }).getByRole("article", { name: `${groupName}, ${sixToSevenThirty}` });
    await expect(ownSession("السبت")).toBeVisible();
    await expect(ownSession("الثلاثاء")).toBeVisible();
    await expect(page.getByRole("article", { name: new RegExp(`^${groupName}, `) })).toHaveCount(2);

    // 9. As the linked teacher: the group, its roster and its two weekly sessions, and nothing else.
    await teacherPage.getByRole("navigation").getByRole("link", { name: "المجموعات" }).click();
    await expect(teacherPage.getByRole("link", { name: groupName })).toBeVisible();
    await expect(teacherPage.getByRole("row")).toHaveCount(2);
    await expect(teacherPage.getByRole("button", { name: "إنشاء مجموعة" })).toHaveCount(0);

    await teacherPage.getByRole("link", { name: groupName }).click();
    await teacherPage.getByRole("tab", { name: "قائمة الطلاب" }).click();
    const roster = teacherPage.getByRole("table");
    await expect(roster.getByText(firstStudent)).toBeVisible();
    await expect(roster.getByText(secondStudent)).toBeVisible();
    await expect(roster.getByRole("row")).toHaveCount(3);
    await expect(roster.getByRole("link")).toHaveCount(0);

    await teacherPage.getByRole("navigation").getByRole("link", { name: "جدول الحصص" }).click();
    await expect(teacherPage.getByRole("heading", { name: "جدول الحصص" })).toBeVisible();
    await teacherPage.goto(`/timetable?week=${nextWeek}`);
    await expect(teacherPage.getByRole("navigation", { name: /^أسبوع/ })).toBeVisible();
    await expect(teacherPage.getByRole("article")).toHaveCount(2);
    await expect(teacherPage.getByRole("article", { name: `${groupName}, ${sixToSevenThirty}` })).toHaveCount(2);
    await expect(teacherPage.getByRole("button", { name: "تمديد الجدول" })).toHaveCount(0);
  } finally {
    await teacherContext.close();
  }

  // 10. As the owner: the audit log has this run's teacher link, both enrollments and both rules. The filter
  // offers no entity type for these, so the newest-first list is read and narrowed to entries since this run began.
  await page.getByRole("navigation").getByRole("link", { name: "سجل النشاط" }).click();
  await expect(page.getByRole("heading", { name: "سجل النشاط" })).toBeVisible();
  const rows = page.locator("div.rounded-xl").filter({ has: page.locator("time[datetime]") });
  await expect(rows.first()).toBeVisible();
  const entries: string[] = [];
  for (const row of await rows.all()) {
    const occurredAt = Date.parse((await row.locator("time[datetime]").getAttribute("datetime")) ?? "");
    if (occurredAt >= startedAt) {
      entries.push((await row.textContent()) ?? "");
    }
  }
  const thisRun = (entityType: string, action: string) =>
    entries.filter((text) => text.includes(ownerName) && text.includes(entityType) && text.includes(action));
  expect(thisRun("معلم", "تم التحديث").filter((text) => text.includes("المستخدم:"))).toHaveLength(1);
  expect(thisRun("تسجيل", "تم الإنشاء")).toHaveLength(2);
  expect(thisRun("قاعدة جدول", "تم الإنشاء")).toHaveLength(2);
});
