import { test, before, after, beforeEach, afterEach } from 'node:test';
import assert from 'node:assert/strict';
import { chromium } from 'playwright';
const url = process.env.NEOUI_TEST_URL ?? 'http://localhost:5198';
const pause = ms => new Promise(r => setTimeout(r, ms));
async function eventually(check) { const start = Date.now(); while (true) { try { await check(); return; } catch (e) { if (Date.now() - start > 20000) throw e; await pause(30); } } }
let browser, page, errors;
before(async () => { browser = await chromium.launch({ channel: process.env.NEOUI_BROWSER_CHANNEL ?? 'msedge', headless: true }); });
after(async () => { await browser?.close(); });
beforeEach(async () => {
    page = await browser.newPage(); page.setDefaultTimeout(30000); errors = [];
    page.on('pageerror', e => { errors.push(String(e)); if (process.env.NEOUI_TRACE_ERRORS) console.error(e.stack); });
    await page.addInitScript(() => {
        const Socket = window.WebSocket; window.__testSockets = [];
        window.WebSocket = class extends Socket { constructor(...args) { super(...args); window.__testSockets.push(this); } };
    });
    await page.goto(url + '/?adoption=true', { waitUntil: 'domcontentloaded' });
    for (const placeholder of ['Form name', 'Form debounced note', 'Boundary input', 'Scoped form name'])
        await page.getByPlaceholder(placeholder, { exact: true }).locator('xpath=self::*[@data-neo-input-ready="true"]').waitFor();
});
afterEach(async () => { await page.close(); assert.deepEqual(errors, []); });
const text = id => page.getByTestId(id).textContent();
const submit = () => page.getByTestId('safe-form').evaluate(e => e.requestSubmit());
const defaults = async () => { await page.getByPlaceholder('Form name', { exact: true }).fill('Valid name'); await page.getByPlaceholder('Form debounced note', { exact: true }).fill('Final note'); };

test('fast oninput drafts and debounced fields flush before validation and save without blur', async () => {
    const initial = await page.getByTestId('safe-form').evaluate(form => {
        const name = form.querySelector('#form-name'), note = form.querySelector('#form-note');
        name.focus();
        for (const value of ['Earlier', 'Newest name']) { name.value = value; name.dispatchEvent(new Event('input', { bubbles: true })); }
        note.value = 'Newest note'; note.dispatchEvent(new Event('input', { bubbles: true }));
        form.requestSubmit();
        return document.querySelector('[data-testid="form-saves"]').textContent;
    });
    assert.equal(initial, '0');
    await eventually(async () => assert.equal(await text('form-saved'), 'Newest name|Newest note'));
    assert.equal(await text('form-saves'), '1'); assert.equal(await text('form-invalid'), '0');
    await eventually(async () => assert.equal(await page.getByTestId('safe-form').getAttribute('aria-busy'), 'false'));
    assert.equal(await text('form-modified'), 'true');
});

test('Enter and programmatic submission use the same barrier', async () => {
    await defaults();
    await page.getByPlaceholder('Form debounced note', { exact: true }).press('Enter');
    await eventually(async () => assert.equal(await text('form-saved'), 'Valid name|Final note'));
    await eventually(async () => assert.equal(await page.getByTestId('safe-form').getAttribute('aria-busy'), 'false'));
    await page.getByPlaceholder('Form debounced note', { exact: true }).fill('Programmatic note');
    await page.getByTestId('form-programmatic').evaluate(e => e.click());
    await eventually(async () => assert.equal(await text('form-saved'), 'Valid name|Programmatic note'));
    assert.equal(await text('form-saves'), '2');
});

test('validation reads the final invalid draft and does not save an earlier valid value', async () => {
    await defaults();
    await page.getByTestId('safe-form').evaluate(form => {
        const name = form.querySelector('#form-name'); name.value = 'x'; name.dispatchEvent(new Event('input', { bubbles: true })); form.requestSubmit();
    });
    await eventually(async () => assert.equal(await text('form-invalid'), '1'));
    assert.equal(await text('form-saves'), '0'); assert.equal(await text('form-model'), 'x|Final note');
    assert.ok((await page.getByTestId('safe-form').textContent()).includes('minimum length'));
});

