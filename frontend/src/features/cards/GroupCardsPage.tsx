import { useState } from "react";
import { useQueryClient } from "@tanstack/react-query";
import { useTranslation } from "react-i18next";
import { asApiError } from "@/api/apiFetch";
import { messageFor } from "@/api/errorMessages";
import { getGetStudentCardQueryKey, issueGroupCards, useGetGroup, useGetGroupRoster, type IssuedGroupCards } from "@/api/generated/tutoring-centre";
import { Button } from "@/components/ui/button";
import { useSession } from "@/features/session/useSession";
import { CardSheet } from "./CardSheet";
import { printCards } from "./printCards";

export function GroupCardsPage({ groupId }: { groupId: string }) {
  const { t } = useTranslation("cards");
  const { t: groups } = useTranslation("groups");
  const { me } = useSession();
  const queryClient = useQueryClient();
  const group = useGetGroup(groupId);
  const roster = useGetGroupRoster(groupId, { query: { enabled: !!group.data } });
  const [selected, setSelected] = useState<Set<string> | null>(null);
  const [confirmed, setConfirmed] = useState(false);
  const [issued, setIssued] = useState<IssuedGroupCards | null>(null);
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState<string>();
  const items = roster.data?.items ?? [];
  const ids = items.filter((item) => selected === null || selected.has(item.studentId)).map((item) => item.studentId);
  const centreName = me?.memberships.find((membership) => membership.centreId === me.activeCentreId)?.centreName ?? "";
  async function issue() {
    if (!confirmed || ids.length === 0 || ids.length > 60) return;
    setBusy(true);
    setError(undefined);
    try {
      // Keep the one-time response out of both query and mutation caches.
      setIssued(await issueGroupCards(groupId, { studentIds: ids }));
      await Promise.all(ids.map((id) => queryClient.invalidateQueries({ queryKey: getGetStudentCardQueryKey(id) })));
    } catch (thrown) {
      const failure = asApiError(thrown);
      if (failure.code === "card.student_not_in_group") {
        await roster.refetch();
        setSelected(null);
        setError(t("studentNotInGroup"));
      } else setError(failure.code === "card.concurrent_issue" ? t("concurrentIssue") : messageFor(failure));
    } finally { setBusy(false); }
  }
  if (group.error) return <div role="alert"><h1 className="text-xl font-semibold">{asApiError(group.error).kind === "notFound" ? groups("notFound.title") : messageFor(asApiError(group.error))}</h1></div>;
  if (group.isPending || roster.isPending) return <p role="status">{t("loading")}</p>;
  if (roster.error) return <div role="alert">{messageFor(asApiError(roster.error))}<Button onClick={() => { void roster.refetch(); }}>{t("retry")}</Button></div>;
  return <section className="space-y-5">
    <div><h1 className="text-2xl font-semibold">{t("printCards")}</h1><p dir="auto" className="text-muted-foreground">{group.data.name}</p></div>
    {issued ? <>
      <p>{t("oneTime")}</p>
      <Button onClick={() => { void printCards(); }}>{t("print")}</Button>
      <CardSheet cards={issued.cards.map((card) => ({ cardId: card.cardId, token: card.token, studentName: card.studentName, groupName: issued.groupName, centreName }))} />
    </> : <div className="max-w-2xl space-y-4 rounded-xl border bg-card p-5">
      <p className="text-sm text-muted-foreground">{t("selectionHelp")}</p>
      <div className="flex flex-wrap items-center gap-2">
        <Button variant="outline" disabled={busy} onClick={() => { setSelected(null); }}>{t("selectAll")}</Button>
        <Button variant="outline" disabled={busy} onClick={() => { setSelected(new Set()); }}>{t("selectNone")}</Button>
        <p role="status" className="ms-auto text-sm">{t("selected", { count: ids.length })}</p>
      </div>
      <fieldset disabled={busy} className="divide-y rounded-lg border">
        <legend className="sr-only">{t("students")}</legend>
        {items.map((item) => <label key={item.studentId} className="flex cursor-pointer items-center gap-3 p-3">
          <input type="checkbox" checked={ids.includes(item.studentId)} onChange={(event) => {
            const next = new Set(ids); if (event.target.checked) next.add(item.studentId); else next.delete(item.studentId); setSelected(next);
          }} /><span dir="auto">{item.studentName}</span>
        </label>)}
      </fieldset>
      {ids.length > 60 && <p role="alert" className="text-sm text-destructive">{t("limit")}</p>}
      <label className="flex items-start gap-3 text-sm"><input type="checkbox" checked={confirmed} onChange={(event) => { setConfirmed(event.target.checked); }} disabled={busy} /><span>{t("bulkConfirm")}</span></label>
      {error && <p role="alert" className="text-sm text-destructive">{error}</p>}
      <Button disabled={busy || !confirmed || ids.length === 0 || ids.length > 60} onClick={() => { void issue(); }}>{t("issueSelected")}</Button>
    </div>}
  </section>;
}
