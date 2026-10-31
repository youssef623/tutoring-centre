import { Atom, BookOpen, FlaskConical, Radical, Languages } from "lucide-react";

/** A visual mnemonic derived from the actual subject name, with a neutral book fallback. */
export function SubjectGlyph({ name }: { name: string }) {
  const value = name.toLocaleLowerCase();
  const Icon = /math|رياض|جبر|هندس/.test(value)
    ? Radical
    : /phys|فيزي/.test(value)
      ? Atom
      : /chem|كيمي/.test(value)
        ? FlaskConical
        : /english|عربي|لغة/.test(value)
          ? Languages
          : BookOpen;
  return (
    <span className="subject-glyph" aria-hidden="true">
      <Icon />
    </span>
  );
}
