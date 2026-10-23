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


async function tagReady(page){const root=page.getByTestId('sweep-tags');await root.locator('[data-neo-state-ready="true"]').waitFor();const input=root.getByPlaceholder('Sweep tags');await page.waitForFunction(()=>Boolean(document.getElementById(document.querySelector('[placeholder="Sweep tags"]').getAttribute('aria-controls'))));return {root,input};}
test('tag suggestions, hover, tokenization, paste and removal stay local',async()=>{
    const {browser,page}=await open();
    try{
        const {root,input}=await tagReady(page);
        await page.evaluate(()=>{window.__nativeSend=WebSocket.prototype.send;WebSocket.prototype.send=function(){};});
        await input.fill('Des');const option=page.getByRole('option',{name:'Design',exact:true});await option.waitFor();await option.hover();
        assert.equal(await option.getAttribute('aria-selected'),'true');await input.press('Enter');assert.equal(await root.locator('[data-neo-tag-values]').textContent(),'Design×');
        await input.fill('Product');await input.press(',');assert.match(await root.locator('[data-neo-tag-values]').textContent(),/Product/);
        await input.evaluate(element=>element.dispatchEvent(new ClipboardEvent('paste',{bubbles:true,cancelable:true,clipboardData:(()=>{const data=new DataTransfer();data.setData('text','One,Two');return data;})()})));
        assert.match(await root.locator('[data-neo-tag-values]').textContent(),/One×Two×/);
        await input.press('Backspace');assert.doesNotMatch(await root.locator('[data-neo-tag-values]').textContent(),/Two/);
    }finally{await page.evaluate(()=>{if(window.__nativeSend)WebSocket.prototype.send=window.__nativeSend;}).catch(()=>{});await page.close();}
});
test('Save captures an un-tokenized tag draft and server rejection/reset reconcile locally',async()=>{
    const {browser,page}=await open();
    try{
        const {root,input}=await tagReady(page);await input.fill('Latest');await page.getByTestId('sweep-tags-save').click();
        await page.getByTestId('sweep-tags-saved').filter({hasText:'Latest'}).waitFor();
        await input.fill('reject');await input.press('Enter');
        await page.waitForFunction(()=>!document.querySelector('[data-testid="sweep-tags"] [data-neo-browser-field]').hasAttribute('data-neo-field-pending'));
        assert.doesNotMatch(await root.locator('[data-neo-tag-values]').textContent(),/reject/);
        await page.getByTestId('sweep-tags-reset').click();await root.locator('[data-neo-tag-values]').filter({hasText:'Reset'}).waitFor();assert.equal(await input.inputValue(),'');
    }finally{await page.close();}
});
