import { type Page } from "@playwright/test";

/** Follow the same disclosure-and-link interaction as the floating masthead. */
export async function navigateFromMasthead(page: Page, label: string): Promise<void> {
  const navigation = page.locator(".desktop-navigation");
  await navigation.waitFor({ state: "visible" });
  const link = navigation.locator("a").filter({ hasText: new RegExp(`^${label}$`) });
  await link.waitFor({ state: "attached" });
  const disclosure = link.locator("xpath=ancestor::details");
  if (await disclosure.count()) {
    if ((await disclosure.getAttribute("open")) === null) {
      await disclosure.locator("summary").click();
    }
  }
  const destination = await link.getAttribute("href");
  await navigation.getByRole("link", { name: label, exact: true }).click();
  if (destination) {
    const target = new URL(destination, page.url());
    await page.waitForURL((url) => url.pathname === target.pathname);
    if (target.pathname !== "/") await page.locator(".dashboard").waitFor({ state: "hidden" });
  }
}
