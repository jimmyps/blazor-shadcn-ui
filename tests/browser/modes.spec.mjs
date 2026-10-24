import { test, before, after } from 'node:test';
import assert from 'node:assert/strict';
import { chromium, webkit } from 'playwright';
import { readFile } from 'node:fs/promises';
const url=process.env.NEOUI_TEST_URL??'http://localhost:5198';
const script=await readFile(new URL('./node_modules/@floating-ui/core/dist/floating-ui.core.umd.js',import.meta.url),'utf8')+'\n'+await readFile(new URL('./node_modules/@floating-ui/dom/dist/floating-ui.dom.umd.js',import.meta.url),'utf8');
let browser;
before(async()=>{browser=await (process.env.NEOUI_BROWSER_ENGINE==='webkit'?webkit.launch({headless:true,timeout:15000}):chromium.launch({channel:'msedge',headless:true,timeout:15000}));});
after(async()=>{await browser?.close();});
async function open(mode,families=false){
 const page=await browser.newPage(),errors=[];page.setDefaultTimeout(process.env.NEOUI_TEST_RENDERER==='wasm'?30000:7000);page.setDefaultNavigationTimeout(30000);page.on('pageerror',error=>errors.push(String(error)));try { await page.addInitScript(script);
 await page.addInitScript(()=>{const add=EventTarget.prototype.addEventListener;EventTarget.prototype.addEventListener=function(type,...args){const result=add.call(this,type,...args);if((this instanceof HTMLInputElement||this instanceof HTMLTextAreaElement)&&['input','change'].includes(type))this.dataset.matrixListenerReady='true';return result;};});
 await page.goto(url+`/?${families?"matrixmenus=true":"sweep=true&global=true"}&mode=${mode.toLowerCase()}`);await page.locator('[data-testid="test-host-ready"][data-ready="true"]').waitFor({state:'attached'});if(families){await page.getByTestId('menu-families').waitFor();await page.getByTestId('family-render').click();await page.getByTestId('family-pill-render').filter({hasText:'1'}).waitFor();return {browser,page,errors};}await page.getByTestId('p1-sweep').waitFor();
 await page.getByTestId('sweep-independent').click();await page.getByTestId('sweep-independent').filter({hasText:'Independent 1'}).waitFor();
 await page.waitForFunction(()=>['Sweep single line','Sweep textarea','Sweep group input','Sweep group textarea'].every(name=>document.querySelector(`[placeholder="${name}"]`)?.dataset.matrixListenerReady==='true'));
 if(mode==='Browser')await page.locator('[data-testid="sweep-special"] [data-neo-state-ready="true"]').first().waitFor();
 return {browser,page,errors};
 }catch(error){await page.close();throw error;}
}
async function attribute(locator,name,value){await locator.page().waitForFunction(({selector,name,value})=>document.querySelector(selector)?.getAttribute(name)===value,{selector:await locator.evaluate(el=>{el.dataset.matrixTarget='current';return '[data-matrix-target="current"]'}),name,value},{timeout:5000});await locator.evaluate(el=>delete el.dataset.matrixTarget);}
for(const mode of ['Blazor','Browser']){
 test(`${mode}: checkbox indicators visibly match checked, mixed, disabled and reset states`,async()=>{
  const {page,errors}=await open(mode);try{
   const indicators=async(control,state,expected)=>{
    await attribute(control,'aria-checked',state);
    assert.deepEqual(await control.locator('svg').evaluateAll(icons=>icons.filter(icon=>getComputedStyle(icon).display!=='none').map(icon=>icon.dataset.neoFieldIndicator)),expected);
   };
   const initial=page.getByTestId('sweep-checkbox-indicators');
   if(mode==='Browser')await initial.locator('[data-neo-state-ready="true"]').nth(2).waitFor();
   await indicators(initial.getByRole('checkbox',{name:'Initially checked indicator',exact:true}),'true',['checked']);
   await indicators(initial.getByRole('checkbox',{name:'Initially mixed indicator',exact:true}),'mixed',['mixed']);
   const disabled=initial.getByRole('checkbox',{name:'Disabled checked indicator',exact:true});
   assert.equal(await disabled.isDisabled(),true);await indicators(disabled,'true',['checked']);
   const check=page.getByRole('checkbox',{name:'Sweep checkbox',exact:true});
   await indicators(check,'false',[]);
   if(mode==='Browser')await page.evaluate(()=>{window.__nativeSend=WebSocket.prototype.send;WebSocket.prototype.send=function(){};});
   await check.click();await indicators(check,'true',['checked']);
   if(mode==='Browser')await check.press('Space');else await check.click();await indicators(check,'false',[]);
   if(mode==='Browser')await page.evaluate(()=>WebSocket.prototype.send=window.__nativeSend);
   await check.click();await indicators(check,'true',['checked']);
   await page.getByTestId('sweep-fields-reset').click();await indicators(check,'false',[]);
   assert.deepEqual(errors,[]);
  }finally{await page.evaluate(()=>{if(window.__nativeSend)WebSocket.prototype.send=window.__nativeSend;}).catch(()=>{});await page.close();}
 });
 test(`${mode}: masked typing keeps the caret at the next editable slot`,async()=>{
  const {page,errors}=await open(mode);try{
   const input=page.getByPlaceholder('Sweep caret mask',{exact:true});
   if(mode==='Browser')await input.locator('xpath=self::*[@data-neo-input-ready="true"]').waitFor();
   else await input.locator('xpath=self::*[@data-matrix-listener-ready="true"]').waitFor();
   await input.focus();await input.press('ControlOrMeta+A');await input.press('Backspace');
   for(const [key,value,caret] of [['1','1__-___',1],['2','12_-___',2],['3','123-___',4],['4','123-4__',5],['5','123-45_',6],['6','123-456',7]]){
    await input.press(key);assert.equal(await input.inputValue(),value);
    assert.equal(await input.evaluate(el=>el.selectionStart),caret,`caret after ${key}`);
   }
   if(mode==='Browser'){
    await input.evaluate(el=>el.setSelectionRange(4,4));await input.press('Backspace');
    assert.equal(await input.inputValue(),'124-56_');assert.equal(await input.evaluate(el=>el.selectionStart),2);
    await input.press('ControlOrMeta+A');await input.press('Backspace');await input.press('7');
    assert.equal(await input.inputValue(),'7__-___');assert.equal(await input.evaluate(el=>el.selectionStart),1);
   }
   assert.deepEqual(errors,[]);
  }finally{await page.close();}
 });

 test(`${mode}: text adapters, formatted inputs, binding and reset preserve common behavior`,async()=>{
  const {browser,page,errors}=await open(mode);try{
   if(mode==='Blazor')assert.equal(await page.locator('[data-neo-state-host]').count(),0);
   for(const [name,value] of [['Sweep single line','single'],['Sweep textarea','note\nnext'],['Sweep group input','group'],['Sweep group textarea','group\nnote']]){await page.getByPlaceholder(name,{exact:true}).fill(value);await page.getByPlaceholder(name,{exact:true}).blur();}
   await page.getByTestId('sweep-save').click();await page.getByTestId('sweep-saved').filter({hasText:'single|note\nnext|group|group\nnote'}).waitFor();
   await page.getByTestId('sweep-number-reset').click();await page.waitForFunction(()=>document.querySelector('[placeholder="Sweep numeric"]').value==='10');
   const num=page.getByPlaceholder('Sweep numeric',{exact:true});await num.fill('37');await num.blur();await page.getByTestId('sweep-number-save').click();await page.getByTestId('sweep-number-saved').filter({hasText:'37|20|123456'}).waitFor();
   assert.deepEqual(errors,[]);
  }finally{await page.close();}
 });
 test(`${mode}: checked, pressed, radio, range and controlled resets remain functional`,async()=>{
  const {browser,page,errors}=await open(mode);try{
   const fields=page.getByTestId('sweep-fields');const check=fields.getByRole('checkbox',{name:'Sweep checkbox'});
   await check.click();await attribute(check,'aria-checked','true');await fields.getByRole('switch').click();await attribute(fields.getByRole('switch'),'aria-checked','true');
   await fields.getByRole('button',{name:'Sweep toggle',exact:true}).click();await attribute(fields.getByRole('button',{name:'Sweep toggle',exact:true}),'aria-pressed','true');
   await fields.getByRole('radio',{name:'Sweep choice two'}).click();await attribute(fields.getByRole('radio',{name:'Sweep choice two'}),'aria-checked','true');
   await fields.getByRole('slider').fill('75');await page.getByTestId('sweep-fields-save').click();await page.getByTestId('sweep-fields-saved').filter({hasText:'True|True|True|two|75'}).waitFor();
   await page.getByTestId('sweep-fields-reset').click();await attribute(check,'aria-checked','false');
   const special=page.getByTestId('sweep-special'),radio=special.getByRole('radio',{name:'Sweep radio two'});await radio.click();await attribute(radio,'aria-checked','true');
   const handle=special.getByRole('slider').first();await handle.focus();await handle.press('ArrowRight');await attribute(handle,'aria-valuenow','21');
   await page.getByTestId('sweep-special-reset').click();await attribute(handle,'aria-valuenow','20');assert.deepEqual(errors,[]);
  }finally{await page.close();}
 });
 test(`${mode}: disclosure, tabs, hover and tooltips preserve activation and dismissal`,async()=>{
  const {browser,page,errors}=await open(mode);try{
   const first=page.getByRole('button',{name:'First disclosure',exact:true});await first.click();await page.getByRole('button',{name:'First disclosure action',exact:true}).waitFor();await first.click();await attribute(first,'aria-expanded','false');await attribute(page.getByRole('region',{includeHidden:true}).filter({has:page.getByRole('button',{name:'First disclosure action',exact:true,includeHidden:true})}),'data-state','closed');
   const tab=page.getByRole('tab',{name:'Second local tab',exact:true});await tab.click();await attribute(tab,'aria-selected','true');await page.getByText('Second local panel',{exact:true}).waitFor();
   await page.getByRole('button',{name:'Standalone disclosure',exact:true}).click();await page.getByText('Standalone disclosure body',{exact:true}).waitFor();await page.getByTestId('sweep-disclosure-actions').filter({hasText:'1'}).waitFor();
   await page.getByRole('button',{name:'Sweep hover',exact:true}).hover();await page.getByTestId('sweep-hover-child').waitFor();await page.mouse.move(1,1);await page.getByTestId('sweep-hover-child').waitFor({state:'hidden'});
   await page.getByRole('button',{name:'Sweep tooltip',exact:true}).hover();await page.getByRole('tooltip').waitFor();await page.mouse.move(1,1);await page.getByRole('tooltip').waitFor({state:'hidden'});assert.deepEqual(errors,[]);
  }finally{await page.close();}
 });
 test(`${mode}: empty segmented inputs are reachable with Tab without a picker or imperative focus`,async()=>{
  const {browser,page,errors}=await open(mode);try{
   for(const name of ['Empty date API','Empty time API']){
    await page.getByTestId(name==='Empty date API'?'keyboard-date-entry':'keyboard-time-entry').focus();
    await page.keyboard.press('Tab');await page.waitForFunction(label=>{const group=[...document.querySelectorAll('[role="group"]')].find(el=>el.getAttribute('aria-label')===label);return group?.contains(document.activeElement)&&document.activeElement.getAttribute('role')==='spinbutton';},name,{timeout:2500});
    const firstId=await page.evaluate(()=>document.activeElement.id||document.activeElement.getAttribute('aria-label'));
    await page.keyboard.press('Tab');await page.waitForFunction(({name,firstId})=>{const group=[...document.querySelectorAll('[role="group"]')].find(el=>el.getAttribute('aria-label')===name),active=document.activeElement;return group?.contains(active)&&active.getAttribute('role')==='spinbutton'&&(active.id||active.getAttribute('aria-label'))!==firstId;},{name,firstId},{timeout:2500});
    await page.keyboard.press('Shift+Tab');await page.waitForFunction(firstId=>(document.activeElement.id||document.activeElement.getAttribute('aria-label'))===firstId,firstId,{timeout:2500});

   }
   assert.deepEqual(errors,[]);
  }finally{await page.close();}
 });
}

