import { test, before, after, beforeEach, afterEach } from 'node:test';
import assert from 'node:assert/strict';
import { readFile } from 'node:fs/promises';
import { chromium } from 'playwright';

const url = process.env.NEOUI_TEST_URL ?? 'http://localhost:5198';
const pause = ms => new Promise(r => setTimeout(r, ms));
async function eventually(check, timeout = process.env.NEOUI_TEST_RENDERER === 'wasm' ? 30000 : 10000) {
    const start = Date.now();
    while (true) {
        try { await check(); return; }
        catch (error) { if (Date.now() - start > timeout) throw error; await pause(30); }
    }
}
let browser, page, errors;
before(async () => { browser = await chromium.launch({ channel: process.env.NEOUI_BROWSER_CHANNEL ?? 'msedge', headless: true }); });
after(async () => { await browser?.close(); });
beforeEach(async () => {
    page = await browser.newPage();
    page.setDefaultTimeout(process.env.NEOUI_TEST_RENDERER === 'wasm' ? 30000 : 10000);
    page.setDefaultNavigationTimeout(30000);
    errors = [];
    page.on('pageerror', error => errors.push(String(error)));
    const core = await readFile(new URL('./node_modules/@floating-ui/core/dist/floating-ui.core.umd.js', import.meta.url), 'utf8');
    const dom = await readFile(new URL('./node_modules/@floating-ui/dom/dist/floating-ui.dom.umd.js', import.meta.url), 'utf8');
    await page.addInitScript(core + '\n' + dom);
    await page.addInitScript(() => {
        const NativeWebSocket = window.WebSocket;
        window.__testSockets = [];
        window.WebSocket = class extends NativeWebSocket {
            constructor(...args) { super(...args); window.__testSockets.push(this); }
        };
    });
    await page.goto(url + "/?adoption=true", { waitUntil: "domcontentloaded" });
    await page.waitForFunction(() => document.querySelectorAll('[data-neo-browser-content]').length >= 7);
    await pause(400);
});
afterEach(async () => { await page.close(); assert.deepEqual(errors, []); });
const filter = name => page.getByRole('menuitemcheckbox', { name, exact: true, includeHidden: true });
const radio = name => page.getByRole('menuitemradio', { name, exact: true, includeHidden: true });
const openFilters = async () => {
    await page.getByTestId('filter-menu').click();
    await filter('Unread only').waitFor({ state: 'visible' });
};
const checked = async (item, value) => eventually(async () => assert.equal(await item.getAttribute('aria-checked'), String(value)));
const accepted = () => page.getByTestId('accepted-filters').textContent();

test('policy precedence distinguishes omitted and explicit modes and crosses portals', async () => {
    const appMode = (await page.getByTestId('application-mode').textContent()).trim();
    if (process.env.NEOUI_EXPECT_DEFAULT) assert.equal(appMode, process.env.NEOUI_EXPECT_DEFAULT);
    const cases = [['app-default', appMode], ['app-select', appMode], ['app-menu', appMode],
        ['explicit-select-blazor', 'Blazor'], ['explicit-menu-blazor', 'Blazor'], ['explicit-blazor', 'Blazor'],
        ['scope-blazor', 'Blazor'], ['explicit-browser', 'Browser'], ['nested-browser', 'Browser']];
    // The roots all keep their content mounted. Inspect controller ownership directly,
    // isolating policy resolution from the legacy renderer's dismissal behavior.
    for (const [id, expected] of cases.filter(([, mode]) => mode === 'Browser')) {
        const trigger = page.getByTestId(id).locator('..');
        const content = page.locator(`[id="${await trigger.getAttribute('aria-controls')}"]`);
        await content.waitFor({ state: 'attached' });
        await eventually(async () => assert.notEqual(await content.getAttribute('data-neo-browser-content'), null, id));
    }
    for (const [id] of cases.filter(([, mode]) => mode === 'Blazor')) {
        const trigger = page.getByTestId(id).locator('..');
        const content = page.locator(`[id="${await trigger.getAttribute('aria-controls')}"]`);
        await content.waitFor({ state: 'attached' });
        assert.equal(await content.getAttribute('data-neo-browser-content'), null, id);
    }
    await page.getByTestId('nested-browser').locator('..').click();
    const nested = page.getByTestId('portal-scope').locator('..');
    await nested.click();
    const child = page.locator(`[id="${await nested.getAttribute('aria-controls')}"]`);
    await child.waitFor({ state: 'visible' });
    assert.notEqual(await child.getAttribute('data-neo-browser-content'), null);
});

