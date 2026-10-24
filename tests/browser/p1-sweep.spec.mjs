import { test } from 'node:test';
import assert from 'node:assert/strict';
import { chromium, webkit } from 'playwright';
import { readFile } from 'node:fs/promises';
const floatingScript = await readFile(new URL('./node_modules/@floating-ui/core/dist/floating-ui.core.umd.js', import.meta.url),'utf8') + '\n' + await readFile(new URL('./node_modules/@floating-ui/dom/dist/floating-ui.dom.umd.js', import.meta.url),'utf8');
const url = process.env.NEOUI_TEST_URL ?? 'http://localhost:5198';
const launch = () => process.env.NEOUI_BROWSER_ENGINE === 'webkit' ? webkit.launch({headless:true}) : chromium.launch({channel:'msedge',headless:true});
const sweepPath = process.env.NEOUI_EXPECT_DEFAULT === 'Browser' || process.env.NEOUI_TEST_RENDERER === 'wasm' ? '/?sweep=true&global=true' : '/?sweep=true';
const pause = ms => new Promise(resolve => setTimeout(resolve, ms));

async function open() {
    const browser = await launch();
    const page = await browser.newPage();
    await page.addInitScript(floatingScript);
    await page.goto(url + sweepPath);
    await page.getByTestId('p1-sweep').waitFor();
    return {browser,page};
}

test('application Browser policy covers multiline and grouped controls and the form flushes all drafts', async () => {
    const {browser,page} = await open();
    try {
        for (const [name,value] of [['Sweep single line','Single'],['Sweep textarea','Line one\nLine two'],['Sweep group input','Grouped'],['Sweep group textarea','Group line one\nGroup line two']]) {
            const input = page.getByPlaceholder(name,{exact:true});
            await input.locator('xpath=self::*[@data-neo-input-ready="true"]').waitFor({timeout:3000});
            await input.fill(value);
        }
        await page.getByTestId('sweep-save').click();
        await page.getByTestId('sweep-saved').filter({hasText:'Single|Line one\nLine two|Grouped|Group line one\nGroup line two'}).waitFor();
    } finally { await browser.close(); }
});

test('MultiSelect acknowledges the latest query without waiting for cascading render completion', async t => {
    if(process.env.NEOUI_TEST_RENDERER==='wasm'){t.skip('Server render backpressure');return;}
    const browser=await launch();const page=await browser.newPage();
    await page.addInitScript(floatingScript);
    await page.addInitScript(()=>{const W=window.WebSocket;window.__held=[];window.WebSocket=class extends W {send(d){const message=typeof d==='string'?d:new TextDecoder().decode(d);if(window.__hold&&message.includes('OnRenderCompleted'))window.__held.push(()=>super.send(d));else super.send(d)}};window.__release=()=>{window.__hold=false;for(const send of window.__held.splice(0))send()};});
    try {
        await page.goto(url+sweepPath);
        const trigger=page.getByRole('combobox').filter({hasText:'Sweep select'});
        await page.waitForFunction(()=>{const button=document.querySelector('[data-testid="p1-sweep"] button[role="combobox"]');return document.getElementById(button?.getAttribute('aria-controls'))?.hasAttribute('data-neo-browser-content');});
        await trigger.click();
        const input=page.getByPlaceholder('Sweep multi search',{exact:true});
        await input.locator('xpath=self::*[@data-neo-input-ready="true"]').waitFor();
        await page.evaluate(()=>{window.__hold=true;setTimeout(window.__release,8000)});
        for(let index=0;index<16;index++){await input.fill(index%2?'Product':'Design');await pause(420);}
        await input.fill('latest draft');await page.waitForFunction(()=>!window.__hold);
        await page.waitForFunction(()=>!document.querySelector('input[placeholder="Sweep multi search"]').hasAttribute('data-neo-input-pending'));
        assert.equal(await input.inputValue(),'latest draft');
    } finally { await browser.close(); }
});


test('hover and tooltip reveal, persist across pointer transitions and dismiss without circuit traffic', async () => {
    const {browser,page}=await open();
    try {
        const hover=page.getByRole('button',{name:'Sweep hover',exact:true});
        const tooltip=page.getByRole('button',{name:'Sweep tooltip',exact:true});
        const content=page.getByTestId('sweep-hover-child');
        await content.locator('xpath=ancestor::*[@data-neo-browser-content]').waitFor({state:'attached',timeout:3000});
        await page.getByRole('tooltip',{includeHidden:true}).locator('xpath=self::*[@data-neo-browser-content]').waitFor({state:'attached',timeout:3000});
        await page.evaluate(()=>{window.__nativeSend=WebSocket.prototype.send;WebSocket.prototype.send=function(){};});
        await hover.hover();await content.waitFor({state:'visible'});
        await content.hover();await pause(150);assert.equal(await content.isVisible(),true);
        await page.mouse.move(5,5);await content.waitFor({state:'hidden'});
        await tooltip.focus();await page.getByRole('tooltip').waitFor({state:'visible'});
        assert.match(await tooltip.getAttribute('aria-describedby'),/tooltip/);
        await tooltip.press('Escape');await page.getByRole('tooltip',{includeHidden:true}).waitFor({state:'hidden'});
    } finally { await page.evaluate(()=>{if(window.__nativeSend)WebSocket.prototype.send=window.__nativeSend;}).catch(()=>{});await browser.close(); }
});


