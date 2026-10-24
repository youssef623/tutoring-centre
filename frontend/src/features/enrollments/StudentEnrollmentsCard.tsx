import { Link } from "@tanstack/react-router";
import { UserPlus } from "lucide-react";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { asApiError } from "@/api/apiFetch";
import { messageFor } from "@/api/errorMessages";
import { useListStudentEnrollments, type StudentEnrollmentDto } from "@/api/generated/tutoring-centre";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import { Skeleton } from "@/components/ui/skeleton";
import { EnrollInGroupDialog } from "@/features/enrollments/EnrollInGroupDialog";
import { EnrollmentRowMenu } from "@/features/enrollments/EnrollmentRowMenu";
import { TransferDialog } from "@/features/enrollments/TransferDialog";
import { WithdrawEnrollmentDialog, type EnrollmentTarget } from "@/features/enrollments/WithdrawEnrollmentDialog";
import { Permissions } from "@/features/session/permissions";
import { useCan } from "@/features/session/useCan";

/**
 * A student's groups on their own page: active enrollments first with their actions, then ended ones, muted, with
 * the end date and reason. The caller renders this only with `enrollments.view`, so without it no enrollment
 * request is sent. Enroll, withdraw and transfer are the same components and mutation as the roster tab.
 */
export function StudentEnrollmentsCard({
  student,
  canEnroll,
}: {
  student: { id: string; fullName: string };
  canEnroll: boolean;
}) {
  const { t, i18n } = useTranslation("enrollments");
  const { t: tCommon } = useTranslation("common");
  const canManage = useCan(Permissions.EnrollmentsManage);
  const linkGroups = useCan(Permissions.GroupsView);
  const { data, error, isPending, refetch } = useListStudentEnrollments(student.id);
  const [enrollOpen, setEnrollOpen] = useState(false);
  const [withdrawTarget, setWithdrawTarget] = useState<EnrollmentTarget | null>(null);
  const [transferTarget, setTransferTarget] = useState<EnrollmentTarget | null>(null);
  const items = data?.items ?? [];
  const active = items.filter((item) => item.status === "active");
  const ended = items.filter((item) => item.status === "ended");
  const activeGroupIds = new Set(active.map((item) => item.groupId));
  const dateFormat = new Intl.DateTimeFormat(i18n.language, { dateStyle: "medium" });

  const toTarget = (item: StudentEnrollmentDto): EnrollmentTarget => ({
    enrollmentId: item.enrollmentId,
    version: item.version,
    studentId: student.id,
    studentName: student.fullName,
    groupId: item.groupId,
    groupName: item.groupName,
  });

  const groupName = (item: StudentEnrollmentDto) =>
    linkGroups ? (
      <Link
        to="/groups/$groupId"
        params={{ groupId: item.groupId }}
        search={{ tab: "roster" }}
        className="font-medium hover:underline"
        dir="auto"
      >
        {item.groupName}
      </Link>
    ) : (
      <span className="font-medium" dir="auto">
        {item.groupName}
      </span>
    );

  return (
    <section aria-labelledby="student-enrollments-title" className="space-y-3 rounded-xl border border-border p-4">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <h2 id="student-enrollments-title" className="text-base font-semibold">
          {t("studentCard.title")}
        </h2>
        {canManage && canEnroll && (
          <Button
            type="button"
            variant="outline"
            size="sm"
            onClick={() => {
              setEnrollOpen(true);
            }}
          >
            <UserPlus className="size-4" aria-hidden="true" />
            {t("studentCard.enroll")}
          </Button>
        )}
      </div>

      {isPending ? (
        <div role="status" aria-label={t("studentCard.loadingLabel")} className="space-y-2">
          <Skeleton className="h-12 w-full" />
          <Skeleton className="h-12 w-full" />
        </div>
      ) : error ? (
        <Alert variant="destructive">
          <AlertTitle>{t("studentCard.errorTitle")}</AlertTitle>
          <AlertDescription className="flex flex-col gap-2">
            <p>{messageFor(asApiError(error))}</p>
            <Button
              type="button"
              variant="outline"
              size="sm"
              className="self-start"
              onClick={() => {
                void refetch();
              }}
            >
              {tCommon("retry")}
            </Button>
          </AlertDescription>
        </Alert>
      ) : items.length === 0 ? (
        <p className="rounded-lg border border-dashed border-border px-3 py-6 text-center text-sm text-muted-foreground">
          {t("studentCard.empty")}
        </p>
      ) : (
        <>
          {active.length > 0 && (
            <ul className="divide-y divide-border rounded-lg border border-border" aria-label={t("studentCard.current")}>
              {active.map((item) => (
                <li key={item.enrollmentId} className="flex items-center gap-3 px-3 py-2 text-sm">
                  <div className="min-w-0 flex-1 space-y-0.5">
                    {groupName(item)}
                    <p className="text-xs text-muted-foreground">
                      <span dir="auto">{item.subjectName}</span>
                      {" · "}
                      <span dir="auto">{item.teacherName}</span>
                      {" · "}
                      {t("studentCard.since", { date: dateFormat.format(new Date(item.enrolledAt)) })}
                    </p>
                  </div>
                  {canManage && (
                    <EnrollmentRowMenu
                      label={item.groupName}
                      onTransfer={() => {
                        setTransferTarget(toTarget(item));
                      }}
                      onWithdraw={() => {
                        setWithdrawTarget(toTarget(item));
                      }}
                    />
                  )}
                </li>
              ))}
            </ul>
          )}

          {ended.length > 0 && (
            <div className="space-y-2">
              <h3 className="text-sm font-medium text-muted-foreground">{t("studentCard.past")}</h3>
              <ul className="divide-y divide-border rounded-lg border border-border text-muted-foreground">
                {ended.map((item) => (
                  <li key={item.enrollmentId} className="space-y-0.5 px-3 py-2 text-sm">
                    <p>
                      <span dir="auto">{item.groupName}</span>
                    </p>
                    <p className="text-xs">
                      <span dir="auto">{item.subjectName}</span>
                      {" · "}
                      {t("studentCard.ended", {
                        date: item.endedAt === null ? "—" : dateFormat.format(new Date(item.endedAt)),
                        reason: item.endReason === null ? "" : t(`endReasons.${item.endReason}`),
                      })}
                    </p>
                  </li>
                ))}
              </ul>
            </div>
          )}
        </>
      )}

      {canManage && (
        <>
          <EnrollInGroupDialog
            student={student}
            open={enrollOpen}
            onOpenChange={setEnrollOpen}
            activeGroupIds={activeGroupIds}
          />
          <WithdrawEnrollmentDialog
            target={withdrawTarget}
            onClose={() => {
              setWithdrawTarget(null);
            }}
          />
          <TransferDialog
            target={transferTarget}
            onClose={() => {
              setTransferTarget(null);
            }}
          />
        </>
      )}
    </section>
  );
}
