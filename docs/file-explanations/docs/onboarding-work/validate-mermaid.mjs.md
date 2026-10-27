# `docs/onboarding-work/validate-mermaid.mjs`

## Purpose

Current script file. The source excerpt and structural map below expose its content without inferring behaviour from its filename.

## Where it fits

Category: **script**. [Open current file](../../../onboarding-work/validate-mermaid.mjs). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 23 lines and 1,301 bytes. Source hash: `b1494dfde52bcf6b59cfea26620478d43bb4f1ed5ef71bb2e5ba57d0c6a97e3b`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `import fs from 'node:fs';`
- Line 2: `import path from 'node:path';`
- Line 3: `import { fileURLToPath } from 'node:url';`
- Line 4: `import { JSDOM } from '../../frontend/node_modules/jsdom/lib/api.js';`

## Walkthrough and declaration map

Read the data or directives in order; there are no recognized code declarations in this file.

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

````text
import fs from 'node:fs';
import path from 'node:path';
import { fileURLToPath } from 'node:url';
import { JSDOM } from '../../frontend/node_modules/jsdom/lib/api.js';

const here=path.dirname(fileURLToPath(import.meta.url));
const root=path.resolve(here,'../..');
const dom=new JSDOM('<!doctype html><html><body></body></html>');
globalThis.window=dom.window;
globalThis.document=dom.window.document;
const {default:mermaid}=await import('./.mermaid-tools/node_modules/mermaid/dist/mermaid.esm.mjs');
mermaid.initialize({startOnLoad:false,securityLevel:'strict'});
const file=path.join(root,'docs/PROJECT_OVERVIEW3.md');
const text=fs.readFileSync(file,'utf8');
const results=[];
for(const [index,match] of [...text.matchAll(/^```mermaid\r?\n([\s\S]*?)^```\s*$/gm)].entries()){
  const line=text.slice(0,match.index).split('\n').length;
  try{await mermaid.parse(match[1]);results.push({diagram:index+1,line,passed:true});}
  catch(error){results.push({diagram:index+1,line,passed:false,error:String(error)});}
}
fs.writeFileSync(path.join(here,'mermaid-results.json'),JSON.stringify(results,null,2)+'\n');
console.log(JSON.stringify({diagrams:results.length,passed:results.filter(x=>x.passed).length,failures:results.filter(x=>!x.passed)},null,2));
if(results.some(x=>!x.passed))process.exitCode=1;
````

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
