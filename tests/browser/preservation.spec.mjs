import { test, before, after } from 'node:test';
import assert from 'node:assert/strict';
import { chromium, webkit } from 'playwright';
import { readFile } from 'node:fs/promises';
const url=process.env.NEOUI_TEST_URL??'http://localhost:5198';
const floating=await readFile(new URL('./node_modules/@floating-ui/core/dist/floating-ui.core.umd.js',import.meta.url),'utf8')+'\n'+await readFile(new URL('./node_modules/@floating-ui/dom/dist/floating-ui.dom.umd.js',import.meta.url),'utf8');
let browser;
before(async()=>{browser=await(process.env.NEOUI_BROWSER_ENGINE==='webkit'?webkit.launch({headless:true}):chromium.launch({channel:'msedge',headless:true}));});
after(async()=>await browser?.close());
async function open(mode,pageOptions={}){
 const page=await browser.newPage(pageOptions);page.setDefaultTimeout(process.env.NEOUI_TEST_RENDERER==='wasm'?40000:12000);
 const errors=[];page.on('pageerror',error=>errors.push(String(error)));
 await page.addInitScript(floating);
 await page.addInitScript(()=>{const add=EventTarget.prototype.addEventListener;EventTarget.prototype.addEventListener=function(type,...args){const result=add.call(this,type,...args);if(this instanceof HTMLInputElement&&['paste','change'].includes(type))this.dataset['preserve'+type]= 'true';return result;};});
 await page.goto(url+`/?preservation=true&mode=${mode.toLowerCase()}`);
 await page.locator('[data-testid="test-host-ready"][data-ready="true"]').waitFor({state:'attached'});
 const mask=page.getByPlaceholder('Preserved mask',{exact:true});
 await mask.locator(mode==='Browser'?'xpath=self::*[@data-neo-input-ready="true"]':'xpath=self::*[@data-preservepaste="true"]').waitFor();
 return {page,errors};
}
async function close(page,errors){await page.evaluate(()=>{if(window.__nativeSend)WebSocket.prototype.send=window.__nativeSend;}).catch(()=>{});await page.close();assert.deepEqual(errors,[]);}
async function block(page){if(process.env.NEOUI_TEST_RENDERER!=='wasm')await page.evaluate(()=>{window.__nativeSend=WebSocket.prototype.send;WebSocket.prototype.send=function(){};});}
async function value(input,text){await input.evaluate(el=>{for(const other of document.querySelectorAll('[data-preserve-watch]'))delete other.dataset.preserveWatch;el.dataset.preserveWatch='current';});await input.page().waitForFunction(text=>document.querySelector('[data-preserve-watch="current"]')?.value===text,text);}
async function paste(input,text){await input.evaluate((el,text)=>{const data=new DataTransfer();data.setData('text',text);el.dispatchEvent(new ClipboardEvent('paste',{bubbles:true,cancelable:true,clipboardData:data}));},text);}
const initialMoney='12.34567|98.76543|123.456|1.234567|6.78901|3.45678';
for(const mode of ['Blazor','Browser']){
 test(`${mode}: existing mask engine preserves templates, literal digits, caret and selected paste`,async()=>{
  const {page,errors}=await open(mode);try{
   if(mode==='Browser')await block(page);
   const mask=page.getByPlaceholder('Preserved mask',{exact:true});await mask.focus();await value(mask,'___-___');
   for(const [key,text,caret] of [['1','1__-___',1],['2','12_-___',2],['3','123-___',4],['4','123-4__',5],['5','123-45_',6],['6','123-456',7]]){await mask.press(key);assert.equal(await mask.inputValue(),text);assert.equal(await mask.evaluate(el=>el.selectionStart),caret);}
   await mask.evaluate(el=>{el.setSelectionRange(3,3);el.dispatchEvent(new MouseEvent('click',{bubbles:true}));});await page.waitForFunction(()=>document.querySelector('[placeholder="Preserved mask"]').selectionStart===4);
   await mask.evaluate(el=>el.setSelectionRange(1,5));await paste(mask,'98');assert.equal(await mask.inputValue(),'198-56_');assert.equal(await mask.evaluate(el=>el.selectionStart),4);
   const prefix=page.getByPlaceholder('Literal digit mask',{exact:true});await prefix.focus();await prefix.fill('12345');assert.equal(await prefix.inputValue(),'12345');
   const phone=page.getByPlaceholder('Phone mask',{exact:true});await phone.focus();await value(phone,'+1 (___) ___-____');await paste(phone,'+1 (234) 567-8901');assert.equal(await phone.inputValue(),'+1 (234) 567-8901');
   const compact=page.getByPlaceholder('Compact mask',{exact:true});await compact.focus();await value(compact,'');await compact.press('A');assert.equal(await compact.inputValue(),'A');await compact.press('B');assert.equal(await compact.inputValue(),'AB-');
   const custom=page.getByPlaceholder('Custom mask',{exact:true});await custom.focus();await value(custom,'···-···');await custom.press('Z');assert.equal(await custom.inputValue(),'Z··-···');
   const readonly=page.getByPlaceholder('Readonly mask',{exact:true});await readonly.focus();await readonly.press('Backspace');await paste(readonly,'999');assert.equal(await readonly.inputValue(),'123-456');
  }finally{await close(page,errors);}
 });
 test(`${mode}: currency focus and untouched blur preserve exact values across precision and culture settings`,async()=>{
  const {page,errors}=await open(mode);try{
   for(const [name,raw] of [['Precise USD','12.34567'],['Precise EUR',mode==='Browser'?'98,76543':'98.76543'],['Precise JPY','123.456'],['Precise KWD','1.234567'],['Precise plain','6.78901']]){
    const input=page.getByPlaceholder(name,{exact:true});await input.locator(mode==='Browser'?'xpath=self::*[@data-neo-input-ready="true"]':'xpath=self::*[@data-preservechange="true"]').waitFor();const display=await input.inputValue();
    for(let count=0;count<2;count++){await input.focus();await value(input,raw);await input.blur();await value(input,display);}
   }
   assert.equal(await page.getByTestId('money-model').textContent(),initialMoney);
   await page.getByTestId('preserve-save').click();await page.getByTestId('money-saved').filter({hasText:initialMoney}).waitFor();
   await page.getByTestId('preserve-reset').click();const usd=page.getByPlaceholder('Precise USD',{exact:true});await value(usd,'$54.32');await usd.focus();await value(usd,'54.32109');
  }finally{await close(page,errors);}
 });
 test(`${mode}: tag container focus, keyboard blur and portaled suggestion selection preserve local UX`,async()=>{
  const {page,errors}=await open(mode);try{
   await page.getByPlaceholder('Preserved tags',{exact:true}).evaluate(el=>el.dataset.preservedTagInput='true');
   const input=page.locator('[data-preserved-tag-input]');
   if(mode==='Browser')await input.locator('xpath=ancestor::*[@data-neo-state-ready="true"][1]').waitFor();
   else await input.locator('xpath=self::*[@data-preservepaste="true"]').waitFor();
   if(mode==='Browser')await block(page);
   await page.getByTestId('after-tags').focus();await input.locator('xpath=..').evaluate(el=>el.click());assert.equal(await input.evaluate(el=>document.activeElement===el),true);
   await input.fill('Des');const option=page.getByRole('option',{name:'Design',exact:true});await option.waitFor();await input.press('Tab');await option.waitFor({state:'hidden'});assert.notEqual(await input.getAttribute('aria-expanded'),'true');
   await input.fill('Des');await option.waitFor();await option.click();await value(input,'');
   assert.equal(await input.evaluate(el=>document.activeElement===el),true);
  }finally{await close(page,errors);}
 });
 test(`${mode}: navigation retains Tailwind entry and directional motion`,async()=>{
  const {page,errors}=await open(mode);try{
   if(mode==='Browser'){await page.locator('[data-neo-navigation-ready="true"]').waitFor({state:'attached'});await block(page);}
   const first=page.getByRole('menuitem',{name:'Preserved products',exact:true}),second=page.getByRole('menuitem',{name:'Preserved resources',exact:true});
   const firstPanel=page.locator(`[id="${await first.getAttribute('aria-controls')}"]`),secondPanel=page.locator(`[id="${await second.getAttribute('aria-controls')}"]`);
   await firstPanel.evaluate(el=>el.style.animationDuration='2s');await secondPanel.evaluate(el=>el.style.animationDuration='2s');
   await first.click();await page.waitForFunction(id=>document.getElementById(id)?.dataset.state==='open',await firstPanel.getAttribute('id'));
   assert.equal(await firstPanel.getAttribute('data-motion'),'');assert.notEqual(await firstPanel.evaluate(el=>getComputedStyle(el).animationName),'none');
   await second.click();await page.waitForFunction(id=>document.getElementById(id)?.dataset.motion==='from-end',await secondPanel.getAttribute('id'));
   assert.notEqual(await secondPanel.evaluate(el=>getComputedStyle(el).animationName),'none');
   if(mode==='Browser'){
    assert.equal(await firstPanel.getAttribute('data-motion'),'to-start');assert.equal(await firstPanel.evaluate(el=>el.inert),true);
    assert.notEqual(await firstPanel.evaluate(el=>getComputedStyle(el).display),'none');
    // Reopening invalidates the pending first exit; completing old motion must not hide it.
    await first.click();await firstPanel.evaluate(async el=>{for(const animation of el.getAnimations())animation.finish();await Promise.resolve();});assert.notEqual(await firstPanel.evaluate(el=>getComputedStyle(el).display),'none');
    await first.press('Escape');await firstPanel.evaluate(el=>{for(const animation of el.getAnimations())animation.finish();});await page.waitForFunction(id=>getComputedStyle(document.getElementById(id)).display==='none',await firstPanel.getAttribute('id'));
    await firstPanel.evaluate(el=>el.style.animationDuration='0s');await first.click();await first.press('Escape');assert.equal(await firstPanel.evaluate(el=>getComputedStyle(el).display),'none');
   }
  }finally{await close(page,errors);}
 });
}

