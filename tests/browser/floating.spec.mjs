import { test, before, after, beforeEach, afterEach } from 'node:test';
import assert from 'node:assert/strict';
import { readFile } from 'node:fs/promises';
import { chromium } from 'playwright';

const url = process.env.NEOUI_TEST_URL ?? 'http://localhost:5198';
let browser, page, sent, errors, lastFrameAt;
const sleep = ms => new Promise(resolve => setTimeout(resolve, ms));
async function eventually(check, timeout = 5000) {
    const started = Date.now();
    while (true) {
        try { await check(); return; }
        catch (error) { if (Date.now() - started > timeout) throw error; await sleep(30); }
    }
}
const popup = async trigger => page.locator(`[id="${await trigger.getAttribute('aria-controls')}"]`);
const selectedText = () => page.getByTestId('select-trigger').locator('[data-neo-value-display]').textContent();
before(async () => {
    browser = await chromium.launch({ headless: true, channel: process.env.NEOUI_BROWSER_CHANNEL ?? 'msedge' });
});
after(async () => { await browser?.close(); });
beforeEach(async () => {
    page = await browser.newPage();
    sent = []; errors = []; lastFrameAt = Date.now();
    page.setDefaultTimeout(10000);
    page.on('pageerror', error => errors.push(String(error)));
    page.on('console', message => { if (message.type() === 'error') errors.push(message.text()); });
    page.on('websocket', socket => socket.on('framesent', frame => { sent.push(frame.payload); lastFrameAt = Date.now(); }));
    // Preload the same positioning engine locally; regression tests don't depend on a CDN.
    const core = await readFile(new URL('./node_modules/@floating-ui/core/dist/floating-ui.core.umd.js', import.meta.url), 'utf8');
    const dom = await readFile(new URL('./node_modules/@floating-ui/dom/dist/floating-ui.dom.umd.js', import.meta.url), 'utf8');
    await page.addInitScript(core + '\n' + dom);
    await page.goto(url);
    await page.waitForFunction(() => document.querySelectorAll('[data-neo-browser-content]').length >= 7);
    await eventually(async () => assert.equal(await selectedText(), 'Apple'));
    // Measure interaction traffic after setup interop has settled, including on a busy host.
    await eventually(async () => assert.ok(Date.now() - lastFrameAt >= 500), 15000);
    sent.length = 0;
});
afterEach(async () => {
    await page.close();
    assert.deepEqual(errors, []);
});

test('menu click, keyboard navigation and dismissal send no circuit messages', async () => {
    const trigger = page.getByTestId('menu-trigger');
    const menu = await popup(trigger);
    await trigger.click();
    await eventually(async () => assert.equal(await menu.getAttribute('data-state'), 'open'));
    await page.keyboard.press('ArrowDown');
    await eventually(async () => assert.equal(await page.locator(':focus').textContent(), 'Native link'));
    await page.keyboard.press('Escape');
    assert.equal(await trigger.getAttribute('aria-expanded'), 'false');
    assert.equal(await trigger.evaluate(e => document.activeElement === e), true);
    assert.equal(sent.length, 0);
    await trigger.click();
    await page.getByTestId('outside').click();
    assert.equal(await menu.getAttribute('data-state'), 'closed');
    assert.equal(sent.length, 0);
});

test('submenus open and dismiss with keyboard without a server render', async () => {
    const trigger = page.getByTestId('menu-trigger');
    await trigger.click();
    await page.keyboard.press('End');
    assert.equal((await page.locator(':focus').textContent()).trim(), 'More');
    await page.keyboard.press('ArrowRight');
    const nested = page.getByRole('menuitem', { name: 'Nested action', exact: true });
    await nested.waitFor({ state: 'visible' });
    assert.equal(await nested.evaluate(e => document.activeElement === e), true);
    await page.keyboard.press('ArrowLeft');
    assert.equal((await page.locator(':focus').textContent()).trim(), 'More');
    await page.keyboard.press('Escape');
    assert.equal(await trigger.getAttribute('aria-expanded'), 'false');
    assert.equal(sent.length, 0);
});

