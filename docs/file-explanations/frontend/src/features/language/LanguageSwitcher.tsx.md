# `frontend/src/features/language/LanguageSwitcher.tsx`

## Purpose

Source comment (intent): Toggles between Arabic and English. The i18n module (@/i18n) persists the choice and flips document direction.

## Where it fits

Category: **source**. [Open current file](../../../../../../frontend/src/features/language/LanguageSwitcher.tsx). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 18 lines and 596 bytes. Source hash: `45edd734046854330555e14a0b84947b62e21f6f112d0c77fda17d8696fbc2b4`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import { useTranslation } from "react-i18next";`
- Line 2: `import { Button } from "@/components/ui/button";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 5](../../../../../../frontend/src/features/language/LanguageSwitcher.tsx#L5): `export function LanguageSwitcher() {`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/features/language/LanguageSwitcher.test.tsx:4](../../../../../../frontend/src/features/language/LanguageSwitcher.test.tsx#L4)
- [frontend/src/features/shell/AppShell.tsx:23](../../../../../../frontend/src/features/shell/AppShell.tsx#L23)
- [frontend/src/routes/change-password.tsx:17](../../../../../../frontend/src/routes/change-password.tsx#L17)
- [frontend/src/routes/login.tsx:16](../../../../../../frontend/src/routes/login.tsx#L16)
- [frontend/src/routes/select-centre.tsx:26](../../../../../../frontend/src/routes/select-centre.tsx#L26)
- [frontend/src/routes/status.tsx:2](../../../../../../frontend/src/routes/status.tsx#L2)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```tsx
import { useTranslation } from "react-i18next";
import { Button } from "@/components/ui/button";

/** Toggles between Arabic and English. The i18n module (@/i18n) persists the choice and flips document direction. */
export function LanguageSwitcher() {
  const { i18n, t } = useTranslation("common");
  const isArabic = i18n.language === "ar";

  const toggle = () => {
    void i18n.changeLanguage(isArabic ? "en" : "ar");
  };

  return (
    <Button type="button" variant="ghost" size="sm" onClick={toggle}>
      {isArabic ? t("language.english") : t("language.arabic")}
    </Button>
  );
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
