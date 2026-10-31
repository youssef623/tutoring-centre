import { Link } from "@tanstack/react-router";
import { CheckCircle2, CircleAlert, Clock, Camera } from "lucide-react";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { z } from "zod";
import { asApiError } from "@/api/apiFetch";
import { messageFor } from "@/api/errorMessages";
import { useGetSessionAttendance } from "@/api/generated/tutoring-centre";
import { Button } from "@/components/ui/button";
import { buttonVariants } from "@/components/ui/button-variants";
import { formatTime, formatTimeRange } from "@/features/schedule/centreTime";
import { useCamera } from "./useCamera";
import { useScanQueue } from "./useScanQueue";

const TokenShape = z
  .string()
  .trim()
  .regex(/^TCM1\.[A-Za-z0-9_-]{43}$/);

export function ScannerPage({ sessionId }: { sessionId: string }) {
  const query = useGetSessionAttendance(sessionId);
  const { t, i18n } = useTranslation("scanner");
  const [manual, setManual] = useState(false);
  const [input, setInput] = useState("");
  const [invalid, setInvalid] = useState(false);
  const scans = useScanQueue(sessionId);
  const open =
    query.data?.window === "open" && query.data.session.status !== "cancelled" && !scans.closed;
  const { video: videoRef, state: cameraState } = useCamera(
    open && !manual && !scans.paused,
    (text) => {
      void scans.submit(text);
    },
  );
  const back = (
    <Link
      to="/attendance/sessions/$sessionId"
      params={{ sessionId }}
      className={buttonVariants({ variant: "outline" })}
    >
      {t("back")}
    </Link>
  );
  if (query.isPending) return <p role="status">{t("loading")}</p>;
  if (query.error)
    return (
      <section role="alert">
        <h1 className="text-xl font-semibold">
          {asApiError(query.error).status === 404
            ? t("notFound")
            : messageFor(asApiError(query.error))}
        </h1>
        {back}
      </section>
    );
  const data = query.data;
  const status = (value: string) => t(`status.${value}`);
  const time = (value: string) => formatTime(value, data.timeZoneId, i18n.language);
  const notice = scans.notice;
  return (
    <section className="scanner-workspace mx-auto max-w-3xl space-y-5">
      <header className="space-y-2">
        <h1 className="text-2xl font-semibold">{t("title")}</h1>
        <p dir="auto">
          {data.session.groupName} · {data.session.teacherName}
        </p>
        <p>
          {formatTimeRange(
            data.session.startsAt,
            data.session.endsAt,
            data.timeZoneId,
            i18n.language,
          )}{" "}
          · {t(`window.${open ? "open" : "closed"}`)}
        </p>
      </header>
      {back}
      {!open ? (
        <div role="alert" className="rounded-xl border border-destructive p-4">
          {t("closed")}
        </div>
      ) : (
        <>
          <div className="flex gap-2" role="group" aria-label={t("mode")}>
            <Button
              aria-pressed={!manual}
              variant={manual ? "outline" : "default"}
              onClick={() => {
                setManual(false);
              }}
            >
              {t("camera")}
            </Button>
            <Button
              aria-pressed={manual}
              variant={manual ? "default" : "outline"}
              onClick={() => {
                setManual(true);
              }}
            >
              {t("manual")}
            </Button>
          </div>
          {!manual && (
            <div className="space-y-3 rounded-xl border bg-card p-3">
              <video
                ref={videoRef}
                muted
                playsInline
                autoPlay
                aria-label={t("video")}
                className="scanner-video aspect-[4/3] w-full rounded-xl bg-[#171827] object-cover"
              />
              <p role="status" className="flex items-center gap-2 text-sm">
                <Camera className="size-4" aria-hidden="true" />
                {t(`cameraState.${cameraState}`)}
              </p>
              {(cameraState === "denied" || cameraState === "unavailable") && (
                <>
                  <p>{t(cameraState === "denied" ? "allowCamera" : "noCamera")}</p>
                  <Button
                    variant="outline"
                    onClick={() => {
                      setManual(true);
                    }}
                  >
                    {t("useManual")}
                  </Button>
                </>
              )}
            </div>
          )}
          {manual && (
            <form
              className="space-y-3 rounded-xl border bg-card p-4"
              autoComplete="off"
              onSubmit={(event) => {
                event.preventDefault();
                const value = input;
                setInput("");
                const parsed = TokenShape.safeParse(value);
                setInvalid(!parsed.success);
                if (parsed.success) void scans.submit(parsed.data);
              }}
            >
              <label className="grid gap-2 text-sm">
                {t("tokenLabel")}
                <input
                  value={input}
                  onChange={(event) => {
                    setInput(event.target.value);
                  }}
                  autoComplete="off"
                  spellCheck={false}
                  autoCapitalize="none"
                  dir="ltr"
                  className="h-12 rounded-md border bg-background px-3 font-mono"
                  disabled={scans.paused}
                />
              </label>
              {invalid && (
                <p role="alert" className="text-sm text-destructive">
                  {t("invalid")}
                </p>
              )}
              <Button type="submit" disabled={scans.paused}>
                {t("submit")}
              </Button>
            </form>
          )}
        </>
      )}
      {notice && (
        <div
          role="status"
          aria-live="polite"
          data-outcome={notice.tone}
          className={`flex items-start gap-3 rounded-xl border p-4 ${notice.tone === "green" ? "border-emerald-600 bg-emerald-50 text-emerald-950" : notice.tone === "amber" ? "border-amber-600 bg-amber-50 text-amber-950" : "border-red-600 bg-red-50 text-red-950"}`}
        >
          {notice.tone === "green" ? (
            <CheckCircle2 className="size-6 shrink-0" aria-hidden="true" />
          ) : notice.tone === "amber" ? (
            <Clock className="size-6 shrink-0" aria-hidden="true" />
          ) : (
            <CircleAlert className="size-6 shrink-0" aria-hidden="true" />
          )}
          <div>
            {notice.tone === "red" ? (
              t(`errors.${notice.code}`, {
                defaultValue: messageFor({
                  code: notice.code,
                  kind: "rule",
                  status: 422,
                  message: "",
                }),
              })
            ) : (
              <>
                <p className="font-semibold" dir="auto">
                  {notice.result.studentName}
                </p>
                <p>
                  {t(notice.tone === "green" ? "recorded" : "already", {
                    status: status(notice.result.status),
                    time: time(notice.result.recordedAt),
                  })}
                </p>
              </>
            )}
          </div>
        </div>
      )}
      <div className="space-y-2">
        <h2 className="font-semibold">{t("last")}</h2>
        <ol className="divide-y rounded-xl border">
          {scans.last.map((result, index) => (
            <li
              key={`${result.recordedAt}-${String(index)}`}
              className="flex flex-wrap justify-between gap-2 p-3 text-sm"
            >
              <span dir="auto">{result.studentName}</span>
              <span>
                {status(result.status)} · {time(result.recordedAt)} ·{" "}
                {t(`outcome.${result.outcome}`)}
              </span>
            </li>
          ))}
        </ol>
      </div>
    </section>
  );
}
