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
