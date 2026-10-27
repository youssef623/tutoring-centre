"""Build a complete, evidence-linked structural file guide from current bytes.

This tool reports declarations and source comments, not inferred runtime callers.
"""
from pathlib import Path
import collections, hashlib, html, json, os, re, subprocess
from inventory import ROOT, OUT, category

DEST=ROOT/'docs/file-explanations'
paths=subprocess.check_output(['git','ls-files'],cwd=ROOT,text=True).splitlines()
paths=[p for p in paths if not p.startswith('docs/file-explanations/')]
paths += ['docs/PROJECT_OVERVIEW3.md']
purposes={}
for name in ('backend-purposes.json','frontend-purposes.json','generated-model-guide-notes.json'):
    p=OUT/name
    if p.exists(): purposes.update(json.loads(p.read_text(encoding='utf8')))

# A lexical consumer map is evidence of named references, not an invented runtime graph.
texts={}
occurrences=collections.defaultdict(list)
definitions=collections.defaultdict(set)
for path in paths:
    p=ROOT/path
    if p.suffix not in {'.cs','.ts','.tsx','.mjs','.py','.sh'} or not p.exists(): continue
    text=p.read_text(encoding='utf-8-sig'); texts[path]=text
    seen=set()
    for n,line in enumerate(text.splitlines(),1):
        if line.lstrip().startswith(('//','///','*','#')): continue
        for token in set(re.findall(r'\b[A-Za-z_]\w*\b',line)):
            if token not in seen: occurrences[token].append((path,n));seen.add(token)
    for symbol in re.findall(r'\b(?:class|interface|enum|record(?:\s+struct)?|function)\s+(\w+)|^export\s+(?:const|type)\s+(\w+)',text,re.M):
        definitions[next(x for x in symbol if x)].add(path)

def relative(a,b): return os.path.relpath(b,a).replace('\\','/')
def escape(s): return s.replace('|','&#124;').replace('\n',' ')
def description(path, text):
    if path in purposes: return purposes[path]
    if path.endswith('.md'):
        headings=re.findall(r'^#{1,3} (.+)$',text,re.M)
        return 'Documentation covering '+('; '.join(headings[:5]) or 'the repository')+'. Treat historical claims according to the revision described in the file.'
    comments=re.findall(r'<summary>(.*?)</summary>',text,re.S)
    if comments:
        comment=re.sub(r'<see\s+cref="([^"]+)"\s*/>',r'`\1`',comments[0])
        comment=re.sub(r'<[^>]+>','',comment)
        comment=re.sub(r'^\s*///\s?','',comment,flags=re.M)
        return 'Source summary (intent, not independent execution proof): '+re.sub(r'\s+',' ',comment).strip()
    docs=re.search(r'/\*\*(.*?)\*/',text,re.S)
    if docs:
        return 'Source comment (intent): '+re.sub(r'\s+',' ',re.sub(r'^\s*\*\s?','',docs[1],flags=re.M)).strip()
    if path.endswith('.md'):
        headings=re.findall(r'^#{1,3} (.+)$',text,re.M)
        return 'Documentation covering '+('; '.join(headings[:5]) or 'the repository')+'. Treat historical claims according to the revision described in the file.'
    definitions=re.findall(r'\b(?:class|interface|enum|record(?:\s+struct)?|function)\s+(\w+)',text)
    if definitions: return 'Declares '+', '.join('`'+x+'`' for x in dict.fromkeys(definitions[:8]))+'. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.'
    if '.test.' in path or path.startswith('tests/'):
        names=re.findall(r'\b(?:public\s+(?:async\s+)?(?:Task|void)\s+)(\w+)\s*\(',text)
        return 'Test/support source. '+('Named checks include '+', '.join('`'+x+'`' for x in names[:5])+'.' if names else 'Read its assertions and fixture setup together; test source is not evidence that the tests were executed.')
    if path.endswith('.json'):
        try:
            obj=json.loads(text); keys=list(obj)[:12] if isinstance(obj,dict) else []
            return 'JSON data with top-level keys '+', '.join('`'+str(x)+'`' for x in keys)+'. Its consumers determine whether this is build configuration, a contract or a language resource.'
        except ValueError: pass
    return 'Current '+category(Path(path))+' file. The source excerpt and structural map below expose its content without inferring behaviour from its filename.'

