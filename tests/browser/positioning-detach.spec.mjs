import {test,before,after} from 'node:test';
import assert from 'node:assert/strict';
import {createRequire} from 'node:module';
import {readFile} from 'node:fs/promises';
const require=createRequire(import.meta.url);
const {chromium,webkit}=require(process.env.NEOUI_PLAYWRIGHT_MODULE || 'playwright');
const source=await readFile(process.env.NEOUI_POSITIONING_MODULE || new URL('../../src/NeoUI.Blazor.Primitives/wwwroot/js/primitives/positioning.js',import.meta.url),'utf8');
let browser;
before(async()=>browser=await (process.env.NEOUI_BROWSER_ENGINE==='webkit'?webkit.launch({headless:true}):chromium.launch({channel:'msedge',headless:true})));
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

// Exercise the real Browser controller and legacy showFloating path. The blur is
// on the wrapper, so its radius must match before the first visible paint.
const browserFloatingSource=await readFile(new URL('../../src/NeoUI.Blazor.Primitives/wwwroot/js/primitives/browser-floating.js',import.meta.url),'utf8');
for(const mode of ['Blazor','Browser'])test(mode+' synchronizes translucent wrapper corners on open, reopen and resize',async()=>{
  const page=await browser.newPage(),errors=[];page.on('pageerror',e=>errors.push(String(e)));
  try {
    await setup(page);
    await page.evaluate(async({source,browserFloatingSource,mode})=>{
      document.head.insertAdjacentHTML('beforeend','<style>.rounded-md{border-radius:6px}.rounded-3xl{border-radius:24px}[data-portal-content]{backdrop-filter:blur(18px)}</style>');
      const wrapper=document.getElementById('floating');
      wrapper.setAttribute('data-portal-content','');wrapper.className='keep-wrapper-class';
      wrapper.innerHTML='<div id="content" data-state="closed" class="rounded-md" tabindex="-1">Menu</div>';
      const url=URL.createObjectURL(new Blob([source],{type:'text/javascript'}));
      window.radiusPositioning=await import(url);
      const controllerUrl=URL.createObjectURL(new Blob([browserFloatingSource.replace("'./positioning.js'",JSON.stringify(url))],{type:'text/javascript'}));
      const floatingModule=await import(controllerUrl);
      window.radiusController=mode==='Browser'?floatingModule.createController(wrapper,{kind:'popover',triggerId:'reference',contentId:'content',open:false},{placement:'bottom'}):null;
      URL.revokeObjectURL(controllerUrl);URL.revokeObjectURL(url);
      window.openRadius=async()=>{if(radiusController)await radiusController.open('none');else await radiusPositioning.showFloating(wrapper,document.getElementById('reference'),{});};
      window.closeRadius=()=>{if(radiusController)radiusController.close();else radiusPositioning.hideFloating(wrapper);};
    },{source,browserFloatingSource,mode});
    for(const radius of ['6px','24px']){
      await page.evaluate(async radius=>{
        document.getElementById('content').className=radius==='6px'?'rounded-md':'rounded-3xl';
        await openRadius();
      },radius);
      // No extra animation-frame wait: opening itself must install the radius.
      assert.equal(await page.locator('#floating').evaluate(el=>getComputedStyle(el).borderTopLeftRadius),radius);
      assert.equal(await page.locator('#floating').evaluate(el=>el.classList.contains('keep-wrapper-class')),true);
      await page.evaluate(async()=>{
        window.resizeUpdate();window.observerUpdate();
        await new Promise(resolve=>requestAnimationFrame(()=>requestAnimationFrame(resolve)));
      });
      assert.equal(await page.locator('#floating').evaluate(el=>getComputedStyle(el).borderTopLeftRadius),radius);
      const classWrites=await page.evaluate(async()=>{
        let writes=0;const wrapper=document.getElementById('floating');
        const observer=new MutationObserver(records=>writes+=records.length);
        observer.observe(wrapper,{attributes:true,attributeFilter:['class']});
        radiusPositioning.applyPosition(wrapper,{x:0,y:20});
        await Promise.resolve();observer.disconnect();return writes;
      });
      assert.equal(classWrites,0,'unchanged positioning must not rewrite radius classes');
      await page.evaluate(()=>closeRadius());
    }
    assert.deepEqual(errors,[]);
  }finally{await page.close();}
});
