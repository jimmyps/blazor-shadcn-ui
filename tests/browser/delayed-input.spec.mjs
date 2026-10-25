import { test, before, after, beforeEach, afterEach } from 'node:test';
import assert from 'node:assert/strict';
import { chromium, webkit } from 'playwright';
const url = process.env.NEOUI_TEST_URL ?? 'http://localhost:5198';
const pause = ms => new Promise(resolve => setTimeout(resolve, ms));
let browser, page, errors;
before(async () => { browser = await (process.env.NEOUI_BROWSER_ENGINE === 'webkit' ? webkit.launch({ headless: true }) : chromium.launch({ channel: 'msedge', headless: true })); });
after(async () => { await browser?.close(); });
beforeEach(async () => {
    page = await browser.newPage(); errors = [];
    page.on('pageerror', e => errors.push(String(e)));
    await page.goto(url + '/?delayedinput=true' + (process.env.NEOUI_EXPECT_DEFAULT === 'Browser' || process.env.NEOUI_TEST_RENDERER === 'wasm' ? '' : '&scope=true'));
    await page.getByPlaceholder('Delayed name', { exact: true }).locator('xpath=self::*[@data-neo-input-ready="true"]').waitFor();
});
afterEach(async () => { await page.close(); assert.deepEqual(errors, []); });
const field = name => page.getByPlaceholder('Delayed ' + name, { exact: true });
async function loaded() {
    await page.getByTestId('load-release').click();
    await page.getByTestId('load-status').filter({ hasText: 'Loaded' }).waitFor();
}
async function eventually(check) {
    const start = Date.now();
    while (true) { try { await check(); return; } catch (error) { if (Date.now() - start > 5000) throw error; await pause(25); } }
}
test('ordinary async binding updates pristine text and formatted fields without callbacks or revisions', async () => {
    await page.getByTestId('load-start').click();
    await page.getByTestId('load-status').filter({ hasText: 'Loading' }).waitFor();
    await field('name').focus(); // Focus alone is not an edit.
    await loaded();
    await eventually(async () => assert.equal(await field('name').inputValue(), 'Existing segment'));
    assert.equal(await field('description').inputValue(), 'Loaded description');
    assert.equal(await field('number').inputValue(), '123.45');
    assert.equal(await field('currency').inputValue(), '$1,234.57');
    assert.equal(await field('reference').inputValue(), '12-34');
    assert.equal(await field('legacy').inputValue(), 'Legacy name');
    assert.equal(await page.getByTestId('delayed-calls').textContent(), '0');
    assert.equal(await page.getByTestId('adapter-calls').textContent(), '0');
    assert.equal(await field('name').getAttribute('data-neo-input-pending'), null);
    // The currency's exact editing metadata must advance with its display.
    await field('currency').focus();
    assert.equal(await field('currency').inputValue(), '1234.5678');
});
test('typing before loading completes protects newer drafts from the load and stale acknowledgements', async () => {
    await page.getByTestId('load-start').click();
    await field('name').fill('first');
    await eventually(async () => assert.equal(await page.getByTestId('delayed-calls').textContent(), '1'));
    await field('name').fill('latest draft');
    await field('description').fill('my description');
    await field('number').fill('42');
    await field('reference').fill('5678');
    await loaded();
    assert.equal(await field('name').inputValue(), 'latest draft');
    assert.equal(await field('description').inputValue(), 'my description');
    assert.equal(await field('number').inputValue(), '42');
    assert.equal(await field('reference').inputValue(), '56-78');
    await eventually(async () => assert.equal(await page.getByTestId('delayed-accepted').textContent(), 'latest draft'));
    assert.equal(await field('name').inputValue(), 'latest draft');
    assert.deepEqual(errors, []);
});
test('ValueRevision still replaces edited values and same-value resets cancel in-flight edits', async () => {
    await field('name').fill('in flight');
    await page.getByTestId('delayed-reset').click();
    await eventually(async () => assert.equal(await field('name').inputValue(), ''));
    await pause(650);
    assert.equal(await field('name').inputValue(), '');
    await field('name').fill('another edit');
    await page.getByTestId('delayed-reset').click();
    await eventually(async () => assert.equal(await field('name').inputValue(), ''));
    await pause(650);
    assert.equal(await field('name').inputValue(), '');
});

