import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import { PhoneInput } from "./PhoneInput";

describe("PhoneInput", () => {
  it("is always left-to-right, even when rendered inside an rtl document", () => {
    document.documentElement.dir = "rtl";
    try {
      render(<PhoneInput id="phone" value="+201012345678" onChange={() => {}} />);

      const input = screen.getByRole("textbox");
      expect(input).toHaveAttribute("dir", "ltr");
      expect(input).toHaveAttribute("type", "tel");
    } finally {
      document.documentElement.dir = "ltr";
    }
  });
});
