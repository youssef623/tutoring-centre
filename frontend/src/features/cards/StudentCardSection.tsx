import { useState } from "react";
import { useTranslation } from "react-i18next";
import { asApiError } from "@/api/apiFetch";
import { messageFor } from "@/api/errorMessages";
import { issueStudentCard, useGetStudentCard, useListStudentEnrollments, type IssuedCard, type StudentDetailDto } from "@/api/generated/tutoring-centre";
import { Button } from "@/components/ui/button";
import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { useSession } from "@/features/session/useSession";
import { CardFace } from "./CardFace";
import { CardSheet } from "./CardSheet";
import { printCards } from "./printCards";
import { RevokeCardDialog } from "./RevokeCardDialog";

export function StudentCardSection({ student }: { student: StudentDetailDto }) {
  const { t, i18n } = useTranslation("cards");
  const { me } = useSession();
  const query = useGetStudentCard(student.id);
  const enrollments = useListStudentEnrollments(student.id);
  const [issued, setIssued] = useState<IssuedCard | null>(null);
  const [confirm, setConfirm] = useState(false);
  const [revoke, setRevoke] = useState(false);
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState<string>();
  const card = query.data?.card;
  const centreName = me?.memberships.find((membership) => membership.centreId === me.activeCentreId)?.centreName ?? "";
  const groupName = new Intl.ListFormat(i18n.language).format(enrollments.data?.items.filter((item) => item.status === "active").map((item) => item.groupName) ?? []);
  const date = (value: string) => new Intl.DateTimeFormat(i18n.language, { dateStyle: "medium" }).format(new Date(value));

  async function issue() {
    setConfirm(false);
    setBusy(true);
    setError(undefined);
    try {
      // Deliberately call the generated function, not a mutation hook: its secret response
      // must never enter React Query's mutation cache. Only metadata is queried/cached.
      setIssued(await issueStudentCard(student.id));
      await query.refetch();
    } catch (thrown) {
      const failure = asApiError(thrown);
      setError(failure.code === "card.concurrent_issue" ? t("concurrentIssue") : messageFor(failure));
    } finally { setBusy(false); }
  }
  const printable = issued ? { cardId: issued.cardId, token: issued.token, studentName: issued.studentName, centreName, groupName } : null;
  return <section className="space-y-4 rounded-xl border border-border bg-card p-4" aria-label={t("title")}>
    <h2 className="text-lg font-semibold">{t("title")}</h2>
    {query.isPending ? <p role="status">{t("loading")}</p> : query.error ? <>
      <p role="alert">{messageFor(asApiError(query.error))}</p><Button variant="outline" onClick={() => { void query.refetch(); }}>{t("retry")}</Button>
    </> : <>
      {!card ? <p className="text-sm text-muted-foreground">{t("none")}</p> : card.status === "active"
        ? <p className="text-sm text-muted-foreground">{t("issuedBy", { date: date(card.issuedAt), name: card.issuedByName })}</p>
        : <p className="text-sm text-muted-foreground">{t("revokedAt", { reason: t(`reasons.${card.revocationReason ?? "lost"}`), date: date(card.revokedAt ?? card.issuedAt) })}</p>}
      {error && <p role="alert" className="text-sm text-destructive">{error}</p>}
      {student.status === "archived" && <p className="text-sm text-muted-foreground">{t("archived")}</p>}
      <div className="flex flex-wrap gap-2">
        <Button disabled={busy || student.status === "archived"} onClick={() => { if (card?.status === "active") setConfirm(true); else void issue(); }}>{t(card?.status === "active" ? "reissue" : "issue")}</Button>
        {card?.status === "active" && <Button variant="outline" onClick={() => { setRevoke(true); }}>{t("revoke")}</Button>}
      </div>
    </>}
    <Dialog open={confirm} onOpenChange={setConfirm}><DialogContent>
      <DialogHeader><DialogTitle>{t("reissue")}</DialogTitle><DialogDescription>{t("reissueBody")}</DialogDescription></DialogHeader>
      <DialogFooter><Button variant="outline" onClick={() => { setConfirm(false); }}>{t("cancel")}</Button><Button onClick={() => { void issue(); }}>{t("confirmReissue")}</Button></DialogFooter>
    </DialogContent></Dialog>
    {issued && printable && <Dialog open onOpenChange={(open) => { if (!open) setIssued(null); }}><DialogContent className="sm:max-w-lg">
      <DialogHeader><DialogTitle>{t("ready")}</DialogTitle><DialogDescription>{t("oneTime")}</DialogDescription></DialogHeader>
      <div className="overflow-auto"><CardFace {...printable} /></div>
      <CardSheet cards={[printable]} screen={false} />
      <DialogFooter><Button variant="outline" onClick={() => { setIssued(null); }}>{t("close")}</Button><Button onClick={() => { void printCards(); }}>{t("print")}</Button></DialogFooter>
    </DialogContent></Dialog>}
    {revoke && card && <RevokeCardDialog card={card} onRefresh={() => query.refetch()} onClose={() => { setRevoke(false); }} />}
  </section>;
}
