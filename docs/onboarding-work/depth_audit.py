from pathlib import Path
import json,re
W=Path(__file__).resolve().parent
t=(W.parent/'PROJECT_OVERVIEW3.md').read_text(encoding='utf8')
ms=list(re.finditer(r'<a id="([^"]+)"></a>\s*\n## (6\.\d+) ([^\n]+)',t))
current={}
for i,m in enumerate(ms):
    end=ms[i+1].start() if i+1<len(ms) else t.index('\n# 7.',m.end())
    current[m[1]]={'number':m[2],'title':m[3],'words':len(t[m.start():end].split())}
mapping=[['backend-architecture'],['backend-di'],['backend-dispatcher'],['backend-results'],['backend-validation'],['backend-domain'],['backend-transactions'],['backend-persistence'],['backend-persistence'],['backend-concurrency'],['backend-tenancy'],['backend-http','foundation-json'],['infra-logging'],['infra-options-health'],['infra-architecture-tests'],['infra-test-doubles','infra-test-strategy'],['frontend-react','frontend-generation','frontend-shared-ui'],['foundation-interfaces'],['foundation-async'],['foundation-types'],['foundation-exceptions'],['foundation-metadata'],['foundation-json'],['auth-cookie'],['auth-csrf'],['auth-rate'],['auth-revalidation'],['auth-identity'],['auth-authorization'],['infra-architecture-tests'],['frontend-generation'],['frontend-sessions'],['frontend-forms'],['frontend-i18n'],['infra-playwright','frontend-testing']]
baseline=json.loads((W/'v2-benchmark.json').read_text(encoding='utf8'))
rows=[]
for old,targets in zip(baseline['concepts'],mapping):
    missing=[x for x in targets if x not in current]
    size=sum(current[x]['words'] for x in targets if x in current)
    rows.append({'old':old['number'],'old_title':old['title'],'old_words':old['words'],'current_anchors':targets,'current_words':size,'missing':missing,'meets_size_floor':not missing and size>=old['words']})
(W/'depth-comparison.json').write_text(json.dumps(rows,indent=2),encoding='utf8')
print(json.dumps([r for r in rows if not r['meets_size_floor']],indent=2))
print('Concept mapping:',len(rows),'passing size floors:',sum(r['meets_size_floor'] for r in rows))
for key in ['frontend-generation','frontend-i18n','backend-di']: print(key,current.get(key))
