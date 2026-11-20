import {test,before,after} from 'node:test';
import assert from 'node:assert/strict';
import {chromium,webkit,devices} from 'playwright';
import {readFile} from 'node:fs/promises';
const url=process.env.NEOUI_TEST_URL??'http://localhost:5213';
const floating=await readFile(new URL('./node_modules/@floating-ui/core/dist/floating-ui.core.umd.js',import.meta.url),'utf8')+'\n'+await readFile(new URL('./node_modules/@floating-ui/dom/dist/floating-ui.dom.umd.js',import.meta.url),'utf8');
let browser;
before(async()=>browser=await(process.env.NEOUI_BROWSER_ENGINE==='webkit'?webkit.launch({headless:true}):chromium.launch({channel:'msedge',headless:true})));
after(async()=>browser?.close());
async function setup(mode='Browser',extra='') {
 const page=await browser.newPage({...devices['iPhone 13']}),errors=[];
 page.setDefaultTimeout(process.env.NEOUI_TEST_RENDERER==='wasm'?40000:10000);page.on('pageerror',e=>errors.push(String(e)));
 await page.addInitScript(floating);await page.goto(url+'/?mobilesidebar=true&mode='+mode.toLowerCase()+extra);
 await page.locator('[data-testid="test-host-ready"][data-ready="true"]').waitFor({state:'attached'});
 if(mode==='Browser')await page.getByTestId('sheet-content').locator('xpath=ancestor::*[@data-neo-state-host][@data-neo-state-ready="true"]').waitFor({state:'attached'});
 else await page.waitForTimeout(300);
 return {page,errors};
}
const panel=page=>page.getByRole('dialog');
const expand=page=>page.locator('[data-neo-pill-expand]');
for(const mode of ['Browser','Blazor'])test(mode+': mobile pill opens live content and dismisses',async()=>{
 const {page,errors}=await setup(mode);
 try {
  await expand(page).tap();await panel(page).waitFor({state:'visible'});
  await page.getByTestId('sheet-action').tap();await page.getByTestId('sheet-content').filter({hasText:'Live content 1'}).waitFor();
  assert.equal(await panel(page).isVisible(),true);
  if(mode==='Browser')await page.locator('[data-neo-sheet-overlay]').tap({position:{x:380,y:600}});else await page.keyboard.press('Escape');
  await panel(page).waitFor({state:'hidden'});
  await page.getByTestId('sheet-independent').tap();await page.getByTestId('sheet-independent').filter({hasText:'Independent 1'}).waitFor();
  assert.deepEqual(errors,[]);
 }finally{await page.close();}
});
test('Browser: rapid taps, focus, scroll lock and Escape work while transport is paused',async()=>{
 const {page,errors}=await setup();
 try {
  await page.evaluate(()=>{window.held=[];const send=WebSocket.prototype.send;window.release=()=>{WebSocket.prototype.send=send;for(const [socket,data]of held)send.call(socket,data);};WebSocket.prototype.send=function(data){held.push([this,data]);};});
  await expand(page).tap();await panel(page).waitFor({state:'visible'});
  assert.equal(await page.evaluate(()=>document.body.style.overflow),'hidden');
  assert.equal(await panel(page).evaluate(el=>el.contains(document.activeElement)),true);
  for(let i=0;i<3;i++){
   await page.getByTestId('sheet-close').tap();
   assert.equal(await page.locator('[data-neo-sheet-panel]').getAttribute('aria-hidden'),'true');
   assert.equal(await page.evaluate(()=>document.body.style.overflow),'');
   await expand(page).tap();assert.equal(await panel(page).isVisible(),true);
  }
  await page.keyboard.press('Escape');await panel(page).waitFor({state:'hidden'});
  assert.equal(await expand(page).getAttribute('aria-expanded'),'false');
  await page.evaluate(()=>release());await page.getByTestId('sheet-independent').tap();await page.getByTestId('sheet-independent').filter({hasText:'Independent 1'}).waitFor();
  await page.waitForTimeout(300);assert.equal(await panel(page).isVisible(),false,'stale observations must not reopen the sheet');
  assert.deepEqual(errors,[]);
 }finally{await page.close();}
});
test('Browser: nested menus consume Escape before the sheet, and keyboard shortcut toggles once',async()=>{
 const {page,errors}=await setup();
 try {
  await page.keyboard.press('Control+b');await panel(page).waitFor({state:'visible'});
  await page.getByTestId('sheet-nested').tap();await page.getByRole('menu').waitFor({state:'visible'});
  await page.keyboard.press('Escape');await page.getByRole('menu').waitFor({state:'hidden'});assert.equal(await panel(page).isVisible(),true);
  await page.keyboard.press('Escape');await panel(page).waitFor({state:'hidden'});assert.deepEqual(errors,[]);
 }finally{await page.close();}
});
test('Browser: server revisions reset visibility and widening releases modal state',async()=>{
 const {page,errors}=await setup();
 try {
  await expand(page).tap();await panel(page).waitFor({state:'visible'});
  // Programmatic reset remains possible even before an open observation is accepted.
  await page.getByTestId('sheet-reset').evaluate(el=>el.click());
  await page.getByTestId('sheet-reset').filter({hasText:'Reset 1'}).waitFor();
  await panel(page).waitFor({state:'hidden'});
  await expand(page).tap();await panel(page).waitFor({state:'visible'});
  await page.setViewportSize({width:1100,height:800});await page.locator('aside[data-sidebar=sidebar]').waitFor({state:'attached'});await page.waitForFunction(()=>document.body.style.overflow!=='hidden');
  await page.setViewportSize({width:390,height:844});await page.locator('[data-neo-state-ready="true"]').waitFor({state:'attached'});
  assert.equal(await panel(page).isVisible(),false);assert.deepEqual(errors,[]);
 }finally{await page.close();}
});

