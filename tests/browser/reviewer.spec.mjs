import {test,before,after} from 'node:test';
import assert from 'node:assert/strict';
import {chromium} from 'playwright';
import {readFile} from 'node:fs/promises';
const url=process.env.NEOUI_TEST_URL??'http://localhost:5198';
const floating=await readFile(new URL('./node_modules/@floating-ui/core/dist/floating-ui.core.umd.js',import.meta.url),'utf8')+'\n'+await readFile(new URL('./node_modules/@floating-ui/dom/dist/floating-ui.dom.umd.js',import.meta.url),'utf8');
let browser;before(async()=>browser=await chromium.launch({channel:'msedge',headless:true}));after(async()=>browser?.close());
async function open(legacy=false,setup){const page=await browser.newPage(),errors=[];page.on('pageerror',e=>errors.push(String(e)));page.setDefaultTimeout(30000);await page.addInitScript(floating);if(setup)await setup(page);await page.goto(url+'/?reviewer=true'+(legacy?'&legacy=true':''));await page.locator('[data-testid="test-host-ready"][data-ready="true"]').waitFor({state:'attached'});if(!legacy)await page.getByPlaceholder('Review plain').locator('xpath=self::*[@data-neo-input-ready="true"]').waitFor();return{page,errors};}
for(const label of ['Review date','Review time'])test('partial '+label+' blocks novalidate Save and direct field flush',async()=>{const{page,errors}=await open();try{const group=page.getByRole('group',{name:label,exact:true}),host=group.locator('xpath=ancestor::*[@data-neo-browser-field]');await host.locator('xpath=self::*[@data-neo-state-ready]').waitFor();await group.locator('[data-neo-segment]').first().press('Delete');const result=await host.evaluate(el=>el[Symbol.for('NeoUI.BrowserField')].flush(3000));assert.equal(result.status,'Invalid');await page.getByTestId('review-save').click();await page.getByTestId('review-failure').filter({hasText:'Invalid'}).waitFor();assert.equal(await page.getByTestId('review-saves').textContent(),'0');assert.deepEqual(errors,[]);}finally{await page.close();}});
test('invalid numeric draft reports a synchronization failure instead of silently suppressing Save',async()=>{const{page,errors}=await open();try{const input=page.getByPlaceholder('Review number');await input.locator('xpath=self::*[@data-neo-input-ready]').waitFor();await input.fill('-');await page.getByTestId('review-save').click();await page.getByTestId('review-failure').filter({hasText:'Invalid'}).waitFor();assert.equal(await page.getByTestId('review-saves').textContent(),'0');assert.notEqual(await input.evaluate(field=>field.validationMessage),'');assert.deepEqual(errors,[]);}finally{await page.close();}});
test('MultiSelect badge removal and clear buttons reach their handlers without toggling the shell',async()=>{const{page,errors}=await open();try{await page.getByTestId('review-seed-multi').click();await page.getByRole('button',{name:'Remove One',exact:true}).click();await page.getByTestId('review-selected').filter({hasText:'Two'}).waitFor();await page.getByRole('button',{name:'Clear all',exact:true}).click();await page.waitForFunction(()=>document.querySelector('[data-testid="review-selected"]').textContent==='');assert.equal(await page.getByRole('combobox').filter({hasText:'Review multi'}).getAttribute('aria-expanded'),'false');assert.deepEqual(errors,[]);}finally{await page.close();}});
test('changing provider mode recreates its subtree and preserves the circuit',async()=>{const{page,errors}=await open();try{for(const browserMode of [false,true]){await page.getByTestId('review-mode').click();const input=page.getByPlaceholder('Review plain');if(browserMode)await input.locator('xpath=self::*[@data-neo-input-ready]').waitFor();else await page.waitForFunction(()=>!document.querySelector('input[placeholder="Review plain"]').hasAttribute('data-neo-input-config'));}await page.getByTestId('review-alive').click();await page.getByTestId('review-alive').filter({hasText:'Alive 1'}).waitFor();assert.deepEqual(errors,[]);}finally{await page.close();}});
test('Blazor Input does not request Browser modules and disposal interop errors preserve independent actions',async()=>{const requested=[];const{page,errors}=await open(true,async page=>{page.on('request',request=>{if(/browser-input|markdown-editor/.test(request.url()))requested.push(request.url());});await page.route(/\/js\/input(?:\.[^/]+)?\.js(?:\?.*)?$/,async route=>{const response=await route.fetch();let source=await response.text();source=source.replace('export async function disposeInput(elementId)',"export async function disposeInput(elementId) { throw new Error('Expected disposal fault'); }\nasync function unusedDisposeInput(elementId)");await route.fulfill({response,body:source,contentType:'text/javascript'});});});try{await page.getByPlaceholder('Review plain').fill('Legacy draft');await page.getByTestId('review-toggle-input').click();await page.getByTestId('review-alive').click();await page.getByTestId('review-alive').filter({hasText:'Alive 1'}).waitFor();assert.deepEqual(requested,[]);assert.ok(errors.every(error=>error==='Error: Expected disposal fault'));}finally{await page.close();}});
test('cancelled range pointer preview restores its value without invoking an application callback',async()=>{const{page,errors}=await open();try{const group=page.getByRole('group',{name:'Review range'}),host=group.locator('xpath=ancestor::*[@data-neo-browser-field]');await host.locator('xpath=self::*[@data-neo-state-ready]').waitFor();const track=group.locator('[data-neo-field-range]'),box=await track.boundingBox();await page.mouse.move(box.x+box.width*.2,box.y+box.height/2);await page.mouse.down();await page.mouse.move(box.x+box.width*.4,box.y+box.height/2);await host.dispatchEvent('pointercancel',{pointerId:1});await page.mouse.up();assert.equal(await group.locator('[data-neo-field-thumb="0"]').getAttribute('aria-valuenow'),'20');assert.equal(await page.getByTestId('review-range-calls').textContent(),'0');assert.deepEqual(errors,[]);}finally{await page.close();}});
test('known disconnected field work is retained and a later flush sends it once without another edit',async()=>{const{page}=await open();try{await page.evaluate(async()=>{const connection=await import('/_content/NeoUI.Blazor.Primitives/js/primitives/browser-connection.js');connection.setConnectionAvailable(false);const module=await import('/_content/NeoUI.Blazor.Primitives/js/primitives/browser-field.js');const root=document.createElement('div');root.id='offline-field';root.dataset.neoStateHost=JSON.stringify({field:true,kind:'checkbox',values:[],version:1,notify:true,server:true});root.innerHTML='<button data-neo-field-item="checked">Check</button>';root.setAttribute('data-neo-browser-field','');document.body.append(root);window.__offlineCalls=0;window.__offlineController=module.initialize(root,{invokeMethodAsync:async(name,values)=>{window.__offlineCalls++;return{values};}});root.querySelector('button').click();});assert.equal(await page.evaluate(()=>window.__offlineCalls),0);assert.equal((await page.evaluate(()=>window.__offlineController.flush(100))).status,'Uncertain');await page.evaluate(async()=>{(await import('/_content/NeoUI.Blazor.Primitives/js/primitives/browser-connection.js')).setConnectionAvailable(true);});assert.equal((await page.evaluate(()=>window.__offlineController.flush(3000))).status,'Success');assert.equal(await page.evaluate(()=>window.__offlineCalls),1);}finally{await page.close();}});
test('initial Browser disclosures and active tab are visible in prerendered HTML before JavaScript',{skip:process.env.NEOUI_TEST_RENDERER==='wasm'},async()=>{const page=await browser.newPage({javaScriptEnabled:false});try{await page.goto(url+'/?reviewer=true');for(const id of ['review-initial-tab','review-initial-accordion','review-initial-collapse'])assert.equal(await page.getByTestId(id).isVisible(),true,id);assert.equal(await page.getByText('Other tab body',{exact:true}).isVisible(),false);}finally{await page.close();}});

