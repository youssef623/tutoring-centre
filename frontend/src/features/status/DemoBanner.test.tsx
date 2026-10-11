import { screen, waitFor } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { describe, expect, it } from "vitest";
import { server } from "@/test/msw/server";
import { renderWithQueryClient } from "@/test/render";
import { DemoBanner } from "./DemoBanner";

describe("DemoBanner", () => {
  it("renders nothing while loading and nothing once demoMode resolves to false", async () => {
    renderWithQueryClient(<DemoBanner />);

    // Null in both states (loading, then settled-false): confirm it stays null after the query settles.
    expect(screen.queryByRole("status")).not.toBeInTheDocument();
    await waitFor(() => {
      expect(screen.queryByRole("status")).not.toBeInTheDocument();
    });
  });

  it("renders the message when demoMode is true", async () => {
    server.use(
      http.get("*/api/system/info", () =>
        HttpResponse.json({
          applicationVersion: "1.0.0",
          latestMigration: "20261012_InitialPlatform",
          databaseUpToDate: true,
          demoMode: true,
        }),
      ),
    );

    renderWithQueryClient(<DemoBanner />);

    expect(
      await screen.findByText("Demo environment — data may be reset. Do not enter real personal data."),
    ).toBeInTheDocument();
  });
});
