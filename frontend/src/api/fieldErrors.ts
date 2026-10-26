import i18next from "@/i18n";
import { messageFor } from "./errorMessages";
import type { ApiError } from "./errors";

type SetFieldError<TField extends string> = (field: TField, option: { type: string; message: string }) => void;

/** True when errors.json has a message for this exact code (not just its kind or the generic fallback). */
function hasCodeMessage(error: ApiError): boolean {
  return i18next.t(`byCode.${error.code}`, { ns: "errors", defaultValue: "" }) !== "";
}

/**
 * Like `applyFieldErrors`, but the message under each field is the translated message for the error's code
 * (e.g. `group.name_taken`, `group.capacity_invalid`) whenever errors.json has one, so an Arabic session sees
 * Arabic under the field; the server's own field text is only the fallback. The `type` stays "server", so a
 * form renders `message` as-is rather than treating it as an i18n key.
 * @returns true when at least one known field received an error.
 */
export function applyTranslatedFieldErrors<TField extends string>(
  error: ApiError,
  setError: SetFieldError<TField>,
  knownFields: readonly TField[],
): boolean {
  if (error.fieldErrors === undefined) {
    return false;
  }

  const translated = hasCodeMessage(error) ? messageFor(error) : undefined;
  let applied = false;

  for (const [field, messages] of Object.entries(error.fieldErrors)) {
    const message = translated ?? messages.at(0);
    if (message !== undefined && (knownFields as readonly string[]).includes(field)) {
      setError(field as TField, { type: "server", message });
      applied = true;
    }
  }

  return applied;
}

/** Puts the translated message for a coded refusal (one the server reports without a field) under a chosen field. */
export function setCodedFieldError<TField extends string>(error: ApiError, setError: SetFieldError<TField>, field: TField): void {
  setError(field, { type: "server", message: messageFor(error) });
}