test('positioning keeps scroll callbacks immediate and defers only observed element resizes',async()=>{
 const page=await browser.newPage();try{
  await page.addInitScript(floating);await page.route('**/reviewer-empty',route=>route.fulfill({contentType:'text/html',body:'<div id="anchor" style="width:200px;height:20px"></div><div id="floating">Panel</div>'}));await page.goto(url+'/reviewer-empty');
  const result=await page.evaluate(async()=>{
   let callback,resize,frames=0;const lib=window.FloatingUIDOM,nativeAuto=lib.autoUpdate,nativeObserver=window.ResizeObserver,nativeFrame=window.requestAnimationFrame;let options;
   lib.autoUpdate=(a,b,update,opts)=>{callback=update;options=opts;return()=>{};};window.ResizeObserver=class{constructor(cb){resize=cb;}observe(){}disconnect(){}};
   const module=await import('/_content/NeoUI.Blazor.Primitives/js/primitives/positioning.js');const handle=await module.autoUpdate(document.getElementById('anchor'),document.getElementById('floating'));
   window.requestAnimationFrame=()=>{frames++;return 1;};callback();await Promise.resolve();const scrollFrames=frames;resize([]);const resizeFrames=frames;handle.apply();lib.autoUpdate=nativeAuto;window.ResizeObserver=nativeObserver;window.requestAnimationFrame=nativeFrame;return{scrollFrames,resizeFrames,elementResize:options.elementResize};
  });assert.deepEqual(result,{scrollFrames:0,resizeFrames:1,elementResize:false});
 }finally{await page.close();}
});

