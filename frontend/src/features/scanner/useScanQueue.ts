import { useCallback, useEffect, useRef, useState } from "react";
import { useQueryClient } from "@tanstack/react-query";
import { asApiError } from "@/api/apiFetch";
import { getGetSessionAttendanceQueryKey, recordScan, useRecordScan, type ScanResult } from "@/api/generated/tutoring-centre";

type Arrival = Pick<ScanResult, "studentName" | "status" | "recordedAt" | "outcome">;
export type ScanNotice = { tone: "green" | "amber"; result: Arrival } | { tone: "red"; code: string };

/** Tokens exist only in the bounded pending queue and active request ref, never in a query/mutation cache. */
export function useScanQueue(sessionId: string) {
  const queryClient = useQueryClient();
  const activeToken = useRef("");
  const pending = useRef<string[]>([]);
  const recent = useRef(new Map<string, number>());
  const busy = useRef(false);
  const blocked = useRef(false);
  const mounted = useRef(true);
  const abort = useRef<AbortController | null>(null);
  const timer = useRef<ReturnType<typeof setTimeout> | undefined>(undefined);
  const [notice, setNotice] = useState<ScanNotice | null>(null);
  const [last, setLast] = useState<Arrival[]>([]);
  const [paused, setPaused] = useState(false);
  const [closed, setClosed] = useState(false);
  const mutation = useRecordScan({ mutation: {
    gcTime: 0,
    retry: false,
    // Credentials are not mutation variables. The placeholder is intentionally empty.
    mutationFn: ({ sessionId: id }) => recordScan(id, { token: activeToken.current }, { signal: abort.current?.signal }),
  } });
  const mutate = mutation.mutateAsync;
  const reset = mutation.reset;
  const canRun = useCallback(() => mounted.current && !blocked.current, []);
  const isMounted = useCallback(() => mounted.current, []);

  useEffect(() => {
    mounted.current = true;
    blocked.current = false;
    const queue = pending.current;
    const seen = recent.current;
    return () => {
      mounted.current = false; blocked.current = true;
      queue.length = 0; activeToken.current = ""; seen.clear();
      abort.current?.abort(); clearTimeout(timer.current);
    };
  }, []);

  const drain = useCallback(async () => {
    if (busy.current || !canRun()) return;
    busy.current = true;
    try {
      while (pending.current.length && canRun()) {
        activeToken.current = pending.current.shift() ?? "";
        abort.current = new AbortController();
        try {
          let result: ScanResult;
          try { result = await mutate({ sessionId, data: { token: "" } }); }
          catch (first) {
            if (asApiError(first).code !== "attendance.concurrent_mark") throw first;
            result = await mutate({ sessionId, data: { token: "" } });
          }
          if (!isMounted()) return;
          const arrival = { studentName: result.studentName, status: result.status, recordedAt: result.recordedAt, outcome: result.outcome };
          setNotice({ tone: result.outcome === "recorded" ? "green" : "amber", result: arrival });
          setLast((rows) => [arrival, ...rows].slice(0, 10));
          if (result.outcome === "recorded") {
            (navigator as Partial<Navigator>).vibrate?.(60);
            void queryClient.invalidateQueries({ queryKey: getGetSessionAttendanceQueryKey(sessionId) });
          }
        } catch (error) {
          if (!isMounted()) return;
          const failure = asApiError(error);
          setNotice({ tone: "red", code: failure.code });
          if (failure.code === "attendance.window_closed" || failure.code === "attendance.session_cancelled" || failure.code === "attendance.session_not_open") {
            blocked.current = true; pending.current.length = 0; setClosed(true);
          } else if (failure.status === 429) {
            blocked.current = true; pending.current.length = 0; setPaused(true);
            timer.current = setTimeout(() => {
              if (!isMounted()) return;
              blocked.current = false; setPaused(false);
            }, (failure.retryAfterSeconds ?? 60) * 1000);
          }
        } finally { activeToken.current = ""; abort.current = null; reset(); }
      }
    } finally { busy.current = false; }
  }, [canRun, isMounted, mutate, queryClient, reset, sessionId]);

  const submit = useCallback(async (token: string) => {
    if (!token.startsWith("TCM1.") || !canRun()) return;
    // Retain a digest for repeat filtering, never the previous credential itself.
    const bytes = await crypto.subtle.digest("SHA-256", new TextEncoder().encode(token));
    if (!canRun()) return;
    const fingerprint = Array.from(new Uint8Array(bytes), (byte) => byte.toString(16).padStart(2, "0")).join("");
    const now = Date.now();
    for (const [key, at] of recent.current) if (now - at >= 3000) recent.current.delete(key);
    if (recent.current.has(fingerprint) || pending.current.length >= 5) return;
    recent.current.set(fingerprint, now);
    pending.current.push(token);
    void drain();
  }, [canRun, drain]);
  return { submit, notice, last, paused, closed };
}
