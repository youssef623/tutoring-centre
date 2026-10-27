from pathlib import Path
import subprocess,re,json,hashlib
root=Path.cwd()
tracked=subprocess.check_output(['git','ls-files'],text=True).splitlines()
paths=[p for p in tracked if p.startswith(('tests/','.github/','smoke/','docs/adr/','docs/architecture/','docs/operations/')) or p in ['README.md','CLAUDE.md','Dockerfile','compose.yaml','compose.prod.yaml','Caddyfile','Directory.Build.props','Directory.Packages.props','global.json','.dockerignore','.editorconfig','.env.example','.gitattributes','.gitignore','.gitleaksignore','.config/dotnet-tools.json','TutoringCentre.slnx','later.md','db/bootstrap-roles.sh','database/demos/week4-rls.sql']]
ledger=[]; catalog=[]
for p in paths:
    data=(root/p).read_bytes()
    try: text=data.decode('utf-8-sig')
    except UnicodeDecodeError: continue
    lines=text.splitlines()
    ledger.append({'path':p,'lines':len(lines),'sha256':hashlib.sha256(data).hexdigest()})
    if p.startswith('tests/') and p.endswith('.cs'):
        tests=[]
        for i,line in enumerate(lines):
            if re.search(r'public\s+(?:async\s+)?(?:Task(?:<[^>]+>)?|void)\s+\w+\(',line):
                if any(re.search(r'\[(?:Fact|Theory)',s) for s in lines[max(0,i-8):i]):
                    tests.append((i+1,line.strip()))
        summary=re.search(r'<summary>(.*?)</summary>',text,re.S)
        purpose=re.sub(r'\s+',' ',re.sub(r'///|<[^>]*>','',summary.group(1))).strip() if summary else ''
        catalog.append(f'### `{p}`\n\n'+(purpose+'\n\n' if purpose else '')+'Declared test entry points (declarations are not execution results):\n\n'+'\n'.join(f'- Line {n}: `{name}`' for n,name in tests)+'\n')
out=root/'docs/onboarding-work'
out.mkdir(exist_ok=True,parents=True)
(out/'tests-infra-read-ledger.json').write_text(json.dumps(ledger,indent=2),encoding='utf-8')
(out/'test-catalog.md').write_text('# Backend test file catalogue\n\nEvery tracked backend test source is read by the inventory pass; this catalogue records its declared test entry points. Fixture and helper files without test methods remain in the read ledger. Names describe assertions to inspect; passing results are reported separately.\n\n'+'\n'.join(catalog),encoding='utf-8')
print(json.dumps({'files':len(ledger),'test_source_files':len(catalog),'catalog_words':len((out/'test-catalog.md').read_text().split())}))