test('Browser: mask mobile deletion, composition, resets and change boundaries use one notification path',async()=>{
 const {page,errors}=await open('Browser');try{
  const input=page.getByPlaceholder('Preserved mask',{exact:true});await input.fill('123456');assert.equal(await input.inputValue(),'123-456');
  await input.evaluate(el=>{el.setSelectionRange(4,4);el.dispatchEvent(new InputEvent('beforeinput',{inputType:'deleteContentBackward',bubbles:true,cancelable:true}));});assert.equal(await input.inputValue(),'124-56_');assert.equal(await input.evaluate(el=>el.selectionStart),2);
  await input.evaluate(el=>{el.dispatchEvent(new CompositionEvent('compositionstart',{bubbles:true}));el.value='987654';el.dispatchEvent(new InputEvent('input',{bubbles:true,isComposing:true}));});assert.equal(await input.inputValue(),'987654');await input.evaluate(el=>el.dispatchEvent(new CompositionEvent('compositionend',{bubbles:true})));assert.equal(await input.inputValue(),'987-654');
  await input.blur();await page.getByTestId('mask-model').filter({hasText:'987654'}).waitFor();await page.getByTestId('preserve-reset').click();await value(input,'');
  const prefix=page.getByPlaceholder('Literal digit mask',{exact:true});await value(prefix,'12678');
 }finally{await close(page,errors);}
});

