import { test, before, after, beforeEach, afterEach } from 'node:test';
import assert from 'node:assert/strict';
import { chromium } from 'playwright';
const url = process.env.NEOUI_TEST_URL ?? 'http://localhost:5198';
let browser, page, errors;
const pause = ms => new Promise(resolve => setTimeout(resolve, ms));
async function eventually(check) { const start = Date.now(); for (;;) { try { await check(); return; } catch (error) { if (Date.now() - start > 15000) throw error; await pause(30); } } }
before(async () => { browser = await chromium.launch({ channel: 'msedge', headless: true }); });
after(async () => { await browser?.close(); });
beforeEach(async () => {
    page = await browser.newPage(); errors = []; page.on('pageerror', error => errors.push(String(error)));
    await page.goto(url + '/?editform=true' + (process.env.NEOUI_EXPECT_DEFAULT === 'Browser' ? '&global=true' : ''));
    await page.waitForFunction(() => document.querySelectorAll('[data-neo-edit-form-ready]').length === 2);
    for (const placeholder of ['Standard automatic name', 'Standard manual name'])
        await page.getByPlaceholder(placeholder).locator('xpath=self::*[@data-neo-input-ready="true"]').waitFor();
});
afterEach(async () => { await page.close(); assert.deepEqual(errors, []); });
const text = id => page.getByTestId('standard-' + id).textContent();
const input = () => page.getByPlaceholder('Standard automatic name');
const submit = () => page.getByTestId('standard-auto').evaluate(form => form.requestSubmit());
const idle = () => eventually(async () => assert.equal(await page.getByTestId('standard-auto').getAttribute('data-neo-edit-form-busy'), 'false'));

test('existing valid/invalid handlers validate the latest draft exactly once with the original EditContext', async () => {
    await input().fill('Newest draft'); await submit();
    await eventually(async () => assert.equal(await text('saved'), 'Newest draft')); await idle();
    assert.equal(await text('valid'), '1'); assert.equal(await text('invalid'), '0'); assert.equal(await text('validations'), '1');
    assert.equal(await text('context'), 'true');
    assert.equal(await page.getByTestId('standard-auto').getAttribute('class'), 'existing-form');
    await input().fill('x'); await submit();
    await eventually(async () => assert.equal(await text('invalid'), '1')); await idle();
    assert.equal(await text('valid'), '1'); assert.equal(await text('validations'), '2'); assert.equal(await text('failures'), '0');
    assert.match(await page.getByTestId('standard-auto').textContent(), /minimum length/);
});

test('OnSubmit retains manual validation and may receive an invalid synchronized model', async () => {
    const field = page.getByPlaceholder('Standard manual name');
    await field.fill('x'); await field.press('Enter');
    await eventually(async () => assert.equal(await text('manual-calls'), '1'));
    assert.equal(await text('saved'), 'x'); assert.equal(await text('manual-valid'), '');
    assert.equal(await text('validations'), '0');
    await eventually(async () => assert.equal(await page.getByTestId('standard-manual').getAttribute('aria-busy'), 'false'));
    await page.getByTestId('standard-manual-validate').check();
    await page.getByTestId('standard-manual-save').click();
    await eventually(async () => assert.equal(await text('manual-valid'), 'false'));
    assert.equal(await text('manual-calls'), '2');
});

test('duplicate click/Enter/requestSubmit events stay blocked throughout awaited persistence and rerenders', async () => {
    await page.getByTestId('standard-hold').check(); await input().fill('Held draft'); await submit();
    await eventually(async () => assert.equal(await text('saving'), 'true'));
    assert.equal(await input().evaluate(field => field.readOnly), true);
    await page.getByTestId('standard-render').click();
    await page.getByTestId('standard-auto').evaluate(form => { form.requestSubmit(); form.requestSubmit(); form.querySelector('[type="submit"]').click(); });
    await pause(200); assert.equal(await text('valid'), '1'); assert.equal(await text('validations'), '1');
    await page.getByTestId('standard-release').click(); await idle();
    assert.equal(await input().evaluate(field => field.readOnly), false);
    await page.getByTestId('standard-hold').uncheck(); await input().fill('Next draft');
    await page.getByTestId('standard-programmatic').click();
    await eventually(async () => assert.equal(await text('saved'), 'Next draft')); await idle();
    assert.equal(await text('valid'), '2');
});

