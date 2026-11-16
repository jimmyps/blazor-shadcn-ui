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
    // An intermediate Razor render can briefly restore accepted ARIA while the
    // browser queue is still pending. Wait for the reset command, not that echo.
    await eventually(async () => assert.equal(await page.getByTestId('filter-menu').getAttribute('aria-expanded'), 'false'));
    await checked(filter('Unread only'), false);
    await checked(radio('All notifications'), true);
    await pause(1700);
    assert.equal(await accepted(), 'false,false,all,false');
    assert.equal(await filter('Unread only').getAttribute('aria-checked'), 'false');
    assert.equal(await radio('All notifications').getAttribute('aria-checked'), 'true');
});

test('checkbox/radio keyboard, disabled items, dismissal and submenu commits', async () => {
    await openFilters();
    assert.equal(await filter('Unread only').evaluate(e => document.activeElement === e.closest('[role="menu"]')), true);
    await page.keyboard.press('ArrowDown');
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
    await page.getByRole('button', { name: /October 22, 2026/ }).click();
    await eventually(async () => assert.equal(await page.getByTestId('composed-exceptions').locator('[aria-haspopup]').last().getAttribute('aria-expanded'), 'false'));
});

for (let run = 1; run <= Number(process.env.NEOUI_RECONNECT_RUNS ?? 3); run++) {
    test(`a dropped circuit preserves local mechanics and reconnects without replaying commits (run ${run})`, async t => {
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
        // An acknowledgement may remain uncertain across disconnect. The authoritative
        // reset must reconcile both pending state and visibility without waiting for it.
        assert.equal(await page.getByTestId('menu-commits').textContent(), '1');
        await page.getByTestId('menu-reset').evaluate(e => e.click());
        await checked(filter('Unread only'), false);
        await eventually(async () => assert.equal(await filter('Unread only').getAttribute('data-neo-pending'), null));
        await eventually(async () => assert.equal(await page.getByTestId('filter-menu').getAttribute('aria-expanded'), 'false'));
        assert.equal(await page.getByTestId('menu-commits').textContent(), '1');
        await openFilters();
        await filter('Mentions only').click();
        await eventually(async () => assert.equal(await accepted(), 'false,true,all,false'));
        assert.equal(await page.getByTestId('menu-commits').textContent(), '2');
    });

}

