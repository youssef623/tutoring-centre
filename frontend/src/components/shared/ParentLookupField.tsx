import { Loader2 } from "lucide-react";
import { Label } from "@/components/ui/label";
import { PhoneInput } from "@/components/shared/PhoneInput";
import type { useParentLookup } from "@/components/shared/useParentLookup";

/**
 * The phone input half of the parent lookup: a label, a left-to-right phone field, and whatever field error
 * applies (a lookup-time `phone.invalid` takes priority over a submit-time error passed in from the form).
 * It renders no panel of its own — what happens once a parent is found or not is each form's own layout
 * (registration puts relationship between the phone and the branch; a dialog may differ), so that stays with
 * the caller, which reads `lookup.found` after this field.
 */
export function ParentLookupField({
  id,
  label,
  value,
  onChange,
  onBlur,
  disabled,
  error,
  lookup,
}: {
  id: string;
  label: string;
  value: string;
  onChange: (value: string) => void;
  onBlur?: () => void;
  disabled?: boolean;
  error?: string;
  lookup: ReturnType<typeof useParentLookup>;
}) {
  const displayError = lookup.fieldError ?? error;

  return (
    <div className="space-y-1.5">
      <Label htmlFor={id} className="gap-1.5">
        {label}
        {lookup.pending && <Loader2 className="size-3.5 animate-spin text-muted-foreground" aria-hidden="true" />}
      </Label>
      <PhoneInput
        id={id}
        value={value}
        disabled={disabled}
        aria-invalid={displayError !== undefined}
        aria-busy={lookup.pending}
        onChange={(next) => {
          lookup.onPhoneChange(next);
          onChange(next);
        }}
        onBlur={() => {
          onBlur?.();
          void lookup.ensureLookup(value);
        }}
      />
      {displayError !== undefined && <p className="text-sm text-destructive">{displayError}</p>}
    </div>
  );
}
