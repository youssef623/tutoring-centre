import { useTranslation } from "react-i18next";
import type { StudentChargeDto } from "@/api/generated/tutoring-centre";
import { formatCalendarDate } from "@/features/schedule/week";
import { formatMoney } from "./money";
import {
  DataTable,
  DataTableHead,
  DataTableColumn,
  DataTableBody,
  DataTableRow,
  DataTableCell,
} from "@/components/shared/DataTable";

export function ChargesTable({ items }: { items: StudentChargeDto[] }) {
  const { t, i18n } = useTranslation("billing");
  return (
    <DataTable>
      <DataTableHead>
        {["month", "subject", "group", "amount", "dueDate"].map((key) => (
          <DataTableColumn key={key}>{t(key)}</DataTableColumn>
        ))}
      </DataTableHead>
      <DataTableBody>
        {items.map((item) => (
          <DataTableRow key={item.id}>
            <DataTableCell kind="primary">
              <bdi dir="ltr">{item.billingMonth}</bdi>
            </DataTableCell>
            <DataTableCell label={t("subject")}>
              <bdi>{item.subjectName}</bdi>
            </DataTableCell>
            <DataTableCell label={t("group")}>
              <bdi>{item.groupName}</bdi>
            </DataTableCell>
            <DataTableCell label={t("amount")} className="font-semibold tabular-nums">
              <bdi dir="ltr">{formatMoney(item.amount, i18n.language)}</bdi>
            </DataTableCell>
            <DataTableCell label={t("dueDate")}>
              <bdi dir="ltr">{formatCalendarDate(item.dueDate, i18n.language)}</bdi>
            </DataTableCell>
          </DataTableRow>
        ))}
      </DataTableBody>
    </DataTable>
  );
}
