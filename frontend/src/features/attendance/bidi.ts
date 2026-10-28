/**
 * Times and counts keep their own left-to-right order inside right-to-left sentences. A value passed into a
 * translated sentence can't carry markup, so it is wrapped in Unicode isolates instead: "٣:٣٠ م" stays in one
 * piece however the surrounding Arabic wraps or reorders.
 */
export function isolateLtr(text: string): string {
  return `⁦${text}⁩`;
}
