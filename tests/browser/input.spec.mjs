import { test, before, after, beforeEach, afterEach } from 'node:test';
import assert from 'node:assert/strict';
import { chromium } from 'playwright';
const url = process.env.NEOUI_TEST_URL ?? 'http://localhost:5198';
const pause = ms => new Promise(r => setTimeout(r, ms));
async function eventually(check) {
    const start = Date.now();
    while (true) { try { await check(); return; } catch (e) { if (Date.now() - start > 20000) throw e; await pause(30); } }
}
let browser, page, errors;
before(async () => { browser = await chromium.launch({ channel: process.env.NEOUI_BROWSER_CHANNEL ?? 'msedge', headless: true }); });
after(async () => { await browser?.close(); });
beforeEach(async () => {
    page = await browser.newPage(); page.setDefaultTimeout(30000); errors = [];
    page.on('pageerror', e => errors.push(String(e)));
    await page.addInitScript(() => {
        const Socket = window.WebSocket; window.__testSockets = [];
        window.WebSocket = class extends Socket { constructor(...args) { super(...args); window.__testSockets.push(this); } };
    });
    await page.goto(url + '/?adoption=true', { waitUntil: 'domcontentloaded' });
    await page.getByPlaceholder('Browser draft').locator('xpath=self::*[@data-neo-input-ready="true"]').waitFor();
});
afterEach(async () => { await page.close(); assert.deepEqual(errors, []); });
const value = id => page.getByTestId(id).textContent();
const settled = input => eventually(async () => assert.equal(await input.getAttribute('data-neo-input-pending'), null));

test('draft and caret survive stale acknowledgements and unrelated parent renders, including blur', async () => {
    const input = page.getByPlaceholder('Browser draft');
    await input.fill('first');
    await eventually(async () => assert.equal(await value('input-calls'), '1'));
    await input.fill('latest draft');
    await input.evaluate(e => e.setSelectionRange(3, 3));
    await page.getByTestId('input-render').evaluate(e => e.click());
    await eventually(async () => assert.equal(await value('input-renders'), '1'));
    assert.equal(await input.inputValue(), 'latest draft');
    await eventually(async () => assert.equal(await value('input-calls'), '2'));
    assert.equal(await input.inputValue(), 'latest draft');
    assert.equal(await input.evaluate(e => e.selectionStart), 3);
    await input.blur();
    await settled(input);
    assert.equal(await input.inputValue(), 'latest draft');
    assert.equal(await value('input-accepted'), 'latest draft');
});

test('Change mode keeps unsent browser edits through renders and submits on blur', async () => {
    const input = page.getByPlaceholder('Change draft');
    await input.fill('unsent text');
    await page.getByTestId('input-render').evaluate(e => e.click());
    await eventually(async () => assert.equal(await value('input-renders'), '1'));
    assert.equal(await input.inputValue(), 'unsent text');
    assert.equal(await value('change-accepted'), 'seed');
    await input.blur(); await settled(input);
    assert.equal(await value('change-accepted'), 'unsent text');
});

test('same-value explicit reset cancels queued edits and ignores in-flight acknowledgements', async () => {
    const input = page.getByPlaceholder('Browser draft');
    await input.evaluate(e => {
        for (const value of ['in flight', 'queued']) { e.value = value; e.dispatchEvent(new Event('input', { bubbles: true })); }
        document.querySelector('[data-testid="input-reset"]').click();
    });
    await eventually(async () => assert.equal(await input.inputValue(), 'seed'));
    await pause(900);
    assert.equal(await input.inputValue(), 'seed');
    assert.ok(Number(await value('input-calls')) <= 1, 'the queued edit was invalidated');
    await input.fill('after reset'); await settled(input);
    assert.equal(await value('input-accepted'), 'after reset');
});

test('latest rejected and normalized edits reconcile only after asynchronous application acceptance', async () => {
    const input = page.getByPlaceholder('Browser draft');
    await page.getByTestId('input-reject').check();
    await input.fill('rejected');
    assert.equal(await input.inputValue(), 'rejected');
    await settled(input);
    assert.equal(await input.inputValue(), 'seed');
    await page.getByTestId('input-reject').uncheck();
    await page.getByTestId('input-normalize').check();
    await input.fill('normalized'); await settled(input);
    assert.equal(await input.inputValue(), 'NORMALIZED');
});

