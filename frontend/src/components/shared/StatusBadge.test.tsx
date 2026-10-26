import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import { StatusBadge } from "./StatusBadge";

describe("StatusBadge", () => {
  it("renders the active label", () => {
    render(<StatusBadge status="active" />);
    expect(screen.getByText("Active")).toBeInTheDocument();
  });

  it("renders the archived label", () => {
    render(<StatusBadge status="archived" />);
    expect(screen.getByText("Archived")).toBeInTheDocument();
  });
});