test('known disconnected text input can flush after reconnect without another edit or duplicate callback',async()=>{const{page}=await open();try{
 const result=await page.evaluate(async()=>{const module=await import('/_content/NeoUI.Blazor/js/browser-input.js');module.setConnectionAvailable(false);const input=document.createElement('input');input.id='offline-text';input.dataset.neoInputConfig=JSON.stringify({value:'',revision:0,updateOn:'input',debounce:0,server:true});document.body.append(input);let calls=0;module.initialize(input,{invokeMethodAsync:async(name,value,sequence,revision)=>{calls++;return{value,sequence,revision};}});input.value='Offline draft';input.dispatchEvent(new Event('input',{bubbles:true}));await new Promise(resolve=>setTimeout(resolve,20));const before=calls;module.setConnectionAvailable(true);const flushed=await module.flushById(input.id,3000);return{before,after:calls,status:flushed.status,value:input.value};});assert.deepEqual(result,{before:0,after:1,status:'Success',value:'Offline draft'});
 }finally{await page.close();}});
test('tag suggestion and rejection callbacks honor the shared disconnected signal',async()=>{const{page}=await open();try{
 const calls=await page.evaluate(async()=>{const connection=await import('/_content/NeoUI.Blazor.Primitives/js/primitives/browser-connection.js');const module=await import('/_content/NeoUI.Blazor.Primitives/js/primitives/browser-field.js');connection.setConnectionAvailable(false);const root=document.createElement('div');root.id='offline-tags';root.setAttribute('data-neo-browser-field','');root.dataset.neoStateHost=JSON.stringify({field:true,kind:'tags',values:[],version:1,notify:true,server:true,fieldOptions:{search:true,debounce:0,contentId:'tag-suggestions',maxLength:2,maxTags:10,duplicates:false,triggers:1}});root.innerHTML='<div data-neo-tag-values></div><input data-neo-tag-input><span data-neo-tag-status></span>';document.body.append(root);let calls=0;module.initialize(root,{invokeMethodAsync:async()=>{calls++;return{values:[]};}});const input=root.querySelector('input');input.value='toolong';input.dispatchEvent(new Event('input',{bubbles:true}));await new Promise(resolve=>setTimeout(resolve,30));input.dispatchEvent(new KeyboardEvent('keydown',{key:'Enter',bubbles:true}));connection.setConnectionAvailable(true);return calls;});assert.equal(calls,0);
 }finally{await page.close();}});

test('a default-mode calendar removed by its selection callback does not focus a disposed element',async()=>{const{page,errors}=await open(true);try{const calendar=page.getByTestId('review-calendar');assert.ok(await calendar.locator('[data-neo-calendar-day][data-focused]').count()<=1);await calendar.locator('[data-neo-calendar-day]').first().click();await calendar.waitFor({state:'detached'});await page.getByTestId('review-alive').click();await page.getByTestId('review-alive').filter({hasText:'Alive 1'}).waitFor();assert.deepEqual(errors,[]);}finally{await page.close();}});

