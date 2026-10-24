import { Link } from "@tanstack/react-router";
import type { ReactNode } from "react";
import { useTranslation } from "react-i18next";
import type { RosterEntryDto } from "@/api/generated/tutoring-centre";

/**
 * The students currently in a group. Names link to the student page only for sessions that may open it
 * (`students.view`); a group's teacher sees plain names. The row menu column exists only when the caller passes
 * one, which the roster tab does only inside `enrollments.manage`. No contact details: the API returns none.
 */
export function RosterTable({
  items,
  linkStudents,
  rowMenu,
}: {
  items: RosterEntryDto[];
  linkStudents: boolean;
  rowMenu?: (entry: RosterEntryDto) => ReactNode;
}) {
  const { t, i18n } = useTranslation("enrollments");
  const dateFormat = new Intl.DateTimeFormat(i18n.language, { dateStyle: "medium" });

  return (
    <div className="overflow-x-auto rounded-xl border border-border">
      <table className="w-full min-w-[480px] text-start text-sm">
        <thead className="bg-muted/50 text-muted-foreground">
          <tr>
            <th className="px-4 py-2 text-start font-medium">{t("roster.columns.student")}</th>
            <th className="px-4 py-2 text-start font-medium">{t("roster.columns.enrolledAt")}</th>
            {rowMenu !== undefined && (
              <th className="px-4 py-2 text-start font-medium">
                <span className="sr-only">{t("roster.columns.actions")}</span>
              </th>
            )}
          </tr>
        </thead>
        <tbody className="divide-y divide-border">
          {items.map((entry) => (
            <tr key={entry.enrollmentId}>
              <td className="px-4 py-2">
                {linkStudents ? (
                  <Link
                    to="/students/$studentId"
                    params={{ studentId: entry.studentId }}
                    className="font-medium hover:underline"
                    dir="auto"
                  >
                    {entry.studentName}
                  </Link>
                ) : (
                  <span className="font-medium" dir="auto">
                    {entry.studentName}
                  </span>
                )}
              </td>
              <td className="px-4 py-2 text-muted-foreground">
                <time dateTime={entry.enrolledAt}>{dateFormat.format(new Date(entry.enrolledAt))}</time>
              </td>
              {rowMenu !== undefined && <td className="px-4 py-2 text-end">{rowMenu(entry)}</td>}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
