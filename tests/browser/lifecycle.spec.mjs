import { test, before, after } from 'node:test';
import assert from 'node:assert/strict';
import { chromium } from 'playwright';
import { readFile } from 'node:fs/promises';
const url = process.env.NEOUI_TEST_URL ?? 'http://localhost:5198';
let browser;
const pause = ms => new Promise(resolve => setTimeout(resolve, ms));
async function eventually(check) { const start = Date.now(); for (;;) { try { await check(); return; } catch (error) { if (Date.now() - start > 10000) throw error; await pause(30); } } }
before(async () => { browser = await chromium.launch({ channel: 'msedge', headless: true }); });
after(async () => { await browser?.close(); });
for (const kind of ['dropdown', 'popover', 'select', 'submenu']) {
    test(`${kind} removal during a delayed floating-module import preserves the circuit and subsequent mounts`, async () => {
        const page = await browser.newPage(), errors = [];
        let release, entered, completed;
        const held = new Promise(resolve => { release = resolve; });
        const pending = new Promise(resolve => { entered = resolve; });
        const continued = new Promise(resolve => { completed = resolve; });
        page.on('pageerror', error => errors.push(String(error)));
        page.on('console', message => { if (message.type() === 'error') errors.push(message.text()); });
        const core = await readFile(new URL('./node_modules/@floating-ui/core/dist/floating-ui.core.umd.js', import.meta.url), 'utf8');
        const dom = await readFile(new URL('./node_modules/@floating-ui/dom/dist/floating-ui.dom.umd.js', import.meta.url), 'utf8');
        await page.addInitScript(core + '\n' + dom);
        await page.route('**/browser-floating*.js', async route => {
            entered(); await held;
            try { await route.continue(); } finally { completed(); }
        });
        try {
            await page.goto(url + '/?lifecycle=true' + (process.env.NEOUI_EXPECT_DEFAULT === 'Browser' ? '&global=true' : ''));
            await eventually(async () => assert.equal(await page.getByTestId('lifecycle-ready').textContent(), 'true'));
            await page.getByTestId('lifecycle-kind').selectOption(kind);
            await eventually(async () => assert.equal(await page.getByTestId('lifecycle-current-kind').textContent(), kind));
            // A server round trip here also proves fixture hydration before the mount.
            await page.getByTestId('lifecycle-independent').click();
            await eventually(async () => assert.equal(await page.getByTestId('lifecycle-count').textContent(), '1'));
            await page.getByTestId('lifecycle-mount').click();
            await new Promise((resolve, reject) => {
                const timeout = setTimeout(() => reject(new Error('Floating module request was never intercepted')), 10000);
                pending.then(() => { clearTimeout(timeout); resolve(); });
            });
            await page.getByTestId('lifecycle-trigger').waitFor();
            await page.getByTestId('lifecycle-remove').click();
            await page.getByTestId('lifecycle-trigger').waitFor({ state: 'detached' });
            assert.equal(await page.getByTestId('lifecycle-mounted').textContent(), 'false');
            release(); await continued;
            // Let the import completion resume on .NET before checking the independent callback.
            await pause(250);
            await page.getByTestId('lifecycle-independent').click();
            await eventually(async () => assert.equal(await page.getByTestId('lifecycle-count').textContent(), '2'));
            await page.getByTestId('lifecycle-mount').click();
            const trigger = page.getByTestId('lifecycle-trigger');
            await trigger.waitFor();
            const button = kind === 'select' ? trigger : trigger.locator('..');
            await eventually(async () => assert.equal(await button.getAttribute('aria-expanded'), 'false'));
            await button.click();
            await eventually(async () => assert.equal(await button.getAttribute('aria-expanded'), 'true'));
            if (kind === 'select') await page.getByRole('option', { name: 'Lifecycle option', exact: true }).click();
            else if (kind === 'popover') await page.getByTestId('lifecycle-action').click();
            else if (kind === 'submenu') {
                await page.getByRole('menuitem', { name: 'Lifecycle submenu', exact: true }).hover();
                await page.getByRole('menuitem', { name: 'Nested lifecycle action', exact: true }).click();
            } else await page.getByRole('menuitem', { name: 'Lifecycle action', exact: true }).click();
            await eventually(async () => assert.equal(await page.getByTestId('lifecycle-actions').textContent(), '1'));
            assert.deepEqual(errors, []);
        } finally { release(); await page.close(); }
    });
}
