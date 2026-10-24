import { test } from 'node:test';
import assert from 'node:assert/strict';
import { chromium, webkit, devices } from 'playwright';
import { join } from 'node:path';
import { tmpdir } from 'node:os';
import { readFile } from 'node:fs/promises';

const url = process.env.NEOUI_DEMO_URL ?? 'http://localhost:5184/components/browser-interactions';
const pause = ms => new Promise(resolve => setTimeout(resolve, ms));
async function eventually(check) {
    const began = Date.now();
    while (true) {
        try { await check(); return; }
        catch (error) { if (Date.now() - began > 10000) throw error; await pause(40); }
    }
}

test('browser interaction demo works on desktop and mobile', async () => {
    const browser = await chromium.launch({ headless: true, channel: process.env.NEOUI_BROWSER_CHANNEL ?? 'msedge' });
    const page = await browser.newPage({ viewport: { width: 1440, height: 1100 } });
    const origin = new URL(url).origin;
    const imagePrefix = origin.includes('.devtunnels.ms') ? 'neoui-browser-demo-tunnel' : 'neoui-browser-demo';
    if (origin.includes('.devtunnels.ms')) {
        // Scope the tunnel bypass header to this origin; third-party font/CDN requests
        // must not receive it, since that would introduce an unsupported CORS preflight.
        await page.route(origin + '/**', route => route.continue({ headers: {
            ...route.request().headers(), 'X-Tunnel-Skip-AntiPhishing-Page': 'true'
        } }));
    }
    const errors = [], sent = [];
    page.on('pageerror', error => errors.push(String(error)));
    page.on('console', message => { if (message.type() === 'error') errors.push(message.text()); });
    page.on('websocket', socket => socket.on('framesent', frame => sent.push(frame.payload)));
    try {
        await page.goto(url, { waitUntil: 'domcontentloaded' });
        await page.waitForFunction(() => window.FloatingUIDOM &&
            document.querySelectorAll('[data-neo-browser-content]').length >= 4 &&
            document.querySelector('[data-local-events]')?.textContent === '0');
        await pause(600);
        await page.getByRole('heading', { name: 'Interaction Mode', exact: true }).waitFor();
        await page.getByRole('link', { name: 'Interaction Mode', exact: true }).waitFor();
        await page.screenshot({ path: join(tmpdir(), imagePrefix + '-desktop.png'), fullPage: true });
        sent.length = 0;
        await page.getByTestId('browser-menu').click();
        await page.getByRole('menuitem', { name: 'Save draft', exact: false }).waitFor({ state: 'visible' });
        await page.keyboard.press('End');
        await page.keyboard.press('ArrowRight');
        await page.getByRole('menuitem', { name: 'Product', exact: true }).waitFor({ state: 'visible' });
        await page.keyboard.press('Escape');
        await page.keyboard.press('Escape');
        assert.equal(await page.getByTestId('callback-count').textContent(), '0');
        // The gallery can finish unrelated JS initialization while the menu opens.
        // No input event or browser component callback may invoke .NET.
        assert.equal(sent.filter(frame => Buffer.from(frame).includes(Buffer.from('BeginInvokeDotNetFromJS'))).length, 0);
        assert.ok(Number(await page.locator('[data-local-events]').textContent()) >= 4);

        const select = page.getByTestId('workspace-select');
        await select.click();
        await page.getByRole('option', { name: 'Product', exact: true }).click();
        assert.equal(await select.locator('[data-neo-value-display]').textContent(), 'Product');
        assert.equal(await page.getByTestId('accepted-workspace').textContent(), 'Design');
        await eventually(async () => assert.equal(await page.getByTestId('accepted-workspace').textContent(), 'Product'));
        await page.getByTestId('reject-selection').check();
        await select.click();
        await page.getByRole('option', { name: 'Engineering', exact: true }).click();
        assert.equal(await select.locator('[data-neo-value-display]').textContent(), 'Engineering');
        await eventually(async () => assert.equal(await select.locator('[data-neo-value-display]').textContent(), 'Product'));

        await select.click();
        await page.getByTestId('render-parent').evaluate(element => element.click());
        await eventually(async () => assert.match(await page.getByTestId('render-parent').textContent(), /\(1\)/));
        assert.equal(await select.getAttribute('aria-expanded'), 'true');
        await page.keyboard.press('Escape');
        await page.getByTestId('add-workspace').click();
        await select.click();
        await page.getByRole('option', { name: 'Research', exact: true }).waitFor({ state: 'visible' });
        await page.keyboard.press('Escape');

        await page.getByTestId('browser-popover').click();
        await page.getByLabel('Show workspace avatars').waitFor({ state: 'visible' });
        await page.getByLabel('Show workspace avatars').uncheck();
        await page.keyboard.press('Escape');
        assert.equal(await page.getByTestId('callback-count').textContent(), '2');
        assert.ok((await page.locator('[data-event-log] li').count()) > 0);
        await page.getByTestId('standard-menu').click();
        const standardItem = page.getByRole('menuitem', { name: 'Browse workspace', exact: true });
        await standardItem.waitFor({ state: 'visible' });
        await page.getByRole('menuitem', { name: 'Save draft', exact: true }).click();
        await eventually(async () => assert.equal(await page.getByTestId('standard-action').textContent(), 'Draft saved through the standard path.'));
        await standardItem.waitFor({ state: 'hidden' });

        await page.locator('#selection-lab').getByRole('tab', { name: 'Code', exact: true }).click();
        await page.locator('#selection-lab pre').first().waitFor({ state: 'visible' });
        assert.match(await page.locator('#selection-lab pre').first().textContent(), /InteractionMode.Browser/);
        await page.locator('#selection-lab').getByRole('tab', { name: 'Preview', exact: true }).click();
        await select.waitFor({ state: 'visible' });
        await page.setViewportSize({ width: 390, height: 844 });
        await page.evaluate(() => { window.scrollTo(0, 0); document.querySelectorAll('*').forEach(e => { if (e.scrollTop > 0) e.scrollTop = 0; }); });
        assert.equal(await page.evaluate(() => document.documentElement.scrollWidth <= innerWidth), true);
        await page.screenshot({ path: join(tmpdir(), imagePrefix + '-mobile.png'), fullPage: true });
        assert.deepEqual(errors, []);
        console.log(`Verified ${url}`);
    } finally { await browser.close(); }
});