test('Browser: exact currency metadata survives delayed acceptance and immediate form Save',async()=>{
 const {page,errors}=await open('Browser');try{
  const input=page.getByPlaceholder('Delayed currency',{exact:true});await input.locator('xpath=self::*[@data-neo-input-ready="true"]').waitFor();await input.focus();await value(input,'3.45678');
  await input.fill('23.4');await input.fill('23.456789');await page.getByTestId('preserve-save').click();await page.getByTestId('money-saved').filter({hasText:'|23.456789'}).waitFor();
  await input.focus();await value(input,'23.456789');await input.blur();await value(input,'$23.46');assert.match(await page.getByTestId('money-model').textContent(),/\|23\.456789$/);
 }finally{await close(page,errors);}
});

test('Browser: explicitly typing the rounded display is an edit rather than an untouched exact-value alias',async()=>{
 const {page,errors}=await open('Browser');try{
  const input=page.getByPlaceholder('Precise plain',{exact:true});await input.locator('xpath=self::*[@data-neo-input-ready="true"]').waitFor();
  await input.focus();await value(input,'6.78901');await input.fill('6.79');await input.blur();
  await page.waitForFunction(()=>document.querySelector('[data-testid="money-model"]').textContent.split('|')[4]==='6.79');
  await input.focus();await value(input,'6.79');
 }finally{await close(page,errors);}
});