test('rejected tab observations do not mask a later application command or replay its callback',async()=>{const{page,errors}=await open();try{const root=page.getByTestId('review-echo-tabs');await root.locator('[data-neo-state-ready]').waitFor({state:'attached'});for(const[name,count]of[['Rejected second tab','1'],['Rejected third tab','2']]){await root.getByRole('tab',{name,exact:true}).click();await page.getByTestId('review-tab-reports').filter({hasText:count}).waitFor();}await page.getByTestId('review-tab-command').click();await root.getByRole('tab',{name:'Rejected second tab',exact:true}).locator('xpath=self::*[@aria-selected="true"]').waitFor();assert.equal(await root.getByRole('tab',{name:'Rejected third tab',exact:true}).getAttribute('aria-selected'),'false');assert.equal(await page.getByTestId('review-tab-reports').textContent(),'2');assert.deepEqual(errors,[]);}finally{await page.close();}});


async function openLayers(legacyTags=false){
 const page=await browser.newPage(),errors=[];page.on('pageerror',e=>errors.push(String(e)));page.setDefaultTimeout(30000);await page.addInitScript(floating);await page.goto(url+'/?reviewlayers=true'+(legacyTags?'&legacytags=true':''));await page.locator('[data-testid="test-host-ready"][data-ready="true"]').waitFor({state:'attached'});await page.getByPlaceholder('Review layer ready').locator('xpath=self::*[@data-neo-input-ready]').waitFor();return{page,errors};
}

test('Escape on a focused Browser popover trigger dismisses its shell before the enclosing dialog',async()=>{
 const{page,errors}=await openLayers();try{
  await page.getByRole('button',{name:'Review escape dialog',exact:true}).click();const dialog=page.getByRole('dialog');await dialog.waitFor();
  const trigger=dialog.getByRole('button',{name:'Review escape popover',exact:true});const content=page.locator('[id="'+await trigger.getAttribute('aria-controls')+'"]');await content.locator('xpath=self::*[@data-neo-browser-content]').waitFor({state:'attached'});
  await trigger.click();await content.waitFor();await trigger.focus();await trigger.press('Escape');await content.waitFor({state:'hidden'});assert.equal(await dialog.isVisible(),true);assert.equal(await trigger.getAttribute('aria-expanded'),'false');assert.equal(await trigger.evaluate(el=>el===document.activeElement),true);
  await trigger.click();await content.waitFor();const inner=content.getByRole('button',{name:'Review inner popover',exact:true}),innerContent=page.locator('[id="'+await inner.getAttribute('aria-controls')+'"]');await inner.click();await innerContent.waitFor();await inner.focus();await inner.press('Escape');await innerContent.waitFor({state:'hidden'});assert.equal(await content.isVisible(),true);assert.equal(await dialog.isVisible(),true);
  await trigger.focus();await trigger.press('Escape');await content.waitFor({state:'hidden'});await trigger.press('Escape');await dialog.waitFor({state:'hidden'});assert.deepEqual(errors,[]);
 }finally{await page.close();}
});