test('rejected and normalized drafts stop the first save and expose a correction for review', async () => {
    await page.getByTestId('form-reject').check();
    // Capture pending drafts in one task; Playwright round trips can outlast the rejection callback.
    await page.getByTestId('safe-form').evaluate(form => {
        for (const [id,value] of [['form-name','Valid name'],['form-note','Final note']]) {
            const field=form.querySelector('#'+id);field.value=value;field.dispatchEvent(new Event('input',{bubbles:true}));
        }
        form.requestSubmit();
    });
    await eventually(async () => assert.equal(await text('form-status'), 'ValueAdjusted'));
    assert.equal(await text('form-saves'), '0'); assert.equal(await text('form-invalid'), '0');
    assert.equal(await page.getByPlaceholder('Form name', { exact: true }).inputValue(), '');
    await page.getByTestId('form-reject').uncheck(); await page.getByTestId('form-normalize').check();
    await page.getByTestId('safe-form').evaluate(form => {
        const field=form.querySelector('#form-name');field.value='normal name';field.dispatchEvent(new Event('input',{bubbles:true}));form.requestSubmit();
    });
    await eventually(async () => assert.equal(await page.getByPlaceholder('Form name', { exact: true }).inputValue(), 'NORMAL NAME'));
    await eventually(async () => assert.equal(await page.getByTestId('safe-form').getAttribute('aria-busy'), 'false'));
    assert.equal(await text('form-saves'), '0');
    await submit(); await eventually(async () => assert.equal(await text('form-saved'), 'NORMAL NAME|Final note'));
});

test('submit locks inputs through persistence and ignores duplicate submission', async () => {
    await page.getByTestId('form-hold-save').check(); await defaults(); await submit();
    await eventually(async () => assert.equal(await text('form-saving'), 'true'));
    for (const placeholder of ['Form name', 'Form debounced note']) {
        const input = page.getByPlaceholder(placeholder, { exact: true });
        assert.equal(await input.evaluate(e => e.readOnly), true);
        assert.equal(await input.evaluate(e => e.dispatchEvent(new InputEvent('beforeinput', { bubbles: true, cancelable: true, data: 'x' }))), false);
    }
    await submit(); assert.equal(await text('form-saves'), '1');
    await page.getByTestId('form-release').evaluate(e => e.click());
    await eventually(async () => assert.equal(await page.getByTestId('safe-form').getAttribute('aria-busy'), 'false'));
    assert.equal(await page.getByPlaceholder('Form name', { exact: true }).evaluate(e => e.readOnly), false);
});

test('reset invalidates a pending form snapshot and prevents saving', async () => {
    await page.getByTestId('form-hang').check(); await defaults(); await submit();
    await eventually(async () => assert.equal(await page.getByPlaceholder('Form name', { exact: true }).evaluate(e => e.readOnly), true));
    await page.getByTestId('form-reset').evaluate(e => e.click());
    await eventually(async () => assert.equal(await text('form-status'), 'Reset'));
    assert.equal(await text('form-saves'), '0');
    await page.getByTestId('form-release').evaluate(e => e.click());
    await eventually(async () => assert.equal(await page.getByPlaceholder('Form name', { exact: true }).evaluate(e => e.readOnly), false));
});

test('timeout blocks saving, releases locks and does not replay uncertain callbacks', async () => {
    await page.getByTestId('form-short-timeout').click(); await page.getByTestId('form-hang').check(); await defaults(); await submit();
    await eventually(async () => assert.ok(['TimedOut', 'Uncertain'].includes(await text('form-status'))));
    assert.equal(await text('form-saves'), '0');
    await eventually(async () => assert.equal(await page.getByPlaceholder('Form name', { exact: true }).evaluate(e => e.readOnly), false));
    const calls = await text('form-calls'); await submit();
    await eventually(async () => assert.equal(await text('form-status'), 'Uncertain'));
    assert.equal(await text('form-calls'), calls);
    await page.getByTestId('form-release').evaluate(e => e.click());
});

