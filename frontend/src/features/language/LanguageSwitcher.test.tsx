import { fireEvent, render, screen } from "@testing-library/react";
import { afterEach, describe, expect, it } from "vitest";
import i18n from "@/i18n";
import { LanguageSwitcher } from "./LanguageSwitcher";

describe("LanguageSwitcher", () => {
  afterEach(async () => {
    // Leave the document and i18next in English so other test files are unaffected.
    await i18n.changeLanguage("en");
  });

  it("switching to Arabic sets dir=rtl on the document and persists the choice", async () => {
    // Arrange
    render(<LanguageSwitcher />);

    // Act
    fireEvent.click(screen.getByRole("button"));
    await screen.findByRole("button", { name: "English" });

    // Assert
    expect(document.documentElement.dir).toBe("rtl");
    expect(document.documentElement.lang).toBe("ar");
    expect(localStorage.getItem("tcm.lang")).toBe("ar");
  });

  it("switching back to English sets dir=ltr", async () => {
    // Arrange
    render(<LanguageSwitcher />);
    fireEvent.click(screen.getByRole("button"));
    await screen.findByRole("button", { name: "English" });

    // Act
    fireEvent.click(screen.getByRole("button"));
    await screen.findByRole("button", { name: "العربية" });

    // Assert
    expect(document.documentElement.dir).toBe("ltr");
    expect(document.documentElement.lang).toBe("en");
  });
});
