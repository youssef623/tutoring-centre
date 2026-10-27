# Onboarding audit working record

This directory contains reproducible evidence for the current repository onboarding audit. These are working materials, not a replacement for `../PROJECT_OVERVIEW3.md`.

## Requested acceptance conditions

- Preserve `docs/PROJECT_OVERVIEW.md` and `docs/PROJECT_OVERVIEW2.md` byte for byte.
- Reverify repository statements from current files; distinguish source reading, tests actually executed, and interpretations.
- Produce a standalone, detailed seventeen-section `PROJECT_OVERVIEW3.md`, with at least 35 fully taught concepts, no shorter than v2 (4,018 lines, 57,697 whitespace-separated words).
- Rebuild the complete file index and important per-file explanations.
- Verify local Markdown targets and anchors, Mermaid syntax, factual findings, and the historical comparison.
- Commit and push the current branch; do not create a pull request or wait for CI.

## Baseline

The last commit touching v2 is `9d638ceb46099a4fd87ba8bc901b10e947c1f737`. The starting HEAD is `ef486f9`. `git diff --stat` reports 893 changed files, 90,595 insertions and 771 deletions. The detailed benchmark, including every concept's headings and sizes, is in `v2-benchmark.json`. The baseline has fifteen numbered top-level sections, eighteen level-one headings including its title and appendices, 35 concepts, 48 labelled fenced blocks, and twenty Mermaid diagrams.

## Evidence files

`inventory.py` creates `inventory.json` and `inventory-exclusions.json`. The inventory records paths, categories, sizes, hashes, tracking status, and sensitive-file metadata. It never reproduces `.env` content. It excludes dependencies, caches, compiled output and local editor state explicitly. The final index distinguishes its own documentation files from the application inventory to avoid recursive explanation generation.

Independent backend, frontend, and test/infrastructure drafts are being prepared in this directory. Their claims require verification before integration. A machine-read ledger proves which bytes were opened; it does not prove that every statement was manually reviewed. Executed commands and actual outcomes must be recorded separately from source-derived descriptions.

## Completed work and evidence limits

The v2 document was read as the teaching benchmark. Current README, composition root, dispatcher, infrastructure registration, unit of work, database context, permission catalogue, role map, authentication endpoints, session revalidation, transaction locks, scheduling generator, tenant conventions and audit interceptors were independently checked against current source. The backend ledger records full text inspection of nongenerated sources; the generated-model ledger covers all designers and the snapshot using full distinct blocks and exact duplicate verification. Frontend, test/infrastructure and all-file extraction ledgers distinguish structural opening from executed behavior. Baseline overview files remain unchanged. The initial recursive inventory contained 1,352 files before new explanations were generated, including the then-existing explanation documentation.

The overview has all seventeen required top-level sections and sixty-two teaching concepts. Every concept has the nine required teaching headings. `depth_audit.py` maps all thirty-five old concept topics to the current standalone treatment and verifies their word-count floors; a size comparison is a structural gate, not a claim that counting words alone proves teaching quality. `validate_docs.py` checks local targets, anchors and source-line bounds. `validate-mermaid.mjs` uses Mermaid's real parser, not a regular-expression approximation.

Release build, all 1,873 backend tests, model/snapshot agreement, frontend typecheck/build/translation parity, bounded-worker 278 component cases and nineteen Chromium journeys passed. Lint warnings and the unsuccessful initial default-concurrency frontend run remain recorded. Browser checks used disposable isolated PostgreSQL and test-owned servers, all cleaned up. Cloud deployment and production-image/Caddy smoke execution were not performed during this audit. The separately existing PDF is not an output of this Markdown documentation task.

The file-guide generator uses manually verified purpose paragraphs where supplied and labels extracted source comments as intent. Declaration and consumer maps are lexical evidence, not a claim that every recognized name is a runtime call. This distinction avoids inventing a call graph while still making every file navigable. The report and guide trees are generated from the retained authoring/evidence files; local parser dependencies and temporary generated-model packets are ignored.

Final commit and push are reported in the task response rather than embedding a self-referential commit hash in these files.
