import { fireEvent, render, screen } from "@testing-library/react";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { SearchInput } from "./SearchInput";

describe("SearchInput", () => {
  beforeEach(() => {
    vi.useFakeTimers();
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it("debounces typing and calls onChange exactly once after the pause", () => {
    const onChange = vi.fn();
    render(<SearchInput value="" onChange={onChange} />);
    const input = screen.getByRole("searchbox");

    fireEvent.change(input, { target: { value: "a" } });
    fireEvent.change(input, { target: { value: "ah" } });
    fireEvent.change(input, { target: { value: "ahm" } });

    expect(onChange).not.toHaveBeenCalled();
    vi.advanceTimersByTime(300);

    expect(onChange).toHaveBeenCalledTimes(1);
    expect(onChange).toHaveBeenCalledWith("ahm");
  });

  it("the clear button resets immediately, without waiting for the debounce", () => {
    const onChange = vi.fn();
    render(<SearchInput value="احمد" onChange={onChange} />);

    fireEvent.click(screen.getByRole("button", { name: "Clear search" }));

    expect(onChange).toHaveBeenCalledWith("");
    expect(screen.queryByRole("button", { name: "Clear search" })).not.toBeInTheDocument();
  });
});