for (const preference of ['system-dark', 'saved-dark']) {
    test(`gallery theme applies to demo controls and portals with ${preference}`, async () => {
        const browser = await chromium.launch({ headless: true, channel: process.env.NEOUI_BROWSER_CHANNEL ?? 'msedge' });
        const page = await browser.newPage({ colorScheme: preference === 'system-dark' ? 'dark' : 'light' });
        const origin = new URL(url).origin;
        if (origin.includes('.devtunnels.ms')) {
            await page.route(origin + '/**', route => route.continue({ headers: {
                ...route.request().headers(), 'X-Tunnel-Skip-AntiPhishing-Page': 'true'
            } }));
        }
        if (preference === 'saved-dark') {
            await page.addInitScript(() => localStorage.setItem('theme', 'dark'));
        }
        try {
            await page.goto(url, { waitUntil: 'domcontentloaded' });
            await page.waitForFunction(() => document.querySelector('[data-local-events]')?.textContent === '0');
            assert.equal(await page.locator('html').evaluate(e => e.classList.contains('dark')), true);
            for (const id of ['standard-menu', 'workspace-select', 'reset-selection', 'add-workspace', 'render-parent']) {
                const colors = await page.getByTestId(id).evaluate(e => {
                    const style = getComputedStyle(e);
                    const probe = document.createElement('span');
                    probe.style.cssText = 'background:var(--background);color:var(--foreground)';
                    e.append(probe);
                    const expected = getComputedStyle(probe);
                    const result = { background: style.backgroundColor, color: style.color,
                        expectedBackground: expected.backgroundColor, expectedColor: expected.color };
                    probe.remove();
                    return result;
                });
                assert.equal(colors.background, colors.expectedBackground, id + ' background');
                assert.equal(colors.color, colors.expectedColor, id + ' foreground');
                assert.notEqual(colors.background, colors.color, id + ' contrast');
            }
            await page.getByTestId('workspace-select').click();
            const option = page.getByRole('option', { name: 'Product', exact: true });
            await option.waitFor({ state: 'visible' });
            assert.equal(await option.evaluate(e => {
                const style = getComputedStyle(e);
                return style.color !== getComputedStyle(e.closest('[data-neo-browser-content]')).backgroundColor;
            }), true);

            await page.screenshot({ path: join(tmpdir(), `neoui-demo-${preference}.png`), fullPage: true });
            await page.keyboard.press('Escape');
            await page.getByTestId('browser-popover').click();
            await page.getByLabel('Show workspace avatars').waitFor({ state: 'visible' });
            await page.keyboard.press('Escape');
            const themeToggle = page.getByRole('switch', { name: 'Toggle dark mode', exact: true });
            // The initial dark class comes from the head script; await the hydrated
            // server control before exercising its asynchronous persistence callback.
            await eventually(async () => assert.equal(await themeToggle.getAttribute('aria-checked'), 'true'));
            await themeToggle.click();
            await eventually(async () => assert.equal(await page.locator('html').evaluate(e => e.classList.contains('dark')), false));
            await eventually(async () => assert.equal(await page.evaluate(() => localStorage.getItem('theme')), 'light'));
        } finally { await browser.close(); }
    });
}

test('Interaction Mode is discoverable in navigation, indexes, and spotlight', async () => {
    const browser = await chromium.launch({ headless: true, channel: process.env.NEOUI_BROWSER_CHANNEL ?? 'msedge' });
    const page = await browser.newPage({ viewport: { width: 1440, height: 1000 } });
    const origin = new URL(url).origin;
    if (origin.includes('.devtunnels.ms')) {
        await page.route(origin + '/**', route => route.continue({ headers: {
            ...route.request().headers(), 'X-Tunnel-Skip-AntiPhishing-Page': 'true'
        } }));
    }
    try {
        await page.goto(origin + '/');
        await page.getByRole('link', { name: /Interaction Mode Compare/ }).waitFor();
        // The landing-page card can prerender before the interactive sidebar is visible.
        await eventually(async () => {
            const links = await page.locator('aside a').evaluateAll(elements => elements
                .filter(e => e.getClientRects().length > 0).map(e => e.textContent.trim()));
            assert.notEqual(links.indexOf('Components'), -1);
            assert.equal(links[links.indexOf('Components') + 1], 'Interaction Mode');
        });
        await page.goto(origin + '/components');
        await page.getByRole('heading', { name: 'Interaction Mode', exact: true }).first().waitFor();
        const indexLink = page.locator('a[href="/components/browser-interactions"]').filter({ has: page.locator('h4') });
        await indexLink.click();
        // Enhanced navigation can mount every retained gallery portal before its observer initializes.
        // Keep this startup allowance separate from search/interaction assertion deadlines.
        await page.waitForFunction(() => document.querySelector('[data-local-events]')?.textContent === '0', null, {timeout:60000});
        await page.getByRole('button', { name: /Search components/ }).first().click();
        const search = page.getByPlaceholder('Type a command or search...');
        await search.waitFor({ state: 'visible' });
        const session = await page.context().newCDPSession(page);
        await eventually(async () => {
            const inputId = await search.getAttribute('id');
            const { result } = await session.send('Runtime.evaluate', {
                expression: 'document.getElementById(' + JSON.stringify(inputId) + ')'
            });
            const { listeners } = await session.send('DOMDebugger.getEventListeners', { objectId: result.objectId });
            assert.ok(listeners.some(listener => listener.type === 'input'));
        });
        await session.detach();
        for (const query of ['Interaction Mode', 'browser-local', 'MultiSelect', 'date/time pickers', 'BrowserEditForm', 'CommandInput']) {
            // Input installs its debounced listener asynchronously after the dialog mounts.
            // Discovery tests wait for that listener rather than testing first-keystroke hydration.
            await search.fill(query);
            await eventually(async () => {
                assert.equal(await search.inputValue(), query);
                assert.equal(await page.getByRole('option').filter({ hasText: 'Interaction Mode' }).isVisible(), true);
            });
            await pause(400);
            assert.equal(await search.inputValue(), query);
        }
        await page.getByRole('option').filter({ hasText: 'Interaction Mode' }).click();
        await page.getByRole('heading', { name: 'Interaction Mode', exact: true }).waitFor();
        await search.waitFor({ state: 'hidden' });
    } finally { await browser.close(); }
});

