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