test('menu dismisses immediately before its delayed application action', async () => {
    const trigger = page.getByTestId('menu-trigger');
    await trigger.click();
    await page.getByRole('menuitem', { name: 'Run action', exact: true }).click();
    assert.equal(await trigger.getAttribute('aria-expanded'), 'false');
    assert.equal(await page.getByTestId('actions').textContent(), '0');
    await eventually(async () => assert.equal(await page.getByTestId('actions').textContent(), '1'));
});

test('Select opens, moves focus, typeaheads and dismisses locally', async () => {
    const trigger = page.getByTestId('select-trigger');
    const list = await popup(trigger);
    await trigger.click();
    await list.waitFor({ state: 'visible' });
    await page.keyboard.press('c');
    const active = await list.getAttribute('aria-activedescendant');
    assert.equal(await page.locator(`[id="${active}"]`).getAttribute('data-neo-text'), 'Cherry');
    await page.keyboard.press('ArrowUp');
    const previous = await list.getAttribute('aria-activedescendant');
    assert.equal(await page.locator(`[id="${previous}"]`).getAttribute('data-neo-text'), 'Banana');
    await page.keyboard.press('Escape');
    assert.equal(await selectedText(), 'Apple');
    assert.equal(sent.length, 0);
});

test('selection displays locally while delayed commits are serialized; old responses cannot revert it', async () => {
    await page.getByTestId('slow').check();
    const trigger = page.getByTestId('select-trigger');
    await trigger.click();
    await page.getByRole('option', { name: 'Banana', exact: true }).click();
    assert.equal(await selectedText(), 'Banana');
    assert.equal(await page.getByTestId('server-value').textContent(), 'Apple');
    await trigger.click();
    await page.getByRole('option', { name: 'Cherry', exact: true }).click();
    assert.equal(await selectedText(), 'Cherry');
    await sleep(1100);
    assert.equal(await selectedText(), 'Cherry');
    await eventually(async () => assert.equal(await page.getByTestId('server-value').textContent(), 'Cherry'));
    assert.equal(await selectedText(), 'Cherry');
});

test('controlled Select can reject a proposal and explicitly reset to the same value', async () => {
    await page.getByTestId('reject').check();
    const trigger = page.getByTestId('select-trigger');
    await trigger.click();
    await page.getByRole('option', { name: 'Banana', exact: true }).click();
    await eventually(async () => assert.equal(await selectedText(), 'Apple'));
    await page.getByTestId('reset').click();
    assert.equal(await selectedText(), 'Apple');
});

test('parent rerender preserves an open menu and current selection; item updates remain declarative', async () => {
    const trigger = page.getByTestId('select-trigger');
    const list = await popup(trigger);
    await trigger.click();
    await page.getByTestId('render').evaluate(e => e.click());
    await eventually(async () => assert.match(await page.getByTestId('render').textContent(), /\(1\)/));
    assert.equal(await trigger.getAttribute('aria-expanded'), 'true');
    assert.equal(await list.getAttribute('data-state'), 'open');
    await page.getByTestId('add').evaluate(e => e.click());
    await page.getByRole('option', { name: 'Grape', exact: true }).waitFor({ state: 'visible' });
    await page.getByRole('option', { name: 'Grape', exact: true }).click();
    await eventually(async () => assert.equal(await page.getByTestId('server-value').textContent(), 'Grape'));
    await page.getByTestId('remove').click();
    await eventually(async () => assert.equal(await list.locator('[role=option]').count(), 4));
    assert.equal(await selectedText(), 'Grape');
    await page.getByTestId('disable').click();
    await eventually(async () => assert.equal(await trigger.isDisabled(), true));
});

test('Popover focus and dismissal are local; legacy mode remains interactive', async () => {
    const trigger = page.getByTestId('popover-trigger');
    await trigger.click();
    await page.getByTestId('popover-child').waitFor({ state: 'visible' });
    assert.equal(await page.getByTestId('popover-child').evaluate(e => document.activeElement === e), true);
    await page.keyboard.press('Escape');
    assert.equal(sent.length, 0);
    await page.getByTestId('legacy-trigger').click();
    await page.getByRole('menuitem', { name: 'Legacy item', exact: true }).waitFor({ state: 'visible' });
    if (process.env.NEOUI_TEST_RENDERER !== 'wasm') assert.ok(sent.length > 0);
});