test('checkbox feedback is immediate, stays open, and waits for acceptance', async () => {
    await page.getByTestId('menu-slow').check();
    await openFilters();
    const item = filter('Unread only');
    const local = await item.evaluate(e => {
        e.click();
        return { checked: e.getAttribute('aria-checked'), state: e.dataset.state,
            pending: e.hasAttribute('data-neo-pending'), indicator: e.querySelector('[data-neo-selection-indicator]').style.display,
            accepted: document.querySelector('[data-testid="accepted-filters"]').textContent,
            open: document.querySelector('[data-testid="filter-menu"]').getAttribute('aria-expanded') };
    });
    assert.deepEqual(local, { checked: 'true', state: 'checked', pending: true, indicator: '',
        accepted: 'false,false,all,false', open: 'true' });
    await eventually(async () => assert.equal(await accepted(), 'true,false,all,false'));
    await eventually(async () => assert.equal(await item.getAttribute('data-neo-pending'), null));
});

test('rejected checkbox and radio changes restore accepted values', async () => {
    await page.getByTestId('menu-reject').check();
    await page.getByTestId('menu-slow').check();
    await openFilters();
    assert.equal(await filter('Unread only').evaluate(e => { e.click(); return e.getAttribute('aria-checked'); }), 'true');
    const local = await radio('Important notifications').evaluate(e => {
        e.click();
        return { important: e.getAttribute('aria-checked'), all: [...e.parentElement.querySelectorAll('[role="menuitemradio"]')]
            .find(item => item.textContent.trim() === 'All notifications').getAttribute('aria-checked') };
    });
    assert.deepEqual(local, { important: 'true', all: 'false' });
    await checked(filter('Unread only'), false);
    await checked(radio('All notifications'), true);
    await checked(radio('Important notifications'), false);
    assert.equal(await accepted(), 'false,false,all,false');
});

test('rapid checkbox toggles and radio changes ignore stale acknowledgements', async () => {
    await page.getByTestId('menu-slow').check();
    await openFilters();
    const unread = filter('Unread only');
    const local = await unread.evaluate(e => {
        const radios = [...e.closest('[data-neo-browser-content]').querySelectorAll('[role="menuitemradio"]')];
        const important = radios.find(item => item.textContent.trim() === 'Important notifications');
        const all = radios.find(item => item.textContent.trim() === 'All notifications');
        window.__menuAckTrace = [];
        document.addEventListener('neoui:commit', event => window.__menuAckTrace.push({
            key: event.detail.key, checkbox: e.getAttribute('aria-checked'), all: all.getAttribute('aria-checked')
        }));
        e.click(); e.click(); e.click(); important.click(); all.click();
        return { checkboxKey: e.dataset.neoCheckKey, radioKey: all.dataset.neoCheckKey,
            checkbox: e.getAttribute('aria-checked'), all: all.getAttribute('aria-checked') };
    });
    assert.equal(local.checkbox, 'true');
    assert.equal(local.all, 'true');
    await eventually(async () => assert.equal(await unread.getAttribute('data-neo-pending'), null));
    await eventually(async () => assert.equal(await radio('All notifications').getAttribute('data-neo-pending'), null));
    const trace = await page.evaluate(() => window.__menuAckTrace);
    assert.deepEqual(trace.filter(event => event.key === local.checkboxKey).map(event => event.checkbox), ['true', 'true', 'true']);
    assert.deepEqual(trace.filter(event => event.key === local.radioKey).map(event => event.all), ['true', 'true']);
    assert.equal(await page.getByTestId('menu-commits').textContent(), '5');
    assert.equal(await accepted(), 'true,false,all,false');
});

test('controlled reset invalidates pending and queued menu acknowledgements', async () => {
    await page.getByTestId('menu-slow').check();
    await openFilters();
    await filter('Unread only').click(); await filter('Unread only').click();
    await radio('Important notifications').click();
    await page.getByTestId('menu-reset').evaluate(e => e.click());
    await checked(filter('Unread only'), false);
    await checked(radio('All notifications'), true);
    await pause(1700);
    assert.equal(await accepted(), 'false,false,all,false');
    assert.equal(await filter('Unread only').getAttribute('aria-checked'), 'false');
    assert.equal(await radio('All notifications').getAttribute('aria-checked'), 'true');
});

