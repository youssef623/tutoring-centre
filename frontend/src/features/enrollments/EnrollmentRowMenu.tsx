import { ArrowLeftRight, LogOut, MoreVertical } from "lucide-react";
import { useTranslation } from "react-i18next";
import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";

/** The per-enrollment actions, shared by the roster table and the student's enrollments card. */
export function EnrollmentRowMenu({
  label,
  onTransfer,
  onWithdraw,
}: {
  label: string;
  onTransfer: () => void;
  onWithdraw: () => void;
}) {
  const { t } = useTranslation("enrollments");

  return (
    <DropdownMenu>
      <DropdownMenuTrigger
        render={<Button type="button" variant="ghost" size="icon-sm" aria-label={t("actions.openMenu", { name: label })} />}
      >
        <MoreVertical className="size-4" aria-hidden="true" />
      </DropdownMenuTrigger>
      <DropdownMenuContent align="end" className="w-auto">
        <DropdownMenuItem onClick={onTransfer}>
          <ArrowLeftRight className="size-4 rtl:-scale-x-100" aria-hidden="true" />
          {t("actions.transfer")}
        </DropdownMenuItem>
        <DropdownMenuItem variant="destructive" onClick={onWithdraw}>
          <LogOut className="size-4 rtl:-scale-x-100" aria-hidden="true" />
          {t("actions.withdraw")}
        </DropdownMenuItem>
      </DropdownMenuContent>
    </DropdownMenu>
  );
}