rows=[]; ledger=[]
for path in sorted(set(paths)):
    src=ROOT/path
    if not src.exists(): continue
    data=src.read_bytes()
    try: text=data.decode('utf-8-sig')
    except UnicodeDecodeError: text=''
    kind=category(Path(path)); dest=DEST/(path+'.md')
    purpose=description(path,text)
    imports=[]; declarations=[]
    for n,line in enumerate(text.splitlines(),1):
        if re.match(r'^using |^import |^\s*<ProjectReference|^\s*<PackageReference',line): imports.append((n,line.strip()))
        if re.search(r'\b(?:class|interface|enum|record|function)\s+\w+|\b(?:public|internal|private|protected)\s+(?:static\s+)?(?:async\s+)?[\w<>,?\[\]]+\s+\w+(?:<[^>]+>)?\s*\(|\b(?:public|internal|private|protected)\s+\w+\s*\(|^export (?:const|type)',line): declarations.append((n,line.strip()))
    rows.append((path,kind,purpose,dest,imports))
    ledger.append({'path':path,'sha256':hashlib.sha256(data).hexdigest(),'lines':len(text.splitlines()),'bytes':len(data)})
    dest.parent.mkdir(parents=True,exist_ok=True)
    link=relative(dest.parent,src)
    body=[f'# `{path}`','', '## Purpose','',purpose,'','## Where it fits','',f'Category: **{kind}**. [Open current file]({link}). [Complete file index]({relative(dest.parent,DEST/"INDEX.md")}). [Current teaching overview]({relative(dest.parent,ROOT/"docs/PROJECT_OVERVIEW3.md")}).','',f'The reviewed file has {len(text.splitlines())} lines and {len(data):,} bytes. Source hash: `{hashlib.sha256(data).hexdigest()}`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.','', '## Dependencies visible in the file','']
    if imports:
        body+=['These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.','']
        body += [f'- Line {n}: `{line.replace("`", "")}`' for n,line in imports]
    else: body+=['No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.']
    body+=['','## Walkthrough and declaration map','']
    if declarations:
        body+=['Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.','']
        body += [f'- [Line {n}]({link}#L{n}): `{line.replace("`", "")}`' for n,line in declarations]
    else:
        headings=[(n,line) for n,line in enumerate(text.splitlines(),1) if re.match(r'^#{1,4} ',line)]
        body += [f'- Line {n}: {line.lstrip("# ")}' for n,line in headings[:30]] or ['Read the data or directives in order; there are no recognized code declarations in this file.']
    symbols=[s for s,owners in definitions.items() if owners=={path}]
    consumers=sorted({(p,n) for s in symbols for p,n in occurrences[s] if p!=path})
    body+=['','## Used by: verified name references','']
    if consumers:
        body+=['These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.','']
        body += [f'- [{p}:{n}]({relative(dest.parent,ROOT/p)}#L{n})' for p,n in consumers[:20]]
        if len(consumers)>20: body += [f'- {len(consumers)-20} further name-reference locations are omitted from this compact list.']
    else:
        body+=['No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.']
    body+=['','## Current source excerpt','', 'The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.','']
    language={'cs':'csharp','tsx':'tsx','ts':'typescript','json':'json','sh':'bash','yml':'yaml','yaml':'yaml','py':'python'}.get(src.suffix.lstrip('.'),'text')
    # Large machine artifacts get metadata rather than thousands of duplicate generated lines.
    limit=90 if kind not in {'lockfile','generated','documentation','image/font/media'} else 24
    excerpt='\n'.join(text.splitlines()[:limit])
    fence='````' if '```' in excerpt else '```'
    body += [fence+language,excerpt,fence]
    if len(text.splitlines())>limit: body+=['',f'The excerpt stops at line {limit}; use the original file for the remaining {len(text.splitlines())-limit} lines.']
    body+=['','## Before modifying this file','', 'Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.','']
    dest.write_text('\n'.join(body),encoding='utf8')

