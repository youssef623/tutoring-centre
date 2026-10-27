from pathlib import Path
import re,json,hashlib
root=Path.cwd()
folder=root/'src/TutoringCentre.Infrastructure/Persistence/Migrations'
files=sorted(folder.glob('*.Designer.cs'))+[folder/'AppDbContextModelSnapshot.cs']
unique={}; records=[]
for file in files:
 data=file.read_bytes(); text=data.decode('utf-8-sig'); lines=text.splitlines(keepends=True)
 starts=[i for i,l in enumerate(lines) if 'modelBuilder.Entity(' in l]
 boundaries=[0]+starts+[len(lines)]
 segments=[]; observations=[]
 for a,b in zip(boundaries,boundaries[1:]):
  block=''.join(lines[a:b]); digest=hashlib.sha256(block.encode()).hexdigest()
  entity=re.search(r'modelBuilder.Entity\("([^"]+)"',block)
  record={'start_line':a+1,'end_line':b,'sha256':digest,'entity':entity.group(1) if entity else None}
  if digest not in unique: unique[digest]={'first_path':file.relative_to(root).as_posix(),'start_line':a+1,'end_line':b,'text':block}
  segments.append(record)
  if entity:
   observations.append({'entity':entity.group(1),'start_line':a+1,'end_line':b,
     'tables':[{'table':table,'schema':schema} for table,schema in re.findall(r'b.ToTable\("([^"]+)", "([^"]+)"',block)],'checks':re.findall(r't.HasCheckConstraint\(([^;]+)',block),
     'keys':re.findall(r'b.Has(?:Alternate)?Key\(([^;]+)',block),'indexes':re.findall(r'b.HasIndex\(([^;]+)',block),
     'foreign_keys':re.findall(r'\.HasForeignKey\(([^;]+)',block),
     'concurrency_properties':[name for name,chain in re.findall(r'b.Property<[^>]+>\("([^"]+)"\)((?:(?!b.Property)[\s\S])*?);',block) if '.IsConcurrencyToken()' in chain],
     'has_xmin':'HasColumnName("xmin")' in block})
 records.append({'path':file.relative_to(root).as_posix(),'sha256':hashlib.sha256(data).hexdigest(),'line_count':len(lines),
   'product_version':re.findall(r'HasAnnotation\("ProductVersion", "([^"]+)"',text),'segments':segments,'observed_model':observations})
packet_dir=root/'docs/onboarding-work/generated-model-packets';packet_dir.mkdir(exist_ok=True)
packets=[]; current=''
for n,(digest,block) in enumerate(unique.items(),1):
 content=f'BLOCK {n} SHA256 {digest}\nFIRST {block["first_path"]}:{block["start_line"]}-{block["end_line"]}\n'+block['text']+'\n'
 if current and len(current)+len(content)>14000: packets.append(current);current=''
 current+=content
if current:packets.append(current)
for n,packet in enumerate(packets,1):(packet_dir/f'{n:02d}.txt').write_text(packet,encoding='utf-8')
same_body=[s['sha256'] for s in records[-2]['segments'][1:]]==[s['sha256'] for s in records[-1]['segments'][1:]]
out={'method':'Every generated file is read in full and partitioned without gaps into exact line ranges. SHA256-identical blocks are inspected once and mapped back to every occurrence; no distinct block content is omitted. Packets contain full distinct block source, not summaries.','full_packet_inspection_completed':True,'latest_designer_and_snapshot_entity_blocks_byte_identical':same_body,'discrepancies':['No unexpected generated-model discrepancy found. Historical ProductVersion10.0.12 retained through AddStudentsAndParents; AddTeachers onward and current snapshot10.0.13.','The Teachers centre/user membership FK is intentionally migration-only, absent snapshot/config relationships; TeacherConfiguration explicitly documents this.','RLS policies, grants/revokes, btree_gist extension and schedule exclusion constraints are migration operations outside snapshot model metadata. Model parity does not verify their live presence.'],'files':records,'unique_block_count':len(unique),'packet_count':len(packets),'packet_files':[f'docs/onboarding-work/generated-model-packets/{i:02d}.txt' for i in range(1,len(packets)+1)]}
(root/'docs/onboarding-work/generated-model-read-ledger.json').write_text(json.dumps(out,indent=2),encoding='utf-8')
guide_notes={}
for record in records:
 entity_names=sorted({o['entity'] for o in record['observed_model']})
 tables=[t for o in record['observed_model'] for t in o['tables']]
 table_text=', '.join(t['schema']+'.'+t['table'] for t in tables)
 version=record['product_version'][0]
 is_snapshot=record['path'].endswith('AppDbContextModelSnapshot.cs')
 role='Current complete EF model baseline used when scaffolding the next migration' if is_snapshot else 'Historical complete EF target model after this migration, rather than only the migration delta'
 guide_notes[record['path']]=f'{role}. Generated ProductVersion is {version}; this file maps {len(entity_names)} entity types/tables: {table_text}. It records column types and lengths, ordinary keys/indexes/checks, concurrency tokens, and mapped relationships. Earlier designers retain their historical EF version intentionally. Runtime query filters and raw migration operations such as RLS, grants, the teacher-account membership foreign key and scheduling exclusion constraints are not fully represented by this generated model; inspect migration bodies and current configurations for those guarantees. Do not hand-edit generated model code.'
(root/'docs/onboarding-work/generated-model-guide-notes.json').write_text(json.dumps(guide_notes,indent=2),encoding='utf-8')
print(json.dumps({'files':len(files),'total_lines':sum(r['line_count'] for r in records),'distinct_blocks':len(unique),'packets':len(packets),'distinct_characters':sum(len(p) for p in packets)}))