test('Browser RangeSlider mirrors horizontal RTL pointer, fill and arrow behavior while vertical stepping stays unchanged',async()=>{
 const{page,errors}=await openLayers();try{
  const group=page.getByRole('group',{name:'Review range',exact:true}),host=group.locator('xpath=ancestor::*[@data-neo-browser-field]');await host.locator('xpath=self::*[@data-neo-state-ready]').waitFor();await group.evaluate(el=>el.dir='rtl');
  const low=group.locator('[data-neo-field-thumb="0"]');await low.focus();await low.press('ArrowLeft');assert.equal(await low.getAttribute('aria-valuenow'),'21');await low.press('ArrowRight');assert.equal(await low.getAttribute('aria-valuenow'),'20');
  const track=group.locator('[data-neo-field-range]'),box=await track.boundingBox();await page.mouse.click(box.x+box.width*.7,box.y+box.height/2);assert.equal(await low.getAttribute('aria-valuenow'),'30');
  assert.match(await low.getAttribute('style'),/70%/);assert.match(await group.locator('[data-neo-field-fill]').getAttribute('style'),/left: 20%/);
  const vertical=page.getByRole('group',{name:'Review vertical range',exact:true}).locator('[data-neo-field-thumb="0"]');await vertical.focus();await vertical.press('ArrowUp');assert.equal(await vertical.getAttribute('aria-valuenow'),'21');assert.match(await vertical.getAttribute('style'),/bottom: calc\(21%/);assert.deepEqual(errors,[]);
 }finally{await page.close();}
});

test('a portaled TagInput suggestion belongs to its Browser popover and outside clicks still dismiss it',async()=>{
 const{page,errors}=await openLayers();try{
  const trigger=page.getByRole('button',{name:'Review tag popover',exact:true}),content=page.locator('[id="'+await trigger.getAttribute('aria-controls')+'"]');await content.locator('xpath=self::*[@data-neo-browser-content]').waitFor({state:'attached'});await trigger.click();await content.waitFor();
  const input=page.getByPlaceholder('Review nested tags');await input.locator('xpath=ancestor::*[@data-neo-state-ready]').waitFor();await page.waitForFunction(()=>Boolean(document.getElementById(document.querySelector('[placeholder="Review nested tags"]').getAttribute('aria-controls'))));await input.fill('Des');await page.getByRole('option',{name:'Design',exact:true}).click();
  assert.equal(await trigger.getAttribute('aria-expanded'),'true');assert.equal(await content.isVisible(),true);assert.match(await content.locator('[data-neo-tag-values]').textContent(),/Design/);
  await page.getByTestId('review-tags').filter({hasText:'Design'}).waitFor();await page.getByTestId('review-tag-reset').click();await content.waitFor({state:'hidden'});await trigger.click();await content.waitFor();await content.locator('[data-neo-tag-values]').filter({hasText:'Development'}).waitFor();assert.match(await content.locator('[data-neo-tag-values]').textContent(),/Development/);
  await page.getByTestId('review-alive').click();await content.waitFor({state:'hidden'});await page.getByTestId('review-alive').filter({hasText:'Alive 1'}).waitFor();assert.deepEqual(errors,[]);
 }finally{await page.close();}
});


test('disposing a connected input during Browser import invalidates initialization and a fresh lifetime attaches once',async()=>{
 const page=await browser.newPage();let release,requested;const gate=new Promise(resolve=>release=resolve),seen=new Promise(resolve=>requested=resolve);
 try{
  await page.route('**/review-input-empty',route=>route.fulfill({contentType:'text/html',body:'<input id="review-import-input">'}));
  await page.route('**/_content/NeoUI.Blazor/js/browser-input.js',async route=>{requested();await gate;await route.continue();});
  await page.goto(url+'/review-input-empty');await page.evaluate(async()=>{
   const input=document.getElementById('review-import-input');input.dataset.neoInputConfig=JSON.stringify({value:'',revision:0,updateOn:'input',debounce:0,server:false});
   window.__importOldCalls=0;window.__importNewCalls=0;window.__inputModule=await import('/_content/NeoUI.Blazor/js/input.js');
   window.__pendingInputInit=window.__inputModule.initializeInput(input.id,'input',0,{invokeMethodAsync:async()=>{window.__importOldCalls++;throw new Error('Disposed bridge used');}},false,true);
  });await seen;await page.evaluate(()=>window.__inputModule.disposeInput('review-import-input'));release();await page.evaluate(()=>window.__pendingInputInit);
  assert.equal(await page.locator('#review-import-input').getAttribute('data-neo-input-ready'),null);
  const result=await page.evaluate(async()=>{
   const input=document.getElementById('review-import-input');await window.__inputModule.initializeInput(input.id,'input',0,{invokeMethodAsync:async(name,value,sequence,revision)=>{window.__importNewCalls++;return{value,sequence,revision};}},false,true);
   input.value='Fresh lifetime';input.dispatchEvent(new Event('input',{bubbles:true}));const module=await import('/_content/NeoUI.Blazor/js/browser-input.js');const flush=await module.flushById(input.id,3000);return{old:window.__importOldCalls,fresh:window.__importNewCalls,status:flush.status};
  });assert.deepEqual(result,{old:0,fresh:1,status:'Success'});
 }finally{release();await page.close();}
});

for(const base of ['/','/review-base/'])test('connection module has one application-base URL across asset aliases: '+base,async()=>{
 const page=await browser.newPage(),requests=[];try{
  const source=await readFile(new URL('../../src/NeoUI.Blazor/wwwroot/js/browser-input.js',import.meta.url),'utf8');
  const connection=await readFile(new URL('../../src/NeoUI.Blazor.Primitives/wwwroot/js/primitives/browser-connection.js',import.meta.url),'utf8');
  await page.route('**/review-connection-empty',route=>route.fulfill({contentType:'text/html',body:'<base href="'+base+'">'}));
  await page.route('**/review-alias/browser-input.js',route=>route.fulfill({contentType:'text/javascript',body:source.replace("from './browser-input-formats.js'","from '/_content/NeoUI.Blazor/js/browser-input-formats.js'")}));
  await page.route('**'+base+'_content/NeoUI.Blazor.Primitives/js/primitives/browser-connection.js',route=>{requests.push(route.request().url());return route.fulfill({contentType:'text/javascript',body:connection});});
  await page.goto(url+'/review-connection-empty');const result=await page.evaluate(async()=>{
   const canonical=await import('/_content/NeoUI.Blazor/js/browser-input.js'),alias=await import('/review-alias/browser-input.js');alias.setConnectionAvailable(false);const down=document.documentElement.dataset.neoConnectionAvailable;canonical.setConnectionAvailable(true);return{same:canonical.setConnectionAvailable===alias.setConnectionAvailable,down,up:document.documentElement.dataset.neoConnectionAvailable};
  });assert.deepEqual(result,{same:true,down:'false',up:'true'});assert.equal(requests.length,1);assert.equal(new URL(requests[0]).pathname,base+'_content/NeoUI.Blazor.Primitives/js/primitives/browser-connection.js');
 }finally{await page.close();}
});


test('a Blazor TagInput suggestion stays inside a Browser popover and preserves live server callbacks',async()=>{
 const{page,errors}=await openLayers(true);try{
  const trigger=page.getByRole('button',{name:'Review tag popover',exact:true}),content=page.locator('[id="'+await trigger.getAttribute('aria-controls')+'"]');await content.locator('xpath=self::*[@data-neo-browser-content]').waitFor({state:'attached'});await trigger.click();await content.waitFor();
  const input=page.getByPlaceholder('Review nested tags');assert.equal(await input.locator('xpath=ancestor::*[@data-neo-browser-field]').count(),0);await input.fill('Des');await page.getByRole('option',{name:'Design',exact:true}).click();await page.getByTestId('review-tags').filter({hasText:'Design'}).waitFor();assert.equal(await trigger.getAttribute('aria-expanded'),'true');await page.getByTestId('review-alive').click();await content.waitFor({state:'hidden'});await page.getByTestId('review-alive').filter({hasText:'Alive 1'}).waitFor();assert.deepEqual(errors,[]);
 }finally{await page.close();}
});

for (const mode of ['default', 'blazor', 'browser']) test(`same-batch dialog and nested popover content stays current (${mode})`,async()=>{
 const page=await browser.newPage(),errors=[];page.on('pageerror',e=>errors.push(String(e)));page.setDefaultTimeout(10000);await page.addInitScript(floating);
 try{
  await page.goto(url+'/?portalrefresh=true'+(mode==='default'?'':'&mode='+mode));await page.locator('[data-testid="test-host-ready"][data-ready="true"]').waitFor({state:'attached'});
  await page.getByRole('button',{name:'Open refresh dialog',exact:true}).click();await page.getByTestId('refresh-leaf').filter({hasText:'Leaf 0'}).waitFor();
  for(let revision=1;revision<=5;revision++){
   await page.getByTestId('refresh-all').click();
   // No second server event or independent host refresh may rescue stale content.
   await page.waitForFunction(r=>['dialog','parent','leaf'].every(id=>document.querySelector(`[data-testid="refresh-${id}"]`)?.textContent.trim()===`${id==='dialog'?'Dialog':id==='parent'?'Parent':'Leaf'} ${r}`),revision);
  }
  if(mode==='blazor') assert.equal(await page.locator('[data-neo-browser-content]').count(),0);
  for(let revision=6;revision<=8;revision++){
   await page.getByTestId('refresh-late').click();
   await page.waitForFunction(r=>['dialog','parent','leaf','late-leaf'].every(id=>document.querySelector(`[data-testid="refresh-${id}"]`)?.textContent.trim()===`${id==='dialog'?'Dialog':id==='parent'?'Parent':id==='leaf'?'Leaf':'Late'} ${r}`),revision);
  }
  await page.getByTestId('refresh-late-only').click();
  await page.waitForFunction(()=>document.querySelector('[data-testid=refresh-late-leaf]')?.textContent.trim()==='Late 9');
  assert.deepEqual(errors,[]);
 }finally{await page.close();}
});