test('a reset rendered during disconnection is reconciled when the retained circuit resumes', async t => {
    if (process.env.NEOUI_TEST_RENDERER === 'wasm') { t.skip('WebAssembly has no server circuit'); return; }
    await openFilters();
    await filter('Unread only').click();
    await eventually(async () => assert.equal(await accepted(), 'true,false,all,false'));
    await page.getByTestId('menu-reset-delayed').evaluate(e => e.click());
    // Ensure the server entered the delayed event before dropping the transport.
    await eventually(async () => assert.equal(await page.getByTestId('menu-reset-pending').textContent(), 'true'));
    await page.context().setOffline(true);
    await page.evaluate(() => window.__testSockets.filter(s => s.url.includes('_blazor')).forEach(s => s.close()));
    await pause(1400);
    assert.equal(await page.getByTestId('filter-menu').getAttribute('aria-expanded'), 'true');
    await page.context().setOffline(false);
    await eventually(async () => assert.equal(await page.evaluate(async () => {
        try { return await Blazor.reconnect(); } catch { return false; }
    }), true), 20000);
    await checked(filter('Unread only'), false);
    await eventually(async () => assert.equal(await page.getByTestId('filter-menu').getAttribute('aria-expanded'), 'false'));
    assert.equal(await page.getByTestId('menu-commits').textContent(), '1');
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


async function recordQuietTraffic() {
    const frames = [];
    let last = Date.now();
    page.on('websocket', socket => socket.on('framesent', frame => { frames.push(frame.payload); last = Date.now(); }));
    // Existing sockets were created before this helper; use CDP for all subsequent outgoing frames.
    const session = await page.context().newCDPSession(page);
    await session.send('Network.enable');
    session.on('Network.webSocketFrameSent', event => { frames.push(event.response.payloadData); last = Date.now(); });
    await eventually(async () => assert.ok(Date.now() - last > 500));
    frames.length = 0;
    return { frames, clear: () => frames.length = 0, dispose: () => session.detach() };
}

test('browser Command hover and keyboard focus send no circuit traffic and commits stay asynchronous', async () => {
    const root = page.getByTestId('local-command');
    await root.locator('[data-neo-client-ready="true"]').waitFor();
    const input = root.getByPlaceholder('Search local commands');
    await input.focus();
    const traffic = await recordQuietTraffic();
    try {
        const alpha = root.getByRole('option', { name: 'Alpha', exact: true });
        const beta = root.getByRole('option', { name: 'Beta', exact: true });
        await alpha.hover();
        assert.equal(await alpha.getAttribute('data-focused'), 'true');
        await page.keyboard.press('ArrowDown');
        assert.equal(await beta.getAttribute('data-focused'), 'true');
        assert.equal(await input.getAttribute('aria-activedescendant'), await beta.getAttribute('id'));
        await root.getByRole('option', { name: 'Disabled command', exact: true }).hover();
        assert.equal(await beta.getAttribute('data-focused'), 'true');
        await page.keyboard.press('Home');
        assert.equal(await alpha.getAttribute('data-focused'), 'true');
        // A pointerover generated by a scroll/layout change must not override keyboard focus.
        await beta.dispatchEvent('pointerover', { relatedTarget: null });
        assert.equal(await alpha.getAttribute('data-focused'), 'true');
        await beta.hover({ force: true });
        assert.equal(await beta.getAttribute('data-focused'), 'true');
        await root.getByRole('option', { name: 'Disabled command', exact: true }).evaluate(e => e.click());
        assert.equal(traffic.frames.length, 0);
        const beforeCommit = await input.evaluate(element => {
            element.dispatchEvent(new KeyboardEvent('keydown', { key: 'Enter', bubbles: true, cancelable: true }));
            return document.querySelector('[data-testid="local-command-commits"]').textContent;
        });
        assert.equal(beforeCommit, '0');
        await eventually(async () => assert.equal(await page.getByTestId('local-command-commits').textContent(), '1'));
    } finally { await traffic.dispose(); }
});

test('Combobox and MultiSelect hover/navigation remain local and Escape dismisses without a server response', async () => {
    for (const id of ['composed-combo', 'composed-multi']) {
        const trigger = page.getByTestId(id).locator('[aria-haspopup]').first();
        await trigger.click();
        const root = page.locator('[data-neo-browser-content][data-state="open"]').filter({ visible: true });
        await eventually(async () => assert.ok(await root.getAttribute('data-neo-client-ready') === 'true' ||
            await root.locator('[data-neo-client-ready="true"]').count() > 0));
        const input = root.locator('input').first();
        await eventually(async () => assert.equal(await input.evaluate(e => document.activeElement === e), true));
        const traffic = await recordQuietTraffic();
        try {
            const bangkok = root.getByRole('option', { name: 'Bangkok', exact: false });
            await bangkok.hover();
            assert.equal(await bangkok.getAttribute('data-focused'), 'true');
            await page.keyboard.press('End');
            const cairo = root.getByRole('option', { name: 'Cairo', exact: false });
            assert.equal(await cairo.getAttribute('data-focused'), 'true');
            await page.keyboard.press('Home');
            assert.equal(await root.getByRole('option').first().getAttribute('data-focused'), 'true');
            assert.equal(traffic.frames.length, 0);
            const composingActive = await input.getAttribute('aria-activedescendant');
            await input.evaluate(element => {
                for (const key of ['ArrowDown', 'Escape']) element.dispatchEvent(new KeyboardEvent('keydown', { key, isComposing: true, bubbles: true }));
            });
            assert.equal(await input.getAttribute('aria-activedescendant'), composingActive);
            assert.equal(await trigger.getAttribute('aria-expanded'), 'true');
            // Interactive footer content keeps its own keyboard behavior.
            const clientRoot = await root.getAttribute('data-neo-client-ready') === 'true' ? root : root.locator('[data-neo-client-ready="true"]').first();
            await clientRoot.evaluate(element => {
                window.__footerClicks = 0;
                const button = document.createElement('button'); button.textContent = 'Native footer action';
                button.dataset.testid = 'native-list-footer'; button.onclick = () => window.__footerClicks++;
                element.append(button);
            });
            const footer = page.getByTestId('native-list-footer');
            await footer.focus(); await page.keyboard.press('Enter');
            assert.equal(await page.evaluate(() => window.__footerClicks), 1);
            assert.equal(await trigger.getAttribute('aria-expanded'), 'true');
            assert.equal(traffic.frames.length, 0);
            await page.evaluate(() => { window.__savedUiSend = WebSocket.prototype.send; WebSocket.prototype.send = function () {}; });
            await page.keyboard.press('Escape');
            assert.equal(await trigger.getAttribute('aria-expanded'), 'false');
            await root.waitFor({ state: 'hidden' });
            await footer.evaluate(element => element.remove());
            await trigger.click();
            await root.waitFor({ state: 'visible' });
            assert.equal(await input.evaluate(e => document.activeElement === e), true);
            await page.keyboard.press('ArrowDown');
            assert.notEqual(await input.getAttribute('aria-activedescendant'), null);
            await page.keyboard.press('Escape');
            assert.equal(await trigger.getAttribute('aria-expanded'), 'false');
        } finally {
            await page.evaluate(() => { if (window.__savedUiSend) WebSocket.prototype.send = window.__savedUiSend; });
            await traffic.dispose();
        }
    }
});

test('Calendar focus/blur and navigation of mounted days remain browser-local', async () => {
    const trigger = page.getByTestId('composed-date').locator('[aria-haspopup]').first();
    await trigger.click();
    const calendar = page.locator('[data-neo-calendar-ready="true"]');
    await calendar.waitFor({ state: 'visible' });
    await eventually(async () => assert.equal(await calendar.evaluate(e => document.activeElement === e), true));
    const traffic = await recordQuietTraffic();
    try {
        await page.keyboard.press('Home');
        assert.match(await calendar.locator('[data-focused="true"]').getAttribute('data-neo-calendar-day'), /-01$/);
        await page.keyboard.press('ArrowRight');
        assert.match(await calendar.locator('[data-focused="true"]').getAttribute('data-neo-calendar-day'), /-02$/);
        await page.keyboard.press('ArrowDown');
        assert.match(await calendar.locator('[data-focused="true"]').getAttribute('data-neo-calendar-day'), /-09$/);
        await page.getByTestId('plain-button').focus();
        assert.equal(await calendar.locator('[data-focused="true"]').count(), 0);
        await page.getByTestId('plain-button').hover();
        await page.getByTestId('plain-link').hover();
        assert.equal(traffic.frames.length, 0);
        await calendar.focus();
        await page.keyboard.press('PageDown');
        await eventually(async () => assert.equal(await calendar.getAttribute('data-neo-calendar-month'), '2026-11'));
        await eventually(async () => assert.match(await calendar.locator('[data-focused="true"]').getAttribute('data-neo-calendar-day'), /^2026-11-/));
    } finally { await traffic.dispose(); }
});

test('virtualized Command hover is local and navigation materializes unmounted rows without focused-index callbacks', async () => {
    const root = page.getByTestId('local-virtual-command');
    await root.locator('[data-neo-client-ready="true"]').waitFor();
    const input = root.getByPlaceholder('Search virtual commands');
    await root.scrollIntoViewIfNeeded();
    await input.focus();
    await root.getByRole('option').first().waitFor();
    const traffic = await recordQuietTraffic();
    try {
        const first = root.getByRole('option').first();
        await first.hover();
        assert.equal(await first.getAttribute('data-focused'), 'true');
        assert.equal(traffic.frames.length, 0);
        await page.keyboard.press('End');
        await eventually(async () => assert.equal(await root.getByRole('option', { name: 'Virtual 099', exact: true }).getAttribute('data-focused'), 'true'));
        await page.keyboard.press('Home');
        await eventually(async () => assert.equal(await root.getByRole('option', { name: 'Virtual 000', exact: true }).getAttribute('data-focused'), 'true'));
    } finally { await traffic.dispose(); }
});


test('browser scroll thresholds call application loading only near the boundary and suppress duplicate requests', async () => {
    const result = await page.evaluate(async () => {
        const { observeLoadMore } = await import('/_content/NeoUI.Blazor/js/browser-list.js');
        const viewport = document.createElement('div');
        viewport.style.cssText = 'height:100px;overflow:auto';
        viewport.dataset.neoLoadEnabled = 'true';
        const body = document.createElement('div'); body.style.height = '1000px'; viewport.append(body); document.body.append(viewport);
        let calls = 0;
        const controller = observeLoadMore(viewport, { invokeMethodAsync: async () => { calls++; } });
        const scroll = async top => { viewport.scrollTop = top; viewport.dispatchEvent(new Event('scroll')); await new Promise(resolve => setTimeout(resolve, 30)); };
        await scroll(20); const beforeBoundary = calls;
        await scroll(1000); await scroll(1000); const firstPage = calls;
        body.style.height = '1500px'; await scroll(1500); const nextPage = calls;
        viewport.dataset.neoLoadEnabled = 'false'; body.style.height = '2000px'; await scroll(2000); const disabled = calls;
        controller.dispose(); viewport.remove();
        return { beforeBoundary, firstPage, nextPage, disabled };
    });
    assert.deepEqual(result, { beforeBoundary: 0, firstPage: 1, nextPage: 2, disabled: 2 });
});


test('date selection and time Apply/Cancel dismiss locally before delayed value callbacks', async () => {
    await page.getByTestId('menu-slow').check();
    const dateTrigger = page.getByTestId('composed-date').locator('[aria-haspopup]').first();
    await dateTrigger.click();
    const calendar = page.locator('[data-neo-calendar-ready="true"]');
    await calendar.waitFor({ state: 'visible' });
    const localDate = await calendar.getByRole('button', { name: /October 22, 2026/ }).evaluate(element => {
        element.click();
        return { open: document.querySelector('[data-testid="composed-date"] [aria-haspopup]').getAttribute('aria-expanded'),
            accepted: document.querySelector('[data-testid="date-value"]').textContent };
    });
    assert.deepEqual(localDate, { open: 'false', accepted: '2026-10-21' });
    await eventually(async () => assert.equal(await page.getByTestId('date-value').textContent(), '2026-10-22'));
    const timeTrigger = page.getByTestId('composed-time').locator('[aria-haspopup]').first();
    await timeTrigger.click();
    const content = page.locator(`[id="${await timeTrigger.getAttribute('aria-controls')}"]`);
    await content.waitFor({ state: 'visible' });
    await content.locator('select').nth(0).selectOption('9');
    await content.locator('select').nth(1).selectOption('30');
    const localTime = await content.getByRole('button', { name: 'Apply', exact: true }).evaluate(element => {
        element.click();
        return { open: document.querySelector('[data-testid="composed-time"] [aria-haspopup]').getAttribute('aria-expanded'),
            accepted: document.querySelector('[data-testid="time-value"]').textContent };
    });
    assert.deepEqual(localTime, { open: 'false', accepted: 'None' });
    await eventually(async () => assert.equal(await page.getByTestId('time-value').textContent(), '09:30'));
    await timeTrigger.click();
    await content.waitFor({ state: 'visible' });
    await page.evaluate(() => { window.__savedUiSend = WebSocket.prototype.send; WebSocket.prototype.send = function () {}; });
    try {
        await content.getByRole('button', { name: 'Cancel', exact: true }).evaluate(element => element.click());
        assert.equal(await timeTrigger.getAttribute('aria-expanded'), 'false');
        assert.equal(await page.getByTestId('time-value').textContent(), '09:30');
    } finally { await page.evaluate(() => { if (window.__savedUiSend) WebSocket.prototype.send = window.__savedUiSend; }); }
});
