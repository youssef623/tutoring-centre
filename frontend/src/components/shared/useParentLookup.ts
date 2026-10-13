import { useRef, useState } from "react";
import { asApiError } from "@/api/apiFetch";
import { showApiError } from "@/api/showApiError";
import { useLookupParent, type ParentDto } from "@/api/generated/tutoring-centre";

export type ParentLookupOutcome = { ok: true; parent: ParentDto | null } | { ok: false };

/**
 * Owns the parent-lookup mutation and its result, independent of any single form: the registration form
 * and the parents card's "Add parent" dialog each drive their own instance. A result is only ever read
 * against the exact phone it was looked up for (`lookedUpPhoneRef`) — changing the field clears it immediately
 * (`onPhoneChange`), and a response that resolves after the field has moved on to a different number
 * (`latestPhoneRef` no longer matches) is discarded rather than applied, so a later, different number never
 * reuses an earlier lookup's result. The lookup itself is a mutation (Day 42 rules), never a cached query, so
 * the phone never sits in a query-cache key.
 */
export function useParentLookup() {
  const mutation = useLookupParent();
  const [found, setFound] = useState<ParentDto | null>(null);
  const [fieldError, setFieldError] = useState<string | null>(null);
  const lookedUpPhoneRef = useRef<string | null>(null);
  const latestPhoneRef = useRef("");

  const reset = () => {
    setFound(null);
    setFieldError(null);
    lookedUpPhoneRef.current = null;
  };

  const onPhoneChange = (nextPhone: string) => {
    latestPhoneRef.current = nextPhone;
    if (nextPhone !== lookedUpPhoneRef.current) {
      setFound(null);
      setFieldError(null);
    }
  };

  const ensureLookup = async (phone: string, options?: { force?: boolean }): Promise<ParentLookupOutcome> => {
    if (phone.trim() === "") {
      reset();
      return { ok: true, parent: null };
    }

    if (options?.force !== true && lookedUpPhoneRef.current === phone) {
      return { ok: true, parent: found };
    }

    setFieldError(null);
    try {
      const result = await mutation.mutateAsync({ data: { phone } });
      if (latestPhoneRef.current !== phone) {
        return { ok: true, parent: null };
      }

      lookedUpPhoneRef.current = phone;
      setFound(result.parent);
      return { ok: true, parent: result.parent };
    } catch (thrown) {
      if (latestPhoneRef.current !== phone) {
        return { ok: false };
      }

      const apiError = asApiError(thrown);
      const message = apiError.fieldErrors?.phone?.at(0);
      if (message !== undefined) {
        setFieldError(message);
      } else {
        showApiError(apiError);
      }

      return { ok: false };
    }
  };

  return { found, fieldError, pending: mutation.isPending, ensureLookup, onPhoneChange, reset };
}
