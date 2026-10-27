# `frontend/e2e/students.spec.ts`

## Purpose

Source comment (intent): A unique, synthetic Egyptian mobile number per run — real-shaped, never a real person's.

## Where it fits

Category: **test**. [Open current file](../../../../frontend/e2e/students.spec.ts). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 114 lines and 5,786 bytes. Source hash: `6ed9b6345236cf164bc83227106af2ef268b414b5ccc8f1dd353fd2b35470d9e`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { type Page, expect, test } from "@playwright/test";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../frontend/e2e/students.spec.ts#L10): `async function signIn(page: Page, email: string): Promise<void> {`
- [Line 22](../../../../frontend/e2e/students.spec.ts#L22): `function uniquePhone(): string {`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```typescript
import { type Page, expect, test } from "@playwright/test";

// The seeded dev password is never written here: it is supplied by whoever runs the suite
// (the CI job sets a throwaway, Task 18.5; locally the developer sets their own, Task 18.3).
const password: string = process.env.SEED_PASSWORD ?? "";
if (password === "") {
  throw new Error("SEED_PASSWORD must be set to run the E2E students journeys (see README).");
}

async function signIn(page: Page, email: string): Promise<void> {
  await page.goto("/");
  await expect(page.getByRole("heading", { name: "Sign in" })).toBeVisible();
  await page.getByLabel("Email").fill(email);
  await page.getByLabel("Password").fill(password);
  await page.getByRole("button", { name: "Sign in" }).click();
  // Wait for the post-login SPA navigation (dashboard, or the centre picker for a two-centre account)
  // to settle before any further navigation, so a later page.goto never races the login request.
  await expect(page.getByRole("heading", { name: /^(Welcome,|Choose a centre)/ })).toBeVisible();
}

/** A unique, synthetic Egyptian mobile number per run — real-shaped, never a real person's. */
function uniquePhone(): string {
  const digits = Date.now().toString().slice(-8).padStart(8, "0");
  return `+2010${digits}`;
}

test("secretary: registers a student with a new parent in Arabic, then finds them by search", async ({ page }) => {
  const suffix = Date.now().toString();
  const studentName = `أحمد ${suffix}`;
  const parentName = `ولي أمر ${suffix}`;
  const parentPhone = uniquePhone();

  await signIn(page, "secretary@nile.test");

  // Switch to Arabic.
  await page.getByRole("button", { name: "Nile Secretary" }).click();
  await page.getByRole("button", { name: "العربية" }).click();
  await page.keyboard.press("Escape");

  await page.getByRole("navigation").getByRole("link", { name: "الطلاب" }).click();
  await expect(page.getByRole("heading", { name: "الطلاب" })).toBeVisible();
  await page.getByRole("link", { name: "تسجيل طالب" }).click();

  await page.getByLabel("الاسم الكامل").fill(studentName);
  await page.getByLabel("هاتف ولي الأمر").fill(parentPhone);
  await page.getByLabel("هاتف ولي الأمر").press("Tab");
  await expect(page.getByLabel("اسم ولي الأمر الكامل")).toBeVisible();
  await page.getByLabel("اسم ولي الأمر الكامل").fill(parentName);
  await page.getByRole("button", { name: "تسجيل طالب" }).click();

  // Lands on the new student's own detail page.
  await expect(page.getByRole("heading", { name: studentName })).toBeVisible();
  await expect(page.getByText("تم تسجيل الطالب.")).toBeVisible();

  // Back to the list: searching the undotted spelling finds the registered (dotted) name.
  await page.getByRole("navigation").getByRole("link", { name: "الطلاب" }).click();
  await page.getByRole("searchbox").fill("احمد");
  await expect(page.getByRole("row", { name: studentName })).toBeVisible();
});

test("secretary: registering a sibling with the same parent phone links the existing parent", async ({ page }) => {
  const suffix = Date.now().toString();
  const phone = uniquePhone();
  const parentName = `Test Parent ${suffix}`;
  const firstChild = `First Child ${suffix}`;
  const secondChild = `Second Child ${suffix}`;

  await signIn(page, "secretary@nile.test");

  // First child: a brand-new parent.
  await page.goto("/students/new");
  await page.getByLabel("Full name", { exact: true }).fill(firstChild);
  await page.getByLabel("Parent's phone").fill(phone);
  await page.getByLabel("Parent's phone").press("Tab");
  await page.getByLabel("Parent's full name").fill(parentName);
  await page.getByRole("button", { name: "Register student" }).click();
  await expect(page.getByRole("heading", { name: firstChild })).toBeVisible();

  // Second child: the same phone number — the lookup finds the parent just registered above.
  await page.goto("/students/new");
  await page.getByLabel("Full name", { exact: true }).fill(secondChild);
  await page.getByLabel("Parent's phone").fill(phone);
  await page.getByLabel("Parent's phone").press("Tab");
  await expect(page.getByText(`Existing parent: ${parentName}`)).toBeVisible();
  await expect(page.getByLabel("Parent's full name")).not.toBeVisible();
  await page.getByRole("button", { name: "Register student" }).click();
  await expect(page.getByRole("heading", { name: secondChild })).toBeVisible();

  // The parent page lists both children — no second parent was created.
  await page.getByRole("link", { name: parentName }).click();
```

The excerpt stops at line 90; use the original file for the remaining 24 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
