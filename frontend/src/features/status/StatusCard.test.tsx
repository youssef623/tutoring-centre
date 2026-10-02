import { fireEvent, screen } from "@testing-library/react";
import { http, HttpResponse } from "msw";
import { describe, expect, it } from "vitest";
import { server } from "@/test/msw/server";
import { renderWithQueryClient } from "@/test/render";
import { StatusCard } from "./StatusCard";

const readinessUrl = "/health/ready";
const healthy = () => new HttpResponse("Healthy", { status: 200 });
const unavailable = () => new HttpResponse("Unhealthy", { status: 503 });

describe("StatusCard", () => {
  it("shows the healthy state when the API returns 200", async () => {
    // Arrange
    server.use(http.get(readinessUrl, healthy));

    // Act
    renderWithQueryClient(<StatusCard />);

    // Assert
    expect(await screen.findByText("API and database are reachable")).toBeInTheDocument();
  });

  it("shows the database-unavailable state with Retry when the API returns 503", async () => {
    server.use(http.get(readinessUrl, unavailable));

    renderWithQueryClient(<StatusCard />);

    expect(
      await screen.findByText("API is running but the database is unavailable"),
    ).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Retry" })).toBeInTheDocument();
    // 503 is an answer from the API, not a network failure.
    expect(screen.queryByText("Cannot reach the API")).not.toBeInTheDocument();
  });

  it("refetches and shows the healthy state after Retry", async () => {
    server.use(http.get(readinessUrl, unavailable));
    renderWithQueryClient(<StatusCard />);
    const retryButton = await screen.findByRole("button", { name: "Retry" });

    server.use(http.get(readinessUrl, healthy));
    fireEvent.click(retryButton);

    expect(await screen.findByText("API and database are reachable")).toBeInTheDocument();
  });
});
