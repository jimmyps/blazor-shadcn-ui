import {test,before,after} from 'node:test';
import assert from 'node:assert/strict';
import {chromium} from 'playwright';
import {readFile} from 'node:fs/promises';
const url=process.env.NEOUI_TEST_URL??'http://localhost:5198';
const floating=await readFile(new URL('./node_modules/@floating-ui/core/dist/floating-ui.core.umd.js',import.meta.url),'utf8')+'\n'+await readFile(new URL('./node_modules/@floating-ui/dom/dist/floating-ui.dom.umd.js',import.meta.url),'utf8');
let browser;
before(async()=>browser=await chromium.launch({channel:'msedge',headless:true}));
after(async()=>browser?.close());
for(const mode of ['Blazor','Browser'])test(`${mode}: a failed input module import preserves independent controls and blocks an unsynchronized Browser save`,async()=>{
 const page=await browser.newPage();let requested=false;
 try{
  await page.addInitScript(floating);
  await page.route(/\/js\/input(?:\.[^/]+)?\.js(?:\?.*)?$/,route=>{requested=true;return route.abort();});
  await page.goto(url+`/?sweep=true&mode=${mode.toLowerCase()}`);
  await page.locator('[data-testid="test-host-ready"][data-ready="true"]').waitFor({state:'attached'});
  await page.waitForTimeout(250);assert.equal(requested,true);
  for(const count of [1,2]){await page.getByTestId('sweep-independent').click();await page.getByTestId('sweep-independent').filter({hasText:`Independent ${count}`}).waitFor();}
  if(mode==='Browser'){
   await page.getByTestId('sweep-save').click();await page.waitForFunction(()=>document.querySelector('[data-testid="sweep-save"]').closest('form').getAttribute('aria-busy')==='false');
   assert.equal(await page.getByTestId('sweep-saved').textContent(),'None');
  }
 }finally{await page.close();}
});