test('a scope integrates with an existing EditForm and standalone Input.FlushAsync', async () => {
    await page.getByPlaceholder('Scoped form name', { exact: true }).fill('Scoped name');
    await page.getByTestId('standalone-flush').evaluate(e => e.click());
    await eventually(async () => assert.equal(await text('standalone-status'), 'Success'));
    await page.getByPlaceholder('Scoped form name', { exact: true }).fill('Newest scoped name');
    await page.getByTestId('scope-save').evaluate(e => e.form.requestSubmit());
    await eventually(async () => assert.equal(await text('scope-saved'), 'Newest scoped name'));
    assert.equal(await text('form-saves'), '0');
});

test('onchange typing sends only empty/non-empty transitions and the final blur value', async () => {
    const input = page.getByPlaceholder('Boundary input', { exact: true });
    const action = page.getByTestId('boundary-action');
    assert.equal(await action.isDisabled(), true);
    await input.fill('a'); await eventually(async () => assert.equal(await text('boundary-calls'), '1'));
    await eventually(async () => assert.equal(await action.isEnabled(), true));
    await input.fill('abcdef'); await pause(250);
    assert.equal(await text('boundary-calls'), '1'); assert.equal(await text('boundary-accepted'), 'a');
    await input.fill(''); await eventually(async () => assert.equal(await text('boundary-calls'), '2'));
    assert.equal(await action.isDisabled(), true);
    await input.fill('x'); await eventually(async () => assert.equal(await text('boundary-calls'), '3'));
    await input.fill('xyz'); await pause(250); assert.equal(await text('boundary-calls'), '3');
    await input.blur(); await eventually(async () => assert.equal(await text('boundary-calls'), '4'));
    assert.equal(await text('boundary-accepted'), 'xyz');
});

test('browser onchange boundaries respect composition and never send intermediate nonempty typing', async () => {
    const values = await page.evaluate(async () => {
        const module = await import('/_content/NeoUI.Blazor/js/browser-input.js');
        const input = document.createElement('input'); input.id = 'browser-boundary-test'; document.body.append(input);
        input.dataset.neoInputConfig = JSON.stringify({ value: '', revision: 0, updateOn: 'change' });
        const calls = []; module.initialize(input, { invokeMethodAsync: async (_, value, sequence, revision) => { calls.push(value); return { value, sequence, revision }; } });
        for (const value of ['a', 'ab', 'abc', '', 'x', 'xy']) { input.value = value; input.dispatchEvent(new Event('input')); }
        input.dispatchEvent(new Event('change'));
        input.value = ''; input.dispatchEvent(new Event('input'));
        input.dispatchEvent(new CompositionEvent('compositionstart'));
        for (const value of ['partial', '完成']) { input.value = value; input.dispatchEvent(new InputEvent('input', { isComposing: true })); }
        input.dispatchEvent(new CompositionEvent('compositionend'));
        await new Promise(r => setTimeout(r, 30));
        module.dispose(input); input.remove(); return calls;
    });
    assert.deepEqual(values, ['a', '', 'x', 'xy', '', '完成']);
});


test('browser onchange composition sends only the completed emptiness transition', async () => {
    const input = page.getByPlaceholder('Boundary input', { exact: true });
    await input.evaluate(e => {
        e.focus(); e.dispatchEvent(new CompositionEvent('compositionstart', { bubbles: true }));
        for (const value of ['partial', '完成']) { e.value = value; e.dispatchEvent(new InputEvent('input', { bubbles: true, isComposing: true })); }
    });
    await pause(200); assert.equal(await text('boundary-calls'), '0');
    await input.evaluate(e => e.dispatchEvent(new CompositionEvent('compositionend', { bubbles: true })));
    await eventually(async () => assert.equal(await text('boundary-calls'), '1'));
    assert.equal(await text('boundary-accepted'), '完成');
    await input.fill('完成 text'); await pause(200); assert.equal(await text('boundary-calls'), '1');
});

