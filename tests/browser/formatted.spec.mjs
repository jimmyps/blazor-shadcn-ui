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


async function ready(page,name) {
    const input=page.getByPlaceholder(name,{exact:true});
    await input.locator('xpath=self::*[@data-neo-input-ready="true"]').waitFor();return input;
}
test('formatted drafts preserve incomplete numbers, locale decimals and local mask/caret editing',async()=>{
    const {browser,page}=await open();
    try {
        const numeric=await ready(page,'Sweep numeric'),currency=await ready(page,'Sweep currency'),masked=await ready(page,'Sweep masked');
        await page.evaluate(()=>{window.__nativeSend=WebSocket.prototype.send;WebSocket.prototype.send=function(){};});
        await numeric.fill('-');await pause(100);assert.equal(await numeric.inputValue(),'-');
        await numeric.fill('1.');await pause(100);assert.equal(await numeric.inputValue(),'1.');
        await currency.fill('12,34');assert.equal(await currency.inputValue(),'12,34');
        await masked.fill('123456');assert.equal(await masked.inputValue(),'123-456');
        await masked.evaluate(element=>element.setSelectionRange(4,4));await masked.press('Backspace');
        assert.equal(await masked.inputValue(),'124-56_');
        await masked.fill('');assert.equal(await masked.inputValue(),'');
    } finally {await page.evaluate(()=>{if(window.__nativeSend)WebSocket.prototype.send=window.__nativeSend;}).catch(()=>{});await browser.close();}
});
test('form flush commits typed numbers, culture values and raw masks before submit, then revisions reset drafts',async()=>{
    const {browser,page}=await open();
    try {
        const numeric=await ready(page,'Sweep numeric'),currency=await ready(page,'Sweep currency'),masked=await ready(page,'Sweep masked');
        await numeric.fill('150');await currency.fill('12,34');await masked.fill('987654');
        await page.getByTestId('sweep-number-save').click();
        await page.waitForFunction(()=>document.querySelector('[placeholder="Sweep numeric"]').value==='100');
        await page.waitForFunction(()=>document.querySelector('[data-testid="sweep-formatted"] form').getAttribute('aria-busy')==='false');
        await page.getByTestId('sweep-number-save').click();
        await page.getByTestId('sweep-number-saved').filter({hasText:'100|12.34|987654'}).waitFor();
        await page.getByTestId('sweep-number-reset').click();
        await page.waitForFunction(()=>document.querySelector('[placeholder="Sweep numeric"]').value==='10');
        assert.equal(await masked.inputValue(),'123-456');
    } finally {await browser.close();}
});
test('incomplete numeric and required mask drafts prevent submitting an older model',async()=>{
    const {browser,page}=await open();
    try {
        const numeric=await ready(page,'Sweep numeric'),masked=await ready(page,'Sweep masked');
        await numeric.fill('-');await masked.fill('123456');
        await page.getByTestId('sweep-number-save').click();await pause(400);
        assert.equal(await page.getByTestId('sweep-number-saved').textContent(),'None');
        assert.equal(await numeric.evaluate(element=>element.checkValidity()),false);
        await numeric.fill('25');await masked.fill('12');await page.getByTestId('sweep-number-save').click();await pause(400);
        assert.equal(await page.getByTestId('sweep-number-saved').textContent(),'None');
        assert.equal(await masked.evaluate(element=>element.checkValidity()),false);
    } finally {await browser.close();}
});

test('a numerically valid string outside the .NET target range cannot submit an older parsed value',async()=>{
    const {browser,page}=await open();
    try{
        const numeric=await ready(page,'Sweep numeric'),masked=await ready(page,'Sweep masked');
        await numeric.fill('9999999999999999999999999999999999999999999999');await masked.fill('123456');
        await page.getByTestId('sweep-number-save').click();await numeric.locator('xpath=self::*[@data-neo-input-invalid]').waitFor();
        assert.equal(await page.getByTestId('sweep-number-saved').textContent(),'None');
        assert.equal(await numeric.evaluate(element=>element.checkValidity()),false);
        await numeric.fill('12');await page.getByTestId('sweep-number-save').click();
        await page.getByTestId('sweep-number-saved').filter({hasText:'12||123456'}).waitFor();
    }finally{await browser.close();}
});

test('readonly formatted fields participate in Save without forcing a change callback',async()=>{
    const {browser,page}=await open();
    try{
        const numeric=await ready(page,'Sweep numeric'),masked=await ready(page,'Sweep masked'),readonly=await ready(page,'Sweep readonly numeric');
        assert.equal(await readonly.evaluate(element=>element.readOnly),true);
        await numeric.fill('15');await masked.fill('123456');await page.getByTestId('sweep-number-save').click();
        await page.getByTestId('sweep-number-saved').filter({hasText:'15||123456'}).waitFor();
        assert.equal(await page.getByTestId('sweep-readonly-commits').textContent(),'0');
    }finally{await browser.close();}
});

test('an empty mask template sends onchange callbacks only across raw empty/nonempty boundaries',async()=>{
    const {browser,page}=await open();
    try{
        const input=page.getByRole('textbox',{name:'Sweep boundary mask',exact:true});await input.locator('xpath=self::*[@data-neo-input-ready="true"]').waitFor();
        await input.fill('1');await page.getByTestId('sweep-mask-boundaries').filter({hasText:'1'}).waitFor();
        await input.fill('12');await pause(200);assert.equal(await page.getByTestId('sweep-mask-boundaries').textContent(),'1');
        await input.fill('');await page.getByTestId('sweep-mask-boundaries').filter({hasText:'2'}).waitFor();assert.equal(await input.inputValue(),'');
    }finally{await browser.close();}
});