test('disclosures and tabs own expansion, disabled skipping, roving focus and mounted panels locally', async () => {
    const {browser,page}=await open();
    try {
        const root=page.getByTestId('sweep-disclosures');
        await page.waitForFunction(()=>document.querySelectorAll('[data-testid="sweep-disclosures"] [data-neo-state-ready="true"]').length===3);
        await page.evaluate(()=>{window.__nativeSend=WebSocket.prototype.send;WebSocket.prototype.send=function(){};});
        const first=root.getByRole('button',{name:'First disclosure',exact:true});
        await first.click();assert.equal(await first.getAttribute('aria-expanded'),'true');
        await root.getByRole('button',{name:'First disclosure action',exact:true}).waitFor({state:'visible'});
        await first.press('ArrowDown');assert.equal(await root.getByRole('button',{name:'Second disclosure',exact:true}).evaluate(element=>element===document.activeElement),true);
        await page.keyboard.press('Enter');await root.getByText('Second disclosure body',{exact:true}).waitFor();
        assert.equal(await first.getAttribute('aria-expanded'),'false');
        const standalone=root.getByRole('button',{name:'Standalone disclosure',exact:true});await standalone.click();
        await root.getByText('Standalone disclosure body',{exact:true}).waitFor();await standalone.click();
        await root.getByText('Standalone disclosure body',{exact:true}).waitFor({state:'hidden'});
        const tab=root.getByRole('tab',{name:'First local tab',exact:true});await tab.focus();await tab.press('ArrowRight');
        assert.equal(await root.getByRole('tab',{name:'Second local tab',exact:true}).getAttribute('aria-selected'),'true');
        await root.getByRole('tabpanel',{name:'Second local tab',exact:true}).waitFor();
        assert.equal(await root.getByRole('tabpanel',{name:'First local tab',exact:true,includeHidden:true}).count(),1);
    } finally {await page.evaluate(()=>{if(window.__nativeSend)WebSocket.prototype.send=window.__nativeSend;}).catch(()=>{});await browser.close();}
});


test('a browser tooltip preserves its action button callback and closes before application work finishes', async () => {
    const {browser,page}=await open();
    try {
        const button=page.getByRole('button',{name:'Sweep tooltip',exact:true});
        await page.getByRole('tooltip',{includeHidden:true}).locator('xpath=self::*[@data-neo-browser-content]').waitFor({state:'attached'});
        await button.focus();await page.getByRole('tooltip').waitFor();
        await button.click();await page.getByRole('tooltip',{includeHidden:true}).waitFor({state:'hidden'});
        await page.getByTestId('sweep-tooltip-pending').filter({hasText:'Waiting'}).waitFor();
        assert.equal(await page.getByTestId('sweep-tooltip-actions').textContent(),'0');
        await page.getByTestId('sweep-tooltip-release').click();
        await page.getByTestId('sweep-tooltip-actions').filter({hasText:'1'}).waitFor();
    } finally {await browser.close();}
});


test('textarea initial values exist in server-rendered HTML before hydration', {skip:process.env.NEOUI_TEST_RENDERER==='wasm'}, async () => {
    const browser=await launch();const page=await browser.newPage();
    try {
        await page.route('**/_framework/blazor.web.js', route=>route.abort());
        await page.goto(url+sweepPath);
        assert.equal(await page.getByPlaceholder('Sweep textarea',{exact:true}).inputValue(),'Initial note\nSecond initial line');
        assert.equal(await page.getByPlaceholder('Sweep group textarea',{exact:true}).inputValue(),'Initial grouped note');
    } finally {await browser.close();}
});

