# `docs/onboarding-work/normalize_docs.py`

## Purpose

Current script file. The source excerpt and structural map below expose its content without inferring behaviour from its filename.

## Where it fits

Category: **script**. [Open current file](../../../onboarding-work/normalize_docs.py). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 14 lines and 774 bytes. Source hash: `ccac35837e8698393c96c83bce305d075cb45bb5c54bf193bb632b1a5587d051`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Read the data or directives in order; there are no recognized code declarations in this file.

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```python
"""Normalize only this task's documentation outputs to repository LF conventions."""
from pathlib import Path

ROOT=Path(__file__).resolve().parents[2]
files=[ROOT/'docs/PROJECT_OVERVIEW3.md']
for directory in ('docs/file-explanations','docs/onboarding-work'):
    for p in (ROOT/directory).rglob('*'):
        if any(x in {'.mermaid-tools','node_modules','__pycache__','generated-model-packets'} for x in p.parts): continue
        if p.is_file() and (p.suffix in {'.md','.json','.py','.mjs'} or p.name=='.gitignore'): files.append(p)
for p in files:
    text=p.read_text(encoding='utf-8-sig')
    cleaned='\n'.join(line.rstrip() for line in text.splitlines())+'\n'
    p.write_text(cleaned,encoding='utf8',newline='\n')
print('Normalized documentation files:',len(files))
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