test('filter menus reconcile async choices and a browser theme picker retains live Blazor actions', async () => {
    const browser = await chromium.launch({ headless: true, channel: process.env.NEOUI_BROWSER_CHANNEL ?? 'msedge' });
    const page = await browser.newPage({ viewport: { width: 1440, height: 1100 } });
    const origin = new URL(url).origin;
    if (origin.includes('.devtunnels.ms')) {
        await page.route(origin + '/**', route => route.continue({ headers: {
            ...route.request().headers(), 'X-Tunnel-Skip-AntiPhishing-Page': 'true'
        } }));
    }
    try {
        await page.goto(url, { waitUntil: 'domcontentloaded' });
        await page.waitForFunction(() => document.querySelector('[data-local-events]')?.textContent === '0');
        await page.getByTestId('demo-filters').click();
        const unread = page.getByRole('menuitemcheckbox', { name: 'Unread only', exact: true });
        await unread.click();
        assert.equal(await unread.getAttribute('aria-checked'), 'true');
        assert.match(await page.getByTestId('demo-accepted-filters').textContent(), /unread False/);
        await eventually(async () => assert.match(await page.getByTestId('demo-accepted-filters').textContent(), /unread True/));
        assert.equal(await page.getByTestId('demo-filters').getAttribute('aria-expanded'), 'true');
        await page.keyboard.press('Escape');
        const picker = page.getByTestId('browser-theme-picker').getByRole('button').first();
        await picker.click();
        const popup = page.locator(`[id="${await picker.getAttribute('aria-controls')}"]`);
        await popup.waitFor({ state: 'visible' });
        assert.notEqual(await popup.getAttribute('data-neo-browser-content'), null);
        await popup.getByRole('button', { name: 'Slate', exact: true }).click();
        await eventually(async () => assert.equal(await page.locator('html').evaluate(e => e.classList.contains('base-slate')), true));
        assert.equal(await popup.isVisible(), true);
    } finally { await browser.close(); }
});


test('composed demo controls inherit browser shells and retain date, time, search, selection and reset behavior', async () => {
    const browser = await chromium.launch({ headless: true, channel: process.env.NEOUI_BROWSER_CHANNEL ?? 'msedge' });
    const page = await browser.newPage({ viewport: { width: 1440, height: 1100 } });
    const origin = new URL(url).origin;
    if (origin.includes('.devtunnels.ms')) {
        await page.route(origin + '/**', route => route.continue({ headers: {
            ...route.request().headers(), 'X-Tunnel-Skip-AntiPhishing-Page': 'true'
        } }));
    }
    const errors = [];
    page.on('pageerror', error => errors.push(String(error)));
    const trigger = id => page.getByTestId(id).locator('[aria-haspopup]').first();
    const popup = async button => page.locator(`[id="${await button.getAttribute('aria-controls')}"]`);
    try {
        await page.goto(url, { waitUntil: 'domcontentloaded' });
        await page.waitForFunction(() => document.querySelector('[data-local-events]')?.textContent === '0');
        for (const id of ['demo-date-picker', 'demo-time-picker', 'demo-combobox', 'demo-multiselect']) {
            const button = trigger(id);
            const content = await popup(button);
            await eventually(async () => assert.notEqual(await content.getAttribute('data-neo-browser-content'), null));
            await button.click();
            await content.waitFor({ state: 'visible' });
            if (id === 'demo-date-picker') {
                await content.getByRole('button', { name: / 15, / }).first().click();
                await eventually(async () => assert.match(await page.getByTestId('demo-date-value').textContent(), /^\d{4}-\d{2}-15$/));
                await content.waitFor({ state: 'hidden' });
            } else if (id === 'demo-time-picker') {
                const fields = content.locator('select');
                await fields.nth(0).selectOption('9');
                await fields.nth(1).selectOption('45');
                await content.getByRole('button', { name: 'Apply', exact: true }).click();
                await eventually(async () => assert.equal(await page.getByTestId('demo-time-value').textContent(), '09:45'));
                await content.waitFor({ state: 'hidden' });
            } else if (id === 'demo-combobox') {
                const search = page.getByPlaceholder('Search workspaces', { exact: true });
                await eventually(async () => assert.equal(await search.evaluate(e => document.activeElement === e), true));
                assert.equal(await search.evaluate(e => JSON.parse(e.dataset.neoInputConfig).debounce), 250);
                await search.fill('Prod');
                // Browser lists deliberately block selection until the debounced query is accepted.
                await eventually(async () => assert.equal(await search.getAttribute('data-neo-input-pending'), null));
                await content.getByRole('option', { name: 'Product', exact: false }).click();
                await eventually(async () => assert.equal(await page.getByTestId('demo-combo-value').textContent(), 'Product'));
                await content.waitFor({ state: 'hidden' });
            } else {
                const search = page.getByPlaceholder('Search notification workspaces', { exact: true });
                await eventually(async () => assert.equal(await search.evaluate(e => document.activeElement === e), true));
                assert.equal(await search.evaluate(e => JSON.parse(e.dataset.neoInputConfig).debounce), 250);
                await search.fill('Prod');
                // Browser lists deliberately block selection until the debounced query is accepted.
                await eventually(async () => assert.equal(await search.getAttribute('data-neo-input-pending'), null));
                await content.getByRole('option', { name: 'Product', exact: false }).click();
                await eventually(async () => assert.equal(await page.getByTestId('demo-multi-value').textContent(), 'Product'));
                assert.equal(await content.isVisible(), true);
                await search.fill('Eng');
                await eventually(async () => assert.equal(await search.getAttribute('data-neo-input-pending'), null));
                await content.getByRole('option', { name: 'Engineering', exact: false }).click();
                await eventually(async () => assert.match(await page.getByTestId('demo-multi-value').textContent(), /Engineering/));
                await button.click();
                await content.waitFor({ state: 'hidden' });
            }
        }
        await page.getByTestId('demo-composed-reset').click();
        for (const id of ['demo-date-value', 'demo-time-value', 'demo-combo-value', 'demo-multi-value']) {
            await eventually(async () => assert.equal(await page.getByTestId(id).textContent(), 'None'));
        }
        // Recreated scope clears drafts and search as well as the displayed values.
        const multi = trigger('demo-multiselect');
        await eventually(async () => assert.notEqual(await (await popup(multi)).getAttribute('data-neo-browser-content'), null));
        await multi.click();
        const search = page.getByPlaceholder('Search notification workspaces', { exact: true });
        await search.waitFor({ state: 'visible' });
        assert.equal(await search.inputValue(), '');
        await multi.click();
        await page.setViewportSize({ width: 390, height: 844 });
        await page.locator('#composed-controls').scrollIntoViewIfNeeded();
        assert.equal(await page.evaluate(() => document.documentElement.scrollWidth <= innerWidth), true);
        await page.screenshot({ path: join(tmpdir(), 'neoui-composed-controls-mobile.png'), fullPage: true });
        assert.deepEqual(errors, []);
    } finally { await browser.close(); }
});


