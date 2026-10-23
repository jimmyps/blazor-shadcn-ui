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


test('markdown typing, shortcuts, list continuation, undo and toolbar formatting stay local',async()=>{
    const {browser,page}=await open();
    try{
        const input=page.getByPlaceholder('Sweep markdown',{exact:true});await input.locator('xpath=self::*[@data-neo-input-ready="true"]').waitFor();
        await page.evaluate(()=>{window.__nativeSend=WebSocket.prototype.send;WebSocket.prototype.send=function(){};});
        await input.fill('draft');await input.selectText();await input.press('Control+b');assert.equal(await input.inputValue(),'**draft**');
        await input.press('Control+z');assert.equal(await input.inputValue(),'draft');await pause(60);await input.press('Control+y');assert.equal(await input.inputValue(),'**draft**');
        await input.fill('- first');await input.press('End');await input.press('Enter');assert.equal(await input.inputValue(),'- first\n- ');
        await page.getByRole('button',{name:'Italic (Ctrl+I)',exact:true}).click();assert.match(await input.inputValue(),/\*italic text\*/);
    }finally{await page.evaluate(()=>{if(window.__nativeSend)WebSocket.prototype.send=window.__nativeSend;}).catch(()=>{});await page.close();}
});
test('markdown joins the form barrier and explicit reset clears prior undo history',async()=>{
    const {browser,page}=await open();
    try{
        const input=page.getByPlaceholder('Sweep markdown',{exact:true});await input.locator('xpath=self::*[@data-neo-input-ready="true"]').waitFor();
        await input.fill('**Latest markdown**');await page.getByTestId('sweep-markdown-save').click();
        await page.getByTestId('sweep-markdown-saved').filter({hasText:'**Latest markdown**'}).waitFor();
        await page.getByTestId('sweep-markdown-reset').click();await page.waitForFunction(()=>document.querySelector('[placeholder="Sweep markdown"]').value==='Reset document');
        await input.focus();await input.press('Control+z');assert.equal(await input.inputValue(),'Reset document');
    }finally{await page.close();}
});