test('shared Markdown history survives queued frames after undo and document reset',async()=>{
 const {page,errors}=await open('Browser');try{
  const input=page.getByPlaceholder('Preserved markdown',{exact:true});await input.locator('xpath=self::*[@data-neo-input-ready="true"]').waitFor();await block(page);
  const result=await input.evaluate(async textarea=>{
   const {resetHistory}=await import(new URL('_content/NeoUI.Blazor/js/markdown-editor.js',document.baseURI).href);
   const nativeRAF=window.requestAnimationFrame,frames=[];window.requestAnimationFrame=callback=>(frames.push(callback),frames.length);
   const key=value=>textarea.dispatchEvent(new KeyboardEvent('keydown',{key:value,ctrlKey:true,bubbles:true,cancelable:true}));
   try{textarea.value='draft';textarea.setSelectionRange(0,5);textarea.dispatchEvent(new Event('input',{bubbles:true}));key('b');const bold=textarea.value;key('z');const undo=textarea.value;for(const frame of frames.splice(0))frame(performance.now());key('y');const redo=textarea.value;
    textarea.value='New document';resetHistory(textarea);for(const frame of frames.splice(0))frame(performance.now());key('z');const reset=textarea.value;textarea.dispatchEvent(new CompositionEvent('compositionstart',{bubbles:true}));textarea.value='Composed document';textarea.dispatchEvent(new InputEvent('input',{bubbles:true,isComposing:true}));textarea.dispatchEvent(new CompositionEvent('compositionend',{bubbles:true}));key('z');const imeUndo=textarea.value;key('y');return {bold,undo,redo,reset,imeUndo,imeRedo:textarea.value};
   }finally{window.requestAnimationFrame=nativeRAF;}
  });assert.deepEqual(result,{bold:'**draft**',undo:'draft',redo:'**draft**',reset:'New document',imeUndo:'New document',imeRedo:'Composed document'});
 }finally{await close(page,errors);}
});


test('Browser: an omitted mask retains native text and emptiness boundaries',async()=>{
 const {page,errors}=await open('Browser');try{
  await page.evaluate(async()=>{
   const module=await import(new URL('_content/NeoUI.Blazor/js/browser-input.js',document.baseURI).href);
   const input=document.createElement('input');input.id='preserve-unmasked';input.dataset.neoInputConfig=JSON.stringify({value:'',revision:0,updateOn:'change',format:{kind:'mask',mask:'',showMask:true}});document.body.append(input);
   window.__unmaskedCalls=[];module.initialize(input,{invokeMethodAsync:async(method,value,sequence,revision)=>{window.__unmaskedCalls.push(value);return {value,sequence,revision};}});
  });
  const input=page.locator('#preserve-unmasked');await input.fill('a');await page.waitForFunction(()=>window.__unmaskedCalls.length===1);
  await input.fill('abc');assert.equal(await input.inputValue(),'abc');assert.deepEqual(await page.evaluate(()=>window.__unmaskedCalls),['a']);
  await input.fill('');await page.waitForFunction(()=>window.__unmaskedCalls.length===2);assert.deepEqual(await page.evaluate(()=>window.__unmaskedCalls),['a','']);
 }finally{await close(page,errors);}
});

