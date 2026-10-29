import { useEffect, useRef, useState } from "react";

export type CameraState = "starting" | "scanning" | "denied" | "unavailable" | "paused";

export function useCamera(enabled: boolean, onDecoded: (text: string) => void) {
  const video = useRef<HTMLVideoElement>(null);
  const callback = useRef(onDecoded);
  const [state, setState] = useState<CameraState>("starting");
  const [hidden, setHidden] = useState(document.hidden);
  useEffect(() => { callback.current = onDecoded; }, [onDecoded]);
  useEffect(() => {
    const changed = () => { setHidden(document.hidden); };
    document.addEventListener("visibilitychange", changed);
    return () => { document.removeEventListener("visibilitychange", changed); };
  }, []);
  useEffect(() => {
    if (!enabled || hidden) return;
    const lifetime = new AbortController();
    const cancelled = () => lifetime.signal.aborted;
    let stream: MediaStream | undefined;
    let controls: { stop: () => void } | undefined;
    const preview = video.current;
    async function start() {
      try {
        const media = (navigator as Partial<Navigator>).mediaDevices;
        if (!media || !preview) { setState("unavailable"); return; }
        setState("starting");
        stream = await media.getUserMedia({ video: { facingMode: { ideal: "environment" } }, audio: false });
        if (cancelled()) { stream.getTracks().forEach((track) => { track.stop(); }); return; }
        const { startDecoder } = await import("./decoder");
        if (cancelled()) return;
        controls = await startDecoder(stream, preview, (text) => { if (!cancelled()) callback.current(text); });
        if (cancelled()) controls.stop();
        else setState("scanning");
      } catch (error) {
        stream?.getTracks().forEach((track) => { track.stop(); });
        if (!cancelled()) setState(error instanceof DOMException && error.name === "NotAllowedError" ? "denied" : "unavailable");
      }
    }
    void start();
    return () => {
      lifetime.abort();
      controls?.stop();
      stream?.getTracks().forEach((track) => { track.stop(); });
      if (preview) preview.srcObject = null;
    };
  }, [enabled, hidden]);
  return { video, state: !enabled || hidden ? "paused" as const : state };
}
