import { test } from 'node:test';
import assert from 'node:assert/strict';
import { chromium } from 'playwright';
import { readFile } from 'node:fs/promises';
const floatingScript = await readFile(new URL('./node_modules/@floating-ui/core/dist/floating-ui.core.umd.js', import.meta.url),'utf8') + '\n' + await readFile(new URL('./node_modules/@floating-ui/dom/dist/floating-ui.dom.umd.js', import.meta.url),'utf8');
const url = process.env.NEOUI_TEST_URL ?? 'http://localhost:5198';
const pause = ms => new Promise(resolve => setTimeout(resolve, ms));
const path = process.env.NEOUI_EXPECT_DEFAULT === 'Browser' ? '/?sweep=true&global=true' : '/?sweep=true';
test('MultiSelect acknowledges the latest query without waiting for cascading render completion', async t => {
    if(process.env.NEOUI_TEST_RENDERER==='wasm'){t.skip('Server render backpressure');return;}
    const browser=await chromium.launch({channel:'msedge',headless:true});const page=await browser.newPage();
    await page.addInitScript(floatingScript);
    await page.addInitScript(()=>{const W=window.WebSocket;window.__held=[];window.WebSocket=class extends W {send(d){const message=typeof d==='string'?d:new TextDecoder().decode(d);if(window.__hold&&message.includes('OnRenderCompleted'))window.__held.push(()=>super.send(d));else super.send(d)}};window.__release=()=>{window.__hold=false;for(const send of window.__held.splice(0))send()};});
    try {
        await page.goto(url+path);
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
