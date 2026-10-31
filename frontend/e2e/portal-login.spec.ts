import { type Page, expect, test } from "@playwright/test";
import authAr from "../src/i18n/locales/ar/auth.json" with { type: "json" };
import portalAr from "../src/i18n/locales/ar/portal.json" with { type: "json" };

const seedPassword = process.env.SEED_PASSWORD ?? "";
if (seedPassword === "") {
  throw new Error("SEED_PASSWORD must be set to run the portal login journey (see README).");
}

// A one-time password is visible during this journey. Never retain it in recordings or failure screenshots.
test.use({ trace: "off", video: "off", screenshot: "off", locale: "en-US" });

async function signIn(page: Page, email: string, password: string): Promise<void> {
  await page.goto("/login");
  await expect(page.getByRole("heading", { name: "Sign in", exact: true })).toBeVisible();
  await page.getByLabel("Email", { exact: true }).fill(email);
  await page.getByLabel("Password", { exact: true }).fill(password);
  await page.getByRole("button", { name: "Sign in", exact: true }).click();
}

test("owner creates a parent login; parent changes its password and enters an isolated Arabic portal", async ({
  page,
}) => {
  let pageErrorCount = 0;
  page.on("pageerror", () => {
    pageErrorCount += 1;
  });
  const suffix = Date.now().toString();
  const email = `portal-journey-${suffix}@nile.test`;
  const parentName = `Portal Journey Parent ${suffix}`;
  await signIn(page, "owner@nile.test", seedPassword);
  await expect(
    page.getByRole("heading", { name: "Welcome, Nile Owner", exact: true }),
  ).toBeVisible();

  // Isolated synthetic academics setup; the portal account itself is created through its actual detail UI.
  const csrfResponse = await page.request.get("/api/auth/antiforgery");
  expect(csrfResponse.status()).toBe(200);
  const csrf = (await csrfResponse.json()) as { token: string };
  const registration = await page.request.post("/api/students", {
    headers: { "X-XSRF-TOKEN": csrf.token },
    data: {
      fullName: `Portal Journey Student ${suffix}`,
      parent: { fullName: parentName, phone: `+2010${suffix.slice(-8)}` },
      relationship: "father",
    },
  });
  expect(registration.status()).toBe(201);
  const subject = (await registration.json()) as { parentId: string };

  await page.goto(`/parents/${subject.parentId}`);
  await expect(page.getByRole("heading", { name: parentName, exact: true })).toBeVisible();
  const section = page.getByRole("region", { name: "Portal account", exact: true });
  await expect(section.getByText("No account yet", { exact: true })).toBeVisible();
  await section.getByRole("button", { name: "Create account", exact: true }).click();
  const createDialog = page.getByRole("dialog", { name: "Create portal account", exact: true });
  await createDialog.getByLabel("Email", { exact: true }).fill(email);
  await createDialog.getByLabel("Account language", { exact: true }).selectOption("ar");
  await createDialog.getByRole("button", { name: "Create account", exact: true }).click();

  const passwordDialog = page.getByRole("dialog", { name: "Account created", exact: true });
  await expect(
    passwordDialog.getByText("Copy this password now. It cannot be shown again.", { exact: true }),
  ).toBeVisible();
  const temporaryPassword = await passwordDialog.locator("output").innerText();
  await passwordDialog.getByRole("button", { name: "Done", exact: true }).click();
  await expect(page.locator("output")).toHaveCount(0);
  await expect(section.getByText("Account active", { exact: true })).toBeVisible();
  const secretStored = await page.evaluate(
    (secret) =>
      [localStorage, sessionStorage].some((storage) =>
        Array.from({ length: storage.length }, (_, index) =>
          storage.getItem(storage.key(index) ?? ""),
        ).some((value) => value?.includes(secret)),
      ),
    temporaryPassword,
  );
  expect(secretStored).toBe(false);

  await page.getByRole("button", { name: "Nile Owner", exact: true }).click();
  await page.getByRole("menuitem", { name: "Log out", exact: true }).click();
  await signIn(page, email, temporaryPassword);

  await expect(
    page.getByRole("heading", { name: authAr.changePassword.title, exact: true }),
  ).toBeVisible();
  await expect(page.locator("html")).toHaveAttribute("dir", "rtl");
  await page.goto("/portal");
  await expect(page).toHaveURL(/\/change-password$/);
  await page.locator("#current-password").fill(temporaryPassword);
  const newPassword = `Portal-Journey-New-${suffix}!`;
  await page.locator("#new-password").fill(newPassword);
  await page.locator("#confirm-password").fill(newPassword);
  await page.getByRole("button", { name: authAr.changePassword.submit, exact: true }).click();

  await expect(
    page.getByRole("heading", { name: authAr.selectCentre.title, exact: true }),
  ).toBeVisible();
  await page.getByRole("button", { name: /Nile Tutoring Centre/ }).click();
  await expect(page).toHaveURL(/\/portal$/);
  await expect(page.getByRole("heading", { name: portalAr.title, exact: true })).toBeVisible();
  await expect(page.locator("html")).toHaveAttribute("lang", "ar");
  await expect(page.locator("html")).toHaveAttribute("dir", "rtl");
  const navigation = page.getByRole("navigation", { name: portalAr.navigation, exact: true });
  await expect(navigation.getByRole("link")).toHaveCount(2);
  await expect(navigation.getByRole("button")).toHaveCount(0);
  await expect(navigation.getByRole("link", { name: portalAr.chat, exact: true })).toBeEnabled();
  await expect(
    navigation.getByRole("link", { name: portalAr.attendance, exact: true }),
  ).toBeEnabled();

  await page.goto("/students");
  await expect(page).toHaveURL(/\/portal$/);
  await expect(page.getByRole("heading", { name: portalAr.title, exact: true })).toBeVisible();
  for (const path of ["/api/students", "/api/staff", "/api/subjects"]) {
    const denied = await page.request.get(path);
    expect(denied.status()).toBe(403);
    const problem = (await denied.json()) as { code: string; items?: unknown };
    expect(problem.code).toBe("auth.permission_denied");
    expect(problem.items).toBeUndefined();
  }

  const assertNoHorizontalOverflow = async () => {
    const overflows = await page.evaluate(
      "document.documentElement.scrollWidth > window.innerWidth",
    );
    expect(overflows).toBe(false);
  };
  await assertNoHorizontalOverflow();
  if (process.env.PORTAL_VISUAL_QA === "1") {
    // Capture only the final portal, after the credential dialog and password-change form are gone.
    await page.setViewportSize({ width: 1536, height: 1024 });
    await page.evaluate("document.fonts.ready");
    await assertNoHorizontalOverflow();
    await page.screenshot({
      path: "../artifacts/day75-design/portal-browser-desktop.png",
      fullPage: true,
    });
    await page.setViewportSize({ width: 390, height: 844 });
    await assertNoHorizontalOverflow();
    await page.screenshot({
      path: "../artifacts/day75-design/portal-browser-mobile.png",
      fullPage: true,
    });
    await page.getByRole("button", { name: "English", exact: true }).click();
    await expect(page.locator("html")).toHaveAttribute("dir", "ltr");
    await expect(page.getByRole("heading", { name: "Centre portal", exact: true })).toBeVisible();
    await assertNoHorizontalOverflow();
    await page.screenshot({
      path: "../artifacts/day75-design/portal-browser-mobile-en.png",
      fullPage: true,
    });
  }
  expect(pageErrorCount).toBe(0);
});
