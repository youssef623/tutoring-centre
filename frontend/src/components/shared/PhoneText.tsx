/**
 * Renders a phone number isolated as left-to-right text (Day 41): without `dir="ltr"`, a value like
 * "+201012345678" sitting inside Arabic text gets its leading "+" reordered to the wrong end by the bidi
 * algorithm, because the surrounding paragraph's direction, not the number's own, decides how an undirected
 * run of digits and punctuation is laid out. `asLink` renders a `tel:` link for a phone meant to be called
 * directly (a parent's contact number); plain text otherwise (a value shown for reference only).
 */
export function PhoneText({ value, asLink = false }: { value: string; asLink?: boolean }) {
  if (asLink) {
    return (
      <a href={`tel:${value}`} dir="ltr" className="inline-block underline-offset-2 hover:underline">
        {value}
      </a>
    );
  }

  return <span dir="ltr">{value}</span>;
}