test('browser mechanics remain usable when outbound circuit traffic is blocked', async () => {
    // Drop all messages on the established socket; the page and browser code remain loaded.
    await page.evaluate(() => { WebSocket.prototype.send = function () {}; });
    const trigger = page.getByTestId('select-trigger');
    await trigger.click();
    const list = await popup(trigger);
    await list.waitFor({ state: 'visible' });
    await page.keyboard.press('End');
    await page.keyboard.press('Enter');
    assert.equal(await selectedText(), 'Cherry');
    assert.equal(await trigger.getAttribute('aria-expanded'), 'false');
    await page.getByTestId('menu-trigger').click();
    await page.keyboard.press('Escape');
    assert.equal(await page.getByTestId('menu-trigger').getAttribute('aria-expanded'), 'false');
});

test('observed open bindings do not replay old state and explicit commands still work', async () => {
    const trigger = page.getByTestId('command-trigger').locator('..');
    const menu = await popup(trigger);
    await trigger.click();
    await menu.waitFor({ state: 'visible' });
    await page.keyboard.press('Escape');
    assert.equal(await trigger.getAttribute('aria-expanded'), 'false');
    await sleep(600);
    assert.equal(await trigger.getAttribute('aria-expanded'), 'false');
    assert.equal(await page.getByTestId('command-open').textContent(), 'false');
    await page.getByTestId('open-command').evaluate(e => e.click());
    await eventually(async () => assert.equal(await trigger.getAttribute('aria-expanded'), 'true'));
    await page.getByTestId('close-command').evaluate(e => e.click());
    await eventually(async () => assert.equal(await trigger.getAttribute('aria-expanded'), 'false'));
});

test('late positioning cannot reopen a closed layer; rerendered styles and DOM removal are reconciled', async () => {
    const result = await page.evaluate(async () => {
        const { createController } = await import('/_content/NeoUI.Blazor.Primitives/js/primitives/browser-floating.js');
        const host = document.createElement('div');
        host.innerHTML = '<button id="race-trigger">Race</button><div id="race-floating"><div id="race-content" role="menu" tabindex="-1"><div role="menuitem" tabindex="-1">Item</div></div></div>';
        document.body.append(host);
        const floating = host.querySelector('#race-floating');
        const options = { triggerId: 'race-trigger', contentId: 'race-content', kind: 'menu', open: false,
            openVersion: 1, loop: true, closeOnEscape: true, closeOnClickOutside: true };
        let release;
        let cleaned = 0;
        let calls = 0;
        const adapter = {
            computePosition: async () => { if (calls++ === 0) await new Promise(resolve => release = resolve); return { x: 1, y: 2 }; },
            applyPosition: (element, result) => { element.style.left = result.x + 'px'; },
            autoUpdate: async () => ({ apply: () => cleaned++ })
        };
        const controller = createController(floating, options, { placement: 'bottom' }, null, adapter);
        const pending = controller.open();
        controller.close();
        release();
        await pending;
        const stayedClosed = floating.style.visibility === 'hidden';
        await controller.open();
        floating.style.cssText = 'visibility: hidden; opacity: 0';
        controller.update(options, { placement: 'bottom' });
        const restored = floating.style.visibility === 'visible';
        controller.update(options, { placement: 'top' });
        await new Promise(resolve => setTimeout(resolve, 30));
        const repositioned = calls >= 3;
        host.remove();
        await new Promise(resolve => setTimeout(resolve, 0));
        await controller.open();
        return { stayedClosed, restored, repositioned, disposed: !controller.isOpen, cleaned };
    });
    assert.equal(result.stayedClosed, true);
    assert.equal(result.restored, true);
    assert.equal(result.repositioned, true);
    assert.equal(result.disposed, true);
    assert.ok(result.cleaned >= 2);
});

test('native and AsChild disabled menu triggers cannot open', async () => {
    for (const trigger of [page.getByTestId('disabled-native').locator('..'), page.getByTestId('disabled-child')]) {
        assert.equal(await trigger.isDisabled(), true);
        await trigger.evaluate(e => e.click());
        assert.equal(await trigger.getAttribute('aria-expanded'), 'false');
    }
    assert.equal(sent.length, 0);
});
