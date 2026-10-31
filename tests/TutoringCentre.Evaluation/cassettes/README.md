# Real-model cassettes

E1.json through E15.json contain genuine local Ollama responses from `qwen3:4b-instruct-2507-q4_K_M` (model digest prefix `0edcdef34593`), recorded on 2026-10-31. All requests contain synthetic fixture data. The twenty request/response frames are the first complete full-dataset recording, without response selection or scripted replacements.

This baseline intentionally preserves model-quality failures in E3 (Franco attendance) and E11 (centre-wide student request). Both security gates pass, but replay fails the unchanged quality floors. Do not treat these cassettes as a passing evaluation baseline or weaken thresholds to make CI green. A later temperature-zero full-dataset attempt performed worse and is retained only in ignored local artifacts.

Missing/stale recordings deliberately fail replay. Scripted selftests never populate this directory. See `docs/ai/day85-87-verification.md` for validation evidence and the remaining quality blocker.