test('browser input demo preserves drafts under delayed acceptance, renders, rejection and resets', async () => {
    const browser = await chromium.launch({ headless: true, channel: process.env.NEOUI_BROWSER_CHANNEL ?? 'msedge' });
    const page = await browser.newPage({ viewport: { width: 1440, height: 1100 } });
    const origin = new URL(url).origin;
    if (origin.includes('.devtunnels.ms')) await page.route(origin + '/**', route => route.continue({ headers: { ...route.request().headers(), 'X-Tunnel-Skip-AntiPhishing-Page': 'true' } }));
    const errors = []; page.on('pageerror', e => errors.push(String(e)));
    try {
        await page.goto(url, { waitUntil: 'domcontentloaded' });
        const input = page.getByTestId('demo-browser-input');
        await input.locator('xpath=self::*[@data-neo-input-ready="true"]').waitFor();
        await input.fill('first draft'); await input.fill('latest draft');
        await page.getByTestId('demo-input-render').evaluate(e => e.click());
        assert.equal(await input.inputValue(), 'latest draft');
        await eventually(async () => assert.equal(await page.getByTestId('demo-input-accepted').textContent(), 'latest draft'));
        await page.getByTestId('demo-input-reject').check();
        await input.fill('reject this');
        await eventually(async () => assert.equal(await input.inputValue(), 'latest draft'));
        await page.getByTestId('demo-input-reset').evaluate(e => e.click());
        await eventually(async () => assert.equal(await input.inputValue(), 'Hello'));
        const change = page.getByTestId('demo-change-input');
        await page.getByTestId('demo-input-reject').uncheck();
        await change.fill('change draft');
        assert.equal(await page.getByTestId('demo-change-accepted').textContent(), 'Hello');
        await change.blur();
        await eventually(async () => assert.equal(await page.getByTestId('demo-change-accepted').textContent(), 'change draft'));
        await page.locator('#input-drafts').scrollIntoViewIfNeeded();
        await page.screenshot({ path: join(tmpdir(), 'neoui-input-drafts-desktop.png') });
        await page.setViewportSize({ width: 390, height: 844 });
        await page.locator('#input-drafts').scrollIntoViewIfNeeded();
        assert.equal(await page.evaluate(() => document.documentElement.scrollWidth > window.innerWidth), false);
        await page.screenshot({ path: join(tmpdir(), 'neoui-input-drafts-mobile.png') });
        assert.deepEqual(errors, []);
    } finally { await page.close(); await browser.close(); }
});


