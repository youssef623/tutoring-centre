# `frontend/e2e/full-centre-setup.spec.ts`

## Purpose

Source comment (intent): The first Saturday strictly after today in Cairo, as "YYYY-MM-DD": the week after this one, always fully ahead.

## Where it fits

Category: **test**. [Open current file](../../../../frontend/e2e/full-centre-setup.spec.ts). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 254 lines and 16,592 bytes. Source hash: `43dc5e2305046b1f92cb665f35305b27e62b131a703739b6c7b7e221cbc9939a`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { type Locator, type Page, expect, test } from "@playwright/test";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 22](../../../../frontend/e2e/full-centre-setup.spec.ts#L22): `function nextSaturdayInCairo(): string {`
- [Line 34](../../../../frontend/e2e/full-centre-setup.spec.ts#L34): `function uniquePhone(): string {`
- [Line 39](../../../../frontend/e2e/full-centre-setup.spec.ts#L39): `async function signIn(page: Page, email: string, withPassword: string): Promise<void> {`
- [Line 48](../../../../frontend/e2e/full-centre-setup.spec.ts#L48): `async function choose(page: Page, scope: Locator, label: string, option: string | RegExp): Promise<void> {`
- [Line 54](../../../../frontend/e2e/full-centre-setup.spec.ts#L54): `async function addWeeklySession(page: Page, groupPath: string, day: string, ninetyMinutes: string): Promise<void> {`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
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
```

The excerpt stops at line 90; use the original file for the remaining 164 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
