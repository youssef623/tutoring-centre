/** First letter of up to the first two words, for avatar glyphs. Falls back to "?" for an empty name. */
export function initialsOf(name: string): string {
  const initials = name
    .trim()
    .split(/\s+/)
    .slice(0, 2)
    .map((word) => word.charAt(0).toUpperCase())
    .join("");

  return initials === "" ? "?" : initials;
}