test('dialog form demo flushes fast drafts before validation and onchange actions update without blur', async () => {
    const browser = await chromium.launch({ headless: true, channel: process.env.NEOUI_BROWSER_CHANNEL ?? 'msedge' });
    const page = await browser.newPage({ viewport: { width: 1440, height: 1100 } });
    const origin = new URL(url).origin;
    if (origin.includes('.devtunnels.ms')) await page.route(origin + '/**', route => route.continue({ headers: { ...route.request().headers(), 'X-Tunnel-Skip-AntiPhishing-Page': 'true' } }));
    const errors = []; page.on('pageerror', e => errors.push(String(e)));
    try {
        await page.goto(url, { waitUntil: 'domcontentloaded' });
        await page.getByTestId('demo-browser-input').locator('xpath=self::*[@data-neo-input-ready="true"]').waitFor();
        await page.getByTestId('demo-open-input-form').click();
        const form = page.getByTestId('demo-safe-form');
        for (const id of ['demo-profile-name', 'demo-profile-note']) await form.locator(`[id="${id}"][data-neo-input-ready="true"]`).waitFor();
        const immediate = await form.evaluate(form => {
            const name = form.querySelector('#demo-profile-name'), note = form.querySelector('#demo-profile-note');
            for (const value of ['Earlier profile', 'Latest profile']) { name.value = value; name.dispatchEvent(new Event('input', { bubbles: true })); }
            note.value = 'Final note'; note.dispatchEvent(new Event('input', { bubbles: true }));
            form.querySelector('button[type="submit"]').click();
            return document.querySelector('[data-testid="demo-profile-saved"]').textContent;
        });
        assert.equal(immediate, 'None');
        const save = form.getByTestId('demo-form-save');
        await eventually(async () => assert.equal(await save.isDisabled(), true));
        assert.equal(await save.getAttribute('aria-busy'), 'true');
        await save.locator('svg.animate-spin').waitFor({ state: 'visible' });
        assert.equal(await form.isVisible(), true);
        await eventually(async () => assert.equal(await page.getByTestId('demo-profile-saved').textContent(), 'Latest profile · Final note'));
        await form.waitFor({ state: 'hidden' });
        await page.getByTestId('demo-open-input-form').click();
        await form.locator('#demo-profile-name[data-neo-input-ready="true"]').waitFor();
        await form.evaluate(form => {
            const name = form.querySelector('#demo-profile-name'); name.value = 'x'; name.dispatchEvent(new Event('input', { bubbles: true })); form.requestSubmit();
        });
        await form.getByText(/minimum length/).waitFor();
        await eventually(async () => assert.equal(await form.getByTestId('demo-form-feedback').textContent(), 'Review the validation errors before saving.'));
        assert.equal(await form.isVisible(), true);
        await eventually(async () => assert.equal(await form.getByTestId('demo-form-save').isEnabled(), true));
        assert.equal(await page.getByTestId('demo-profile-saved').textContent(), 'Latest profile · Final note');
        await page.setViewportSize({ width: 390, height: 844 });
        await page.screenshot({ path: join(tmpdir(), 'neoui-form-flush-dialog-mobile.png') });
        await page.keyboard.press('Escape'); await form.waitFor({ state: 'hidden' });
        const input = page.getByTestId('demo-boundary-input'), action = page.getByTestId('demo-boundary-action');
        assert.equal(await action.isDisabled(), true);
        await input.fill('a'); await eventually(async () => assert.equal(await action.isEnabled(), true));
        await input.fill('abcdef');
        assert.equal(await page.getByTestId('demo-boundary-calls').textContent(), '1');
        assert.equal(await page.getByTestId('demo-boundary-value').textContent(), 'a');
        for (let i = 0; i < 6; i++) await input.press('Backspace');
        await eventually(async () => assert.equal(await action.isDisabled(), true));
        assert.equal(await page.getByTestId('demo-boundary-calls').textContent(), '2');
        assert.deepEqual(errors, []);
    } finally { await page.close(); await browser.close(); }
});


test('menu-family gallery follows existing demo styling and exposes local interactions', async () => {
    const browser = await chromium.launch({ headless: true, channel: process.env.NEOUI_BROWSER_CHANNEL ?? 'msedge' });
    const page = await browser.newPage({ viewport: { width: 1440, height: 1100 } });
    const errors = []; page.on('pageerror', error => errors.push(String(error)));
    const origin = new URL(url).origin;
    if (origin.includes('.devtunnels.ms')) await page.route(origin + '/**', route => route.continue({ headers: { ...route.request().headers(), 'X-Tunnel-Skip-AntiPhishing-Page': 'true' } }));
    try {
        await page.goto(url, { waitUntil: 'domcontentloaded' });
        const root = page.locator('[data-menu-family-demo]');
        await root.locator('[data-neo-navigation="menubar"][data-neo-navigation-ready]').waitFor();
        await root.scrollIntoViewIfNeeded();
        const checkboxHeading = page.getByRole('heading', { name: 'Checkbox and radio menus', exact: true });
        assert.equal(await checkboxHeading.evaluate(e => Boolean(e.compareDocumentPosition(document.querySelector('#menu-families')) & Node.DOCUMENT_POSITION_FOLLOWING)), true);
        const inbox = root.getByRole('menuitem', { name: 'Inbox', exact: true }); await inbox.click();
        const menu = page.locator(`[id="${await inbox.getAttribute('aria-controls')}"]`);
        const filter = menu.getByRole('menuitemcheckbox', { name: 'Unread only', exact: true });
        await filter.click(); assert.equal(await filter.getAttribute('aria-checked'), 'true');
        await root.getByRole('menuitem', { name: 'View', exact: true }).click(); await page.getByRole('menuitem', { name: 'Compact view', exact: true }).waitFor({ state: 'visible' });
        await page.keyboard.press('Escape');
        await root.getByText('Right-click here, or focus and press Shift+F10', { exact: true }).click({ button: 'right' });
        await page.getByRole('menuitem', { name: 'Open conversation', exact: true }).waitFor({ state: 'visible' }); await page.keyboard.press('Escape');
        const split = root.locator('[data-neo-split-button]').filter({ has: page.getByRole('button', { name: 'Save view', exact: true }) });
        const primary = split.getByRole('button', { name: 'Save view', exact: true });
        const beforeColor = await primary.evaluate(e => getComputedStyle(e).backgroundColor);
        await split.getByRole('button', { name: 'More options', exact: true }).click(); await page.getByRole('menuitem', { name: 'Save as new view', exact: true }).waitFor({ state: 'visible' });
        await eventually(async () => assert.notEqual(await primary.evaluate(e => getComputedStyle(e).backgroundColor), beforeColor));
        await page.keyboard.press('Escape');
        await root.getByRole('button', { name: 'Demo activity', exact: true }).click(); assert.equal(await root.getByRole('button', { name: 'Demo activity', exact: true }).getAttribute('data-active'), 'true');
        await root.screenshot({ path: join(tmpdir(), 'neoui-menu-families-desktop.png') });
        await page.setViewportSize({ width: 390, height: 1000 }); await root.scrollIntoViewIfNeeded();
        assert.equal(await root.evaluate(e => e.scrollWidth <= e.clientWidth + 1), true);
        await root.screenshot({ path: join(tmpdir(), 'neoui-menu-families-mobile.png') });
        assert.deepEqual(errors, []);
    } finally { await browser.close(); }
});


