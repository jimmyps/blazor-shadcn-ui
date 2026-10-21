import { test } from 'node:test';
import assert from 'node:assert/strict';
import { chromium } from 'playwright';
import { join } from 'node:path';
import { tmpdir } from 'node:os';

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
            await page.getByRole('switch', { name: 'Toggle dark mode', exact: true }).click();
            await eventually(async () => assert.equal(await page.locator('html').evaluate(e => e.classList.contains('dark')), false));
            assert.equal(await page.evaluate(() => localStorage.getItem('theme')), 'light');
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
        const links = await page.locator('aside a').evaluateAll(elements => elements
            .filter(e => e.getClientRects().length > 0).map(e => e.textContent.trim()));
        assert.equal(links[links.indexOf('Components') + 1], 'Interaction Mode');
        await page.goto(origin + '/components');
        await page.getByRole('heading', { name: 'Interaction Mode', exact: true }).first().waitFor();
        const indexLink = page.locator('a[href="/components/browser-interactions"]').filter({ has: page.locator('h4') });
        await indexLink.click();
        await page.waitForFunction(() => document.querySelector('[data-local-events]')?.textContent === '0');
        await page.getByRole('button', { name: /Search components/ }).first().click();
        const search = page.getByPlaceholder('Type a command or search...');
        await search.waitFor({ state: 'visible' });
        for (const query of ['Interaction Mode', 'browser-local']) {
            await search.fill(query);
            await page.getByRole('option').filter({ hasText: 'Interaction Mode' }).waitFor({ state: 'visible' });
        }
        await page.getByRole('option').filter({ hasText: 'Interaction Mode' }).click();
        await page.getByRole('heading', { name: 'Interaction Mode', exact: true }).waitFor();
        await search.waitFor({ state: 'hidden' });
    } finally { await browser.close(); }
});
