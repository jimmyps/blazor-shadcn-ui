import { test, before, after } from 'node:test';
import assert from 'node:assert/strict';
import { chromium, webkit, devices } from 'playwright';
import { readFile } from 'node:fs/promises';
const url = process.env.NEOUI_TEST_URL ?? 'http://localhost:5198';
const floatingScript = await readFile(new URL('./node_modules/@floating-ui/core/dist/floating-ui.core.umd.js', import.meta.url), 'utf8') + '\n' + await readFile(new URL('./node_modules/@floating-ui/dom/dist/floating-ui.dom.umd.js', import.meta.url), 'utf8');
let browser;
before(async () => { browser = await (process.env.NEOUI_BROWSER_ENGINE === 'webkit' ? webkit.launch({ headless: true }) : chromium.launch({ channel: 'msedge', headless: true })); });
after(async () => { await browser?.close(); });
async function open(mode) {
    const page = await browser.newPage({ ...devices['iPhone 13'] });
    page.setDefaultTimeout(5000);
    const errors = [];
    page.on('pageerror', error => errors.push(String(error)));
    await page.addInitScript(floatingScript);
    await page.goto(url + '/?sweep=true&global=true&mode=' + mode.toLowerCase());
    await page.locator('[data-testid="test-host-ready"][data-ready="true"]').waitFor({ state: 'attached' });
    if (mode === 'Browser') await page.getByTestId('sweep-hover-child').locator('xpath=ancestor::*[@data-neo-browser-content]').waitFor({ state: 'attached' });
    return { page, errors };
}
for (const mode of ['Blazor', 'Browser']) {
    test(`${mode}: iPhone tap reveals the HoverCard and outside tap dismisses it`, async () => {
        const { page, errors } = await open(mode);
        try {
            const trigger = page.getByRole('button', { name: 'Sweep hover', exact: true });
            const content = page.getByTestId('sweep-hover-child');
            await trigger.tap();
            await content.waitFor({ state: 'visible' });
            await page.getByTestId('sweep-independent').tap();
            await content.waitFor({ state: 'hidden' });
            await page.getByTestId('sweep-independent').filter({ hasText: 'Independent 1' }).waitFor();
            await trigger.tap();
            await content.waitFor({ state: 'visible' });
            assert.deepEqual(errors, []);
        } finally { await page.close(); }
    });
}

test('Browser: informational Tooltip and nonfocusable HoverCard taps work without circuit traffic', async () => {
    const { page, errors } = await open('Browser');
    try {
        const pairs = [['sweep-focus-tooltip', 'sweep-focus-tooltip-content'], ['sweep-touch-tooltip', 'sweep-touch-tooltip-content'], ['sweep-info-tooltip', 'sweep-info-tooltip-content'], ['sweep-touch-hover', 'sweep-touch-hover-content']];
        for (const [, contentId] of pairs) await page.getByTestId(contentId).locator('xpath=ancestor::*[@data-neo-browser-content]').waitFor({ state: 'attached' });
        await page.evaluate(() => { window.__touchNativeSend = WebSocket.prototype.send; WebSocket.prototype.send = function () {}; });
        for (const [triggerId, contentId] of pairs) {
            const trigger = page.getByTestId(triggerId), content = page.getByTestId(contentId);
            await trigger.tap(); await content.waitFor({ state: 'visible' });
            if (triggerId === 'sweep-touch-tooltip') assert.equal(await trigger.evaluate(el => el.parentElement.contains(document.activeElement)), false, 'Tap does not require focus');
            await trigger.tap(); await page.waitForTimeout(150);
            assert.equal(await content.isVisible(), true, 'Compatibility click does not immediately toggle the layer shut');
            if (triggerId === 'sweep-info-tooltip') {
                await trigger.press('Enter'); await content.waitFor({ state: 'hidden' });
                await trigger.tap(); await content.waitFor({ state: 'visible' });
            }
            await page.touchscreen.tap(5, 5); await content.waitFor({ state: 'hidden' });
        }
        assert.deepEqual(errors, []);
    } finally {
        await page.evaluate(() => { if (window.__touchNativeSend) WebSocket.prototype.send = window.__touchNativeSend; }).catch(() => {});
        await page.close();
    }
});

