import { useId, useState } from "react";
import { useTranslation } from "react-i18next";
import { toast } from "sonner";
import { asApiError } from "@/api/apiFetch";
import { messageFor } from "@/api/errorMessages";
import { revokeStudentCard, type StudentCardDto } from "@/api/generated/tutoring-centre";
import { Button } from "@/components/ui/button";
import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog";

const Reasons = ["lost", "damaged", "withdrawn"] as const;
export function RevokeCardDialog({ card, onClose, onRefresh }: { card: StudentCardDto; onClose: () => void; onRefresh: () => Promise<unknown> }) {
  const { t } = useTranslation("cards");
  const reasonId = useId();
  const [reason, setReason] = useState<(typeof Reasons)[number]>("lost");
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState<string>();
  async function revoke() {
    setBusy(true);
    setError(undefined);
    try {
      await revokeStudentCard(card.cardId, { reason, version: card.version });
      await onRefresh();
      toast.success(t("revoked"));
      onClose();
    } catch (thrown) {
      const failure = asApiError(thrown);
      if (failure.code === "concurrency.stale" || failure.code === "card.already_revoked") {
        toast.error(failure.code === "concurrency.stale" ? t("changed") : t("alreadyRevoked"));
        await onRefresh();
        onClose();
      } else setError(messageFor(failure));
    } finally { setBusy(false); }
  }
  return <Dialog open onOpenChange={(open) => { if (!open && !busy) onClose(); }}>
    <DialogContent>
      <DialogHeader><DialogTitle>{t("revoke")}</DialogTitle><DialogDescription>{t("revokeBody")}</DialogDescription></DialogHeader>
      <div className="grid gap-2 text-sm"><label htmlFor={reasonId}>{t("reason")}</label>
        <select id={reasonId} value={reason} onChange={(event) => { const value = event.target.value; if (Reasons.some((item) => item === value)) setReason(value as (typeof Reasons)[number]); }} className="rounded-md border bg-background p-2" disabled={busy}>
          {Reasons.map((item) => <option key={item} value={item}>{t(`reasons.${item}`)}</option>)}
        </select>
      </div>
      {error && <p role="alert" className="text-sm text-destructive">{error}</p>}
      <DialogFooter><Button variant="outline" onClick={onClose} disabled={busy}>{t("cancel")}</Button><Button onClick={() => { void revoke(); }} disabled={busy}>{t("revoke")}</Button></DialogFooter>
    </DialogContent>
  </Dialog>;
}
