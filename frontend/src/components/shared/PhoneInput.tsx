import { Input } from "@/components/ui/input";

/**
 * A telephone input that is always left-to-right, even inside an Arabic form: a phone number has its own
 * reading direction regardless of the surrounding page's, and without `dir="ltr"` the bidi algorithm reorders
 * its digits and leading "+" in an RTL document. Carries no formatting or validation of its own beyond what
 * the browser's own `type="tel"` offers — the server owns the real format (Day 41).
 */
export function PhoneInput({
  id,
  value,
  onChange,
  placeholder,
  required,
}: {
  id?: string;
  value: string;
  onChange: (value: string) => void;
  placeholder?: string;
  required?: boolean;
}) {
  return (
    <Input
      id={id}
      type="tel"
      dir="ltr"
      className="text-start"
      value={value}
      placeholder={placeholder}
      required={required}
      onChange={(event) => {
        onChange(event.target.value);
      }}
    />
  );
}