test('synchronization rejection runs no validation or submit callback and exposes failure', async () => {
    await page.getByTestId('standard-reject').check(); await input().fill('Rejected draft'); await submit();
    await eventually(async () => assert.equal(await text('failure'), 'ValueAdjusted')); await idle();
    assert.equal(await text('valid'), '0'); assert.equal(await text('invalid'), '0'); assert.equal(await text('validations'), '0');
    assert.equal(await text('failures'), '1');
    await page.getByTestId('standard-reject').uncheck(); await input().fill('Accepted draft'); await submit();
    await eventually(async () => assert.equal(await text('saved'), 'Accepted draft')); await idle();
});

test('timeout releases submission protection without replaying business callbacks', async () => {
    await page.getByTestId('standard-timeout').click(); await page.getByTestId('standard-hang').check();
    await input().fill('Held input'); await submit();
    await eventually(async () => assert.equal(await text('failure'), 'TimedOut')); await idle();
    assert.equal(await text('validations'), '0'); assert.equal(await text('valid'), '0');
    await page.getByTestId('standard-release').click(); await page.getByTestId('standard-hang').uncheck();
    await eventually(async () => assert.equal(await input().getAttribute('data-neo-input-pending'), null));
    assert.equal(await text('valid'), '0');
    // A timeout is uncertain delivery: a deliberate new edit authorizes retry, never an automatic replay.
    await input().fill('Retried input'); await submit();
    await eventually(async () => assert.equal(await text('valid'), '1'));
});

test('native constraints are checked after synchronization without invoking EditForm callbacks', async () => {
    await page.getByTestId('standard-novalidate').uncheck(); await input().fill('Native draft'); await submit();
    await eventually(async () => assert.equal(await text('model'), 'Native draft')); await idle();
    assert.equal(await text('validations'), '0'); assert.equal(await text('invalid'), '0'); assert.equal(await text('valid'), '0'); assert.equal(await text('failures'), '0');
    await page.getByTestId('standard-native').fill('present'); await submit();
    await eventually(async () => assert.equal(await text('valid'), '1'));
});

test('submitter formnovalidate is preserved while EditContext validation still runs', async () => {
    await page.getByTestId('standard-novalidate').uncheck(); await input().fill('Skip native draft');
    await page.getByTestId('standard-no-native').click();
    await eventually(async () => assert.equal(await text('valid'), '1')); await idle();
    assert.equal(await text('validations'), '1');
    // The submitter exception must not leak into the next normal submission.
    await submit(); await pause(400); await idle(); assert.equal(await text('valid'), '1');
});

test('forms isolate drafts and continue working after EditContext replacement', async () => {
    await page.getByPlaceholder('Standard manual name').evaluate(field => { field.value = 'Other unsent draft'; field.dispatchEvent(new Event('input', { bubbles: true })); });
    await input().fill('First model'); await submit();
    await eventually(async () => assert.equal(await text('saved'), 'First model')); await idle();
    assert.equal(await text('manual-model'), ''); assert.equal(await text('manual-calls'), '0');
    await page.getByTestId('standard-replace').click();
    await eventually(async () => assert.equal(await input().inputValue(), ''));
    await input().fill('Replacement model'); await submit();
    await eventually(async () => assert.equal(await text('saved'), 'Replacement model')); await idle();
    assert.equal(await text('context'), 'true'); assert.equal(await text('validations'), '2');
});


test('known circuit unavailability blocks native submission locally without wedging or replaying it', { skip: process.env.NEOUI_TEST_RENDERER === 'wasm' }, async () => {
    await input().fill('Unsent offline draft');
    await page.evaluate(async () => {
        window.connectionModule = await import('/_content/NeoUI.Blazor/js/browser-input.js');
        window.connectionModule.setConnectionAvailable(false);
        window.localFailures = [];
        document.addEventListener('neoui:form-flush-failed', event => window.localFailures.push(event.detail.status));
    });
    await submit();
    assert.equal(await page.getByTestId('standard-auto').getAttribute('data-neo-form-flush-status'), 'Uncertain');
    assert.deepEqual(await page.evaluate(() => window.localFailures), ['Uncertain']);
    assert.equal(await text('valid'), '0'); assert.equal(await text('validations'), '0');
    await page.evaluate(() => window.connectionModule.setConnectionAvailable(true));
    assert.equal(await text('valid'), '0');
    await input().fill('Deliberately retried draft'); await submit();
    await eventually(async () => assert.equal(await text('saved'), 'Deliberately retried draft')); await idle();
    assert.equal(await text('valid'), '1');
});