test('menu-family gallery supports iPhone touch switching without flicker', async () => {
    const browser = process.env.NEOUI_BROWSER_ENGINE === 'webkit'
        ? await webkit.launch({ headless: true })
        : await chromium.launch({ headless: true, channel: 'msedge' });
    const page = await browser.newPage({ ...devices['iPhone 13'] });
    const errors = []; page.on('pageerror', error => errors.push(String(error)));
    const origin = new URL(url).origin;
    if (origin.includes('.devtunnels.ms')) await page.route(origin + '/**', route => route.continue({ headers: {
        ...route.request().headers(), 'X-Tunnel-Skip-AntiPhishing-Page': 'true'
    } }));
    try {
        await page.goto(url, { waitUntil: 'domcontentloaded' });
        const root = page.locator('[data-menu-family-demo]');
        await root.locator('[data-neo-navigation="menubar"][data-neo-navigation-ready]').waitFor();
        await eventually(async () => assert.equal(await page.getByRole('menuitem', { name: 'More actions', exact: true, includeHidden: true }).getAttribute('aria-expanded'), 'false'));
        const inbox = root.getByRole('menuitem', { name: 'Inbox', exact: true });
        const view = root.getByRole('menuitem', { name: 'View', exact: true });
        for (let i = 0; i < 3; i++) {
            await inbox.tap(); assert.equal(await inbox.getAttribute('aria-expanded'), 'true');
            await view.tap(); await page.getByRole('menuitem', { name: 'Compact view', exact: true }).waitFor({ state: 'visible' });
            await pause(400); assert.equal(await view.getAttribute('aria-expanded'), 'true');
            assert.equal(await inbox.getAttribute('aria-expanded'), 'false');
            await inbox.tap(); assert.equal(await inbox.getAttribute('aria-expanded'), 'true');
            await inbox.tap(); assert.equal(await inbox.getAttribute('aria-expanded'), 'false');
        }
        assert.deepEqual(errors, []);
    } finally { await browser.close(); }
});

// Run the Server gallery with --BrowserDefault true and NEOUI_EXPECT_DEFAULT=Browser.
// This exercises the actual portaled, grouped and virtualized spotlight, rather
// than substituting the standalone Input examples for CommandInput coverage.
test('spotlight preserves browser drafts and caret while debounced searches are in flight',
    { skip: process.env.NEOUI_EXPECT_DEFAULT !== 'Browser' }, async () => {
    const browser = process.env.NEOUI_BROWSER_ENGINE === 'webkit'
        ? await webkit.launch({ headless: true })
        : await chromium.launch({ headless: true, channel: process.env.NEOUI_BROWSER_CHANNEL ?? 'msedge' });
    const page = await browser.newPage();
    const errors = [];
    page.on('pageerror', error => errors.push(String(error)));
    await page.addInitScript(() => {
        const NativeWebSocket = window.WebSocket;
        window.WebSocket = class extends NativeWebSocket {
            send(data) {
                if (window.__delaySpotlightTraffic) setTimeout(() => super.send(data), 250);
                else super.send(data);
            }
        };
    });
    try {
        await page.goto(url);
        await page.waitForFunction(() => document.querySelector('[data-local-events]')?.textContent === '0');
        await page.getByRole('button', { name: /Search components/ }).first().click();
        const search = page.getByPlaceholder('Type a command or search...', { exact: true });
        await search.waitFor({ state: 'visible' });
        // Configuration is rendered before the asynchronous controller import completes.
        await page.waitForFunction(() => document.querySelector('input[placeholder="Type a command or search..."]')
            ?.getAttribute('data-neo-input-ready') === 'true');
        assert.equal(JSON.parse(await search.getAttribute('data-neo-input-config')).debounce, 300);
        await search.evaluate(element => {
            window.__delaySpotlightTraffic = true;
            window.__spotlightRollbacks = [];
            let expected = element.value;
            element.addEventListener('input', () => { expected = element.value; });
            const check = () => {
                if (!element.isConnected) return;
                if (element.value !== expected) {
                    window.__spotlightRollbacks.push({ expected, actual: element.value });
                    expected = element.value;
                }
                requestAnimationFrame(check);
            };
            requestAnimationFrame(check);
        });
        const accepted = async expected => {
            await eventually(async () => {
                assert.equal(await search.inputValue(), expected);
                assert.equal(JSON.parse(await search.getAttribute('data-neo-input-config')).value, expected);
                assert.equal(await search.getAttribute('data-neo-input-pending'), null);
            });
            assert.deepEqual(await page.evaluate(() => window.__spotlightRollbacks), []);
        };
        for (let run = 0; run < 3; run++) {
            await search.pressSequentially('com', { delay: 90 });
            // Dispatch the first search, then continue before its response arrives.
            await pause(360);
            await search.pressSequentially('bobox', { delay: 90 });
            await accepted('combobox');
            await page.getByRole('option').filter({ hasText: 'Combobox' }).first().waitFor();
            await search.evaluate(element => element.setSelectionRange(3, 3));
            await search.pressSequentially('X', { delay: 90 });
            await accepted('comXbobox');
            assert.equal(await search.evaluate(element => element.selectionStart), 4);
            await search.press('Backspace');
            await accepted('combobox');
            await search.evaluate(element => element.setSelectionRange(element.value.length, element.value.length));
            for (let key = 0; key < 8; key++) {
                await search.press('Backspace');
                await pause(90);
            }
            await accepted('');
        }
        await search.pressSequentially('Interaction Mode', { delay: 50 });
        await accepted('Interaction Mode');
        await page.getByRole('option').filter({ hasText: 'Interaction Mode' }).click();
        await search.waitFor({ state: 'hidden' });
        await page.getByRole('heading', { name: 'Interaction Mode', exact: true }).waitFor();
        assert.deepEqual(errors, []);
    } finally { await browser.close(); }
});


