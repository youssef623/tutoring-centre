"""Assemble reviewed sections and source-derived reference catalogues."""
from pathlib import Path
import json, re, subprocess

ROOT=Path(__file__).resolve().parents[2]
WORK=ROOT/'docs/onboarding-work'

def read(path): return (ROOT/path).read_text(encoding='utf-8-sig')
def cell(value): return str(value).replace('|','&#124;').replace('\n',' ')

def contract_catalog():
    path='frontend/openapi/TutoringCentre.Api.json'
    spec=json.loads(read(path))
    output=['## 8.4 Complete committed API contract','',f'This catalogue is extracted from the current committed OpenAPI document, opened as JSON during this audit. It contains {len(spec["paths"])} path templates and {len(spec.get("components",{}).get("schemas",{}))} schemas. These are contract declarations, not additional runtime routes inferred from names. Health, static files and excluded fallbacks are described separately because they are not part of this document.','', '| Method and path | Operation | Parameters / request body | Declared responses |','| --- | --- | --- | --- |']
    for route, item in spec['paths'].items():
        for method,operation in item.items():
            if method.lower() not in {'get','post','put','patch','delete','head','options'}: continue
            params=[f'{p.get("in")}: {p["name"]}'+(' (required)' if p.get('required') else '') for p in operation.get('parameters',[])]
            for media, body in operation.get('requestBody',{}).get('content',{}).items():
                schema=body.get('schema',{}); params.append(media+': '+schema.get('$ref',str(schema)).split('/')[-1])
            responses=[]
            for status, response in operation.get('responses',{}).items():
                shapes=[]
                for media,entry in response.get('content',{}).items():
                    schema=entry.get('schema',{}); shapes.append(schema.get('$ref',schema.get('type','unspecified')).split('/')[-1])
                responses.append(status+(': '+', '.join(shapes) if shapes else ': no declared body'))
            output.append(f'| `{method.upper()} {route}` | `{operation.get("operationId","unnamed")}` | {cell("; ".join(params) or "No declared input")} | {cell("; ".join(responses))} |')
    output+=['','The declared response list is not a proof that middleware cannot produce another status. Authentication, rate limiting, conversion, CSRF and unexpected faults can add responses outside an individual operation\'s listed success and business outcomes. Endpoint metadata and shared pipeline behaviour must both be read.','']
    return '\n'.join(output)

def schema_catalog():
    spec=json.loads(read('frontend/openapi/TutoringCentre.Api.json'))
    output=['## 9.5 DTO and request schema catalogue','', 'The following fields come directly from the current API schema. A DTO is a transfer shape, not a second Domain entity. Requiredness here describes the contract; the validator and handler establish additional meaning. Generated client types derive from these definitions.','', '| Schema | Fields and declared types | Required fields |','| --- | --- | --- |']
    for name,schema in spec.get('components',{}).get('schemas',{}).items():
        fields=[]
        for key,value in schema.get('properties',{}).items():
            typ=value['$ref'].split('/')[-1] if '$ref' in value else value.get('type','composed')
            if isinstance(typ,list): typ=' or '.join(typ)
            if value.get('format'): typ+=' ('+value['format']+')'
            if value.get('items'): typ+=' of '+value['items'].get('$ref',value['items'].get('type','value')).split('/')[-1]
            fields.append(key+': '+typ)
        if 'enum' in schema: fields.append('Enum values: '+', '.join(map(str,schema['enum'])))
        output.append(f'| `{name}` | {cell("; ".join(fields) or "See composed schema definition")} | {cell(", ".join(schema.get("required",[])) or "None declared")} |')
    output+=['','## 9.6 Migration catalogue','', '| Source migration | Operations visible in Up/Down source |','| --- | --- |']
    for p in sorted((ROOT/'src/TutoringCentre.Infrastructure/Persistence/Migrations').glob('*.cs')):
        text=p.read_text(encoding='utf-8-sig')
        if not re.search(r': Migration\b',text): continue
        operations=[]
        for n,line in enumerate(text.splitlines(),1):
            if re.search(r'migrationBuilder\.(?:Create|Drop|Add|Alter|Rename|Ensure|Sql|Enable|Disable)|\.(?:EnableTenant|DisableTenant)',line):
                operations.append(f'L{n}: '+line.strip())
        output.append(f'| `{p.relative_to(ROOT).as_posix()}` | {cell("; ".join(operations))} |')
    output+=['', 'The operations include both forward and reverse methods; their presence does not prove that a production rollback has been rehearsed. Designers and the model snapshot are separately indexed. SQL helpers centralize grants and tenant policy expressions, so review those helpers as well as a migration that calls them.','']
    return '\n'.join(output)

