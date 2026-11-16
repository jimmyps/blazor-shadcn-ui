import { test, before, after } from 'node:test';
import assert from 'node:assert/strict';
import { chromium, webkit, devices } from 'playwright';
import { readFile } from 'node:fs/promises';
const url = process.env.NEOUI_TEST_URL ?? 'http://localhost:5198';
const floating = await readFile(new URL('./node_modules/@floating-ui/core/dist/floating-ui.core.umd.js', import.meta.url),'utf8') + '\n' + await readFile(new URL('./node_modules/@floating-ui/dom/dist/floating-ui.dom.umd.js', import.meta.url),'utf8');
let browser;
before(async () => { browser = await (process.env.NEOUI_BROWSER_ENGINE === 'webkit' ? webkit.launch({headless:true}) : chromium.launch({channel:'msedge',headless:true})); });
after(async () => { await browser?.close(); });
const names = ['Dropdown', 'Split', 'Pill', 'PillSplit', 'Menubar'];
async function open(mode, touch) {
    const page = await browser.newPage(touch ? {...devices['iPhone 13']} : {viewport:{width:1100,height:900}});
    page.setDefaultTimeout(7000);
    const errors = []; page.on('pageerror', e => errors.push(String(e)));
    await page.addInitScript(floating);
    await page.addInitScript(() => {
        const add = EventTarget.prototype.addEventListener;
        EventTarget.prototype.addEventListener = function(type,...args) {
            const result=add.call(this,type,...args);
            if (type==='keydown' && this instanceof HTMLElement && this.getAttribute('role')==='menu') this.dataset.menuKeyboardReady='true';
            return result;
        };
    });
    await page.goto(url + '/?openingfocus=true&mode=' + mode.toLowerCase());
    await page.locator('[data-testid="test-host-ready"][data-ready="true"]').waitFor({state:'attached'});
    return {page, errors};
}
const item = (page, name) => page.getByRole('menuitem', {name,exact:true,includeHidden:true});
const trigger = (page, name) => name === 'Menubar' ? item(page,'Focus Menubar') : page.getByRole('button',{name:`Focus ${name}${name.includes('Split')?' pages':''}`,exact:true});
const menu = (page,name) => page.getByRole('menu',{includeHidden:true}).filter({has:item(page,`${name} Inbox`)});
async function focusedMenu(page, target) {
    await target.evaluate(el => el.dataset.focusProbe='true');
    await page.waitForFunction(() => document.activeElement?.dataset.focusProbe === 'true');
    await target.evaluate(el => delete el.dataset.focusProbe);
}
for (const mode of ['Blazor','Browser']) for (const touch of [false,true]) {
    test(`${mode}: ${touch?'iPhone touch':'mouse'} opens menu families without a spurious first-item highlight`, async () => {
        const {page,errors} = await open(mode,touch);
        try {
            for (const name of names) {
                const button=trigger(page,name), popup=menu(page,name);
                if (mode==='Browser') await popup.locator('xpath=self::*[@data-neo-browser-content]').waitFor({state:'attached'});
                await button[touch?'tap':'click'](); await popup.waitFor({state:'visible'});
                if (mode==='Blazor') await popup.locator('xpath=self::*[@data-menu-keyboard-ready="true"]').waitFor({state:'attached'});
                await focusedMenu(page,popup);
                assert.equal(await popup.locator('[data-focused="true"]').count(),0,name);
                assert.equal(await item(page,`${name} Campaigns`).evaluate(el=>el.classList.contains('bg-accent')),true,'Current page remains selected');
                const indicator=popup.locator('[data-selection-indicator]');
                await page.waitForFunction(el=>Boolean(el.style.transition),await indicator.elementHandle());
                assert.equal(await indicator.evaluate(el=>el.hasAttribute('data-si-hover')),false,name);
                assert.equal(await indicator.evaluate(el=>Number(getComputedStyle(el).opacity)),0,name);
                await page.keyboard.press('ArrowDown'); await focusedMenu(page,item(page,`${name} Inbox`));
                await page.keyboard.press('ArrowDown'); await focusedMenu(page,item(page,`${name} Campaigns`));
                await page.keyboard.press('Escape'); await popup.waitFor({state:'hidden'});
                await button[touch?'tap':'click'](); await popup.waitFor({state:'visible'});await focusedMenu(page,popup);
                assert.equal(await popup.locator('[data-focused="true"]').count(),0,'Reopen clears old keyboard highlights');
                await page.waitForFunction(el=>!el.hasAttribute('data-si-hover') && Number(getComputedStyle(el).opacity)===0,await popup.locator('[data-selection-indicator]').elementHandle());
                await page.keyboard.press('Escape'); await popup.waitFor({state:'hidden'});
            }
            assert.deepEqual(errors,[]);
        } finally { await page.close(); }
    });
}

