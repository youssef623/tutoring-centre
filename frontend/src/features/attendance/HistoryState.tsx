import { useTranslation } from "react-i18next";
import { asApiError } from "@/api/apiFetch";
import { messageFor } from "@/api/errorMessages";
import { Button } from "@/components/ui/button";

export function HistoryState({
  loading,
  error,
  onRetry,
}: {
  loading: boolean;
  error: unknown;
  onRetry: () => void;
}) {
  const { t } = useTranslation("attendance");
  const { t: common } = useTranslation("common");
  if (error)
    return (
      <div role="alert" className="space-y-2">
        <p>{messageFor(asApiError(error))}</p>
        <Button variant="outline" onClick={onRetry}>
          {common("retry")}
        </Button>
      </div>
    );
  if (loading) return <p role="status">{t("history.loading")}</p>;
  return <p className="py-8 text-center text-muted-foreground">{t("history.empty")}</p>;
}
