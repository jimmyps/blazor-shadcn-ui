import { test, before, after, beforeEach, afterEach } from 'node:test';
import assert from 'node:assert/strict';
import { readFile } from 'node:fs/promises';
import { chromium, webkit } from 'playwright';
const url = process.env.NEOUI_TEST_URL ?? 'http://localhost:5198';
let browser, page, errors;
const pause = ms => new Promise(r => setTimeout(r, ms));
async function eventually(check) { const start = Date.now(); for (;;) { try { await check(); return; } catch (e) { if (Date.now() - start > 15000) throw e; await pause(30); } } }
before(async () => { browser = process.env.NEOUI_BROWSER_ENGINE === 'webkit'
    ? await webkit.launch({ headless: true })
    : await chromium.launch({ channel: 'msedge', headless: true }); });
after(async () => { await browser?.close(); });
beforeEach(async () => {
    page = await browser.newPage({ hasTouch: true }); errors = []; page.on('pageerror', e => errors.push(String(e)));
    const core = await readFile(new URL('./node_modules/@floating-ui/core/dist/floating-ui.core.umd.js', import.meta.url), 'utf8');
    const dom = await readFile(new URL('./node_modules/@floating-ui/dom/dist/floating-ui.dom.umd.js', import.meta.url), 'utf8');
    await page.addInitScript(core + '\n' + dom);
    await page.addInitScript(() => { const WS = window.WebSocket; window.__sockets = []; window.WebSocket = class extends WS { constructor(...args) { super(...args); window.__sockets.push(this); } }; });
    await page.goto(url + '/?adoption=true');
    await page.locator('[data-neo-navigation="menubar"][data-neo-navigation-ready]').waitFor();
    await eventually(async () => assert.equal(await page.getByRole('menuitem', { name: 'Family File', exact: true }).getAttribute('aria-expanded'), 'false'));
    // Nested portals finish mounting after their root; wait for controller hydration before blocking the circuit.
    await eventually(async () => assert.equal(await page.getByRole('menuitem', { name: 'Family Deeper', exact: true, includeHidden: true }).getAttribute('aria-expanded'), 'false'));
});
afterEach(async () => { await page.close(); assert.deepEqual(errors, []); });
const item = name => page.getByRole('menuitem', { name, exact: true, includeHidden: true });
const check = name => page.getByRole('menuitemcheckbox', { name, exact: true, includeHidden: true });
const radio = name => page.getByRole('menuitemradio', { name, exact: true, includeHidden: true });
const file = () => item('Family File');
const expanded = async (trigger, value) => eventually(async () => assert.equal(await trigger.getAttribute('aria-expanded'), String(value)));
const checked = async (element, value) => eventually(async () => assert.equal(await element.getAttribute('aria-checked'), String(value)));
async function blockTransport() { if (process.env.NEOUI_TEST_RENDERER === 'wasm') return; await page.evaluate(() => { for (const socket of window.__sockets) socket.send = () => {}; }); }

