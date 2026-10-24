import { test, before, after } from 'node:test';
import assert from 'node:assert/strict';
import { chromium, webkit } from 'playwright';
import { readFile } from 'node:fs/promises';
const floatingScript = await readFile(new URL('./node_modules/@floating-ui/core/dist/floating-ui.core.umd.js', import.meta.url),'utf8') + '\n' + await readFile(new URL('./node_modules/@floating-ui/dom/dist/floating-ui.dom.umd.js', import.meta.url),'utf8');
const url = process.env.NEOUI_TEST_URL ?? 'http://localhost:5198';
const launch = () => process.env.NEOUI_BROWSER_ENGINE === 'webkit' ? webkit.launch({headless:true}) : chromium.launch({channel:'msedge',headless:true});
const sweepPath = process.env.NEOUI_EXPECT_DEFAULT === 'Browser' || process.env.NEOUI_TEST_RENDERER === 'wasm' ? '/?sweep=true&global=true' : '/?sweep=true';
const pause = ms => new Promise(resolve => setTimeout(resolve, ms));

let sharedBrowser;
before(async()=>{sharedBrowser=await launch();});
after(async()=>{await sharedBrowser?.close();});

async function open() {
    const browser = sharedBrowser;
    const page = await browser.newPage();
    await page.addInitScript(floatingScript);
    await page.goto(url + sweepPath);
    await page.getByTestId('p1-sweep').waitFor();
    return {browser,page};
}


async function fieldsReady(page){await page.waitForFunction(()=>document.querySelectorAll('[data-testid="sweep-fields"] [data-neo-state-ready="true"]').length===5);}
test('checkbox, switch, toggle, group navigation and slider preview update without circuit traffic',async()=>{
    const {browser,page}=await open();
    try{
        await fieldsReady(page);
        const root=page.getByTestId('sweep-fields');
        await page.evaluate(()=>{window.__nativeSend=WebSocket.prototype.send;WebSocket.prototype.send=function(){};});
        const check=root.getByRole('checkbox',{name:'Sweep checkbox'}),toggle=root.getByRole('button',{name:'Sweep toggle',exact:true});
        await check.click();assert.equal(await check.getAttribute('aria-checked'),'true');
        await check.press('Space');assert.equal(await check.getAttribute('aria-checked'),'false');
        await root.getByRole('switch',{name:'Sweep switch'}).click();assert.equal(await root.getByRole('switch').getAttribute('aria-checked'),'true');
        await toggle.click();assert.equal(await toggle.getAttribute('aria-pressed'),'true');
        const first=root.getByRole('radio',{name:'Sweep choice one'}),second=root.getByRole('radio',{name:'Sweep choice two'});
        await first.focus();await first.press('ArrowRight');assert.equal(await second.getAttribute('aria-checked'),'true');
        const slider=root.getByRole('slider');await slider.fill('75');assert.match(await slider.getAttribute('style'),/75%/);
    }finally{await page.evaluate(()=>{if(window.__nativeSend)WebSocket.prototype.send=window.__nativeSend;}).catch(()=>{});await page.close();}
});
test('forms await latest data field acceptance, retain required validation and support revisioned resets',async()=>{
    const {browser,page}=await open();
    try{
        await fieldsReady(page);const root=page.getByTestId('sweep-fields');
        await page.getByTestId('sweep-fields-save').click();await pause(200);assert.equal(await page.getByTestId('sweep-fields-saved').textContent(),'None');
        await root.getByRole('checkbox',{name:'Sweep checkbox'}).click();await root.getByRole('switch').click();await root.getByRole('button',{name:'Sweep toggle',exact:true}).click();
        await root.getByRole('radio',{name:'Sweep choice two'}).click();await root.getByRole('slider').fill('75');
        await page.getByTestId('sweep-fields-save').click();await page.getByTestId('sweep-fields-saved').filter({hasText:'True|True|True|two|75'}).waitFor();
        await page.getByTestId('sweep-fields-reset').click();
        await root.getByRole('checkbox',{name:'Sweep checkbox'}).locator('xpath=self::*[@aria-checked="false"]').waitFor();
        assert.equal(await root.getByRole('slider').inputValue(),'30');
    }finally{await page.close();}
});

test('rapid checked changes ignore stale acceptance, reject latest edits and honor resets during application work',async()=>{
    const {browser,page}=await open();
    try{
        const root=page.getByTestId('sweep-delayed');await root.locator('[data-neo-state-ready="true"]').waitFor();
        const check=root.getByRole('checkbox',{name:'Delayed sweep checkbox'});
        await check.click();await check.click();assert.equal(await check.getAttribute('aria-checked'),'false');
        await page.waitForFunction(()=>!document.querySelector('[data-testid="sweep-delayed"] [data-neo-browser-field]').hasAttribute('data-neo-field-pending'));
        assert.equal(await check.getAttribute('aria-checked'),'false');assert.equal(await page.getByTestId('sweep-delay-count').textContent(),'2');
        await page.getByTestId('sweep-delay-reject').click();
        // Read optimistic feedback in the click task, before a fast WASM rejection can complete.
        assert.equal(await check.evaluate(element=>{element.click();return element.getAttribute('aria-checked');}),'true');
        await check.locator('xpath=self::*[@aria-checked="false"]').waitFor();
        await page.getByTestId('sweep-delay-reject').click();await check.click();await page.getByTestId('sweep-delay-reset').click();
        await pause(700);assert.equal(await check.getAttribute('aria-checked'),'false');
        assert.equal(await page.getByTestId('sweep-delay-value').textContent(),'False');
    }finally{await page.close();}
});
test('retained-circuit reconciliation applies a field reset without replaying its business callback',{skip:process.env.NEOUI_TEST_RENDERER==='wasm'},async()=>{
    const {browser,page}=await open();
    try{
        const root=page.getByTestId('sweep-delayed');await root.locator('[data-neo-state-ready="true"]').waitFor();const check=root.getByRole('checkbox');
        await check.click();await page.getByTestId('sweep-delay-value').filter({hasText:'True'}).waitFor();
        await page.getByTestId('sweep-delay-offline-reset').click();await page.getByTestId('sweep-delay-reset-pending').filter({hasText:'True'}).waitFor();
        await page.context().setOffline(true);await pause(1500);await page.context().setOffline(false);
        await check.locator('xpath=self::*[@aria-checked="false"]').waitFor();assert.equal(await page.getByTestId('sweep-delay-count').textContent(),'1');
    }finally{await page.close();}
});
