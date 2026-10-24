import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import { SeatMeter } from "@/features/enrollments/SeatMeter";

describe("SeatMeter", () => {
  it("below 80% shows only the seat pair", () => {
    render(<SeatMeter enrolled={7} capacity={10} />);

    expect(screen.getByText("7 / 10")).toHaveAttribute("dir", "ltr");
    expect(screen.queryByText("Almost full")).not.toBeInTheDocument();
    expect(screen.queryByText("Full")).not.toBeInTheDocument();
  });

  it("from 80% says Almost full in words", () => {
    render(<SeatMeter enrolled={8} capacity={10} />);

    expect(screen.getByText("Almost full")).toBeInTheDocument();
    expect(screen.queryByText("Full")).not.toBeInTheDocument();
  });

  it("at capacity says Full instead", () => {
    render(<SeatMeter enrolled={10} capacity={10} />);

    expect(screen.getByText("Full")).toBeInTheDocument();
    expect(screen.queryByText("Almost full")).not.toBeInTheDocument();
  });
});