test('IME composition is not submitted midway and debounce flushes on blur', async () => {
    const input = page.getByPlaceholder('Debounced draft');
    await input.evaluate(e => {
        e.focus(); e.dispatchEvent(new CompositionEvent('compositionstart', { bubbles: true }));
        e.value = 'composing'; e.dispatchEvent(new InputEvent('input', { bubbles: true, isComposing: true }));
    });
    await pause(600);
    assert.equal(await value('debounced-accepted'), 'seed');
    await input.evaluate(e => {
        e.value = '完成'; e.dispatchEvent(new CompositionEvent('compositionend', { bubbles: true })); e.blur();
    });
    await settled(input);
    assert.equal(await value('debounced-accepted'), '完成');
});

test('a failed delivery preserves the draft without replay and explicit commands still reconcile', async () => {
    const result = await page.evaluate(async () => {
        const module = await import('/_content/NeoUI.Blazor/js/browser-input.js');
        const input = document.createElement('input'); document.body.append(input);
        input.dataset.neoInputConfig = JSON.stringify({ value: '', revision: 0, updateOn: 'input' });
        let calls = 0;
        module.initialize(input, { invokeMethodAsync: async () => { calls++; throw new Error('disconnected'); } });
        input.value = 'offline draft'; input.dispatchEvent(new Event('input'));
        await new Promise(r => setTimeout(r, 50));
        const preserved = { value: input.value, uncertain: input.hasAttribute('data-neo-input-uncertain'), calls };
        input.dataset.neoInputConfig = JSON.stringify({ value: 'reset', revision: 1, updateOn: 'input' });
        await new Promise(r => setTimeout(r, 0));
        const reset = input.value;
        module.dispose(input);
        input.value = 'disposed'; input.dispatchEvent(new Event('input'));
        await new Promise(r => setTimeout(r, 0)); input.remove();
        return { preserved, reset, calls };
    });
    assert.deepEqual(result, { preserved: { value: 'offline draft', uncertain: true, calls: 1 }, reset: 'reset', calls: 1 });
});


test('composed search drafts stay local and stale results cannot be selected while the latest query is pending', async () => {
    for (const [id, placeholder] of [['slow-search-combo', 'Slow combo query'], ['slow-search-multi', 'Slow multi query']]) {
        const trigger = page.getByTestId(id).locator('[aria-haspopup]').first();
        const content = page.locator(`[id="${await trigger.getAttribute('aria-controls')}"]`);
        await content.locator('xpath=self::*[@data-neo-browser-content]').waitFor({state:'attached'});
        await trigger.click();
        const input = page.getByPlaceholder(placeholder);
        await input.locator('xpath=self::*[@data-neo-input-ready="true"]').waitFor();
        const local = await input.evaluate(e => {
            for (const value of ['first query', 'latest query']) { e.value = value; e.dispatchEvent(new Event('input', { bubbles: true })); }
            const root = e.closest('[data-neo-client-list]');
            [...root.querySelectorAll('[role="option"]')].find(item => item.textContent.includes('Bangkok')).click();
            return { pending: root.hasAttribute('data-neo-search-pending'), value: e.value,
                selected: document.querySelector('[data-testid="slow-search-values"]').textContent };
        });
        assert.deepEqual(local, { pending: true, value: 'latest query', selected: '|' });
        await settled(input);
        assert.equal(await value('slow-search-last'), 'latest query');
        assert.equal(await input.inputValue(), 'latest query');
        await page.keyboard.press('Escape');
    }
});

test('MultiSelect explicit close works when its browser open observation never reached the server', async t => {
    if (process.env.NEOUI_TEST_RENDERER === 'wasm') { t.skip('WebAssembly has no server circuit'); return; }
    const trigger = page.getByTestId('composed-multi').locator('[aria-haspopup]').first();
    const layer = page.locator(`[id="${await trigger.getAttribute('aria-controls')}"]`);
    await layer.locator('xpath=self::*[@data-neo-browser-content]').waitFor({ state: 'attached' });
    await layer.locator('[data-neo-input-ready="true"]').waitFor({ state: 'attached' });
    await page.evaluate(() => { window.__send = WebSocket.prototype.send; WebSocket.prototype.send = function() {}; });
    try {
        assert.equal(await trigger.evaluate(e => { e.click(); return e.getAttribute('aria-expanded'); }), 'true');
    } finally { await page.evaluate(() => { WebSocket.prototype.send = window.__send; }); }
    await page.getByTestId('multi-close').evaluate(e => e.click());
    await eventually(async () => assert.equal(await trigger.getAttribute('aria-expanded'), 'false'));
});


