import { test, before, after } from 'node:test';
import assert from 'node:assert/strict';
import { chromium } from 'playwright';
import { readFile } from 'node:fs/promises';
const policy=process.env.NEOUI_FAMILY_POLICY;
const enabled=['conservative','reverse'].includes(policy);
const url=process.env.NEOUI_TEST_URL??'http://localhost:5198';
const floating=await readFile(new URL('./node_modules/@floating-ui/core/dist/floating-ui.core.umd.js',import.meta.url),'utf8')+'\n'+await readFile(new URL('./node_modules/@floating-ui/dom/dist/floating-ui.dom.umd.js',import.meta.url),'utf8');
let browser;
before(async()=>{browser=await chromium.launch({channel:'msedge',headless:true});});
after(async()=>{await browser?.close();});
async function open(){
 const page=await browser.newPage(),errors=[];page.setDefaultTimeout(7000);page.on('pageerror',error=>errors.push(String(error)));
 await page.addInitScript(floating);await page.goto(url+'/?familypolicy=true');
 await page.locator('[data-testid="test-host-ready"][data-ready="true"]').waitFor({state:'attached'});
 await page.getByPlaceholder('Scoped browser input',{exact:true}).locator('xpath=self::*[@data-neo-input-ready="true"]').waitFor();
 return {page,errors};
}
async function fieldMode(page,name,browserMode){
 const field=page.getByPlaceholder(name,{exact:true});
 if(browserMode)await field.locator('xpath=self::*[@data-neo-input-ready="true"]').waitFor();
 assert.equal(await field.getAttribute('data-neo-input-config')!==null,browserMode,name);
}
async function shellMode(page,trigger,browserMode){
 if(browserMode)await page.waitForFunction(id=>document.getElementById(id)?.hasAttribute('data-neo-browser-content'),await trigger.getAttribute('aria-controls'));
 else assert.equal(await page.locator('[id="'+await trigger.getAttribute('aria-controls')+'"][data-neo-browser-content]').count(),0);
 await trigger.click();await trigger.locator('xpath=self::*[@aria-expanded="true"]').waitFor();
 await page.locator('[id="'+await trigger.getAttribute('aria-controls')+'"]').waitFor();
 // Legacy dismissal handlers are registered after asynchronous portal positioning.
 await page.waitForTimeout(350);
 await page.keyboard.press('Escape');await trigger.locator('xpath=self::*[@aria-expanded="false"]').waitFor();
}
test('family defaults isolate floating, menu, field, disclosure and composed selection behavior',{skip:!enabled},async()=>{
 const {page,errors}=await open();const local=policy==='conservative';try{
  await fieldMode(page,'Policy input',!local);await fieldMode(page,'Policy textarea',!local);await fieldMode(page,'Policy command',!local);
  const checkbox=page.getByRole('checkbox',{name:'Policy checkbox'});
  assert.equal(await checkbox.locator('xpath=ancestor::*[@data-neo-state-host]').count()>0,!local);
  await shellMode(page,page.getByRole('button',{name:'Policy popover',exact:true}),local);
  await shellMode(page,page.getByRole('button',{name:'Policy menu',exact:true}),local);
  await shellMode(page,page.getByRole('combobox').filter({hasText:'Policy select'}),!local);
  for(const label of ['Policy combobox','Policy multi']){
   const trigger=page.getByRole('combobox').filter({hasText:label});await trigger.click();
   await fieldMode(page,label==='Policy combobox'?'Policy combo search':'Policy multi search',!local);
   await page.getByPlaceholder(label==='Policy combobox'?'Policy combo search':'Policy multi search',{exact:true}).focus();
   await page.waitForTimeout(350);
   await page.keyboard.press('Escape');await trigger.locator('xpath=self::*[@aria-expanded="false"]').waitFor();
   await page.getByPlaceholder(label==='Policy combobox'?'Policy combo search':'Policy multi search',{exact:true}).waitFor({state:'hidden'});
  }
  for(const id of ['policy-date','policy-time','policy-range'])await shellMode(page,page.getByTestId(id).getByRole('button').first(),!local);
  const disclosure=page.getByRole('button',{name:'Policy accordion',exact:true});assert.equal(await disclosure.locator('xpath=ancestor::*[@data-neo-state-host]').count()>0,local);await disclosure.click();await disclosure.locator('xpath=self::*[@aria-expanded="true"]').waitFor();
  await page.getByTestId('policy-alive').click();await page.getByTestId('policy-alive').filter({hasText:'Alive 1'}).waitFor();assert.deepEqual(errors,[]);
 }finally{await page.close();}
});
test('explicit component and scoped policies override application family settings',{skip:!enabled},async()=>{
 const {page,errors}=await open();try{
  await fieldMode(page,'Explicit legacy input',false);await fieldMode(page,'Scoped browser input',true);
  await fieldMode(page,'Menu scoped input',policy==='reverse');
  await fieldMode(page,'Nested menu scoped input',false);
  await shellMode(page,page.getByRole('button',{name:'Menu scoped trigger',exact:true}),true);
  await fieldMode(page,'Scoped legacy input',false);await fieldMode(page,'Scoped explicit legacy input',false);
  await shellMode(page,page.getByRole('button',{name:'Explicit legacy popover',exact:true}),false);
  await shellMode(page,page.getByRole('button',{name:'Scoped legacy popover',exact:true}),false);
  await shellMode(page,page.getByRole('button',{name:'Scoped explicit browser popover',exact:true}),true);
  const combo=page.getByRole('combobox').filter({hasText:'Scoped combo'});await combo.click();await fieldMode(page,'Scoped search',true);await page.keyboard.press('Escape');assert.deepEqual(errors,[]);
 }finally{await page.close();}
});
test('an input inside an ordinary Popover respects its own family policy',{skip:!enabled},async()=>{
 const {page,errors}=await open();try{
  await page.getByRole('button',{name:'Policy popover',exact:true}).click();
  await fieldMode(page,'Policy nested input',policy==='reverse');assert.deepEqual(errors,[]);
 }finally{await page.close();}
});

test('omitting family settings preserves legacy explicit Popover exceptions for inputs and Command',{skip:enabled},async()=>{
 const {page,errors}=await open();try{
  await page.getByRole('button',{name:'Legacy composed shell',exact:true}).click();
  await fieldMode(page,'Legacy composed input',false);await fieldMode(page,'Legacy composed command',false);assert.deepEqual(errors,[]);
 }finally{await page.close();}
});

test('an explicit Inputs family scope overrides an ordinary Popover shell without application family settings',async()=>{
 const {page,errors}=await open();try{
  await page.getByRole('button',{name:'Input scoped shell',exact:true}).click();
  await fieldMode(page,'Input scoped nested field',true);assert.deepEqual(errors,[]);
 }finally{await page.close();}
});