current={p for p,_,_,_,_ in rows}
stale=[]
for p in DEST.rglob('*.md'):
    rel=p.relative_to(DEST).as_posix()
    if rel=='INDEX.md' or re.match(r'^0\d-',rel): continue
    original=rel[:-3]
    if original not in current:
        p.write_text(f'# Historical file explanation\n\n`{original}` is absent from the current application inventory. This entry is retained as a tombstone rather than describing removed code as current. See [the current index]({relative(p.parent,DEST/"INDEX.md")}).\n',encoding='utf8')
        stale.append(original)
index=['# Complete current repository file index','', 'This index categorizes every tracked project file outside the explanation set itself. Local untracked files, sensitive-file metadata and excluded build/tool directories are captured separately in [the recursive inventory](../onboarding-work/inventory.json) and [exclusions](../onboarding-work/inventory-exclusions.json). The explanation set is listed as a companion artifact, not recursively given its own explanations.','',f'Application inventory: **{len(rows)} files**. Each has a mirrored guide. Generated artifacts, lockfiles and media have explicit categories. Empty categories contain zero files; none is silently merged into source.','', '## Counts by category','', '| Category | Count |','| --- | --- |']
counts=collections.Counter(r[1] for r in rows)
for cat in ['source','test','configuration','documentation','migration','database/schema','script','CI/CD','Docker/infrastructure','lockfile','generated','binary','image/font/media','vendored','other']: index.append(f'| {cat} | {counts[cat]} |')
index+=['','## Current files','', '| Path | Type | Purpose | Explanation |','| --- | --- | --- | --- |']
for path,kind,purpose,dest,imports in rows: index.append(f'| `{path}` | {kind} | {escape(purpose)} | [Guide]({relative(DEST,dest)}) |')
index+=['','## Removed entries','']+([f'- `{p}`: historical guide marked as absent.' for p in stale] or ['No explanation path in the prior set refers to a removed application file.'])
index+=['','## Companion documentation','', 'The `docs/file-explanations/` tree contains this index, retained area guides and one mirrored guide per application file. The complete recursive snapshot in `docs/onboarding-work/inventory.json` includes the explanation files that existed at audit start. This finite inventory rule prevents creating explanations of explanations indefinitely.','']
(DEST/'INDEX.md').write_text('\n'.join(index),encoding='utf8')
areas={
 '01-root-and-infrastructure.md':lambda p:not p.startswith(('src/','tests/','frontend/','docs/')),
 '02-domain.md':lambda p:p.startswith('src/TutoringCentre.Domain/'),
 '03-application.md':lambda p:p.startswith('src/TutoringCentre.Application/'),
 '04-infrastructure.md':lambda p:p.startswith('src/TutoringCentre.Infrastructure/'),
 '05-api.md':lambda p:p.startswith('src/TutoringCentre.Api/'),
 '06-backend-tests.md':lambda p:p.startswith('tests/'),
 '07-frontend.md':lambda p:p.startswith('frontend/')}
for name,predicate in areas.items():
    selected=[row for row in rows if predicate(row[0])]
    content=['# Current area file guide','',f'This refreshed area contains {len(selected)} application files. [Complete index](INDEX.md). [Current teaching overview](../PROJECT_OVERVIEW3.md). Historical overview documents remain unchanged.','', 'The per-file guides distinguish source-comment intent, recognized declarations and executed verification. Read the complete execution paths in the overview before changing shared boundaries. A source excerpt is evidence of current bytes, not proof that all branches passed a test.','', '| File | Category | Current guide |','| --- | --- | --- |']
    content += [f'| `{p}` | {cat} | [Explanation]({relative(DEST,d)}) |' for p,cat,purpose,d,imports in selected]
    (DEST/name).write_text('\n'.join(content)+'\n',encoding='utf8')
(OUT/'file-guide-ledger.json').write_text(json.dumps(ledger,indent=2),encoding='utf8')
print('Guides:',len(rows),'Tombstones:',len(stale))