test('menubar opening, hover switching, disabled skipping and keyboard navigation stay local', async () => {
    await blockTransport(); await file().click(); await check('Family filter').waitFor({ state: 'visible' });
    await item('Family Edit').hover(); await item('Family Undo').waitFor({ state: 'visible' }); await expanded(file(), false);
    await item('Family Edit').focus(); await page.keyboard.press('ArrowLeft'); await expanded(file(), true);
    assert.equal(await file().evaluate(e => e === document.activeElement), true);
    await page.keyboard.press('ArrowDown'); assert.equal(await item('Family action').evaluate(e => e === document.activeElement), true); await page.keyboard.press('End');
    await page.keyboard.press('Escape'); await expanded(file(), false);
});
test('menubar checkbox/radio feedback, nested submenus and dismissal stay local without a circuit', async () => {
    await blockTransport(); await file().click(); await check('Family filter').click(); await checked(check('Family filter'), true); await expanded(file(), true);
    await radio('Family B').click(); await checked(radio('Family B'), true); await checked(radio('Family A'), false);
    await item('Family More').hover(); await check('Family nested filter').waitFor({ state: 'visible' });
    await item('Family Deeper').hover(); await item('Family deep action').waitFor({ state: 'visible' });
    await item('Family deep action').click(); await expanded(file(), false);
});
test('menubar delayed rejected changes and controlled reset reconcile safely', async () => {
    await page.getByTestId('family-slow').check(); await page.getByTestId('family-reject').check();
    await file().click(); assert.equal(await check('Family filter').evaluate(e => { e.click(); return e.getAttribute('aria-checked'); }), 'true');
    await checked(check('Family filter'), false); await expanded(file(), true);
    await page.getByTestId('family-reject').uncheck(); await file().click(); await check('Family filter').click(); await checked(check('Family filter'), true);
    await page.getByTestId('family-reset').click(); await expanded(file(), false);
    await file().click(); await checked(check('Family filter'), false); await pause(1300); await checked(check('Family filter'), false);
});
test('context menu uses pointer coordinates, local checks, nested menus and keyboard context gesture', async () => {
    await blockTransport(); const target = page.getByTestId('family-context'); await target.click({ button: 'right', position: { x: 12, y: 8 } });
    await item('Context action').waitFor({ state: 'visible' }); await check('Context filter').click(); await checked(check('Context filter'), true);
    assert.equal(await page.evaluate(() => document.body.style.overflow), 'hidden');
    await item('Context More').hover(); await item('Context nested').waitFor({ state: 'visible' }); await item('Context nested').click();
    await eventually(async () => assert.equal(await page.evaluate(() => document.body.style.overflow), ''));
    await target.locator('..').focus(); await page.keyboard.press('Shift+F10'); await item('Context action').waitFor({ state: 'visible' });
    await page.keyboard.press('Escape'); await item('Context action').waitFor({ state: 'hidden' });
});
test('context programmatic opening and controlled reset preserve the browser controller', async () => {
    await page.getByTestId('family-programmatic').click(); await item('Context action').waitFor({ state: 'visible' });
    await page.getByTestId('family-reset').evaluate(e => e.click()); await item('Context action').waitFor({ state: 'hidden' });
    await page.getByTestId('family-programmatic').click(); await item('Context action').waitFor({ state: 'visible' });
});
test('navigation panels switch and dismiss locally with native links and focus', async () => {
    await blockTransport(); const products = item('Family Products'), resources = item('Family Resources');
    await products.hover(); await page.getByRole('menuitem', { name: 'Family Product One', exact: true }).waitFor({ state: 'visible' });
    await resources.hover(); await item('Family Docs').waitFor({ state: 'visible' }); await expanded(products, false);
    await resources.focus(); await page.keyboard.press('ArrowLeft'); await expanded(products, true);
    await page.keyboard.press('ArrowDown'); assert.equal(await item('Family Product One').evaluate(e => e === document.activeElement), true);
    await page.keyboard.press('Escape'); await expanded(products, false);
    await products.click();
    const dismissed = await item('Family Product One').evaluate(async link => {
        const trigger = document.querySelector('[data-neo-navigation="navigation"] [aria-controls]');
        link.addEventListener('click', event => event.preventDefault(), { once: true });
        link.click(); await new Promise(resolve => setTimeout(resolve, 0));
        return trigger.getAttribute('aria-expanded');
    });
    assert.equal(dismissed, 'false');
    await products.click(); await item('Family Product One').click();
    // The minimal WASM host has no Router: native href activation may load a new document.
    await eventually(async () => assert.equal(new URL(page.url()).hash, '#one'));
});
test('split and pill dropdowns inherit browser policy with explicit Blazor escape hatch', async () => {
    await blockTransport(); const split = page.getByTestId('family-split').getByRole('button', { name: 'More options' });
    await split.click(); await item('Family Save As').waitFor({ state: 'visible' }); await item('Family Save As').click(); await expanded(split, false);
    const legacy = page.getByTestId('family-split-blazor').getByRole('button', { name: 'More options' });
    const legacyId = await legacy.getAttribute('aria-controls'); assert.equal(await page.locator(`[id="${legacyId}"]`).getAttribute('data-neo-browser-content'), null);
    await page.getByRole('button', { name: 'Family Pill Menu', exact: true }).click(); await item('Family Pill Action').waitFor({ state: 'visible' }); await item('Family Pill Action').click();
    const pill = page.locator('[data-neo-navigation="pill"]'); await pill.getByRole('button', { name: 'More options' }).click(); await item('Family Pill Split Action').waitFor({ state: 'visible' });
});
test('pill active feedback is immediate and revision resets it', async () => {
    await page.getByTestId('family-slow').check(); const home = page.getByRole('button', { name: 'Family Home', exact: true }), inbox = page.getByRole('button', { name: 'Family Inbox', exact: true });
    await inbox.click(); assert.equal(await inbox.getAttribute('data-active'), 'true'); assert.equal(await home.getAttribute('data-active'), 'false');
    await page.getByTestId('family-reset').click(); await eventually(async () => assert.equal(await home.getAttribute('data-active'), 'true'));
});

