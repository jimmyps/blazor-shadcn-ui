import {test,before,after} from 'node:test';
import assert from 'node:assert/strict';
import {chromium,webkit} from 'playwright';
import {readFile} from 'node:fs/promises';
const url=process.env.NEOUI_TEST_URL??'http://localhost:5198';
const floating=await readFile(new URL('./node_modules/@floating-ui/core/dist/floating-ui.core.umd.js',import.meta.url),'utf8')+'\n'+await readFile(new URL('./node_modules/@floating-ui/dom/dist/floating-ui.dom.umd.js',import.meta.url),'utf8');
let browser;before(async()=>browser=await(process.env.NEOUI_BROWSER_ENGINE==='webkit'?webkit.launch({headless:true}):chromium.launch({channel:'msedge',headless:true})));after(async()=>browser?.close());
async function open(mode){
 const page=await browser.newPage({hasTouch:true,viewport:{width:1100,height:900}}),errors=[];page.setDefaultTimeout(process.env.NEOUI_TEST_RENDERER==='wasm'?40000:15000);page.on('pageerror',e=>errors.push(String(e)));if(new URL(url).pathname!=='/')page.on('request',request=>{if(new URL(request.url()).pathname.startsWith('/_content/'))errors.push('Asset escaped application base: '+request.url());});
 await page.addInitScript(floating);
 await page.addInitScript(()=>{const add=EventTarget.prototype.addEventListener;EventTarget.prototype.addEventListener=function(type,...args){const result=add.call(this,type,...args);if(this instanceof HTMLInputElement&&type==='paste')this.dataset.ownershipReady='true';return result;};});
 await page.goto(url+`/?interactionownership=true&mode=${mode.toLowerCase()}`);await page.locator('[data-testid="test-host-ready"][data-ready="true"]').waitFor({state:'attached'});
 const input=page.getByRole('textbox',{name:'Ownership tags',exact:true});
 if(mode==='Browser'){await input.locator('xpath=ancestor::*[@data-neo-state-ready="true"]').waitFor();await page.waitForFunction(()=>Boolean(document.getElementById(document.querySelector('input[aria-label="Ownership tags"]').getAttribute('aria-controls'))));}
 else await input.locator('xpath=self::*[@data-ownership-ready="true"]').waitFor();
 return {page,input,errors};
}
async function close(page,errors){await page.close();assert.deepEqual(errors,[]);}
test('sub-path serves helper modules only beneath the application base',{skip:new URL(url).pathname==='/'},async()=>{
 const context=await browser.newContext();try{const asset='_content/NeoUI.Blazor.Primitives/js/primitives/tag-suggestions.js';assert.equal((await context.request.get(new URL('/'+asset,url).href)).status(),404);assert.equal((await context.request.get(url+'/'+asset)).status(),200);}finally{await context.close();}
});