test('Browser: keyboard and assistive opening still focus first/last enabled items across menu families', async () => {
    const {page,errors}=await open('Browser',false);
    try {
        for (const name of names) for (const key of ['Enter','Space','ArrowDown','ArrowUp','click']) {
            const button=trigger(page,name), popup=menu(page,name);
            await popup.locator('xpath=self::*[@data-neo-browser-content]').waitFor({state:'attached'});
            await button.focus();
            if(key==='click')await button.evaluate(el=>el.click());else await button.press(key);
            await popup.waitFor({state:'visible'});
            const last=['Dropdown','Pill'].includes(name)?'More':'Campaigns';
            await focusedMenu(page,item(page,`${name} ${key==='ArrowUp'?last:'Inbox'}`));
            await page.keyboard.press('Escape');await popup.waitFor({state:'hidden'});
        }
        assert.deepEqual(errors,[]);
    } finally {await page.close();}
});

test('Browser: non-looping pointer-opened menus accept ArrowUp from the container', async () => {
    const {page,errors}=await open('Browser',false);
    try {
        const button=trigger(page,'Nonloop'),popup=menu(page,'Nonloop');
        await popup.locator('xpath=self::*[@data-neo-browser-content]').waitFor({state:'attached'});
        await button.click();await popup.waitFor({state:'visible'});await focusedMenu(page,popup);
        await page.keyboard.press('ArrowUp');await focusedMenu(page,item(page,'Nonloop More'));
        await page.keyboard.press('ArrowUp');await focusedMenu(page,item(page,'Nonloop Campaigns'));
        assert.deepEqual(errors,[]);
    } finally {await page.close();}
});

test('Browser: pointer submenus focus their container; keyboard submenus focus the first item', async () => {
    const {page,errors}=await open('Browser',false);
    try {
        const popup=menu(page,'Dropdown');await popup.locator('xpath=self::*[@data-neo-browser-content]').waitFor({state:'attached'});
        await trigger(page,'Dropdown').click();await popup.waitFor({state:'visible'});
        const more=item(page,'Dropdown More');
        await more.locator('xpath=self::*[@aria-expanded="false"]').waitFor({state:'attached'});
        const nested=page.getByRole('menu',{includeHidden:true}).filter({has:item(page,'Dropdown Nested first')});
        await nested.locator('xpath=self::*[@data-neo-browser-content]').waitFor({state:'attached'});
        await popup.evaluate(el => Promise.all(el.getAnimations({subtree:true}).map(a=>a.finished.catch(()=>{}))));
        const box=await more.boundingBox();
        await page.mouse.move(box.x+box.width/2,box.y+box.height/2);
        await nested.waitFor({state:'visible'});await focusedMenu(page,nested);
        assert.equal(await nested.locator('[data-focused="true"]').count(),0);
        await page.keyboard.press('ArrowLeft');await nested.waitFor({state:'hidden'});await focusedMenu(page,more);
        await page.keyboard.press('ArrowRight');await nested.waitFor({state:'visible'});await focusedMenu(page,item(page,'Dropdown Nested first'));
        assert.deepEqual(errors,[]);
    } finally {await page.close();}
});
