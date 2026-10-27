# `docs/onboarding-work/reconcile_backend_ledger.py`

## Purpose

Current script file. The source excerpt and structural map below expose its content without inferring behaviour from its filename.

## Where it fits

Category: **script**. [Open current file](../../../onboarding-work/reconcile_backend_ledger.py). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 21 lines and 2,220 bytes. Source hash: `32c7a5d12caae090a08f150c4196e96b45266f70a938ff82d9f98eccc6c4ed04`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import hashlib`
- Line 2: `import json`

## Walkthrough and declaration map

Read the data or directives in order; there are no recognized code declarations in this file.

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```python
import hashlib
import json
from pathlib import Path

root = Path(__file__).resolve().parents[2]
target = root / 'docs/onboarding-work/backend-fulltext-ledger.json'
ledger = json.loads(target.read_text(encoding='utf-8'))
purposes = json.loads((root / 'docs/onboarding-work/backend-purposes.json').read_text(encoding='utf-8'))
for name in list(purposes) + ['src/.semantic.md']:
    if name not in ledger:
        raw = (root / name).read_bytes()
        ledger[name] = {'sha256': hashlib.sha256(raw).hexdigest(), 'lines': len(raw.decode('utf-8-sig').splitlines()), 'status': 'opened and semantically inspected for backend teaching chapter'}
for name, entry in ledger.items():
    if entry['status'] == 'full text returned for manual inspection':
        entry['status'] = 'full text opened and read in backend inspection packets; truncated slices reopened separately'
target.write_text(json.dumps(ledger, indent=2) + '\n', encoding='utf-8')
rows = ['# Backend full-text read ledger', '', 'This ledger combines the original fifty focused chapter reads with the subsequent full-text packet inspection. Application declarations, validators, handlers and ports, domain types, API files, infrastructure adapters, configurations, and every migration Up/Down body were opened and read. The generated cumulative designers and snapshot are inspected separately by the infrastructure/test subtask; they are deliberately absent here until that evidence is merged. Machine symbol extraction is not the basis of this manual read claim. No backend runtime tests were executed by this documentation subtask.', '', 'Packet output truncation was recovered by reopening the missing SeedCommand slice and the enrollment/schedule migration and access-helper/health-check files. The schedule migration had already been read in the original focused review.', '', '| Path | Lines | SHA256 | Inspection |', '| --- | --- | --- | --- |']
for name, entry in sorted(ledger.items()):
    rows.append(f"| `{name}` | {entry['lines']} | `{entry['sha256']}` | {entry['status']} |")
(root / 'docs/onboarding-work/backend-read-ledger.md').write_text('\n'.join(rows) + '\n', encoding='utf-8')
print(f'{len(ledger)} backend paths in manual full-text ledger')
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
