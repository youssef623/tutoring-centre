import { useTranslation } from "react-i18next";
import type { StudentChargeDto } from "@/api/generated/tutoring-centre";
import { formatCalendarDate } from "@/features/schedule/week";
import { formatMoney } from "./money";

export function ChargesTable({ items }: { items: StudentChargeDto[] }) {
  const { t, i18n } = useTranslation("billing");
  return (
    <div className="overflow-x-auto">
      <table className="w-full text-start text-sm">
        <thead>
          <tr>
            {["month", "subject", "group", "amount", "dueDate"].map((key) => (
              <th key={key} className="p-3 text-start">
                {t(key)}
              </th>
            ))}
          </tr>
        </thead>
        <tbody>
          {items.map((item) => (
            <tr key={item.id} className="border-t">
              <td className="p-3">
                <bdi dir="ltr">{item.billingMonth}</bdi>
              </td>
              <td className="p-3">
                <bdi>{item.subjectName}</bdi>
              </td>
              <td className="p-3">
                <bdi>{item.groupName}</bdi>
              </td>
              <td className="p-3 tabular-nums">
                <bdi dir="ltr">{formatMoney(item.amount, i18n.language)}</bdi>
              </td>
              <td className="p-3">
                <bdi dir="ltr">{formatCalendarDate(item.dueDate, i18n.language)}</bdi>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
