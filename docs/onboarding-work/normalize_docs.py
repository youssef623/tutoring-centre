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
