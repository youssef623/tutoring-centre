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