test('menu actions dismiss locally before their application callback completes', async () => {
    await page.getByTestId('family-slow').check(); await file().click();
    const feedback = await item('Family action').evaluate(element => {
        const trigger = [...document.querySelectorAll('[role="menuitem"][aria-controls]')].find(e => e.textContent.trim() === 'Family File');
        element.click();
        return { expanded: trigger.getAttribute('aria-expanded'), actions: document.querySelector('[data-testid="family-actions"]').textContent.trim() };
    });
    assert.deepEqual(feedback, { expanded: 'false', actions: '0' });
    await eventually(async () => assert.equal((await page.getByTestId('family-actions').textContent()).trim(), '1'));
});
test('retained-circuit reconnect reconciles a menu reset without replaying actions', { skip: process.env.NEOUI_TEST_RENDERER === 'wasm' }, async () => {
    await file().click(); await check('Family filter').click(); await checked(check('Family filter'), true);
    await eventually(async () => assert.match(await page.getByTestId('family-accepted').textContent(), /^True/));
    await page.getByTestId('family-reset-delayed').evaluate(e => e.click());
    await eventually(async () => assert.equal(await page.getByTestId('family-reset-pending').textContent(), 'true'));
    await page.context().setOffline(true); await pause(2000); await page.context().setOffline(false);
    await expanded(file(), false); await file().click(); await checked(check('Family filter'), false);
    assert.equal((await page.getByTestId('family-actions').textContent()).trim(), '0');
});

test('controlled menubar reset reapplies a selection while keeping the requested menu open', async () => {
    await file().click(); await check('Family filter').click(); await checked(check('Family filter'), true);
    await page.getByTestId('family-reset-open').evaluate(e => e.click()); await checked(check('Family filter'), false);
    await expanded(file(), true); await pause(300); await expanded(file(), true);
});
test('pill feedback survives unrelated renders and late click echoes after a reset', async () => {
    await page.getByTestId('family-slow').check(); const inbox = page.getByRole('button', { name: 'Family Inbox', exact: true }), home = page.getByRole('button', { name: 'Family Home', exact: true });
    await inbox.click(); await page.getByTestId('family-render').click();
    await eventually(async () => assert.equal(await page.getByTestId('family-pill-render').textContent(), '1'));
    assert.equal(await inbox.getAttribute('data-active'), 'true');
    await page.getByTestId('family-reset').click(); await eventually(async () => assert.equal(await home.getAttribute('data-active'), 'true'));
    await pause(1300); assert.equal(await home.getAttribute('data-active'), 'true');
});


test('clicking a sibling menubar trigger after hover switching keeps the new menu open', async () => {
    await blockTransport(); await file().click(); await expanded(file(), true);
    const edit = item('Family Edit');
    await edit.click(); await expanded(edit, true); await item('Family Undo').waitFor({ state: 'visible' });
    await pause(250); await expanded(edit, true); await expanded(file(), false);
    assert.equal(await item('Family Undo').evaluate(e => document.activeElement === e), true);
    await file().click(); await expanded(file(), true); await expanded(edit, false);
    await file().click(); await expanded(file(), false);
});


test('touch menubar taps switch siblings once and keep submenus usable without a circuit', async () => {
    await blockTransport(); const edit = item('Family Edit');
    for (let i = 0; i < 3; i++) {
        await file().tap(); await expanded(file(), true);
        await edit.tap(); await expanded(edit, true); await expanded(file(), false);
        await pause(300); await expanded(edit, true);
        await file().tap(); await expanded(file(), true); await expanded(edit, false);
        await item('Family More').tap(); await check('Family nested filter').waitFor({ state: 'visible' });
        await check('Family nested filter').tap(); await expanded(file(), true);
        await file().tap(); await expanded(file(), false);
    }
    await file().tap(); await expanded(file(), true);
    // The context target sits underneath the open menu in Chromium's fixture layout.
    // Tap the empty right edge of the viewport to exercise real outside dismissal.
    const viewport = page.viewportSize();
    await page.touchscreen.tap(viewport.width - 8, viewport.height - 8); await expanded(file(), false);
});

test('touch navigation taps stay closed after dismissal instead of running hover timers', async () => {
    await blockTransport(); const products = item('Family Products'), resources = item('Family Resources');
    await products.tap(); await expanded(products, true);
    await resources.tap(); await expanded(resources, true); await expanded(products, false);
    await resources.tap(); await expanded(resources, false);
    await pause(600); await expanded(resources, false);
    await products.tap(); await expanded(products, true);
    await page.getByTestId('family-context').tap(); await expanded(products, false);
    await pause(600); await expanded(products, false);
});