test('pristine values can advance repeatedly; editing, even back to the original value, requires an explicit reset', async () => {
    const result = await page.evaluate(async () => {
        const module = await import('./_content/NeoUI.Blazor/js/browser-input.js');
        const input = document.createElement('input'); document.body.append(input);
        let config = { value: '', revision: 0, updateOn: 'change' }, calls = [];
        const update = async changes => { config = { ...config, ...changes }; input.dataset.neoInputConfig = JSON.stringify(config); await new Promise(r => setTimeout(r, 0)); };
        await update({});
        module.initialize(input, { invokeMethodAsync: async (method, value, sequence, revision) => { calls.push(value); return { value, sequence, revision }; } });
        await update({ value: 'first load' });
        const first = input.value;
        await update({ value: 'second load' });
        const second = input.value;
        input.focus(); input.setSelectionRange(2, 2);
        await update({ debounce: 250 });
        const caret = input.selectionStart;
        input.value = ''; input.dispatchEvent(new Event('input'));
        await new Promise(r => setTimeout(r, 0));
        const boundaryCalls = [...calls];
        await update({ value: '' });
        await update({ value: 'late load after editing' });
        const edited = input.value;
        await update({ value: 'reset', revision: 1 });
        const reset = input.value;
        await update({ value: 'next pristine load' });
        const afterReset = input.value;
        module.dispose(input); input.remove();
        return { first, second, caret, boundaryCalls, edited, reset, afterReset };
    });
    assert.deepEqual(result, { first: 'first load', second: 'second load', caret: 2, boundaryCalls: [''], edited: '', reset: 'reset', afterReset: 'next pristine load' });
});
test('delayed hydration accepts the latest pristine parameter but preserves text typed before initialization', async () => {
    const result = await page.evaluate(async () => {
        const module = await import('./_content/NeoUI.Blazor/js/browser-input.js');
        const create = (liveValue) => {
            const element = document.createElement('input'); element.defaultValue = ''; element.value = liveValue;
            element.dataset.neoInputConfig = JSON.stringify({ value: 'loaded before import', revision: 0, updateOn: 'input' });
            document.body.append(element); return element;
        };
        const untouched = create(''), typed = create('pre-hydration draft'); let calls = [];
        const bridge = { invokeMethodAsync: async (method, value, sequence, revision) => { calls.push(value); return { value, sequence, revision }; } };
        module.initialize(untouched, bridge); module.initialize(typed, bridge);
        await new Promise(r => setTimeout(r, 0));
        const result = { untouched: untouched.value, typed: typed.value, calls };
        for (const element of [untouched, typed]) { module.dispose(element); element.remove(); }
        return result;
    });
    assert.deepEqual(result, { untouched: 'loaded before import', typed: 'pre-hydration draft', calls: ['pre-hydration draft'] });
});
test('controlled stale acknowledgements and parameter echoes never turn an edited field pristine again', async () => {
    const result = await page.evaluate(async () => {
        const module = await import('./_content/NeoUI.Blazor/js/browser-input.js');
        const input = document.createElement('input'); document.body.append(input);
        input.dataset.neoInputConfig = JSON.stringify({ value: '', revision: 0, updateOn: 'input' });
        const pending = [], tick = () => new Promise(r => setTimeout(r, 0));
        module.initialize(input, { invokeMethodAsync: (method, value, sequence, revision) => new Promise(resolve => pending.push(() => resolve({ value, sequence, revision }))) });
        const edit = value => { input.value = value; input.dispatchEvent(new Event('input')); };
        const parameter = async value => { input.dataset.neoInputConfig = JSON.stringify({ value, revision: 0, updateOn: 'input' }); await tick(); };
        edit('first'); edit('latest');
        await parameter('async loaded name'); const duringLoad = input.value;
        pending.shift()(); await tick(); const afterStaleAck = input.value;
        await parameter('first'); const afterStaleRender = input.value;
        pending.shift()(); await tick();
        await parameter('latest'); await parameter('later application value');
        const afterAcceptedEdit = input.value;
        module.dispose(input); input.remove();
        return { duringLoad, afterStaleAck, afterStaleRender, afterAcceptedEdit };
    });
    assert.deepEqual(result, { duringLoad: 'latest', afterStaleAck: 'latest', afterStaleRender: 'latest', afterAcceptedEdit: 'latest' });
});

test('starting IME composition protects the field before any composed characters arrive', async () => {
    const result = await page.evaluate(async () => {
        const module = await import('./_content/NeoUI.Blazor/js/browser-input.js');
        const input = document.createElement('input'); document.body.append(input);
        input.dataset.neoInputConfig = JSON.stringify({ value: '', revision: 0, updateOn: 'input' });
        let calls = 0;
        module.initialize(input, { invokeMethodAsync: async (method, value, sequence, revision) => { calls++; return { value, sequence, revision }; } });
        input.focus(); input.dispatchEvent(new CompositionEvent('compositionstart'));
        input.dataset.neoInputConfig = JSON.stringify({ value: 'async load during composition', revision: 0, updateOn: 'input' });
        await new Promise(r => setTimeout(r, 0));
        const value = input.value;
        module.dispose(input); input.remove();
        return { value, calls };
    });
    assert.deepEqual(result, { value: '', calls: 0 });
});