for(const mode of ['Blazor','Browser']){
 test(`${mode}: chooser keyboard, label and checkbox clicks update real columns exactly once`,async()=>{
  const {page,errors}=await open(mode);try{
   const counted=page.getByRole('checkbox',{name:'Counted checkbox',exact:true});await counted.locator('xpath=self::*[@data-neo-checkbox-ready="true"]').waitFor();await counted.focus();await page.keyboard.press('Enter');await page.waitForTimeout(300);assert.equal(await page.getByTestId('checkbox-commits').textContent(),'0');assert.equal(await counted.getAttribute('aria-checked'),'false');
   for(const [i,key] of ['Space','Space','Space','Space'].entries()){await page.keyboard.press(key);await page.getByTestId('checkbox-commits').filter({hasText:new RegExp(`^${i+1}$`)}).waitFor();assert.equal(await counted.getAttribute('aria-checked'),String(i%2===0));}
   const disabled=page.getByRole('checkbox',{name:'Disabled checkbox',exact:true});assert.equal(await disabled.isDisabled(),true);await disabled.dispatchEvent('click');assert.equal(await page.getByTestId('checkbox-commits').textContent(),'4');
   await page.getByRole('button',{name:'Columns',exact:true}).first().click();const check=page.locator('[data-portal-content]').getByRole('checkbox',{name:'Name',exact:true});await check.waitFor();
   for(let i=0;i<4;i++){await check.focus();await page.keyboard.press('Space');await page.waitForFunction(expected=>Boolean([...document.querySelectorAll('thead th')].find(el=>el.textContent.trim()==='Name'))===expected,i%2===1);assert.equal(await check.getAttribute('aria-checked'),String(i%2===1));}
   await check.locator('xpath=ancestor::label').locator('span.select-none').click();await page.waitForFunction(()=>![...document.querySelectorAll('thead th')].some(el=>el.textContent.trim()==='Name'));assert.equal(await check.getAttribute('aria-checked'),'false');
   await check.click();await page.waitForFunction(()=>[...document.querySelectorAll('thead th')].some(el=>el.textContent.trim()==='Name'));assert.equal(await check.getAttribute('aria-checked'),'true');
  }finally{await close(page,errors);}
 });
 test(`${mode}: tag acknowledgement and parent render retain the focused badge`,async()=>{
  const {page,input,errors}=await open(mode);try{
   await input.fill('New');await input.press('Enter');const remove=input.locator('..').getByRole('button',{name:'Remove Research',exact:true});if(mode==='Blazor')await page.getByTestId('ownership-tags-model').filter({hasText:'Design,Research,New'}).waitFor();await remove.focus();await remove.evaluate(el=>window.ownershipRemove=el);
   await page.getByTestId('ownership-tags-model').filter({hasText:'Design,Research,New'}).waitFor();
   if(mode==='Browser')await page.waitForFunction(()=>!document.querySelector('input[aria-label="Ownership tags"]').closest('[data-neo-browser-field]').hasAttribute('data-neo-field-pending'));
   assert.equal(await remove.evaluate(el=>el===window.ownershipRemove&&document.activeElement===el),true);
   await page.getByTestId('ownership-render').evaluate(el=>el.click());await page.getByTestId('ownership-render').filter({hasText:'Render 1'}).waitFor();assert.equal(await remove.evaluate(el=>document.activeElement===el),true);
   if(mode==='Browser'){await page.getByTestId('ownership-reorder').evaluate(el=>el.click());await page.getByTestId('ownership-tags-model').filter({hasText:'New,Research,Design'}).waitFor();await page.waitForFunction(()=>document.activeElement===window.ownershipRemove);}
   await remove.press('Enter');await page.waitForFunction(()=>!document.querySelector('[data-testid="ownership-tags-model"]').textContent.includes('Research'));
   if(mode==='Browser')assert.equal(await input.evaluate(el=>document.activeElement===el),true);
   await input.fill('reject');await input.press('Enter');if(mode==='Browser'){await page.waitForFunction(()=>!document.querySelector('input[aria-label="Ownership tags"]').closest('[data-neo-browser-field]').hasAttribute('data-neo-field-pending'));assert.equal(await input.locator('..').getByRole('button',{name:'Remove reject',exact:true}).count(),0);}
   await page.getByTestId('ownership-reset').click();await page.getByTestId('ownership-tags-model').filter({hasText:/^Reset$/}).waitFor();await input.locator('..').getByRole('button',{name:'Remove Reset',exact:true}).waitFor();
  }finally{await close(page,errors);}
 });
 test(`${mode}: cancelled tag touches never commit; completed tap and mouse click do`,async()=>{
  const {page,input,errors}=await open(mode);try{
   await input.fill('Dev');const option=page.getByRole('option',{name:'Development',exact:true});await option.waitFor();
   await option.evaluate(el=>{el.dispatchEvent(new PointerEvent('pointerdown',{bubbles:true,cancelable:true,pointerId:7,pointerType:'touch',isPrimary:true}));el.dispatchEvent(new PointerEvent('pointermove',{bubbles:true,pointerId:7,pointerType:'touch',clientY:90}));el.dispatchEvent(new PointerEvent('pointercancel',{bubbles:true,pointerId:7,pointerType:'touch'}));el.dispatchEvent(new MouseEvent('click',{bubbles:true,cancelable:true,detail:1,button:0}));const r=el.getBoundingClientRect(),opts={bubbles:true,cancelable:true,pointerId:8,pointerType:'touch',button:0,clientX:r.x+r.width/2,clientY:r.y+r.height/2};el.dispatchEvent(new PointerEvent('pointerdown',opts));el.dispatchEvent(new PointerEvent('pointermove',{...opts,clientY:opts.clientY+25}));el.dispatchEvent(new PointerEvent('pointerup',opts));el.dispatchEvent(new MouseEvent('click',{bubbles:true,cancelable:true,detail:1,button:0}));});
   await page.waitForTimeout(650);assert.equal(await page.getByTestId('ownership-tags-model').textContent(),'Design,Research');assert.equal(await input.inputValue(),'Dev');await option.waitFor();
   await option.tap();await page.getByTestId('ownership-tags-model').filter({hasText:'Design,Research,Development'}).waitFor();await input.locator('..').getByRole('button',{name:'Remove Development',exact:true}).waitFor();
   await input.fill('Oth');await page.getByRole('option',{name:'Other',exact:true}).click();await page.getByTestId('ownership-tags-model').filter({hasText:'Design,Research,Development,Other'}).waitFor();
   if(mode==='Browser'){await input.fill('Des');await page.getByRole('option',{name:'Design',exact:true}).click();await page.getByTestId('ownership-tags-model').filter({hasText:'Design,Research,Development,Other,Design'}).waitFor();assert.equal(await input.locator('..').getByRole('button',{name:'Remove Design',exact:true}).count(),2);}
  }finally{await close(page,errors);}
 });
}