test('removing text adapters during module import leaves the circuit and subsequent controls usable', async () => {
    const browser=await launch();const page=await browser.newPage();
    let release;const held=new Promise(resolve=>release=resolve);let imported=false;
    try {
        await page.route(/\/js\/input(?:\.[^/]+)?\.js(?:\?.*)?$/,async route=>{imported=true;await held;await route.continue();});
        await page.goto(url+sweepPath);
        await page.waitForFunction(()=>Boolean(window.Blazor));
        for(let attempt=0;attempt<100&&!imported;attempt++)await pause(50);
        assert.equal(imported,true,'Input module request was delayed');
        await page.getByTestId('sweep-toggle-inputs').click();
        await page.getByPlaceholder('Sweep textarea',{exact:true}).waitFor({state:'detached'});
        release();await page.getByTestId('sweep-independent').click();
        await page.getByTestId('sweep-independent').filter({hasText:'Independent 1'}).waitFor();
        await page.getByTestId('sweep-toggle-inputs').click();
        const input=page.getByPlaceholder('Sweep textarea',{exact:true});
        await input.locator('xpath=self::*[@data-neo-input-ready="true"]').waitFor();
        await input.fill('Remounted draft');await page.getByTestId('sweep-save').click();
        await page.getByTestId('sweep-saved').filter({hasText:'Remounted draft'}).waitFor();
    } finally {release();await browser.close();}
});

test('Escape dismisses a pointer-opened hover card when focus remains outside', async () => {
    const {browser,page}=await open();
    try {
        const content=page.getByTestId('sweep-hover-child');
        await content.locator('xpath=ancestor::*[@data-neo-browser-content]').waitFor({state:'attached'});
        await page.getByRole('button',{name:'Sweep hover',exact:true}).hover();await content.waitFor();
        await page.keyboard.press('Escape');await content.waitFor({state:'hidden'});
    } finally {await browser.close();}
});


test('manual vertical tabs move focus locally and a controlled reset invalidates delayed echoes', async () => {
    const {browser,page}=await open();
    try {
        const root=page.getByTestId('sweep-controlled');
        await root.locator('[data-neo-state-ready="true"]').waitFor();
        const first=root.getByRole('tab',{name:'Controlled first',exact:true}),second=root.getByRole('tab',{name:'Controlled second',exact:true});
        await first.focus();await first.press('ArrowDown');
        assert.equal(await second.evaluate(element=>element===document.activeElement),true);
        assert.equal(await first.getAttribute('aria-selected'),'true');
        await second.press('Enter');assert.equal(await second.getAttribute('aria-selected'),'true');
        await page.getByTestId('sweep-tab-started').filter({hasText:'1'}).waitFor();
        await page.getByTestId('sweep-reset-tab').click();
        await first.locator('xpath=self::*[@aria-selected="true"]').waitFor();
        await pause(900);assert.equal(await first.getAttribute('aria-selected'),'true');
        assert.equal(await page.getByTestId('sweep-tab-value').textContent(),'one');
        assert.equal(await page.getByTestId('sweep-tab-started').textContent(),'1','Reset does not replay an application callback');
    } finally {await browser.close();}
});

test('removing a shared field during controller import preserves the circuit and subsequent mounts', async () => {
    const browser=await launch(),page=await browser.newPage();
    let release;const held=new Promise(resolve=>release=resolve);let imported=false;
    try{
        await page.route(/\/js\/primitives\/browser-state(?:\.[^/]+)?\.js(?:\?.*)?$/,async route=>{imported=true;await held;await route.continue();});
        await page.goto(url+sweepPath);for(let attempt=0;attempt<100&&!imported;attempt++)await pause(50);
        assert.equal(imported,true);await page.getByTestId('sweep-toggle-field').click();await page.getByTestId('sweep-delayed').waitFor({state:'detached'});
        release();await page.getByTestId('sweep-independent').click();await page.getByTestId('sweep-independent').filter({hasText:'Independent 1'}).waitFor();
        await page.getByTestId('sweep-toggle-field').click();const root=page.getByTestId('sweep-delayed');await root.locator('[data-neo-state-ready="true"]').waitFor();
        await root.getByRole('checkbox').click();await page.getByTestId('sweep-delay-value').filter({hasText:'True'}).waitFor();
    }finally{release();await browser.close();}
});

test('an AsChild disclosure preserves its application action without waiting to expand',async()=>{
    const {browser,page}=await open();
    try{
        const root=page.getByTestId('sweep-disclosures');await page.waitForFunction(()=>document.querySelectorAll('[data-testid="sweep-disclosures"] [data-neo-state-ready="true"]').length===3);
        const immediate=await root.getByRole('button',{name:'Standalone disclosure',exact:true}).evaluate(button=>{button.click();return {visible:!document.getElementById(button.getAttribute('aria-controls')).hidden,count:document.querySelector('[data-testid="sweep-disclosure-actions"]').textContent};});
        assert.deepEqual(immediate,{visible:true,count:'0'});await page.getByTestId('sweep-disclosure-actions').filter({hasText:'1'}).waitFor();
    }finally{await browser.close();}
});


