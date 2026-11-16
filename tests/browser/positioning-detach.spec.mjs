import {test,before,after} from 'node:test';
import assert from 'node:assert/strict';
import {createRequire} from 'node:module';
import {readFile} from 'node:fs/promises';
const require=createRequire(import.meta.url);
const {chromium}=require(process.env.NEOUI_PLAYWRIGHT_MODULE || 'playwright');
const source=await readFile(process.env.NEOUI_POSITIONING_MODULE || new URL('../../src/NeoUI.Blazor.Primitives/wwwroot/js/primitives/positioning.js',import.meta.url),'utf8');
let browser;
before(async()=>browser=await chromium.launch({channel:'msedge',headless:true}));
after(async()=>browser?.close());
async function setup(page) {
  await page.setContent('<button id="reference">Anchor</button><div id="floating">Menu</div>');
  await page.evaluate(async source=>{
    window.cleanups=0;window.updates=0;
    window.FloatingUIDOM={
      offset:()=>({}),flip:()=>({}),shift:()=>({}),
      computePosition:async()=>{window.updates++;return {x:0,y:20,strategy:'absolute',placement:'bottom',middlewareData:{}};},
      autoUpdate:(_r,_f,update)=>{window.observerUpdate=update;return ()=>window.cleanups++;}
    };
    window.ResizeObserver=class {constructor(callback){window.resizeUpdate=callback;}observe(){}disconnect(){}};
    const url=URL.createObjectURL(new Blob([source],{type:'text/javascript'}));
    window.positioning=await import(url);URL.revokeObjectURL(url);
  },source);
}
for(const entry of ['autoUpdate','showFloating'])for(const detach of ['reference','floating'])for(const virtual of [false,true])
test(entry+' ignores detached '+detach+' (virtual='+virtual+')',async()=>{
  const page=await browser.newPage(),errors=[];page.on('pageerror',e=>errors.push(String(e)));
  try {
    await setup(page);
    const updates=await page.evaluate(async({entry,detach,virtual})=>{
      const anchor=document.getElementById('reference'),floating=document.getElementById('floating');
      const reference=virtual?{contextElement:anchor,getBoundingClientRect:()=>anchor.getBoundingClientRect()}:anchor;
      if(entry==='autoUpdate')await positioning.autoUpdate(reference,floating);else await positioning.showFloating(floating,reference,{});
      const initial=window.updates;
      // Resize callback queues a frame before removal; scroll callback arrives after.
      window.resizeUpdate();document.getElementById(detach).remove();window.observerUpdate();
      await new Promise(resolve=>requestAnimationFrame(()=>requestAnimationFrame(resolve)));
      return {initial,final:window.updates};
    },{entry,detach,virtual});
    await page.waitForTimeout(50);
    assert.equal(updates.final,updates.initial);assert.deepEqual(errors,[]);
  }finally{await page.close();}
});
for(const action of ['hide','show-again'])test('first observer (id zero) is cleaned by '+action,async()=>{
  const page=await browser.newPage();
  try {
    await setup(page);
    const result=await page.evaluate(async action=>{
      const anchor=document.getElementById('reference'),floating=document.getElementById('floating');
      await positioning.showFloating(floating,anchor,{});
      const id=floating._autoUpdateCleanupId;
      if(action==='hide')positioning.hideFloating(floating);else await positioning.showFloating(floating,anchor,{});
      return {id,cleanups:window.cleanups};
    },action);
    assert.equal(result.id,0);assert.equal(result.cleanups,1);
  }finally{await page.close();}
});
test('attached observers still update and invalid initial anchors still reject',async()=>{
  const page=await browser.newPage();
  try {
    await setup(page);
    const result=await page.evaluate(async()=>{
      const anchor=document.getElementById('reference'),floating=document.getElementById('floating');
      const handle=await positioning.autoUpdate(anchor,floating);
      window.observerUpdate();window.resizeUpdate();
      await new Promise(resolve=>requestAnimationFrame(()=>requestAnimationFrame(resolve)));
      handle.apply();const updates=window.updates;window.observerUpdate();
      let rejected=false;
      try{await positioning.computePosition(document.createElement('button'),floating);}catch{rejected=true;}
      return {updates,afterDispose:window.updates,rejected};
    });
    assert.equal(result.updates,3);assert.equal(result.afterDispose,3);assert.equal(result.rejected,true);
  }finally{await page.close();}
});
