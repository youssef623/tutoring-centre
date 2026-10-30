import { fireEvent, screen } from "@testing-library/react";
import { beforeEach, expect, it } from "vitest";
import i18n from "@/i18n";
import { renderWithQueryClient } from "@/test/render";
import { RunBlock } from "./RunBlock";
beforeEach(async () => {
  await i18n.changeLanguage("en");
});
it("renders translated allowed, denied and guarded evidence without raw arguments", () => {
  renderWithQueryClient(
    <RunBlock
      run={{
        id: "run",
        status: "completed",
        guardReason: "guard.identifier",
        steps: 2,
        toolCalls: [
          {
            toolName: "get_attendance",
            argumentSummary: "child_1",
            decision: "allowed",
            resultSummary: "1 record",
            durationMs: 10,
          },
          {
            toolName: "get_attendance",
            argumentSummary: "child_2",
            decision: "denied",
            reasonCode: "tool.handle_unknown",
            durationMs: 1,
          },
        ],
      }}
    />,
  );
  fireEvent.click(screen.getByText("Assistant activity"));
  expect(screen.getByText("Allowed")).toBeInTheDocument();
  expect(screen.getByText("Denied")).toBeInTheDocument();
  expect(screen.getByText("The reply contained an internal identifier.")).toBeInTheDocument();
  expect(screen.getByText("1 record")).toBeInTheDocument();
});

it("labels a capped reply without claiming a fallback was sent", () => {
  renderWithQueryClient(
    <RunBlock
      run={{
        id: "capped",
        status: "completed",
        guardReason: "assistant.reply_capped",
        steps: 1,
        toolCalls: [],
      }}
    />,
  );
  fireEvent.click(screen.getByText("Assistant activity"));
  expect(screen.getByText("The reply was shortened to the reply limit.")).toBeInTheDocument();
  expect(screen.queryByText("A safe fallback reply was sent.")).not.toBeInTheDocument();
});
