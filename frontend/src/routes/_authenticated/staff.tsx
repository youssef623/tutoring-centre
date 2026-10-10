import { createFileRoute } from "@tanstack/react-router";
import { Plus } from "lucide-react";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { ChangeRoleDialog } from "@/features/staff/ChangeRoleDialog";
import { StaffFormDialog } from "@/features/staff/StaffFormDialog";
import { StaffList } from "@/features/staff/StaffList";
import type { StaffMemberDto } from "@/api/generated/tutoring-centre";
import { Button } from "@/components/ui/button";
import { Can } from "@/features/session/Can";
import { RouteGuard } from "@/features/session/RouteGuard";
import { Permissions } from "@/features/session/permissions";

export const Route = createFileRoute("/_authenticated/staff")({
  component: StaffPage,
});

function StaffPage() {
  return (
    <RouteGuard permission={Permissions.StaffView}>
      <StaffPageContent />
    </RouteGuard>
  );
}

function StaffPageContent() {
  const { t } = useTranslation("staff");
  const [createOpen, setCreateOpen] = useState(false);
  const [roleTarget, setRoleTarget] = useState<StaffMemberDto | null>(null);

  return (
    <section className="space-y-4">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <h1 className="text-2xl font-semibold tracking-tight">{t("page.title")}</h1>
        <Can permission={Permissions.StaffManage}>
          <Button
            type="button"
            onClick={() => {
              setCreateOpen(true);
            }}
          >
            <Plus className="size-4" aria-hidden="true" />
            {t("actions.create")}
          </Button>
        </Can>
      </div>

      <StaffList
        onChangeRole={(member) => {
          setRoleTarget(member);
        }}
      />

      <StaffFormDialog open={createOpen} onOpenChange={setCreateOpen} />
      <ChangeRoleDialog
        member={roleTarget}
        onOpenChange={(open) => {
          if (!open) {
            setRoleTarget(null);
          }
        }}
      />
    </section>
  );
}
