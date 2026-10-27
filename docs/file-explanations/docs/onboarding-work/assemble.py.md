# `docs/onboarding-work/assemble.py`

## Purpose

Current script file. The source excerpt and structural map below expose its content without inferring behaviour from its filename.

## Where it fits

Category: **script**. [Open current file](../../../onboarding-work/assemble.py). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 161 lines and 13,591 bytes. Source hash: `418cd6e383dccd3d5b2ff7f1c87b2b949c6104f0ce0f422f2162f020f7cbec8b`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 3: `import json, re, subprocess`

## Walkthrough and declaration map

- Line 131: Each draft has a reserved range. Retain original explicit anchors but make displayed numbers contiguous.

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```python
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
```

The excerpt stops at line 90; use the original file for the remaining 71 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