test('checkbox/radio keyboard, disabled items, dismissal and submenu commits', async () => {
    await openFilters();
    assert.equal(await filter('Unread only').evaluate(e => document.activeElement === e), true);
    await page.keyboard.press('Space');
    await checked(filter('Unread only'), true);
    await page.keyboard.press('ArrowDown'); await page.keyboard.press('Enter');
    await checked(filter('Mentions only'), true);
    await eventually(async () => assert.equal(await page.getByTestId('menu-commits').textContent(), '2'));
    const before = await page.getByTestId('menu-commits').textContent();
    await filter('Disabled filter').evaluate(e => e.click());
    await radio('Disabled notification').evaluate(e => e.click());
    await pause(150);
    assert.equal(await page.getByTestId('menu-commits').textContent(), before);
    await filter('Close after checking').click();
    assert.equal(await page.getByTestId('filter-menu').getAttribute('aria-expanded'), 'false');
    await openFilters();
    await page.keyboard.press('End'); await page.keyboard.press('ArrowRight');
    await filter('Assigned to me').waitFor({ state: 'visible' });
    await page.keyboard.press('Space');
    await checked(filter('Assigned to me'), true);
    await eventually(async () => assert.match(await accepted(), /,true$/));
    await page.keyboard.press('Escape'); await page.keyboard.press('Escape');
    await openFilters();
    await radio('No notifications').click();
    assert.equal(await page.getByTestId('filter-menu').getAttribute('aria-expanded'), 'false');
    await eventually(async () => assert.match(await accepted(), /,none,/));
});

test('composed controls inherit browser shells while preserving server content and explicit exceptions', async () => {
    for (const id of ['composed-combo', 'composed-multi', 'composed-date']) {
        const trigger = page.getByTestId(id).locator('[aria-haspopup]').first();
        await trigger.click();
        const content = page.locator('[data-neo-browser-content][data-state="open"]').filter({ visible: true });
        await content.waitFor({ state: 'visible' });
        if (id === 'composed-combo') {
            const input = page.getByPlaceholder('Search cities');
            await eventually(async () => assert.equal(await input.evaluate(e => document.activeElement === e), true));
            await input.fill('Bang');
            await page.getByRole('option', { name: 'Bangkok', exact: false }).click();
            await eventually(async () => assert.equal(await page.getByTestId('combo-value').textContent(), 'Bangkok'));
            await content.waitFor({ state: 'hidden' });
        } else if (id === 'composed-multi') {
            const input = page.getByPlaceholder('Search selected cities');
            await eventually(async () => assert.equal(await input.evaluate(e => document.activeElement === e), true));
            await input.fill('Cai');
            await page.getByRole('option', { name: 'Cairo', exact: false }).click();
            await eventually(async () => assert.equal(await page.getByTestId('multi-value').textContent(), 'Cairo'));
            await page.getByTestId('multi-close').evaluate(e => e.click());
            await content.waitFor({ state: 'hidden' });
            await trigger.click();
            await input.waitFor({ state: 'visible' });
            await eventually(async () => assert.equal(await input.inputValue(), ''));
            await page.getByTestId('multi-close').evaluate(e => e.click());
            await content.waitFor({ state: 'hidden' });
        } else {
            await content.getByRole('button', { name: /October 22, 2026/ }).click();
            await eventually(async () => assert.equal(await page.getByTestId('date-value').textContent(), '2026-10-22'));
            await content.waitFor({ state: 'hidden' });
        }
    }
    await page.getByTestId('composed-reset').click();
    await eventually(async () => assert.equal(await page.getByTestId('combo-value').textContent(), ''));
    assert.equal(await page.getByTestId('multi-value').textContent(), '');
    assert.equal(await page.getByTestId('date-value').textContent(), '');
    await page.getByTestId('composed-date').locator('[aria-haspopup]').first().click();
    await page.getByRole('button', { name: /October 22, 2026/ }).click();
    await eventually(async () => assert.equal(await page.getByTestId('date-value').textContent(), '2026-10-22'));
    await eventually(async () => assert.equal(await page.getByTestId('composed-date').locator('[aria-haspopup]').first().getAttribute('aria-expanded'), 'false'));
    for (const trigger of await page.getByTestId('composed-exceptions').locator('[aria-haspopup]').all()) {
        // These assertions verify the override; leave legacy dismissal behavior to its
        // own tests and avoid pointer overlap in the compact regression fixture.
        await trigger.evaluate(element => element.click());
        await eventually(async () => assert.equal(await trigger.getAttribute('aria-expanded'), 'true'));
        assert.equal(await page.locator('[data-neo-browser-content][data-state="open"]').count(), 0);
    }
});