test('spotlight acknowledges accepted search independently of delayed render completion',
    { skip: process.env.NEOUI_EXPECT_DEFAULT !== 'Browser' }, async () => {
    const browser = await chromium.launch({ headless: true, channel: process.env.NEOUI_BROWSER_CHANNEL ?? 'msedge' });
    const page = await browser.newPage();
    const errors = [];
    page.on('pageerror', error => errors.push(String(error)));
    await page.addInitScript(() => {
        const NativeWebSocket = window.WebSocket;
        window.__heldSpotlightRenders = [];
        window.WebSocket = class extends NativeWebSocket {
            send(data) {
                const message = typeof data === 'string' ? data : new TextDecoder().decode(data);
                if (window.__holdSpotlightRenders && message.includes('OnRenderCompleted'))
                    window.__heldSpotlightRenders.push(() => super.send(data));
                else super.send(data);
            }
        };
        window.__releaseSpotlightRenders = () => {
            window.__holdSpotlightRenders = false;
            for (const send of window.__heldSpotlightRenders.splice(0)) send();
        };
    });
    try {
        await page.goto(url);
        await page.waitForFunction(() => document.querySelector('[data-local-events]')?.textContent === '0');
        await page.getByRole('button', { name: /Search components/ }).first().click();
        const search = page.getByPlaceholder('Type a command or search...', { exact: true });
        await page.waitForFunction(() => document.querySelector('input[placeholder="Type a command or search..."]')?.dataset.neoInputReady === 'true');
        await search.evaluate(element => {
            window.__spotlightRollbacks = [];
            let expected = element.value;
            element.addEventListener('input', () => { expected = element.value; });
            const descriptor = Object.getOwnPropertyDescriptor(HTMLInputElement.prototype, 'value');
            Object.defineProperty(element, 'value', {
                get() { return descriptor.get.call(this); },
                set(value) {
                    if (value !== expected && value !== descriptor.get.call(this))
                        window.__spotlightRollbacks.push({ expected, actual: value });
                    descriptor.set.call(this, value);
                }
            });
            window.__holdSpotlightRenders = true;
            setTimeout(() => window.__releaseSpotlightRenders(), 8000);
        });
        // Each accepted query renders grouped/virtualized results. Retain render
        // completions until the circuit's render buffer applies backpressure.
        for (let index = 0; index < 16; index++) {
            await search.fill(index % 2 ? 'combobox' : 'input');
            await pause(420);
        }
        await page.waitForFunction(() => window.__heldSpotlightRenders.length > 0);
        // Acknowledge the same final edit while the accepted context is ahead of
        // its descendant render. No second fill is permitted to repair rollback.
        await search.fill('browser-local');
        await page.waitForFunction(() => !window.__holdSpotlightRenders);
        await eventually(async () => {
            assert.equal(await search.inputValue(), 'browser-local');
            assert.equal(await search.getAttribute('data-neo-input-pending'), null);
        });
        await pause(800);
        assert.equal(await search.inputValue(), 'browser-local');
        await page.getByRole('option').filter({ hasText: 'Interaction Mode' }).waitFor();
        assert.deepEqual(await page.evaluate(() => window.__spotlightRollbacks), []);
        assert.deepEqual(errors, []);
    } finally { await browser.close(); }
});


test('CommandInput gallery preserves debounced drafts, keyboard selection and reset during search', async () => {
    const browser = process.env.NEOUI_BROWSER_ENGINE === 'webkit'
        ? await webkit.launch({ headless: true })
        : await chromium.launch({ headless: true, channel: process.env.NEOUI_BROWSER_CHANNEL ?? 'msedge' });
    const page = await browser.newPage({ viewport: { width: 1440, height: 1100 } });
    const errors = [];
    page.on('pageerror', error => errors.push(String(error)));
    const origin = new URL(url).origin;
    if (origin.includes('.devtunnels.ms')) await page.route(origin + '/**', route => route.continue({ headers: {
        ...route.request().headers(), 'X-Tunnel-Skip-AntiPhishing-Page': 'true'
    } }));
    try {
        await page.goto(url + '#command-search');
        const root = page.getByTestId('demo-command-search');
        const search = root.getByPlaceholder('Search demo commands...');
        await search.locator('xpath=self::*[@data-neo-input-ready="true"]').waitFor();
        assert.equal(JSON.parse(await search.getAttribute('data-neo-input-config')).debounce, 300);
        await search.pressSequentially('Create', { delay: 50 });
        await page.getByTestId('demo-command-status').filter({ hasText: 'Processing' }).waitFor();
        await search.pressSequentially(' project', { delay: 50 });
        assert.equal(await search.inputValue(), 'Create project');
        await eventually(async () => {
            assert.equal(await search.inputValue(), 'Create project');
            assert.equal(await page.getByTestId('demo-command-query').textContent(), 'Create project');
            assert.equal(await search.getAttribute('data-neo-input-pending'), null);
        });
        await search.press('ArrowDown'); await search.press('Enter');
        await eventually(async () => assert.equal(await page.getByTestId('demo-command-selected').textContent(), 'create-project'));
        await search.fill('nothing matches');
        await eventually(async () => assert.equal(await search.getAttribute('data-neo-input-pending'), null));
        await root.getByText('No matching commands.', { exact: true }).waitFor();
        await search.fill('theme');
        await page.getByTestId('demo-command-status').filter({ hasText: 'Processing' }).waitFor();
        await page.getByTestId('demo-command-reset').click();
        const reset = root.getByPlaceholder('Search demo commands...');
        await reset.locator('xpath=self::*[@data-neo-input-ready="true"]').waitFor();
        await pause(1100);
        assert.equal(await reset.inputValue(), '');
        assert.equal(await page.getByTestId('demo-command-query').textContent(), '(empty)');
        assert.equal(await page.getByTestId('demo-command-selected').textContent(), 'None');
        assert.equal(await root.getByRole('option', { name: 'Archived workspace', exact: true }).getAttribute('aria-disabled'), 'true');
        await root.screenshot({ path: join(tmpdir(), 'neoui-command-search-demo-desktop.png') });
        await page.setViewportSize({ width: 390, height: 1000 });
        await root.scrollIntoViewIfNeeded();
        assert.equal(await root.evaluate(element => element.scrollWidth <= element.clientWidth + 1), true);
        await root.screenshot({ path: join(tmpdir(), 'neoui-command-search-demo-mobile.png') });
        assert.deepEqual(errors, []);
    } finally { await browser.close(); }
});


