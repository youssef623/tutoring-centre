"""Capture the onboarding inventory and the historical depth benchmark."""
from pathlib import Path
import collections, hashlib, json, re, subprocess, os

ROOT = Path(__file__).resolve().parents[2]
OUT = ROOT / 'docs/onboarding-work'
OUT.mkdir(exist_ok=True)
EXCLUDED = {'.git', 'node_modules', 'bin', 'obj', 'dist', '.vs', '.vite', '.cache', 'playwright-report', 'test-results', 'blob-report', '__pycache__', 'generated-model-packets'}

def category(p):
    s = p.as_posix(); n = p.name; ext = p.suffix.lower()
    if ext == '.md': return 'documentation'
    if ext in {'.png','.jpg','.jpeg','.svg','.ico','.woff','.woff2','.ttf','.gif','.webp'}: return 'image/font/media'
    if ext in {'.dll','.exe','.pdb','.zip','.pdf'}: return 'binary'
    if 'lock' in n and ext in {'.json','.yaml','.yml','.lock'}: return 'lockfile'
    if s.startswith('.github/'): return 'CI/CD'
    if n.startswith('Dockerfile') or n in {'Caddyfile','.dockerignore'} or n.startswith('compose.'): return 'Docker/infrastructure'
    if '/Migrations/' in s: return 'migration'
    if s.startswith(('database/','db/')): return 'database/schema'
    if '/generated/' in s or n.endswith('.gen.ts'): return 'generated'
    if ext == '.md': return 'documentation'
    if s.startswith(('tests/','smoke/tests/','frontend/e2e/')) or '.test.' in n or '/test/' in s: return 'test'
    if ext in {'.sh','.ps1','.py','.mjs'} or '/scripts/' in s: return 'script'
    if ext in {'.cs','.tsx','.ts','.css','.html'} and 'config' not in n: return 'source'
    if ext in {'.json','.yaml','.yml','.props','.csproj','.slnx','.config'} or n.startswith('.') or 'config' in n: return 'configuration'
    return 'other'

tracked = set(subprocess.check_output(['git','ls-files'], cwd=ROOT,text=True).splitlines())
rows=[]; excluded=[]
owned=[]
for directory, dirs, files in os.walk(ROOT):
    for d in dirs:
        if d in EXCLUDED:
            excluded.append({'path':(Path(directory)/d).relative_to(ROOT).as_posix()+'/', 'reason':'version-control metadata, dependency/cache, generated extraction cache, editor state or build/test output'})
    dirs[:] = [d for d in dirs if d not in EXCLUDED]
    owned.extend(Path(directory)/n for n in files)
for p in sorted(owned):
    rel=p.relative_to(ROOT)
    if any(x in EXCLUDED for x in rel.parts): continue
    # Local secrets and editor state are inventoried without reproducing their content.
    if rel.parts[0] in {'.claude','artifacts'}:
        excluded.append({'path':rel.as_posix(),'reason':'local tool state/build artifacts; not tracked' if rel.as_posix() not in tracked else 'tracked artifact; requires review'})
        continue
    data=p.read_bytes()
    rows.append({'path':rel.as_posix(),'category':category(rel),'bytes':len(data),'tracked':rel.as_posix() in tracked,'sha256':None if p.name=='.env' else hashlib.sha256(data).hexdigest(),'sensitive':p.name=='.env'})
(OUT/'inventory.json').write_text(json.dumps(rows,indent=2),encoding='utf8')
(OUT/'inventory-exclusions.json').write_text(json.dumps(excluded,indent=2),encoding='utf8')
v2=(ROOT/'docs/PROJECT_OVERVIEW2.md').read_text(encoding='utf8')
concepts=list(re.finditer(r'^## (6\.\d+) (.+)$',v2,re.M))
benchmark={'lines':len(v2.splitlines()),'words':len(v2.split()),'top_level_sections':len(re.findall(r'^# \d+\.',v2,re.M)),'h1_total':len(re.findall(r'^# ',v2,re.M)),'concept_count':len(concepts),'fenced_blocks':len(re.findall(r'^```\w',v2,re.M)),'mermaid_diagrams':len(re.findall(r'^```mermaid',v2,re.M)),'concepts':[]}
for i,m in enumerate(concepts):
    end=concepts[i+1].start() if i+1<len(concepts) else v2.index('\n# 7.',m.end())
    block=v2[m.start():end]
    benchmark['concepts'].append({'number':m[1],'title':m[2],'words':len(block.split()),'lines':len(block.splitlines()),'headings':re.findall(r'^#{3,6} .+$',block,re.M)})
(OUT/'v2-benchmark.json').write_text(json.dumps(benchmark,indent=2),encoding='utf8')
print(json.dumps({k:v for k,v in benchmark.items() if k!='concepts'},indent=2))
print('Inventory:',len(rows),dict(collections.Counter(r['category'] for r in rows)))
