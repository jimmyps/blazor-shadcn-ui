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


async function specialReady(page){await page.waitForFunction(()=>document.querySelectorAll('[data-testid="sweep-special"] [data-neo-state-ready="true"]').length===5);}
test('radio, OTP, date/time segments and both range handles respond with the circuit blocked',async()=>{
    const {browser,page}=await open();
    try{
        await specialReady(page);const root=page.getByTestId('sweep-special');
        await page.evaluate(()=>{window.__nativeSend=WebSocket.prototype.send;WebSocket.prototype.send=function(){};});
        const radio=root.getByRole('radio',{name:'Sweep radio one'});await radio.focus();await radio.press('ArrowRight');assert.equal(await root.getByRole('radio',{name:'Sweep radio two'}).getAttribute('aria-checked'),'true');
        const slots=root.locator('[data-neo-otp-slot]');await slots.nth(0).fill('1234');assert.deepEqual(await slots.evaluateAll(elements=>elements.map(element=>element.value)),['1','2','3','4']);
        await slots.nth(3).press('Backspace');assert.equal(await slots.nth(3).inputValue(),'');
        const date=root.getByRole('group',{name:'Sweep date',exact:true});await date.getByRole('spinbutton',{name:'day',exact:true}).focus();await page.keyboard.press('ArrowUp');assert.equal(await date.getByRole('spinbutton',{name:'day',exact:true}).getAttribute('aria-valuenow'),'23');
        const time=root.getByRole('group',{name:'Sweep time',exact:true});await time.getByRole('spinbutton',{name:'minute',exact:true}).focus();await page.keyboard.press('ArrowUp');assert.equal(await time.getByRole('spinbutton',{name:'minute',exact:true}).getAttribute('aria-valuenow'),'46');
        const handles=root.getByRole('slider');await handles.nth(0).focus();await handles.nth(0).press('ArrowRight');assert.equal(await handles.nth(0).getAttribute('aria-valuenow'),'21');
        await handles.nth(1).press('ArrowLeft');assert.equal(await handles.nth(1).getAttribute('aria-valuenow'),'79');
    }finally{await page.evaluate(()=>{if(window.__nativeSend)WebSocket.prototype.send=window.__nativeSend;}).catch(()=>{});await page.close();}
});
test('special fields flush a complete OTP and edited segments before Save, and reset without replay',async()=>{
    const {browser,page}=await open();
    try{
        await specialReady(page);const root=page.getByTestId('sweep-special');
        await root.locator('[data-neo-otp-slot]').nth(0).fill('1234');
        await root.getByRole('group',{name:'Sweep date',exact:true}).getByRole('spinbutton',{name:'day',exact:true}).press('ArrowUp');
        await root.getByRole('group',{name:'Sweep time',exact:true}).getByRole('spinbutton',{name:'minute',exact:true}).press('ArrowUp');
        await root.getByRole('slider').nth(0).press('ArrowRight');
        await root.getByRole('radio',{name:'Sweep radio two'}).click();await page.getByTestId('sweep-special-save').click();
        await page.getByTestId('sweep-special-saved').filter({hasText:'two|1234|2026-10-23|13:46|21|80'}).waitFor();
        await page.getByTestId('sweep-special-reset').click();await page.waitForFunction(()=>document.querySelector('[data-neo-otp-slot]').value==='');
        assert.equal(await root.getByRole('group',{name:'Sweep date',exact:true}).getByRole('spinbutton',{name:'day',exact:true}).getAttribute('aria-valuenow'),'22');
    }finally{await page.close();}
});

test('segmented input picker selection and Apply/Cancel dismiss their shells without a server callback',async()=>{
    const {browser,page}=await open();
    try{
        const root=page.getByTestId('sweep-picker-shells');
        await root.locator('[data-neo-state-ready="true"]').first().waitFor();
        await page.waitForFunction(()=>{const triggers=[...document.querySelectorAll('[data-testid="sweep-picker-shells"] [aria-haspopup]')];return triggers.length===2&&triggers.every(trigger=>document.getElementById(trigger.getAttribute('aria-controls'))?.hasAttribute('data-neo-browser-content'));});
        await page.evaluate(()=>{window.__nativeSend=WebSocket.prototype.send;WebSocket.prototype.send=function(){};});
        await root.getByRole('button',{name:'Open calendar',exact:true}).click();
        const day=page.locator('[data-neo-browser-content] [data-neo-calendar-day]:not([disabled])').first();await day.waitFor();await day.click();await day.waitFor({state:'hidden'});
        await root.getByRole('button',{name:'Open time picker',exact:true}).click();const apply=page.getByRole('button',{name:'Apply',exact:true});await apply.waitFor();await apply.click();await apply.waitFor({state:'hidden'});
        await root.getByRole('button',{name:'Open time picker',exact:true}).click();const cancel=page.getByRole('button',{name:'Cancel',exact:true});await cancel.click();await cancel.waitFor({state:'hidden'});
    }finally{await page.evaluate(()=>{if(window.__nativeSend)WebSocket.prototype.send=window.__nativeSend;}).catch(()=>{});await page.close();}
});

test('the existing segmented FocusAsync API reveals empty placeholders and focuses the first local segment',async()=>{
    const {browser,page}=await open();
    try{
        const root=page.getByTestId('sweep-focus-api');await page.waitForFunction(()=>document.querySelectorAll('[data-testid="sweep-focus-api"] [data-neo-state-ready="true"]').length===2);
        await root.getByRole('button',{name:'Focus empty date',exact:true}).click();
        await page.waitForFunction(()=>document.querySelector('[aria-label="Empty date API"] [data-neo-segment]')===document.activeElement);
        assert.equal(await root.getByRole('group',{name:'Empty date API',exact:true}).locator('[data-neo-segment]').first().isVisible(),true);
        await root.getByRole('button',{name:'Focus empty time',exact:true}).click();
        await page.waitForFunction(()=>document.querySelector('[aria-label="Empty time API"] [data-neo-segment]')===document.activeElement);
        assert.equal(await root.getByRole('group',{name:'Empty time API',exact:true}).locator('[data-neo-segment]').first().isVisible(),true);
    }finally{await page.close();}
});