test('iPhone taps keep MultiSelect checkbox feedback and tag badges visible',async()=>{
 const browser=await(process.env.NEOUI_BROWSER_ENGINE==='webkit'?webkit.launch({headless:true}):chromium.launch({headless:true,channel:process.env.NEOUI_BROWSER_CHANNEL??'msedge'}));
 const page=await browser.newPage({...devices['iPhone 13']}),errors=[];page.on('pageerror',error=>errors.push(String(error)));
 try{
  await page.addInitScript(await readFile(new URL('./node_modules/@floating-ui/core/dist/floating-ui.core.umd.js',import.meta.url),'utf8')+'\n'+await readFile(new URL('./node_modules/@floating-ui/dom/dist/floating-ui.dom.umd.js',import.meta.url),'utf8'));
  await page.goto(url);
  const initial=page.getByRole('combobox').filter({hasText:'Notify workspaces'});await page.getByPlaceholder('Search notification workspaces').locator('xpath=ancestor::*[@data-neo-browser-content][1]').waitFor({state:'attached'});
  const trigger=page.locator(`[id="${await initial.getAttribute('id')}"]`);await trigger.tap();
  const row=page.getByRole('option',{name:'Design',exact:true});const marker=row.locator('[data-neo-multi-indicator]');await marker.tap();
  await eventually(async()=>assert.match(await trigger.textContent(),/Design/));assert.equal(await row.getAttribute('aria-selected'),'true');assert.equal(await marker.getAttribute('data-state'),'checked');assert.equal(await marker.locator('svg').isVisible(),true);
  // Empty selection is represented as null by MultiSelect's existing binding contract.
  await marker.tap();await eventually(async()=>assert.equal(await page.getByTestId('demo-multi-value').textContent(),'None'));
  await marker.evaluate(el=>{for(let index=0;index<9;index++)el.click();});
  await eventually(async()=>assert.equal(await page.getByTestId('demo-multi-value').textContent(),'Design'));
  const content=row.locator('xpath=ancestor::*[@data-neo-client-list][1]');
  await content.getByRole('button',{name:'Clear',exact:true}).tap();
  // The parent model label can render before the child option's canonical marker.
  await eventually(async()=>{assert.equal(await page.getByTestId('demo-multi-value').textContent(),'None');assert.equal(await marker.getAttribute('data-state'),'unchecked');assert.match(await trigger.textContent(),/Notify workspaces/);});
  await content.getByRole('button',{name:'Close',exact:true}).tap();
  await eventually(async()=>assert.equal(await trigger.getAttribute('aria-expanded'),'false'));
  const input=page.getByPlaceholder('Type a tag or paste a list');await input.locator('xpath=ancestor::*[@data-neo-browser-field][1][@data-neo-state-ready="true"]').waitFor();await input.scrollIntoViewIfNeeded();
  await page.waitForFunction(()=>Boolean(document.getElementById(document.querySelector('[placeholder="Type a tag or paste a list"]').getAttribute('aria-controls'))));
  await input.fill('Des');await page.getByRole('option',{name:'Design',exact:true}).tap();
  await input.locator('xpath=..').getByRole('button',{name:'Remove Design',exact:true}).waitFor();assert.equal(await input.inputValue(),'');assert.deepEqual(errors,[]);
 }finally{await browser.close();}
});


test('iPhone informational Tooltip and summary HoverCard taps retain late-rendered content', async () => {
    const browser = await (process.env.NEOUI_BROWSER_ENGINE === 'webkit' ? webkit.launch({ headless: true }) : chromium.launch({ headless: true, channel: 'msedge' }));
    const page = await browser.newPage({ ...devices['iPhone 13'] });
    const errors = []; page.on('pageerror', error => errors.push(String(error)));
    const origin = new URL(url).origin;
    if (origin.includes('.devtunnels.ms')) await page.route(origin + '/**', route => route.continue({ headers: { ...route.request().headers(), 'X-Tunnel-Skip-AntiPhishing-Page': 'true' } }));
    try {
        await page.goto(url, { waitUntil: 'domcontentloaded' });
        const root = page.locator('[data-hover-interaction-demo]');
        const hint = root.getByRole('button', { name: 'Helpful hint', exact: true });
        await hint.locator('xpath=self::*[@data-neo-hover-informational="true"]').waitFor();
        await page.getByRole('tooltip', { includeHidden: true }).filter({ hasText: 'Browser timers and dismissal' }).locator('xpath=self::*[@data-neo-browser-content]').waitFor({ state: 'attached' });
        await hint.tap();
        await page.getByRole('tooltip').filter({ hasText: 'Browser timers and dismissal' }).waitFor({ state: 'visible' });
        await page.getByTestId('summary-hover-details').locator('xpath=ancestor::*[@data-neo-browser-content]').waitFor({ state: 'attached' });
        await page.getByTestId('summary-hover-trigger').tap();
        await page.getByTestId('summary-hover-details').waitFor({ state: 'visible' });
        await page.getByTestId('load-hover-details').tap();
        await page.getByTestId('late-hover-details').waitFor({ state: 'visible' });
        await page.touchscreen.tap(5, 5);
        await page.getByTestId('summary-hover-details').waitFor({ state: 'hidden' });
        assert.deepEqual(errors, []);
    } finally { await browser.close(); }
});