def versions():
    lock=json.loads(read('frontend/package-lock.json'))
    manifest=json.loads(read('frontend/package.json'))
    output=['## 2.2 Exact direct frontend resolutions','', 'These versions are read from the committed lockfile, not guessed from semver ranges or copied from the older overview. The range and exact resolution intentionally occupy separate columns.','', '| Package | Manifest range | Locked version |','| --- | --- | --- |']
    for scope in ('dependencies','devDependencies'):
        for name,ran in manifest.get(scope,{}).items():
            entry=lock.get('packages',{}).get('node_modules/'+name,{})
            output.append(f'| `{name}` | `{ran}` | `{entry.get("version","not recorded at root location")}` |')
    return '\n'.join(output)+'\n'

def file_table():
    inventory=json.loads((WORK/'file-guide-ledger.json').read_text(encoding='utf8')) if (WORK/'file-guide-ledger.json').exists() else []
    output=['## 4.3 Complete per-file architecture map','', 'For each file, dependency entries below are literal import/project references and “Used by” describes the evidence level available. Imports may be unused. The linked guide contains the current source summary, declarations and line locations. Runtime consumers must be established by call sites and registration, not by an invented filename relationship.','', '| Path | Type | Purpose | Depends on | Used by |','| --- | --- | --- | --- | --- |']
    for row in inventory:
        path=row['path']
        guide=ROOT/'docs/file-explanations'/(path+'.md')
        text=guide.read_text(encoding='utf8')
        purpose=text.split('## Purpose\n\n',1)[1].split('\n\n##',1)[0]
        # Link the guide rather than pretending an import list establishes consumers.
        deps=[]
        for n,line in enumerate((read(path) if not path.endswith('.md') else '').splitlines(),1):
            if re.match(r'^using |^import .*from |^\s*<ProjectReference',line): deps.append(line.strip())
        dep='; '.join(deps[:5]) or 'No recognized import declaration'
        if len(deps)>5: dep+=f'; {len(deps)-5} more in guide'
        inventory_categories={r['path']:r['category'] for r in json.loads((WORK/'inventory.json').read_text(encoding='utf8'))}
        kind=inventory_categories.get(path,'documentation' if path.endswith('.md') else 'other')
        consumers=re.findall(r'^- \[([^\]]+)\]\([^\n]+\)$',text.split('## Used by: verified name references',1)[-1].split('## Current source excerpt',1)[0],re.M)
        used='; '.join(consumers[:3]) if consumers else 'Tool convention or no unique name reference established'
        output.append(f'| [`{path}`](file-explanations/{path}.md) | {kind} | {cell(purpose)} | {cell(dep)} | {cell(used)} |')
    return '\n'.join(output)+'\n'

def changes():
    result=subprocess.check_output(['git','diff','--name-status','--find-renames','9d638ceb46099a4fd87ba8bc901b10e947c1f737','HEAD'],cwd=ROOT,text=True)
    lines=['# File-level changes since the v2 baseline','', 'Generated directly with Git rename detection between the v2 document baseline and the audit starting HEAD. A status of M means modified; A added; D deleted; R a detected rename. A modification status does not by itself distinguish a rewrite from a small edit.','', '| Git status | Path(s) |','| --- | --- |']
    for row in result.splitlines():
        parts=row.split('\t'); lines.append('| '+parts[0]+' | '+' / '.join('`'+x+'`' for x in parts[1:])+' |')
    (WORK/'changes-since-v2.md').write_text('\n'.join(lines)+'\n',encoding='utf8')
    return '\n[Complete Git name-status catalogue](onboarding-work/changes-since-v2.md).\n'

opening=(WORK/'opening.md').read_text(encoding='utf8')
opening=opening.replace('# 2. Technology Stack',(WORK/'feature-reference.md').read_text(encoding='utf8')+'\n# 2. Technology Stack')
opening=opening.replace('# 3. Architecture',versions()+'\n# 3. Architecture')
opening=opening.replace('# 5. Startup / Bootstrapping',file_table()+'\n# 5. Startup / Bootstrapping')
parts=[]
backend_walkthroughs=''
testing_reference=''
infrastructure_reference=''
for file in ('backend.md','foundations.md','frontend.md','auth-depth.md','tests-infra.md','logging-depth.md'):
    p=WORK/file
    if p.exists():
        draft=p.read_text(encoding='utf8')
        if file=='backend.md':
            draft, sep, backend_walkthroughs=draft.partition('# Backend action walkthroughs')
            if draft.startswith('# '): draft=draft.split('\n',1)[1]
        if file=='tests-infra.md':
            draft,sep,refs=draft.partition('# Testing reference for main section 12')
            testing_reference,sep,infrastructure_reference=refs.partition('# Infrastructure reference for main section 13')
            if draft.startswith('# '): draft=draft.split('\n',1)[1]
        parts.append(draft)