test('Browser: a null canonical selection is an authoritative rejection rather than a stale parameter fallback',async()=>{
 const {browser,page}=await open('Browser');try{
  const one=page.getByRole('radio',{name:'Canonical choice one'}),two=page.getByRole('radio',{name:'Canonical choice two'});
  await one.locator('xpath=ancestor::*[@data-neo-state-ready="true"]').waitFor();
  await two.click();await attribute(two,'aria-checked','false');await attribute(one,'aria-checked','false');
 }finally{await page.close();}
});
for(const mode of ['Blazor','Browser'])test(`${mode}: menu, context, navigation, split and pill families remain interactive`,async()=>{
 const {browser,page,errors}=await open(mode,true);try{
  const item=name=>page.getByRole('menuitem',{name,exact:true});
  await item('Family File').click();await item('Family action').waitFor();await item('Family action').click();await page.getByTestId('family-actions').filter({hasText:'1'}).waitFor();
  await item('Family Edit').click();await item('Family Undo').waitFor();await page.getByTestId('family-render').click();await item('Family Undo').waitFor({state:'hidden'});
  await page.getByTestId('family-context').click({button:'right'});await item('Context action').waitFor();await item('Context action').click();await page.getByTestId('family-actions').filter({hasText:'2'}).waitFor();
  await item('Family Products').hover();await item('Family Product One').waitFor();await page.getByTestId('family-render').click();
  await page.getByTestId('family-split').locator('button').last().click();await item('Family Save As').waitFor();await item('Family Save As').click();await page.getByTestId('family-actions').filter({hasText:'3'}).waitFor();
  await page.getByRole('button',{name:'Family Inbox',exact:true}).click();await attribute(page.getByRole('button',{name:'Family Inbox',exact:true}),'data-active','true');
  await page.getByRole('button',{name:'Family Pill Menu',exact:true}).click();await item('Family Pill Action').waitFor();await item('Family Pill Action').click();assert.deepEqual(errors,[]);
 }finally{await page.close();}
});
for(const mode of ['Blazor','Browser'])test(`${mode}: segmented minute steps and current-date/time shortcuts preserve existing behavior`,async()=>{
 const {browser,page,errors}=await open(mode);try{
  const stepped=page.getByRole('group',{name:'Stepped time',exact:true}).getByRole('spinbutton',{name:'minute',exact:true});await stepped.focus();await stepped.press('ArrowUp');await attribute(stepped,'aria-valuenow','15');await stepped.press('ArrowDown');await attribute(stepped,'aria-valuenow','0');
  const date=page.getByRole('group',{name:'Sweep date',exact:true}),day=date.getByRole('spinbutton',{name:'day',exact:true});const today=await page.evaluate(()=>({year:new Date().getFullYear(),month:new Date().getMonth()+1,day:new Date().getDate(),hour:new Date().getHours(),minute:new Date().getMinutes()}));
  await day.focus();await day.press('c');for(const part of ['year','month','day'])await attribute(date.getByRole('spinbutton',{name:part,exact:true}),'aria-valuenow',String(today[part]));
  const time=page.getByRole('group',{name:'Sweep time',exact:true}),minute=time.getByRole('spinbutton',{name:'minute',exact:true});await minute.focus();await minute.press('c');await attribute(minute,'aria-valuenow',String(today.minute));await attribute(time.getByRole('spinbutton',{name:'hour',exact:true}),'aria-valuenow',String(today.hour));assert.deepEqual(errors,[]);
 }finally{await page.close();}
});
test('Browser: a null canonical multiple selection clears the optimistic selection',async()=>{
 const {browser,page}=await open('Browser');try{
  const one=page.getByRole('checkbox',{name:'Canonical multiple one'}),two=page.getByRole('checkbox',{name:'Canonical multiple two'});
  await one.locator('xpath=ancestor::*[@data-neo-state-ready="true"]').waitFor();await two.click();await attribute(two,'aria-checked','false');await attribute(one,'aria-checked','false');
 }finally{await page.close();}
});
test('Blazor: navigation hover timers and removal preserve the circuit and independent actions',async()=>{
 const {page}=await open('Blazor',true);try{
  const trigger=page.getByRole('menuitem',{name:'Family Products',exact:true});
  await trigger.hover();await page.getByRole('menuitem',{name:'Family Product One',exact:true}).waitFor();await page.mouse.move(1,1);await attribute(trigger,'aria-expanded','false');
  await page.getByTestId('family-render').click();await page.getByTestId('family-pill-render').filter({hasText:'2'}).waitFor();
  await trigger.hover();await page.getByTestId('family-toggle-navigation').evaluate(button=>button.click());await trigger.waitFor({state:'detached'});await page.waitForTimeout(450);
  await page.getByTestId('family-render').click();await page.getByTestId('family-pill-render').filter({hasText:'3'}).waitFor();
 }finally{await page.close();}
});
for(const mode of ['Blazor','Browser'])test(`${mode}: floating width follows anchor resizing and parent renders without observer errors`,async()=>{
 const {page,errors}=await open(mode);try{
  const trigger=page.getByRole('combobox').filter({hasText:'Sweep select'});await trigger.click();await page.getByPlaceholder('Sweep multi search',{exact:true}).waitFor();
  for(const width of [320,240]){
   await trigger.evaluate((element,width)=>element.style.width=`${width}px`,width);
   await page.waitForFunction(()=>{const trigger=[...document.querySelectorAll('[role="combobox"]')].find(el=>el.textContent.includes('Sweep select'));const portal=document.querySelector('[placeholder="Sweep multi search"]')?.closest('[data-portal-content]');return portal&&Math.abs(portal.getBoundingClientRect().width-trigger.getBoundingClientRect().width)<1;});
   await page.getByTestId('sweep-independent').evaluate(button=>button.click());await page.getByTestId('sweep-independent').filter({hasText:`Independent ${width===320?2:3}`}).waitFor();
  }
  await page.keyboard.press('Escape');await page.getByPlaceholder('Sweep multi search',{exact:true}).waitFor({state:'hidden'});assert.deepEqual(errors,[]);
 }finally{await page.close();}
});
