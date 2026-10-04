import i18next from "@/i18n";
import type { ApiError } from "./errors";

const Namespace = "errors";

/** The one function every form and toast uses to turn an ApiError into user-facing text. */
export function messageFor(error: ApiError): string {
  const byCode = i18next.t(`byCode.${error.code}`, { ns: Namespace, defaultValue: "" });
  if (byCode !== "") {
    return byCode;
  }

  const byKind = i18next.t(`byKind.${error.kind}`, { ns: Namespace, defaultValue: "" });
  if (byKind !== "") {
    return byKind;
  }

  return i18next.t("generic", { ns: Namespace });
}
