import { screen } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { describe, expect, it } from "vitest";
import { server } from "@/test/msw/server";
import { renderWithQueryClient } from "@/test/render";
import { SystemInfoCard } from "./SystemInfoCard";

describe("SystemInfoCard", () => {
  it("shows version, latest migration and the up-to-date badge on success", async () => {
    // Arrange (default MSW handler)

    // Act
    renderWithQueryClient(<SystemInfoCard />);

    // Assert
    expect(await screen.findByText("1.0.0")).toBeInTheDocument();
    expect(screen.getByText("20261012_InitialPlatform")).toBeInTheDocument();
    expect(screen.getByText("Database schema up to date")).toBeInTheDocument();
  });

  it("shows the translated error message and the correlation reference on a 500 Problem Details response", async () => {
    // Arrange
    server.use(
      http.get("*/api/system/info", () =>
        HttpResponse.json(
          {
            title: "An unexpected error occurred.",
            status: 500,
            code: "server.unexpected",
            traceId: "trace-1",
            correlationId: "corr-500",
          },
          { status: 500 },
        ),
      ),
    );

    // Act
    renderWithQueryClient(<SystemInfoCard />);

    // Assert
    expect(await screen.findByText("Something went wrong. Please try again.")).toBeInTheDocument();
    expect(screen.getByText("Reference: corr-500")).toBeInTheDocument();
  });
});
