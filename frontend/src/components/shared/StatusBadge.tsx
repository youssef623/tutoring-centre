import { useTranslation } from "react-i18next";
import { Badge } from "@/components/ui/badge";

/**
 * Active/archived as label plus colour, shared across every list that carries this same two-state lifecycle
 * (Day 41; Weeks 8 and 9 reuse it). Month 2's `SubjectStatusBadge`, if one exists under that name, is left as
 * it is — this is a new, generic component, not a replacement.
 */
export function StatusBadge({ status }: { status: "active" | "archived" }) {
  const { t } = useTranslation("common");
  const isArchived = status === "archived";

  return <Badge variant={isArchived ? "outline" : "default"}>{t(isArchived ? "status.archived" : "status.active")}</Badge>;
}