for(const mode of ['Browser','Blazor'])test(mode+': desktop pill expansion preserves the desktop sidebar path',async()=>{
 const {page,errors}=await setup(mode);
 try {
  await page.setViewportSize({width:1100,height:800});
  await page.locator('aside[data-sidebar=sidebar]').waitFor({state:'attached'});
  await expand(page).click();
  await page.locator('aside[data-sidebar=sidebar][data-state=expanded]').waitFor({state:'attached'});
  assert.equal(await page.locator('[data-neo-sheet-owner]').count(),0);
  await page.evaluate(()=>{document.body.tabIndex=-1;document.body.focus();});await page.keyboard.press('Control+b');
  await page.locator('aside[data-sidebar=sidebar][data-state=closed]').waitFor({state:'attached'});assert.deepEqual(errors,[]);
 }finally{await page.close();}
});

for(const keepOpen of [false,true])test('Browser: link dismissal respects CloseMobileOnNavigate='+!keepOpen,async()=>{
 const {page,errors}=await setup('Browser',keepOpen?'&keepopen=true':'');
 try {
  await expand(page).tap();await panel(page).waitFor({state:'visible'});
  await page.getByTestId('sheet-link').tap();
  if(keepOpen) {await page.waitForTimeout(250);assert.equal(await panel(page).isVisible(),true);}
  else await panel(page).waitFor({state:'hidden'});
  assert.deepEqual(errors,[]);
 }finally{await page.close();}
});
test('Browser: modal Tab navigation wraps within the sheet',async()=>{
 const {page,errors}=await setup();
 try {
  await expand(page).tap();await panel(page).waitFor({state:'visible'});
  await page.getByTestId('sheet-link').focus();await page.keyboard.press('Tab');
  assert.equal(await page.getByTestId('sheet-close').evaluate(el=>el===document.activeElement),true);
  await page.keyboard.press('Shift+Tab');assert.equal(await page.getByTestId('sheet-link').evaluate(el=>el===document.activeElement),true);
  assert.deepEqual(errors,[]);
 }finally{await page.close();}
});

for(const mode of ['Browser','Blazor'])test(mode+': an additional Browser provider keeps independent mobile state',async()=>{
 const {page,errors}=await setup(mode,'&multiple=true');
 try {
  await page.getByTestId('secondary-content').locator('xpath=ancestor::*[@data-neo-state-host][@data-neo-state-ready="true"]').waitFor({state:'attached'});
  await page.getByTestId('secondary-trigger').tap();await page.getByTestId('secondary-content').waitFor({state:'visible'});
  assert.equal(await page.getByTestId('sheet-content').isVisible(),false);
  await page.getByTestId('secondary-action').tap();await page.getByTestId('secondary-action').filter({hasText:'Secondary 1'}).waitFor();
  await page.keyboard.press('Escape');await page.getByTestId('secondary-content').waitFor({state:'hidden'});
  await expand(page).tap();await page.getByTestId('sheet-content').waitFor({state:'visible'});
  assert.equal(await page.getByTestId('secondary-content').isVisible(),false);assert.deepEqual(errors,[]);
 }finally{await page.close();}
});

test('Browser: the pill remains ready at tablet widths without waiting for close acknowledgements',async()=>{
 const {page,errors}=await setup();
 try {
  await page.setViewportSize({width:700,height:844});
  await page.locator('[data-sidebar="pill-nav"]').evaluate(el=>el.classList.remove('!opacity-100','!translate-y-0','!pointer-events-auto'));
  await expand(page).tap();await panel(page).waitFor({state:'visible'});
  await page.waitForTimeout(200); // Let the server accept open before blocking close.
  await page.evaluate(()=>{const send=WebSocket.prototype.send;window.release=()=>WebSocket.prototype.send=send;WebSocket.prototype.send=function(){};});
  await page.keyboard.press('Escape');await panel(page).waitFor({state:'hidden'});
  const style=await page.locator('[data-sidebar="pill-nav"]').evaluate(el=>({opacity:getComputedStyle(el).opacity,pointerEvents:getComputedStyle(el).pointerEvents}));
  assert.equal(style.opacity,'1');assert.equal(style.pointerEvents,'auto');
  await expand(page).tap();await panel(page).waitFor({state:'visible'});
  await page.evaluate(()=>release());assert.deepEqual(errors,[]);
 }finally{await page.close();}
});
