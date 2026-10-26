/**
 * Wraps a left-to-right value (a correlation ID, say) in Unicode's left-to-right isolate (U+2066 … U+2069) before it
 * is interpolated into a translated sentence. Without it, an ID that starts with digits is reordered against the
 * Arabic label around it by the bidi algorithm; with it, the ID reads as one left-to-right run and the sentence
 * keeps its own direction. Plain text, so it works inside any `t()` call; the audit page's `<span dir="ltr">` is the
 * same idea for markup.
 */
export function isolateLtr(value: string): string {
  return `\u2066${value}\u2069`;
}
