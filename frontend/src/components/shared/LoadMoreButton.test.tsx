import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { LoadMoreButton } from "./LoadMoreButton";

describe("LoadMoreButton", () => {
  it("calls onClick when pressed, and is disabled while pending", () => {
    const onClick = vi.fn();
    const { rerender } = render(<LoadMoreButton onClick={onClick} />);

    fireEvent.click(screen.getByRole("button", { name: "Load more" }));
    expect(onClick).toHaveBeenCalledTimes(1);

    rerender(<LoadMoreButton onClick={onClick} pending />);
    expect(screen.getByRole("button", { name: "Load more" })).toBeDisabled();
  });
});
