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
