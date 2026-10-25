import { UserPlus, UsersRound } from "lucide-react";
import { useMemo, useState } from "react";
import { useTranslation } from "react-i18next";
import { asApiError } from "@/api/apiFetch";
import { messageFor } from "@/api/errorMessages";
import { useGetGroupRoster, type GroupDto, type RosterEntryDto } from "@/api/generated/tutoring-centre";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import { Skeleton } from "@/components/ui/skeleton";
import { Tooltip, TooltipContent, TooltipTrigger } from "@/components/ui/tooltip";
import { EnrollmentRowMenu } from "@/features/enrollments/EnrollmentRowMenu";
import { EnrollStudentDialog } from "@/features/enrollments/EnrollStudentDialog";
import { RosterTable } from "@/features/enrollments/RosterTable";
import { SeatMeter } from "@/features/enrollments/SeatMeter";
import { TransferDialog } from "@/features/enrollments/TransferDialog";
import { WithdrawEnrollmentDialog, type EnrollmentTarget } from "@/features/enrollments/WithdrawEnrollmentDialog";
import { Can } from "@/features/session/Can";
import { Permissions } from "@/features/session/permissions";
import { useCan } from "@/features/session/useCan";
import { isolateLtr } from "@/lib/bidi";

/**
 * One view, three permission levels: anyone with `enrollments.view` (including the group's own teacher) sees the
 * roster; `enrollments.manage` adds the actions; `students.view` turns names into links. The view is never hidden,
 * only the controls are gated.
 */
export function GroupRosterTab({ group }: { group: GroupDto }) {
  const { t } = useTranslation("enrollments");
  const { t: tCommon } = useTranslation("common");
  const linkStudents = useCan(Permissions.StudentsView);
  const canManage = useCan(Permissions.EnrollmentsManage);
  const { data, error, isPending, refetch } = useGetGroupRoster(group.id);
  const [enrollOpen, setEnrollOpen] = useState(false);
  const [withdrawTarget, setWithdrawTarget] = useState<EnrollmentTarget | null>(null);
  const [transferTarget, setTransferTarget] = useState<EnrollmentTarget | null>(null);
  const enrolledStudentIds = useMemo(() => new Set(data?.items.map((entry) => entry.studentId)), [data]);

  if (isPending) {
    return (
      <div role="status" aria-label={t("roster.loadingLabel")} className="space-y-2">
        <Skeleton className="h-12 w-48" />
        <Skeleton className="h-10 w-full" />
        <Skeleton className="h-10 w-full" />
        <Skeleton className="h-10 w-full" />
      </div>
    );
  }

  if (error) {
    const apiError = asApiError(error);

    return (
      <Alert variant="destructive">
        <AlertTitle>{t("roster.errorTitle")}</AlertTitle>
        <AlertDescription className="flex flex-col gap-2">
          <p>{messageFor(apiError)}</p>
          {apiError.correlationId !== undefined && (
            <p>{t("roster.errorReference", { correlationId: isolateLtr(apiError.correlationId) })}</p>
          )}
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
    );
  }

  const enrolled = Number(data.enrolledCount);
  const capacity = Number(data.capacity);
  const isFull = capacity > 0 && enrolled >= capacity;
  const isArchived = group.status === "archived";
  const toTarget = (entry: RosterEntryDto): EnrollmentTarget => ({
    enrollmentId: entry.enrollmentId,
    version: entry.version,
    studentId: entry.studentId,
    studentName: entry.studentName,
    groupId: group.id,
    groupName: group.name,
  });
  const enrollButton = (
    <EnrollButton
      disabledReason={isArchived ? "archived" : isFull ? "full" : null}
      onClick={() => {
        setEnrollOpen(true);
      }}
    />
  );

  return (
    <div className="space-y-4">
      <div className="flex flex-wrap items-end justify-between gap-4">
        <div className="w-full max-w-xs">
          <SeatMeter enrolled={enrolled} capacity={capacity} size="full" />
        </div>
        {data.items.length > 0 && <Can permission={Permissions.EnrollmentsManage}>{enrollButton}</Can>}
      </div>

      {data.items.length === 0 ? (
        <div className="flex flex-col items-center gap-2 rounded-xl border border-dashed border-border px-6 py-10 text-center">
          <UsersRound className="size-8 text-muted-foreground" aria-hidden="true" />
          <p className="text-sm font-semibold">{t("roster.emptyTitle")}</p>
          <p className="max-w-sm text-sm text-muted-foreground">{t("roster.emptyDescription")}</p>
          <Can permission={Permissions.EnrollmentsManage}>
            <div className="pt-2">{enrollButton}</div>
          </Can>
        </div>
      ) : (
        <RosterTable
          items={data.items}
          linkStudents={linkStudents}
          rowMenu={
            canManage
              ? (entry) => (
                  <EnrollmentRowMenu
                    label={entry.studentName}
                    onTransfer={() => {
                      setTransferTarget(toTarget(entry));
                    }}
                    onWithdraw={() => {
                      setWithdrawTarget(toTarget(entry));
                    }}
                  />
                )
              : undefined
          }
        />
      )}

      <Can permission={Permissions.EnrollmentsManage}>
        <EnrollStudentDialog
          group={group}
          open={enrollOpen}
          onOpenChange={setEnrollOpen}
          enrolledStudentIds={enrolledStudentIds}
        />
        <WithdrawEnrollmentDialog
          target={withdrawTarget}
          onClose={() => {
            setWithdrawTarget(null);
          }}
        />
        <TransferDialog
          target={transferTarget}
          subjectId={group.subjectId}
          onClose={() => {
            setTransferTarget(null);
          }}
        />
      </Can>
    </div>
  );
}

/**
 * Disabled at capacity or when archived, as a courtesy only: the server is the capacity control, and its 409
 * is handled as a normal outcome. The button stays focusable while disabled so the reason can be read, both in
 * the tooltip and through `aria-describedby`.
 */
function EnrollButton({
  disabledReason,
  onClick,
}: {
  disabledReason: "full" | "archived" | null;
  onClick: () => void;
}) {
  const { t } = useTranslation("enrollments");

  if (disabledReason === null) {
    return (
      <Button type="button" onClick={onClick}>
        <UserPlus className="size-4" aria-hidden="true" />
        {t("actions.enroll")}
      </Button>
    );
  }

  const reasonId = `enroll-disabled-${disabledReason}`;

  return (
    <Tooltip>
      <TooltipTrigger
        render={
          <Button
            type="button"
            disabled
            focusableWhenDisabled
            aria-describedby={reasonId}
            className="cursor-not-allowed opacity-50"
          />
        }
      >
        <UserPlus className="size-4" aria-hidden="true" />
        {t("actions.enroll")}
      </TooltipTrigger>
      <TooltipContent>{t(`enrollDisabled.${disabledReason}`)}</TooltipContent>
      <span id={reasonId} className="sr-only">
        {t(`enrollDisabled.${disabledReason}`)}
      </span>
    </Tooltip>
  );
}
