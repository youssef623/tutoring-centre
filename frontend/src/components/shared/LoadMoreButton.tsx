import { useTranslation } from "react-i18next";
import { Button } from "@/components/ui/button";

/** The "load more" action for an infinite query over a cursor API (Day 41; Month 2's audit page has its own inline version). */
export function LoadMoreButton({ onClick, pending }: { onClick: () => void; pending?: boolean }) {
  const { t } = useTranslation("common");

  return (
    <Button type="button" variant="outline" disabled={pending} onClick={onClick}>
      {t("loadMore")}
    </Button>
  );
}
