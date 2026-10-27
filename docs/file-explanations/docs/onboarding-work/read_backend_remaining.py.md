# `docs/onboarding-work/read_backend_remaining.py`

## Purpose

Current script file. The source excerpt and structural map below expose its content without inferring behaviour from its filename.

## Where it fits

Category: **script**. [Open current file](../../../onboarding-work/read_backend_remaining.py). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 34 lines and 1,704 bytes. Source hash: `906c1dc640a12a78d54406ba3cc1ae5b2777377467161f5c864b1a599defd916`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import hashlib`
- Line 2: `import json`
- Line 4: `import sys`

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
import sys
sys.stdout.reconfigure(encoding='utf-8')

root = Path(__file__).resolve().parents[2]
purposes = json.loads((root / 'docs/onboarding-work/backend-purposes.json').read_text(encoding='utf-8-sig'))
area, packet = sys.argv[1], int(sys.argv[2])
ledger_path = root / 'docs/onboarding-work/backend-fulltext-ledger.json'
ledger = json.loads(ledger_path.read_text(encoding='utf-8')) if ledger_path.exists() else {}
paths = sorted(p for p in (root / 'src' / area).rglob('*') if p.is_file() and not {'bin', 'obj'} & set(p.parts) and p.relative_to(root).as_posix() not in purposes)
if len(sys.argv) > 3 and sys.argv[3] == 'pending-source':
    paths = [p for p in paths if p.relative_to(root).as_posix() not in ledger and not p.name.endswith('.Designer.cs') and p.name != 'AppDbContextModelSnapshot.cs']
packets, current, size = [], [], 0
for path in paths:
    length = path.stat().st_size
    if current and size + length > 24000:
        packets.append(current)
        current, size = [], 0
    current.append(path)
    size += length
if current:
    packets.append(current)
print(f'{area}: packet {packet + 1}/{len(packets)}, remaining files {len(paths)}')
for path in packets[packet]:
    raw = path.read_bytes()
    content = raw.decode('utf-8-sig')
    rel = path.relative_to(root).as_posix()
    print('\nFILE ' + rel)
    for i, line in enumerate(content.splitlines(), 1):
        print(f'{i}: {line}')
    ledger[rel] = {'sha256': hashlib.sha256(raw).hexdigest(), 'lines': len(content.splitlines()), 'packet': packet, 'status': 'full text returned for manual inspection'}
ledger_path.write_text(json.dumps(ledger, indent=2), encoding='utf-8')
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
