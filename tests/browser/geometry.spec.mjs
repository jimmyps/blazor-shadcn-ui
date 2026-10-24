import {test,before,after} from 'node:test';
import assert from 'node:assert/strict';
import {chromium,webkit} from 'playwright';
import {readFile} from 'node:fs/promises';
const url=process.env.NEOUI_TEST_URL??'http://localhost:5198';
const floating=await readFile(new URL('./node_modules/@floating-ui/core/dist/floating-ui.core.umd.js',import.meta.url),'utf8')+'\n'+await readFile(new URL('./node_modules/@floating-ui/dom/dist/floating-ui.dom.umd.js',import.meta.url),'utf8');
let browser;before(async()=>browser=await(process.env.NEOUI_BROWSER_ENGINE==='webkit'?webkit.launch({headless:true}):chromium.launch({channel:'msedge',headless:true})));after(async()=>browser?.close());
for(const mode of ['Blazor','Browser'])for(const persistent of [true,false])for(const kind of ['select','popover'])test(`${mode}: ${kind} keeps matched width on every visible frame (mount=${persistent})`,async()=>{
 const page=await browser.newPage({viewport:{width:1210,height:800}}),errors=[];page.setDefaultTimeout(process.env.NEOUI_TEST_RENDERER==='wasm'?40000:15000);page.on('pageerror',e=>errors.push(String(e)));
 try{
  await page.addInitScript(floating);await page.goto(url+`/?geometry=true&mode=${mode.toLowerCase()}&mount=${persistent}`);
  await page.locator('[data-testid="test-host-ready"][data-ready="true"]').waitFor({state:'attached'});
  const trigger=page.getByTestId(`geometry-${kind}`),triggerId=await trigger.getAttribute('id'),id=await trigger.getAttribute('aria-controls');
  if(mode==='Browser')await page.locator(`[id="${id}"][data-neo-browser-content]`).waitFor({state:'attached'});
  const capture=async()=>page.evaluate(({id,triggerId})=>{
   window.geometryTrace={running:true,seen:false,frames:[]};const trace=window.geometryTrace;
   const sample=()=>{if(!trace.running)return;const content=document.getElementById(id),anchor=document.getElementById(triggerId),wrapper=content?.closest('[data-portal-content]');
    const visible=Boolean(wrapper&&getComputedStyle(wrapper).visibility==='visible'&&getComputedStyle(wrapper).display!=='none');
    if(visible)trace.seen=true;if(trace.seen)trace.frames.push({visible,width:wrapper?.getBoundingClientRect().width??0,anchor:anchor.getBoundingClientRect().width});requestAnimationFrame(sample);};requestAnimationFrame(sample);
  },{id,triggerId});
  const ready=async(opening)=>kind==='select'?page.waitForFunction(id=>document.activeElement?.id===id,id):page.getByTestId('geometry-ready').filter({hasText:String(opening)}).waitFor();
  const finish=async()=>{const frames=await page.evaluate(()=>{window.geometryTrace.running=false;return window.geometryTrace.frames;});assert.ok(frames.length>=5,'collect visible animation frames');const bad=frames.filter(frame=>!frame.visible||Math.abs(frame.width-frame.anchor)>1);assert.equal(bad.length,0,JSON.stringify({samples:frames.length,bad:bad.slice(0,8)}));};
  for(let opening=0;opening<2;opening++){
   await capture();await trigger.click();await page.waitForFunction(id=>{const wrapper=document.getElementById(id)?.closest('[data-portal-content]');return wrapper&&getComputedStyle(wrapper).visibility==='visible';},id);
   await ready(opening+1);await page.getByTestId('geometry-render').evaluate(el=>el.click());await page.getByTestId('geometry-render').filter({hasText:`Render ${opening+1}`}).waitFor();await page.waitForTimeout(650);await page.waitForFunction(()=>window.geometryTrace.frames.length>=8);await finish();
   await page.keyboard.press('Escape');await page.waitForFunction(id=>document.getElementById(id)?.getAttribute('aria-expanded')==='false',triggerId);await page.waitForFunction(({id,unmounted})=>{const content=document.getElementById(id);return !content||!unmounted&&getComputedStyle(content.closest('[data-portal-content]')).visibility==='hidden';},{id,unmounted:mode==='Blazor'&&!persistent});
  }
  await trigger.click();await page.waitForFunction(id=>{const wrapper=document.getElementById(id)?.closest('[data-portal-content]');return wrapper&&getComputedStyle(wrapper).visibility==='visible';},id);await ready(3);await page.getByTestId('geometry-resize').evaluate(el=>el.click());await page.waitForFunction(({id,triggerId})=>{const wrapper=document.getElementById(id)?.closest('[data-portal-content]'),anchor=document.getElementById(triggerId);return wrapper&&Math.abs(wrapper.getBoundingClientRect().width-anchor.getBoundingClientRect().width)<1&&Math.abs(anchor.getBoundingClientRect().width-440.333)<0.1;},{id,triggerId});
  await page.keyboard.press('Escape');await page.waitForFunction(id=>document.getElementById(id)?.getAttribute('aria-expanded')==='false',triggerId);await page.waitForFunction(({id,unmounted})=>{const content=document.getElementById(id);return !content||!unmounted&&getComputedStyle(content.closest('[data-portal-content]')).visibility==='hidden';},{id,unmounted:mode==='Blazor'&&!persistent});await page.getByTestId('geometry-render').click();await page.getByTestId('geometry-render').filter({hasText:'Render 3'}).waitFor();
  const closed=await page.evaluate(id=>{const content=document.getElementById(id);return !content||getComputedStyle(content.closest('[data-portal-content]')).visibility==='hidden';},id);assert.equal(closed,true);assert.deepEqual(errors,[]);
 }finally{await page.close();}
});
