import { toast } from "sonner";
import { messageFor, type Lang } from "./errorMessages";
import type { ApiError } from "./errors";

/** Shows a toast with the translated message; the correlation reference lets a user report an error that can be found in the logs. */
export function showApiError(error: ApiError, lang: Lang): void {
  const options =
    error.correlationId === undefined ? {} : { description: `Reference: ${error.correlationId}` };

  toast.error(messageFor(error, lang), options);
}

/**
 * Maps server field errors onto form fields. Only fields the form knows about are applied (unknown keys are ignored);
 * the first message per field is used. Compatible with React Hook Form's setError (Day 17).
 */
export function applyFieldErrors<TField extends string>(
  error: ApiError,
  setError: (field: TField, option: { type: string; message: string }) => void,
  knownFields: readonly TField[],
): void {
  if (error.fieldErrors === undefined) {
    return;
  }

  const isKnownField = (field: string): field is TField =>
    (knownFields as readonly string[]).includes(field);

  for (const [field, messages] of Object.entries(error.fieldErrors)) {
    const message = messages.at(0);
    if (message !== undefined && isKnownField(field)) {
      setError(field, { type: "server", message });
    }
  }
}
