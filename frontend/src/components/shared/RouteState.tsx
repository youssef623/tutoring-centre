import { Link } from "@tanstack/react-router";
import { Compass, RefreshCw, TriangleAlert } from "lucide-react";
import { useTranslation } from "react-i18next";
import { HessaWordmark } from "@/components/brand/HessaLogo";
import { Button } from "@/components/ui/button";
import { buttonVariants } from "@/components/ui/button-variants";

export function MissingPage() {
  const { t } = useTranslation("common");
  return (
    <RouteState missing title={t("route.notFound")} description={t("route.notFoundDescription")} />
  );
}

export function FailedPage({ reset }: { reset: () => void }) {
  const { t } = useTranslation("common");
  return (
    <RouteState title={t("route.error")} description={t("route.errorDescription")} reset={reset} />
  );
}

function RouteState({
  title,
  description,
  missing = false,
  reset,
}: {
  title: string;
  description: string;
  missing?: boolean;
  reset?: () => void;
}) {
  const { t } = useTranslation("common");
  const Icon = missing ? Compass : TriangleAlert;
  return (
    <main className="auth-utility flex min-h-dvh flex-col p-6 sm:p-10">
      <HessaWordmark />
      <section className="m-auto flex max-w-md flex-col items-center gap-5 py-16 text-center">
        <span className="grid size-16 place-items-center rounded-2xl bg-accent text-primary">
          <Icon className="size-8" aria-hidden="true" />
        </span>
        <h1 className="text-3xl font-semibold tracking-tight">{title}</h1>
        <p className="text-muted-foreground">{description}</p>
        <div className="flex flex-wrap justify-center gap-3">
          {reset && (
            <Button onClick={reset}>
              <RefreshCw className="size-4" aria-hidden="true" />
              {t("retry")}
            </Button>
          )}
          <Link to="/" className={buttonVariants({ variant: "outline" })}>
            {t("route.home")}
          </Link>
        </div>
      </section>
    </main>
  );
}
