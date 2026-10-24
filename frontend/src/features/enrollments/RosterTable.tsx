import { Link } from "@tanstack/react-router";
import type { ReactNode } from "react";
import { useTranslation } from "react-i18next";
import type { RosterEntryDto } from "@/api/generated/tutoring-centre";
import {
  DataTable,
  DataTableBody,
  DataTableCell,
  DataTableColumn,
  DataTableHead,
  DataTableRow,
} from "@/components/shared/DataTable";

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
    <DataTable className="sm:min-w-[480px]">
      <DataTableHead>
        <DataTableColumn>{t("roster.columns.student")}</DataTableColumn>
        <DataTableColumn>{t("roster.columns.enrolledAt")}</DataTableColumn>
        {rowMenu !== undefined && <DataTableColumn hidden>{t("roster.columns.actions")}</DataTableColumn>}
      </DataTableHead>
      <DataTableBody>
        {items.map((entry) => (
          <DataTableRow key={entry.enrollmentId}>
            <DataTableCell kind="primary">
              {linkStudents ? (
                <Link to="/students/$studentId" params={{ studentId: entry.studentId }} className="font-medium hover:underline">
                  <bdi>{entry.studentName}</bdi>
                </Link>
              ) : (
                <bdi className="font-medium">{entry.studentName}</bdi>
              )}
            </DataTableCell>
            <DataTableCell label={t("roster.columns.enrolledAt")} className="text-muted-foreground">
              <time dateTime={entry.enrolledAt}>{dateFormat.format(new Date(entry.enrolledAt))}</time>
            </DataTableCell>
            {rowMenu !== undefined && <DataTableCell kind="actions">{rowMenu(entry)}</DataTableCell>}
          </DataTableRow>
        ))}
      </DataTableBody>
    </DataTable>
  );
}