for(const mode of ['Blazor','Browser']){
 test(`${mode}: tapping MultiSelect checkbox markers updates row, checkmark and trigger together`,async()=>{
  const {page,errors}=await open(mode,{hasTouch:true,viewport:{width:1100,height:900}});try{
   const initialTrigger=page.getByRole('combobox').filter({hasText:'Preserved multi'});const trigger=page.locator(`[id="${await initialTrigger.getAttribute('id')}"]`);await trigger.tap();
   const row=page.getByRole('option',{name:'Design',exact:true});await row.waitFor();
   const marker=row.locator('[data-neo-multi-indicator]');
   await marker.tap();await page.getByTestId('preserve-multi-model').filter({hasText:'Design'}).waitFor();
   assert.equal(await row.getAttribute('aria-selected'),'true');assert.equal(await marker.getAttribute('data-state'),'checked');assert.equal(await marker.locator('svg').isVisible(),true);assert.match(await trigger.textContent(),/Design/);
   await marker.tap();await page.waitForFunction(()=>document.querySelector('[data-testid="preserve-multi-model"]').textContent==='');
   assert.equal(await marker.getAttribute('data-state'),'unchecked');assert.equal(await marker.locator('svg').count(),0);assert.match(await trigger.textContent(),/Preserved multi/);
   await marker.tap();await page.getByTestId('preserve-multi-model').filter({hasText:'Design'}).waitFor();
   const all=page.getByRole('option',{name:/^Select all$/i}).locator('[data-neo-multi-indicator]');assert.equal(await all.getAttribute('data-state'),'indeterminate');
   await all.tap();await page.waitForFunction(()=>document.querySelector('[data-testid="preserve-multi-model"]').textContent==='Design,Development,Research');assert.equal(await all.getAttribute('data-state'),'checked');
   await all.tap();await page.waitForFunction(()=>document.querySelector('[data-testid="preserve-multi-model"]').textContent==='');assert.equal(await all.getAttribute('data-state'),'unchecked');
  }finally{await close(page,errors);}
 });
 test(`${mode}: tag suggestion touch activation retains the target and shows accepted badges`,async()=>{
  const {page,errors}=await open(mode,{hasTouch:true,viewport:{width:1100,height:900}});try{
   await page.getByPlaceholder('Preserved tags').evaluate(el=>el.dataset.touchTags='true');const input=page.locator('[data-touch-tags]');await input.scrollIntoViewIfNeeded();
   if(mode==='Browser')await page.waitForFunction(()=>Boolean(document.getElementById(document.querySelector('[placeholder="Preserved tags"]').getAttribute('aria-controls'))));
   else await input.locator('xpath=self::*[@data-preservepaste="true"]').waitFor();
   await input.fill('Des');const option=page.getByRole('option',{name:'Design',exact:true});await option.waitFor();
   if(mode==='Browser'){
    const activated=await option.evaluate(el=>{el.dispatchEvent(new PointerEvent('pointerover',{bubbles:true,pointerType:'touch'}));const connected=el.isConnected;const rect=el.getBoundingClientRect(),options={bubbles:true,cancelable:true,pointerType:'touch',pointerId:3,button:0,clientX:rect.x+rect.width/2,clientY:rect.y+rect.height/2};el.dispatchEvent(new PointerEvent('pointerdown',options));el.dispatchEvent(new PointerEvent('pointerup',options));el.dispatchEvent(new MouseEvent('click',{bubbles:true,cancelable:true,button:0,detail:1}));return {connected,badge:document.querySelector('[data-neo-tag-values]').textContent};});
    assert.equal(activated.connected,true);assert.equal(activated.badge,'Design×');
   }else await option.tap();
   await page.getByTestId('preserve-tags-model').filter({hasText:'Design'}).waitFor();
   const root=input.locator('xpath=..');await root.getByRole('button',{name:'Remove Design',exact:true}).waitFor();assert.equal(await input.inputValue(),'');
   await input.fill('Res');await page.getByRole('option',{name:'Research',exact:true}).tap();
   await page.getByTestId('preserve-tags-model').filter({hasText:'Design,Research'}).waitFor();await root.getByRole('button',{name:'Remove Research',exact:true}).waitFor();
   await root.getByRole('button',{name:'Remove Design',exact:true}).tap();await page.waitForFunction(()=>document.querySelector('[data-testid="preserve-tags-model"]').textContent==='Research');
  }finally{await close(page,errors);}
 });
}
