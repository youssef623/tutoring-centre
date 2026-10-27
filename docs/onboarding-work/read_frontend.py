from pathlib import Path
import hashlib,json,re,os
root=Path(__file__).resolve().parents[2]
files=[]
for folder,dirs,names in os.walk(root/'frontend'):
 dirs[:]=[d for d in dirs if d not in ('node_modules','dist','coverage','.tanstack','playwright-report','test-results','blob-report')]
 files.extend(Path(folder)/name for name in names)
files.sort()
out=['# Frontend read ledger','', 'Every listed file was opened as bytes and decoded as UTF-8 where possible. This ledger records extraction, not proof that every behavior was executed.','']
for p in files:
 data=p.read_bytes(); rel=p.relative_to(root).as_posix()
 try: text=data.decode('utf-8-sig')
 except UnicodeDecodeError:
  out += [f'## {rel}',f'Binary, {len(data)} bytes; SHA256 `{hashlib.sha256(data).hexdigest()}`.',''];continue
 lines=text.splitlines()
 out += [f'## {rel}',f'{len(lines)} lines; {len(data)} bytes; SHA256 `{hashlib.sha256(data).hexdigest()}`.','']
 if p.name=='package-lock.json':
  lock=json.loads(text)
  for name in json.loads((root/'frontend/package.json').read_text())['dependencies']|json.loads((root/'frontend/package.json').read_text())['devDependencies']:
   out.append(f"- `{name}`: `{lock['packages'].get('node_modules/'+name,{}).get('version','unknown')}`")
 elif p.suffix=='.json':
  try:obj=json.loads(text)
  except json.JSONDecodeError:obj={}
  if 'paths' in obj:
   for path,ops in obj['paths'].items():
    for method,op in ops.items():
     if isinstance(op,dict):out.append(f"- `{method.upper()} {path}`: `{op.get('operationId','')}`")
  else:
   out.append('Top-level keys: '+', '.join(f'`{k}`' for k in obj))
 else:
  for n,line in enumerate(lines,1):
   if re.search(r'^export |^(?:async )?function |^const \w+.*(?:=|new)|\b(?:it|test|describe)\(|use[A-Z]\w+\(|invalidateQueries\(|queryKey:|createFileRoute|permission=|version:|data:',line):
    if len(line)<350:out.append(f'- L{n}: `{line.strip().replace(chr(96),chr(39))}`')
 out.append('')
(root/'docs/onboarding-work/frontend-ledger.md').write_text('\n'.join(out),encoding='utf-8')
print(f'Read {len(files)} frontend files. Ledger written.')