test('reverting a debounced draft settles and repeated values retain their newest edit identity', async () => {
    const debounced = page.getByPlaceholder('Debounced draft');
    await debounced.fill('temporary'); await debounced.fill('seed');
    await settled(debounced);
    assert.equal(await value('debounced-accepted'), 'seed');
    const input = page.getByPlaceholder('Browser draft');
    await input.fill('repeat');
    await eventually(async () => assert.equal(await value('input-calls'), '1'));
    await input.fill('intermediate'); await input.fill('repeat');
    await settled(input);
    assert.equal(await input.inputValue(), 'repeat');
    assert.equal(await value('input-calls'), '3');
});

test('pre-initialization typing is submitted and an acknowledgement never interrupts IME composition', async () => {
    const result = await page.evaluate(async () => {
        const module = await import('/_content/NeoUI.Blazor/js/browser-input.js');
        const input = document.createElement('input'); document.body.append(input); input.focus();
        input.dataset.neoInputConfig = JSON.stringify({ value: 'seed', revision: 0, updateOn: 'input' });
        input.value = 'before hydration';
        let resolve, calls = 0;
        module.initialize(input, { invokeMethodAsync: async (method, value, sequence, revision) => {
            calls++;
            if (calls === 1) return await new Promise(r => { resolve = () => r({ value: 'NORMALIZED', sequence, revision }); });
            return { value, sequence, revision };
        } });
        input.dispatchEvent(new CompositionEvent('compositionstart'));
        resolve(); await new Promise(r => setTimeout(r, 0));
        const during = input.value;
        input.value = '完成'; input.dispatchEvent(new InputEvent('input', { isComposing: true }));
        input.dispatchEvent(new CompositionEvent('compositionend'));
        await new Promise(r => setTimeout(r, 0));
        const after = input.value, pending = input.hasAttribute('data-neo-input-pending');
        module.dispose(input); input.remove();
        return { during, after, pending, calls };
    });
    assert.deepEqual(result, { during: 'before hydration', after: '完成', pending: false, calls: 2 });
});


test('a reset releases new edits even if an old delivery never settles', async () => {
    const result = await page.evaluate(async () => {
        const module = await import('/_content/NeoUI.Blazor/js/browser-input.js');
        const input = document.createElement('input'); document.body.append(input);
        input.dataset.neoInputConfig = JSON.stringify({ value: '', revision: 0, updateOn: 'input' });
        let calls = 0;
        module.initialize(input, { invokeMethodAsync: async (method, value, sequence, revision) => {
            calls++; if (calls === 1) return await new Promise(() => {});
            return { value, sequence, revision };
        } });
        input.value = 'lost acknowledgement'; input.dispatchEvent(new Event('input'));
        input.dataset.neoInputConfig = JSON.stringify({ value: 'reset', revision: 1, updateOn: 'input' });
        await new Promise(r => setTimeout(r, 0));
        input.value = 'after reset'; input.dispatchEvent(new Event('input'));
        await new Promise(r => setTimeout(r, 0));
        const result = { value: input.value, pending: input.hasAttribute('data-neo-input-pending'), calls };
        module.dispose(input); input.remove(); return result;
    });
    assert.deepEqual(result, { value: 'after reset', pending: false, calls: 2 });
});

test('retained-circuit reconnect preserves the input draft and resets without replaying business work', async t => {
    if (process.env.NEOUI_TEST_RENDERER === 'wasm') { t.skip('WebAssembly has no server circuit'); return; }
    const input = page.getByPlaceholder('Browser draft');
    await input.fill('before disconnect');
    await eventually(async () => assert.equal(await value('input-calls'), '1'));
    await page.context().setOffline(true);
    await page.evaluate(() => window.__testSockets.filter(s => s.url.includes('_blazor')).forEach(s => s.close()));
    await input.evaluate(e => { e.value = 'offline draft'; e.dispatchEvent(new Event('input', { bubbles: true })); });
    assert.equal(await input.inputValue(), 'offline draft');
    await page.context().setOffline(false);
    await eventually(async () => assert.equal(await page.evaluate(async () => {
        try { return await Blazor.reconnect(); } catch { return false; }
    }), true));
    await page.waitForFunction(() => window.__testSockets.some(s => s.url.includes('_blazor') && s.readyState === WebSocket.OPEN));
    await page.getByTestId('input-reset').evaluate(e => e.click());
    await eventually(async () => assert.equal(await input.inputValue(), 'seed'));
    const before = Number(await value('input-calls'));
    await pause(800);
    assert.equal(Number(await value('input-calls')), before);
    await input.fill('after reconnect'); await settled(input);
    assert.equal(await value('input-accepted'), 'after reconnect');
    assert.equal(Number(await value('input-calls')), before + 1);
});
