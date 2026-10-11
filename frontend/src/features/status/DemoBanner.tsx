import { useTranslation } from "react-i18next";
import { useGetSystemInfo } from "@/api/generated/tutoring-centre";

/**
 * Non-dismissible, driven by the server's `demoMode` flag (Task 35.7) so it can never be forgotten in a build —
 * it shows on every route, including the login page, and renders nothing until the flag is known to be true.
 */
export function DemoBanner() {
  const { t } = useTranslation("common");
  const { data, isPending } = useGetSystemInfo();

  if (isPending || data?.demoMode !== true) {
    return null;
  }

  return (
    <div className="w-full bg-amber-500 px-4 py-1.5 text-center text-sm font-medium text-amber-950" role="status">
      {t("demoBanner.message")}
    </div>
  );
}
