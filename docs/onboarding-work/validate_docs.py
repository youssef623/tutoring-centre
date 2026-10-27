"""Check local Markdown targets, anchors, source-line bounds and teaching structure."""
from pathlib import Path
from urllib.parse import unquote
import collections, hashlib, json, re, subprocess

ROOT=Path(__file__).resolve().parents[2]
WORK=ROOT/'docs/onboarding-work'
MAIN=ROOT/'docs/PROJECT_OVERVIEW3.md'

def outside_code(text):
    lines=[]; fence=None
    for line in text.splitlines():
        m=re.match(r'^\s*(`{3,}|~{3,})',line)
        if m:
            if fence is None: fence=(m[1][0],len(m[1]))
            elif m[1][0]==fence[0] and len(m[1])>=fence[1]: fence=None
            lines.append(''); continue
        lines.append(line if fence is None else '')
    return '\n'.join(lines)

def anchors(text):
    clean=outside_code(text); result=set(re.findall(r'<a\s+(?:id|name)="([^"]+)"',clean)); counts=collections.Counter()
    for title in re.findall(r'^#{1,6}\s+(.+?)\s*#*$',clean,re.M):
        title=re.sub(r'\[([^\]]+)\]\([^)]*\)',r'\1',title)
        slug=re.sub(r'[^\w\-\s]','',title.replace('`','').lower(),flags=re.U).replace(' ','-')
        slug=slug.replace('_','_')
        n=counts[slug]; counts[slug]+=1
        result.add(slug+(f'-{n}' if n else ''))
    return result

files=[MAIN]+sorted((ROOT/'docs/file-explanations').rglob('*.md'))
errors=[]; checked=0; cache={}
for file in files:
    text=file.read_text(encoding='utf8');clean=outside_code(text)
    # Inline code may contain things that look like Markdown links; they are examples, not links.
    clean=re.sub(r'`[^`\n]*`','',clean)
    for match in re.finditer(r'(?<!!)\[[^\]\n]*\]\(([^\n]*?)\)',clean):
        target=match[1].strip()
        if not target or re.match(r'^[a-zA-Z][\w+.-]*:',target) or target.startswith('//'): continue
        target=target.split(' "',1)[0].strip('<>'); location,sep,fragment=target.partition('#')
        actual=(file.parent/unquote(location)).resolve() if location else file
        checked+=1
        if not actual.exists(): errors.append({'file':str(file.relative_to(ROOT)),'target':target,'error':'missing target'});continue
        if fragment:
            fragment=unquote(fragment)
            if re.fullmatch(r'L\d+(?:-L\d+)?',fragment):
                count=len(actual.read_text(encoding='utf-8-sig').splitlines())
                if max(map(int,re.findall(r'\d+',fragment)))>count: errors.append({'file':str(file.relative_to(ROOT)),'target':target,'error':'source line out of bounds'})
            elif actual.suffix.lower()=='.md':
                if actual not in cache: cache[actual]=anchors(actual.read_text(encoding='utf8'))
                if fragment not in cache[actual]: errors.append({'file':str(file.relative_to(ROOT)),'target':target,'error':'missing anchor'})

text=MAIN.read_text(encoding='utf8'); clean=outside_code(text)
concepts=list(re.finditer(r'^## (6\.\d+) (.+)$',clean,re.M))
required=['What it means','The problem it solves','How it works','How THIS project implements it','Execution example','Why this project needs it','Alternatives','Important details','What I should understand before modifying this code']
concept_results=[]
for i,m in enumerate(concepts):
    end=concepts[i+1].start() if i+1<len(concepts) else clean.index('\n# 7.',m.end())
    block=clean[m.start():end]
    headings=re.findall(r'^#### (.+)$',block,re.M)
    missing=[h for h in required if h not in headings]
    concept_results.append({'number':m[1],'title':m[2],'words_without_fences':len(block.split()),'missing_teaching_headings':missing})
    if missing: errors.append({'concept':m[1],'error':'missing teaching headings','missing':missing})

old=(ROOT/'docs/PROJECT_OVERVIEW2.md').read_text(encoding='utf8')
unchanged=subprocess.run(['git','diff','--exit-code','HEAD','--','docs/PROJECT_OVERVIEW.md','docs/PROJECT_OVERVIEW2.md'],cwd=ROOT,capture_output=True,text=True).returncode==0
report={'files_checked':len(files),'links_checked':checked,'errors':errors,'v2':{'lines':len(old.splitlines()),'words':len(old.split())},'v3':{'lines':len(text.splitlines()),'words':len(text.split()),'concepts':len(concepts),'numbered_top_level_sections':re.findall(r'^# (\d+)\.',clean,re.M)},'historical_overviews_unchanged':unchanged,'concepts':concept_results}
(WORK/'validation-results.json').write_text(json.dumps(report,indent=2),encoding='utf8')
summary={k:v for k,v in report.items() if k not in {'concepts','errors'}}
summary['error_count']=len(errors)
summary['first_errors']=errors[:10]
print(json.dumps(summary,indent=2))
raise SystemExit(bool(errors) or not unchanged or len(text.split())<len(old.split()) or len(concepts)<35)
