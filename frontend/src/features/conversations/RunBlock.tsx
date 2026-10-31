import { useTranslation } from "react-i18next";
export interface TimelineCall {
  toolName: string;
  decision: string;
  reasonCode?: string | null;
  argumentSummary: string;
  resultSummary?: string | null;
  durationMs: number;
}
export interface RunItem {
  id: string;
  kind?: string;
  at?: string;
  status?: string | null;
  steps?: number | null;
  guardReason?: string | null;
  failureCode?: string | null;
  toolCalls?: TimelineCall[] | null;
}
const colors: Record<string, string> = {
  allowed: "bg-emerald-100 text-emerald-900",
  denied: "bg-red-100 text-red-900",
  invalid: "bg-amber-100 text-amber-900",
  failed: "bg-gray-100 text-gray-900",
};
export function RunBlock({ run }: { run: RunItem }) {
  const { t } = useTranslation("conversations");
  return (
    <details className="w-full rounded-lg border border-dashed px-4 py-3 text-sm">
      <summary className="cursor-pointer font-medium">{t("activity")}</summary>
      <div className="mt-4 space-y-3">
        <p>
          {t(`statuses.${run.status ?? "failed"}`, { defaultValue: run.status ?? "" })} ·{" "}
          {t("steps", { count: run.steps ?? 0 })}
        </p>
        {run.guardReason && (
          <p role="status">
            {t(`reasons.${run.guardReason}`, { defaultValue: t("guardFallback") })}
          </p>
        )}
        {run.failureCode && (
          <p>{t(`reasons.${run.failureCode}`, { defaultValue: t("toolFailed") })}</p>
        )}
        <ul className="divide-y">
          {run.toolCalls?.map((call, index) => (
            <li key={index} className="flex flex-wrap items-center gap-3 py-3">
              <span className="font-medium" dir="ltr">
                {call.toolName}
              </span>
              <span dir="ltr">{call.argumentSummary}</span>
              <span
                className={`rounded px-2 py-1 text-xs font-medium ${colors[call.decision] ?? "bg-gray-100 text-gray-900"}`}
              >
                {t(`decisions.${call.decision}`)}
              </span>
              {call.reasonCode && (
                <span>{t(`reasons.${call.reasonCode}`, { defaultValue: t("toolFailed") })}</span>
              )}
              {call.resultSummary && <span>{call.resultSummary}</span>}
            </li>
          ))}
        </ul>
      </div>
    </details>
  );
}
