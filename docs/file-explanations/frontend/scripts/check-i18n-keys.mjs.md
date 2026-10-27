# `frontend/scripts/check-i18n-keys.mjs`

## Purpose

Declares `flattenKeys`, `loadNamespace`, `namespacesFor`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **script**. [Open current file](../../../../frontend/scripts/check-i18n-keys.mjs). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 65 lines and 2,393 bytes. Source hash: `db9193dff43feb0ff8c42c3ed00612be6312fcccf0208948aa7c8679c12fbd10`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 5: `import { readFileSync, readdirSync } from "node:fs";`
- Line 6: `import { fileURLToPath } from "node:url";`
- Line 7: `import path from "node:path";`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 13](../../../../frontend/scripts/check-i18n-keys.mjs#L13): `function flattenKeys(value, prefix = "") {`
- [Line 23](../../../../frontend/scripts/check-i18n-keys.mjs#L23): `function loadNamespace(language, namespace) {`
- [Line 29](../../../../frontend/scripts/check-i18n-keys.mjs#L29): `function namespacesFor(language) {`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```text
#!/usr/bin/env node
// Fails CI when the Arabic and English translation keys differ for any namespace (Day 17, Task 17.9).
// Plain Node, no dependencies: the locale files are already the single source of truth.

import { readFileSync, readdirSync } from "node:fs";
import { fileURLToPath } from "node:url";
import path from "node:path";

const scriptDir = path.dirname(fileURLToPath(import.meta.url));
const localesDir = path.join(scriptDir, "..", "src", "i18n", "locales");
const Languages = ["en", "ar"];

function flattenKeys(value, prefix = "") {
  if (value === null || typeof value !== "object" || Array.isArray(value)) {
    return [prefix];
  }

  return Object.entries(value).flatMap(([key, child]) =>
    flattenKeys(child, prefix === "" ? key : `${prefix}.${key}`),
  );
}

function loadNamespace(language, namespace) {
  const filePath = path.join(localesDir, language, namespace);
  const contents = readFileSync(filePath, "utf8");
  return JSON.parse(contents);
}

function namespacesFor(language) {
  return readdirSync(path.join(localesDir, language)).filter((name) => name.endsWith(".json"));
}

const namespacesByLanguage = new Map(Languages.map((language) => [language, new Set(namespacesFor(language))]));
const allNamespaces = new Set(Languages.flatMap((language) => [...namespacesByLanguage.get(language)]));

let hasError = false;

for (const namespace of [...allNamespaces].sort()) {
  const missingNamespaceIn = Languages.filter((language) => !namespacesByLanguage.get(language).has(namespace));
  if (missingNamespaceIn.length > 0) {
    hasError = true;
    console.error(`${namespace}: missing entirely in ${missingNamespaceIn.join(", ")}`);
    continue;
  }

  const keysByLanguage = new Map(
    Languages.map((language) => [language, new Set(flattenKeys(loadNamespace(language, namespace)))]),
  );

  for (const language of Languages) {
    const otherLanguage = Languages.find((candidate) => candidate !== language);
    const missing = [...keysByLanguage.get(language)].filter((key) => !keysByLanguage.get(otherLanguage).has(key));

    if (missing.length > 0) {
      hasError = true;
      console.error(`${namespace}: present in ${language} but missing in ${otherLanguage}: ${missing.join(", ")}`);
    }
  }
}

if (hasError) {
  process.exit(1);
}

console.log(`i18n:check passed — ${allNamespaces.size} namespace(s), keys match in both languages.`);
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