test('a dropped circuit preserves local mechanics and reconnects without replaying commits', async t => {
    if (process.env.NEOUI_TEST_RENDERER === 'wasm') { t.skip('WebAssembly has no server circuit'); return; }
    await page.getByTestId('menu-slow').check();
    await openFilters();
    await filter('Unread only').click();
    await eventually(async () => assert.equal(await page.getByTestId('menu-commits').textContent(), '1'));
    await page.context().setOffline(true);
    await page.evaluate(() => window.__testSockets.filter(s => s.url.includes('_blazor')).forEach(s => s.close()));
    // Invoke DOM events directly while Blazor's reconnect overlay is present.
    await page.evaluate(() => {
        document.querySelector('[data-testid="filter-menu"]').click();
        document.querySelector('[data-testid="filter-menu"]').click();
    });
    assert.equal(await page.getByTestId('filter-menu').getAttribute('aria-expanded'), 'true');
    await page.context().setOffline(false);
    await eventually(async () => assert.equal(await page.evaluate(async () => {
        try { return await Blazor.reconnect(); } catch { return false; }
    }), true), 20000);
    await eventually(async () => assert.equal(await accepted(), 'true,false,all,false'));
    await eventually(async () => assert.equal(await filter('Unread only').getAttribute('data-neo-pending'), null));
    assert.equal(await page.getByTestId('menu-commits').textContent(), '1');
    await page.getByTestId('menu-reset').evaluate(e => e.click());
    await checked(filter('Unread only'), false);
    await eventually(async () => assert.equal(await page.getByTestId('filter-menu').getAttribute('aria-expanded'), 'false'));
    await openFilters();
    await filter('Mentions only').click();
    await eventually(async () => assert.equal(await accepted(), 'false,true,all,false'));
    assert.equal(await page.getByTestId('menu-commits').textContent(), '2');
});

test('persistent mounting exposes the initial DOM cost of large lists and repeated row menus', async () => {
    const results = {};
    let frameBytes = 0;
    page.on('websocket', socket => socket.on('framereceived', frame => frameBytes += Buffer.byteLength(frame.payload)));
    for (const mode of ['blazor', 'browser']) {
        const began = performance.now();
        frameBytes = 0;
        const response = await page.goto(url + '/?cost=' + mode);
        const bytes = (await response.body()).length;
        if (mode === 'browser') await page.waitForFunction(() => document.querySelectorAll('[data-neo-browser-content]').length === 26);
        else await pause(350);
        results[mode] = await page.evaluate(() => ({
            elements: document.querySelectorAll('*').length,
            options: document.querySelectorAll('[role="option"]').length,
            menuItems: document.querySelectorAll('[role="menuitem"]').length,
            controllers: document.querySelectorAll('[data-neo-browser-content]').length
        }));
        Object.assign(results[mode], { initialHtmlBytes: bytes, serverFrameBytes: frameBytes, setupMs: Math.round(performance.now() - began) });
    }
    assert.equal(results.blazor.options, 0);
    assert.equal(results.blazor.menuItems, 0);
    assert.equal(results.browser.options, 1000);
    assert.equal(results.browser.menuItems, 250);
    assert.ok(results.browser.elements > results.blazor.elements + 1250);
    if (process.env.NEOUI_TEST_RENDERER !== 'wasm') assert.ok(results.browser.serverFrameBytes > results.blazor.serverFrameBytes);
    console.log('Persistent mount cost (1000 options + 25 menus × 10 items): ' + JSON.stringify(results));
});

test('popover Tab navigation preserves native child focus until outside dismissal', async () => {
    const trigger = page.getByTestId('popover-trigger').locator('..');
    await trigger.click();
    const child = page.getByTestId('popover-child');
    await child.waitFor({ state: 'visible' });
    await eventually(async () => assert.equal(await child.evaluate(e => document.activeElement === e), true));
    await child.evaluate(e => {
        const input = document.createElement('input'); input.dataset.testid = 'native-popover-input';
        e.after(input);
    });
    await page.keyboard.press('Tab');
    assert.equal(await page.getByTestId('native-popover-input').evaluate(e => document.activeElement === e), true);
    assert.equal(await trigger.getAttribute('aria-expanded'), 'true');
    await page.getByTestId('outside').click();
    await eventually(async () => assert.equal(await trigger.getAttribute('aria-expanded'), 'false'));
});