test('rendered scope reconciliation releases a successful snapshot without an interop release message', async () => {
    const result = await page.evaluate(async () => {
        const module = await import('/_content/NeoUI.Blazor/js/browser-input.js');
        const root = document.createElement('div'), input = document.createElement('input');
        root.dataset.neoInputBusy = 'true'; document.body.append(root, input);
        input.id = 'external-scope-field'; input.dataset.neoInputScope = 'lease-test';
        input.dataset.neoInputConfig = JSON.stringify({ value: '', revision: 0, updateOn: 'change', readonly: false });
        const calls = [];
        module.initialize(input, { invokeMethodAsync: async (_, value, sequence, revision) => { calls.push(value); return { value, sequence, revision }; } });
        input.value = 'a'; input.dispatchEvent(new Event('input'));
        await new Promise(r => setTimeout(r, 0));
        input.value = 'final draft'; input.dispatchEvent(new Event('input'));
        const lease = module.beginScope(root, 'lease-test', 500);
        const result = await lease.wait(); const locked = input.readOnly;
        root.dataset.neoInputBusy = 'false'; await new Promise(r => setTimeout(r, 0));
        const released = !input.readOnly;
        module.dispose(input); root.remove(); input.remove();
        return { status: result.status, locked, released, calls };
    });
    assert.deepEqual(result, { status: 'Success', locked: true, released: true, calls: ['a', 'final draft'] });
});

test('flush refuses incomplete IME composition and uninitialized fields', async () => {
    const result = await page.evaluate(async () => {
        const module = await import('/_content/NeoUI.Blazor/js/browser-input.js');
        const input = document.createElement('input'); input.id = 'composing-flush'; document.body.append(input);
        input.dataset.neoInputConfig = JSON.stringify({ value: '', revision: 0, updateOn: 'input' });
        let calls = 0;
        module.initialize(input, { invokeMethodAsync: async () => { calls++; } });
        input.dispatchEvent(new CompositionEvent('compositionstart'));
        input.value = 'partial'; input.dispatchEvent(new InputEvent('input', { isComposing: true }));
        const composing = await module.flushById(input.id, 100);
        const unlocked = !input.readOnly;
        module.dispose(input);
        const notReady = await module.flushById(input.id, 100);
        input.remove(); return { composing: composing.status, notReady: notReady.status, unlocked, calls };
    });
    assert.deepEqual(result, { composing: 'Composing', notReady: 'NotReady', unlocked: true, calls: 0 });
});


test('disconnect during a form flush never saves stale values and rendered state unlocks after reconnect', async t => {
    if (process.env.NEOUI_TEST_RENDERER === 'wasm') { t.skip('WebAssembly has no server circuit'); return; }
    await page.getByTestId('form-hang').check(); await defaults(); await submit();
    await eventually(async () => assert.equal(await page.getByPlaceholder('Form name', { exact: true }).evaluate(e => e.readOnly), true));
    await page.context().setOffline(true);
    await page.evaluate(() => window.__testSockets.filter(s => s.url.includes('_blazor')).forEach(s => s.close()));
    await pause(8000);
    assert.equal(await text('form-saves'), '0');
    await page.context().setOffline(false);
    await eventually(async () => assert.equal(await page.evaluate(async () => { try { return await Blazor.reconnect(); } catch { return false; } }), true));
    await page.waitForFunction(() => window.__testSockets.some(s => s.url.includes('_blazor') && s.readyState === WebSocket.OPEN));
    await eventually(async () => assert.equal(await page.getByPlaceholder('Form name', { exact: true }).evaluate(e => e.readOnly), false));
    await eventually(async () => assert.ok(['Uncertain', 'TimedOut'].includes(await text('form-status'))));
    const calls = await text('form-calls');
    await page.getByTestId('form-reset').evaluate(e => e.click());
    await page.getByTestId('form-release').evaluate(e => e.click());
    await page.getByTestId('form-hang').uncheck();
    await defaults(); await submit();
    await eventually(async () => assert.equal(await text('form-saved'), 'Valid name|Final note'));
    assert.equal(await text('form-saves'), '1');
    assert.equal(Number(await text('form-calls')), Number(calls) + 1);
});


test('native and EditContext validation run after flushing, including stale custom validity from an earlier edit', async () => {
    const form = page.getByTestId('safe-form'), name = page.getByPlaceholder('Form name', { exact: true });
    assert.equal(await form.evaluate(e => e.noValidate), true);
    await name.fill('x');
    await eventually(async () => assert.equal(await name.evaluate(e => e.validity.customError), true));
    await form.evaluate(form => {
        const name = form.querySelector('#form-name'), note = form.querySelector('#form-note');
        name.value = 'Latest valid name'; name.dispatchEvent(new Event('input', { bubbles: true }));
        note.value = 'Valid note'; note.dispatchEvent(new Event('input', { bubbles: true }));
        form.requestSubmit();
    });
    await eventually(async () => assert.equal(await text('form-saved'), 'Latest valid name|Valid note'));
    assert.equal(await text('form-invalid'), '0');
});


