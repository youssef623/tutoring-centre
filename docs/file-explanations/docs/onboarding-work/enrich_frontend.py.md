# `docs/onboarding-work/enrich_frontend.py`

## Purpose

Current script file. The source excerpt and structural map below expose its content without inferring behaviour from its filename.

## Where it fits

Category: **script**. [Open current file](../../../onboarding-work/enrich_frontend.py). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 34 lines and 1,689 bytes. Source hash: `5d8559adf5afbc5ce089dd35e04266e7361d621ba6f756282e67091ccd2dd343`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Read the data or directives in order; there are no recognized code declarations in this file.

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

````python
from pathlib import Path
root=Path(__file__).resolve().parents[2]
path=root/'docs/onboarding-work/frontend.md'
text=path.read_text(encoding='utf-8')
samples={
40:('frontend/src/main.tsx',23,36),
41:('frontend/orval.config.ts',10,20),
42:('frontend/src/api/apiFetch.ts',156,178),
43:('frontend/src/routes/_authenticated.tsx',16,32),
44:('frontend/src/features/auth/changePasswordSchema.ts',7,21),
45:('frontend/src/lib/bidi.ts',7,9),
46:('frontend/src/features/students/StudentsList.tsx',43,49),
47:('frontend/src/components/shared/useParentLookup.ts',56,65),
48:('frontend/src/features/subjects/SubjectsList.tsx',77,87),
49:('frontend/src/features/enrollments/useEnrollmentMutations.ts',81,94),
50:('frontend/src/features/schedule/centreTime.ts',15,22),
51:('frontend/src/features/schedule/useScheduleMutations.ts',47,59),
52:('frontend/src/components/shared/DataTable.tsx',10,18),
53:('frontend/src/features/status/api.ts',12,24),
54:('frontend/src/test/setup.ts',25,39),
55:('frontend/e2e/full-centre-setup.spec.ts',220,227),
}
for section,(file,start,end) in samples.items():
 marker=f'## 6.{section} '
 begin=text.index(marker)
 endsection=text.find('\n## 6.',begin+1)
 if endsection==-1:endsection=len(text)
 target=text.index('#### Execution example',begin,endsection)
 lines=(root/file).read_text(encoding='utf-8-sig').splitlines()
 snippet='\n'.join(lines[start-1:min(end,len(lines))])
 excerpt=f'Verified source excerpt, `{file}:{start}` (quoted directly from the current file):\n\n```'+('ts' if file.endswith('.ts') else 'tsx')+f'\n{snippet}\n```\n\n'
 text=text[:target]+excerpt+text[target:]
path.write_text(text,encoding='utf-8')
print('Inserted 16 current-source excerpts.')
````

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