for (const mode of ['blazor','browser']) for (const family of ['collapsible','accordion'])
test(`${mode} ${family} animates expansion and collapse without losing final presence`, async()=>{
    const browser=await launch(),page=await browser.newPage(),errors=[];
    page.on('pageerror',error=>errors.push(String(error)));
    try {
        await page.emulateMedia({reducedMotion:'no-preference'});await page.addInitScript(floatingScript);
        await page.goto(url+`/?sweep=true&mode=${mode}`);await page.getByTestId('p1-sweep').waitFor();
        await page.locator('[data-testid="test-host-ready"][data-ready="true"]').waitFor({state:'attached'});
        const root=page.getByTestId('sweep-disclosures');
        if(mode==='browser') await page.waitForFunction(()=>document.querySelectorAll('[data-testid="sweep-disclosures"] [data-neo-state-ready=true]').length===3);
        const trigger=root.getByRole('button',{name:family==='collapsible'?'Standalone disclosure':'First disclosure',exact:true});
        const panel=root.getByRole('region',{includeHidden:true}).filter({hasText:family==='collapsible'?'Standalone disclosure body':'First disclosure action'});
        const initial=await panel.evaluate(el=>({property:getComputedStyle(el).transitionProperty,duration:getComputedStyle(el).transitionDuration,height:el.getBoundingClientRect().height}));
        assert.match(initial.property,/grid-template-rows/);assert.ok(parseFloat(initial.duration)>0);assert.equal(initial.height,0);
        // Cold WASM work can outlast the 200 ms default before transitionrun is
        // dispatched. Slow the CSS clock, then pause/finish it without wall-clock waits.
        await panel.evaluate(el=>el.style.transitionDuration='2s');
        await page.evaluate(()=>new Promise(resolve=>requestAnimationFrame(resolve)));
        if(mode==='browser') await page.evaluate(()=>{window.__nativeSend=WebSocket.prototype.send;WebSocket.prototype.send=function(){};});
        const arm=()=>panel.evaluate(el=>{
            window.__neoDisclosureMotion=null;
            const capture=event=>{
                if(event.target!==el||event.propertyName!=='grid-template-rows')return;
                const animation=el.getAnimations().find(item=>item.transitionProperty==='grid-template-rows');
                if(!animation)return;
                animation.pause();window.__neoDisclosureMotion=animation;el.removeEventListener('transitionrun',capture);
            };
            el.addEventListener('transitionrun',capture);
        });
        const midpoint=async()=>{
            await page.waitForFunction(()=>window.__neoDisclosureMotion?.playState==='paused',undefined,{timeout:10000});
            return panel.evaluate(el=>{const animation=window.__neoDisclosureMotion;animation.currentTime=animation.effect.getComputedTiming().activeDuration/2;return {height:el.getBoundingClientRect().height,hidden:el.hidden,inert:el.inert,aria:el.getAttribute('aria-hidden')};});
        };
        const finish=()=>page.evaluate(async()=>{const animation=window.__neoDisclosureMotion;animation.finish();await animation.finished;await new Promise(resolve=>requestAnimationFrame(resolve));});
        await arm();await trigger.click();const opening=await midpoint();assert.equal(opening.hidden,false);assert.ok(opening.height>0);await finish();
        const fullHeight=await panel.evaluate(el=>el.getBoundingClientRect().height);assert.ok(opening.height<fullHeight,`${family} must have an intermediate opening height`);
        await arm();await trigger.click();const closing=await midpoint();assert.equal(closing.hidden,false);assert.ok(closing.height>0&&closing.height<fullHeight);
        if(mode==='browser'){
            assert.equal(closing.inert,true);assert.equal(closing.aria,'true');
            // A child render during exit must not repaint hidden before motion finishes.
            await panel.evaluate(el=>el.append(document.createComment('render during exit')));
            await page.evaluate(()=>new Promise(resolve=>requestAnimationFrame(resolve)));assert.equal(await panel.evaluate(el=>el.hidden),false);
            // Reverse the paused close. Its cancelled completion must not hide the reopened panel.
            await arm();await trigger.click();await midpoint();await finish();assert.equal(await panel.evaluate(el=>el.hidden||el.inert),false);
            assert.equal(await trigger.getAttribute('aria-expanded'),'true');
            await arm();await trigger.click();await midpoint();
        }
        await finish();await panel.waitFor({state:'hidden'});assert.equal(await trigger.getAttribute('aria-expanded'),'false');
        if(mode==='browser'){
            assert.equal(await panel.evaluate(el=>el.hidden&&el.inert),true);
            // Zero-duration CSS (including reduced-motion overrides) needs no presence delay.
            await panel.evaluate(el=>el.style.transitionDuration='0s');await trigger.click();await panel.waitFor();
            assert.equal(await trigger.evaluate(el=>{el.click();return document.getElementById(el.getAttribute('aria-controls')).hidden;}),true);
        }
        assert.deepEqual(errors,[]);
    } finally {await page.evaluate(()=>{if(window.__nativeSend)WebSocket.prototype.send=window.__nativeSend;}).catch(()=>{});await browser.close();}
});