concepts='\n\n'.join(parts)
supplements=json.loads((WORK/'mechanism-depth.json').read_text(encoding='utf8'))
supplements.update(json.loads((WORK/'auth-supplements.json').read_text(encoding='utf8')))
for key,value in json.loads((WORK/'depth-additions.json').read_text(encoding='utf8')).items():
    supplements[key]=supplements.get(key,'')+'\n\n'+value
for anchor,extra in supplements.items():
    start=concepts.find('<a id="'+anchor+'"></a>')
    if start<0: continue
    next_anchor=concepts.find('<a id="',start+1)
    end=next_anchor if next_anchor>=0 else len(concepts)
    insertion=concepts.find('#### What I should understand before modifying this code',start,end)
    if insertion>=0: concepts=concepts[:insertion]+extra+'\n\n'+concepts[insertion:]
diagrams=json.loads((WORK/'diagrams.json').read_text(encoding='utf8'))
for anchor,diagram in diagrams.items():
    pattern=r'(<a id="'+re.escape(anchor)+r'"></a>\s*\n## 6\.\d+ [^\n]+\n)'
    concepts=re.sub(pattern,lambda m:m[1]+'\n```mermaid\n'+diagram+'\n```\n',concepts,count=1)
# Each draft has a reserved range. Retain original explicit anchors but make displayed numbers contiguous.
counter=[0]
def renumber(m):
    counter[0]+=1
    return '## 6.'+str(counter[0])+' '+m[1]
concepts=re.sub(r'^## 6\.\d+ (.+)$',renumber,concepts,flags=re.M)
reference=(WORK/'reference.md').read_text(encoding='utf8')
reference=reference.replace('AuthEndpoints.cs:144','AuthEndpoints.cs:140')
reference=reference.replace('# 8. Routing and Middleware',(WORK/'auth-reference.md').read_text(encoding='utf8')+'\n# 8. Routing and Middleware')
reference=reference.replace('# 11. Frontend', '\n## 10.5 Additional complete action traces\n'+re.sub(r'^## ', '### ',backend_walkthroughs,flags=re.M)+'\n# 11. Frontend')
def numbered_reference(text,prefix,start):
    n=[start-1]
    def replace(m):
        n[0]+=1
        return '## '+prefix+'.'+str(n[0])+' '
    return re.sub(r'^## ',replace,text,flags=re.M)
reference=reference.replace('# 13. Infrastructure','\n'+numbered_reference(testing_reference,'12',4)+'\n# 13. Infrastructure')
reference=reference.replace('# 14. Code Quality Findings','\n'+numbered_reference(infrastructure_reference,'13',1)+'\n# 14. Code Quality Findings')
reference=reference.replace('# 9. Data Layer',contract_catalog()+'\n# 9. Data Layer')
reference=reference.replace('# 10. Business Logic and Services',schema_catalog()+'\n# 10. Business Logic and Services')
reference=reference.replace('# 17. Glossary and Open Questions',changes()+'\n# 17. Glossary and Open Questions')
checks=WORK/'session-results.md'
if checks.exists(): reference=reference.replace('<!-- SESSION_RESULTS -->',checks.read_text(encoding='utf8'))
browser=WORK/'browser-check-results.md'
if browser.exists():
    reference=reference.replace('## 12.3 Coverage boundaries','The nineteen Chromium browser journeys also passed in this session, using two workers, a disposable PostgreSQL database and test-owned API/Vite processes. Setup, command, cleanup and coverage limits are recorded in [the browser check record](onboarding-work/browser-check-results.md). No production-image/Caddy smoke or cloud deployment run is implied.\n\n## 12.3 Coverage boundaries')
toc=[]
for anchor,title in re.findall(r'<a id="([^"]+)"></a>\s*\n## (6\.\d+ .+)',concepts): toc.append(f'- [{title}](#{anchor})')
result=opening+'\n# 6. Concepts and Patterns\n\nEvery concept is taught from first principles, contrasts a missing mechanism with the implementation, identifies current source and traces execution. Explicit anchors keep learning links stable while the numbered concept catalogue grows.\n\n'+'\n'.join(toc)+'\n\n'+concepts+'\n\n'+reference
(ROOT/'docs/PROJECT_OVERVIEW3.md').write_text(result,encoding='utf8')
print('Concepts:',counter[0],'Lines:',len(result.splitlines()),'Words:',len(result.split()))
