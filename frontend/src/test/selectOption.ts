import { fireEvent, screen } from "@testing-library/react";

/**
 * Opens a Base UI `Select` by its trigger's accessible name and picks the option with the given name.
 *
 * Base UI's `Select.Item` only commits a mouse click when it was preceded by a `pointerdown` on the same
 * item (see `allowMouseSelectionRef` in its source) — a real click is always `pointerdown` then `click`,
 * but `fireEvent.click` alone dispatches only the `click` event, so the selection is silently ignored
 * without this.
 */
export async function selectOption(triggerName: string, optionName: string): Promise<void> {
  fireEvent.click(screen.getByRole("combobox", { name: triggerName }));
  const option = await screen.findByRole("option", { name: optionName });
  fireEvent.pointerDown(option);
  fireEvent.click(option);
}
