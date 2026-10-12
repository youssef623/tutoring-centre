import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import { PhoneText } from "./PhoneText";

describe("PhoneText", () => {
  it("renders left-to-right even when the document is rtl", () => {
    document.documentElement.dir = "rtl";
    try {
      render(<PhoneText value="+201012345678" />);

      expect(screen.getByText("+201012345678")).toHaveAttribute("dir", "ltr");
    } finally {
      document.documentElement.dir = "ltr";
    }
  });

  it("renders a tel: link when asLink is set", () => {
    render(<PhoneText value="+201012345678" asLink />);

    const link = screen.getByRole("link", { name: "+201012345678" });
    expect(link).toHaveAttribute("href", "tel:+201012345678");
    expect(link).toHaveAttribute("dir", "ltr");
  });
});
