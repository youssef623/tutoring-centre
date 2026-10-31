import { createFileRoute } from "@tanstack/react-router";
import { AttendancePage } from "@/features/portal/attendance/AttendancePage";
export const Route = createFileRoute("/portal/attendance")({
  validateSearch: (search: Record<string, unknown>) => ({
    month:
      typeof search.month === "string" && /^\d{4}-(0[1-9]|1[0-2])$/.test(search.month)
        ? search.month
        : undefined,
  }),
  component: AttendancePage,
});