test('Browser: cancelled or scrolling Tooltip and HoverCard gestures do not reveal a layer', async () => {
    const { page, errors } = await open('Browser');
    try {
        for (const [triggerId, contentId] of [['sweep-focus-tooltip', 'sweep-focus-tooltip-content'], ['sweep-touch-tooltip', 'sweep-touch-tooltip-content'], ['sweep-touch-hover', 'sweep-touch-hover-content']]) {
            const trigger = page.getByTestId(triggerId), content = page.getByTestId(contentId);
            await content.locator('xpath=ancestor::*[@data-neo-browser-content]').waitFor({ state: 'attached' });
            for (const cancellation of ['cancel', 'scroll']) {
                await trigger.evaluate((el, cancellation) => {
                    const rect = el.getBoundingClientRect();
                    const options = { bubbles: true, cancelable: true, pointerType: 'touch', pointerId: 42, isPrimary: true, clientX: rect.x + 2, clientY: rect.y + 2 };
                    el.dispatchEvent(new PointerEvent('pointerdown', options));
                    if (cancellation === 'scroll') el.dispatchEvent(new PointerEvent('pointermove', { ...options, clientY: options.clientY + 25 }));
                    el.dispatchEvent(new PointerEvent(cancellation === 'cancel' ? 'pointercancel' : 'pointerup', options));
                    el.dispatchEvent(new MouseEvent('click', { bubbles: true, cancelable: true, detail: 1 }));
                }, cancellation);
                await page.waitForTimeout(150);
                assert.equal(await content.isVisible(), false, cancellation);
            }
        }
        assert.deepEqual(errors, []);
    } finally { await page.close(); }
});

for (const mode of ['Blazor', 'Browser']) {
    test(`${mode}: touching a Tooltip action preserves its callback and Browser dismissal`, async () => {
        const { page, errors } = await open(mode);
        try {
            const action = page.getByRole('button', { name: 'Sweep tooltip', exact: true });
            const tooltip = page.getByRole('tooltip', { includeHidden: true }).filter({ hasText: 'Sweep tooltip content' });
            if (mode === 'Browser') await tooltip.locator('xpath=self::*[@data-neo-browser-content]').waitFor({ state: 'attached' });
            await action.focus(); await tooltip.waitFor({ state: 'visible' });
            await action.tap();
            await page.getByTestId('sweep-tooltip-pending').filter({ hasText: 'Waiting' }).waitFor();
            if (mode === 'Browser') {
                await tooltip.waitFor({ state: 'hidden' });
                await page.waitForTimeout(150); assert.equal(await tooltip.isVisible(), false);
            }
            await page.getByTestId('sweep-tooltip-release').tap();
            await page.getByTestId('sweep-tooltip-actions').filter({ hasText: '1' }).waitFor();
            assert.deepEqual(errors, []);
        } finally { await page.close(); }
    });
}

test('Browser: HoverCard keeps late content live and a later-mounted text trigger works after remount', async () => {
    const { page, errors } = await open('Browser');
    try {
        const trigger = page.getByTestId('sweep-touch-hover'), content = page.getByTestId('sweep-touch-hover-content');
        await content.locator('xpath=ancestor::*[@data-neo-browser-content]').waitFor({ state: 'attached' });
        await trigger.tap(); await content.waitFor({ state: 'visible' });
        await page.getByTestId('sweep-load-hover').evaluate(el => el.click());
        await page.getByTestId('sweep-touch-hover-loaded').waitFor({ state: 'visible' });
        await page.getByTestId('sweep-touch-hover-child').tap();
        await page.getByTestId('sweep-touch-hover-child').filter({ hasText: 'Touch hover action 1' }).waitFor();
        await page.touchscreen.tap(5, 5); await content.waitFor({ state: 'hidden' });
        const conditional = page.getByTestId('sweep-conditional-hover-content');
        await conditional.locator('xpath=ancestor::*[@data-neo-browser-content]').waitFor({ state: 'attached' });
        await page.getByTestId('sweep-conditional-hover').tap(); await conditional.waitFor({ state: 'visible' });
        await page.touchscreen.tap(5, 5); await conditional.waitFor({ state: 'hidden' });
        for (let mount = 0; mount < 2; mount++) {
            await page.getByTestId('sweep-mount-hover').tap();
            const late = page.getByTestId('sweep-late-hover-content');
            await late.locator('xpath=ancestor::*[@data-neo-browser-content]').waitFor({ state: 'attached' });
            await page.getByTestId('sweep-late-hover').tap(); await late.waitFor({ state: 'visible' });
            await page.touchscreen.tap(5, 5); await late.waitFor({ state: 'hidden' });
            await page.getByTestId('sweep-mount-hover').tap(); await late.waitFor({ state: 'detached' });
        }
        await page.getByTestId('sweep-independent').tap();
        await page.getByTestId('sweep-independent').filter({ hasText: 'Independent 1' }).waitFor();
        assert.deepEqual(errors, []);
    } finally { await page.close(); }
});