test('native-only constraints still prevent saving after the final draft is flushed', async () => {
    await defaults(); await page.getByPlaceholder('Form debounced note', { exact: true }).fill('lowercase note');
    await submit();
    await eventually(async () => assert.equal(await text('form-invalid'), '1'));
    assert.equal(await text('form-saves'), '0');
    assert.equal(await text('form-model'), 'Valid name|lowercase note');
    assert.equal(await page.getByPlaceholder('Form debounced note', { exact: true }).evaluate(e => e.validity.patternMismatch), true);
});


test('WebAssembly form flushing keeps local .NET callbacks working while the browser is offline', async t => {
    if (process.env.NEOUI_TEST_RENDERER !== 'wasm') { t.skip('WebAssembly-specific offline behavior'); return; }
    assert.equal(await page.getByPlaceholder('Form name', { exact: true }).evaluate(e => JSON.parse(e.dataset.neoInputConfig).server), false);
    await page.context().setOffline(true);
    try {
        await page.getByTestId('safe-form').evaluate(form => {
            for (const [id, value] of [['form-name', 'Offline local name'], ['form-note', 'Offline local note']]) {
                const input = form.querySelector('#' + id); input.value = value; input.dispatchEvent(new Event('input', { bubbles: true }));
            }
            form.requestSubmit();
        });
        await eventually(async () => assert.equal(await text('form-saved'), 'Offline local name|Offline local note'));
    } finally { await page.context().setOffline(false); }
});


test('stale nonempty acknowledgements still allow a backspace-to-empty change notification', async () => {
    const result = await page.evaluate(async () => {
        const module = await import('/_content/NeoUI.Blazor/js/browser-input.js');
        const input = document.createElement('input'); document.body.append(input);
        input.dataset.neoInputConfig = JSON.stringify({ value: '', revision: 0, updateOn: 'change' });
        const calls = []; let release;
        module.initialize(input, { invokeMethodAsync: (_, value, sequence, revision) => {
            calls.push(value);
            const ack = { value, sequence, revision };
            return calls.length === 1 ? new Promise(resolve => { release = () => resolve(ack); }) : Promise.resolve(ack);
        } });
        for (const value of ['a', 'ab', 'abc']) { input.value = value; input.dispatchEvent(new Event('input')); }
        release(); await Promise.resolve();
        const draftAfterStaleAck = input.value;
        for (const value of ['ab', 'a', '']) { input.value = value; input.dispatchEvent(new InputEvent('input', { inputType: 'deleteContentBackward' })); }
        await Promise.resolve();
        const result = { calls, draftAfterStaleAck, finalValue: input.value, pending: input.hasAttribute('data-neo-input-pending') };
        module.dispose(input); input.remove(); return result;
    });
    assert.deepEqual(result, { calls: ['a', ''], draftAfterStaleAck: 'abc', finalValue: '', pending: false });
});
test('repeated native Backspace clears a fast change-mode draft and disables the bound action without blur', async () => {
    const input = page.getByPlaceholder('Boundary input', { exact: true }), action = page.getByTestId('boundary-action');
    await input.evaluate(e => { e.focus(); for (const value of ['a', 'ab', 'abc', 'abcd', 'abcde', 'abcdef']) { e.value = value; e.dispatchEvent(new Event('input', { bubbles: true })); } });
    await eventually(async () => assert.equal(await text('boundary-accepted'), 'a'));
    await eventually(async () => assert.equal(await action.isEnabled(), true));
    await pause(100);
    for (let i = 0; i < 6; i++) await input.press('Backspace');
    await eventually(async () => assert.equal(await action.isDisabled(), true));
    assert.equal(await text('boundary-calls'), '2'); assert.equal(await input.evaluate(e => document.activeElement === e), true);
    await input.press('Backspace'); assert.equal(await text('boundary-calls'), '2');
});
