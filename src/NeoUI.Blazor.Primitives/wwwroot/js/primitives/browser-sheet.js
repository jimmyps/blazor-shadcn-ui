import { lockBodyScroll } from './portal.js';
import { createFocusTrap } from './focus-trap.js';

// Mobile sidebar content stays owned by Blazor. Only modal presence and focus are
// local; versioned server commands use the same protocol as disclosure controls.
const instances = new Map();
let removals;
export async function initialize(root, bridge) {
    if (!root?.isConnected) return null;
    if (instances.has(root)) return instances.get(root);
    // Release any server-fallback modal resources before acquiring local ones.
    await bridge.invokeMethodAsync('PrepareAsync');
    if (!root.isConnected) return null;
    const doc = root.ownerDocument, sheet = root.querySelector('[data-neo-sheet-owner]');
    const panel = sheet?.querySelector('[data-neo-sheet-panel]'), overlay = sheet?.querySelector('[data-neo-sheet-overlay]');
    if (!panel || !overlay) return null;
    const owner = sheet.dataset.neoSheetOwner;
    const mobile = doc.defaultView.matchMedia('(max-width:767px)');
    const provider = () => [...doc.querySelectorAll('[data-neo-sidebar-provider]')].find(el => el.dataset.neoSidebarProvider === owner);
    const triggers = () => [...(provider()?.querySelectorAll('[data-sidebar="trigger"],[data-neo-pill-expand]') ?? [])].filter(el => el.closest('[data-neo-sidebar-provider]')?.dataset.neoSidebarProvider === owner).concat([...panel.querySelectorAll('[data-sidebar="trigger"]')]);
    let config = JSON.parse(root.dataset.neoStateHost), version = config.version;
    let open = false, disposed = false, sequence = 0, generation = 0, presence = 0;
    let focusTrap, scrollLock, previousFocus, notifications = Promise.resolve();
    const paintTriggers = () => { for (const trigger of triggers()) { trigger.setAttribute('aria-controls',panel.id); trigger.setAttribute('aria-expanded',String(open)); trigger.setAttribute('aria-haspopup','dialog'); } };
    const paint = next => {
        if (disposed) return;
        const changed = next !== open; open = next; paintTriggers();
        if (!changed) return;
        const ticket = ++presence;
        const oldAnimations = new Set([...panel.getAnimations(), ...overlay.getAnimations()]);
        for (const element of [overlay,panel]) {
            element.hidden = false; element.style.removeProperty('display');
            element.inert = !open; element.setAttribute('aria-hidden',String(!open));
            element.dataset.state = open ? 'open' : 'closed';
        }
        if (open) {
            previousFocus = panel.contains(doc.activeElement) ? triggers().find(el=>!panel.contains(el)) : doc.activeElement;
            scrollLock = lockBodyScroll(); focusTrap = createFocusTrap(panel);
            if (!panel.contains(doc.activeElement)) panel.focus({preventScroll:true});
        } else {
            focusTrap?.apply(); focusTrap = null; scrollLock?.apply(); scrollLock = null;
            if (previousFocus?.isConnected) previousFocus.focus({preventScroll:true});
            const hide = () => { if (!disposed && !open && ticket === presence) for (const element of [overlay,panel]) { element.hidden = true; element.style.display = 'none'; } };
            const exits = [...panel.getAnimations(), ...overlay.getAnimations()].filter(animation => !oldAnimations.has(animation) && Number.isFinite(animation.effect?.getComputedTiming().endTime));
            if (exits.length) void Promise.allSettled(exits.map(animation => animation.finished)).then(hide); else hide();
        }
    };
    const notify = () => {
        if (!config.notify || !bridge) return;
        const event = {values:open?['open']:[],sequence:++sequence,version,generation};
        notifications = notifications.then(async()=>{
            if (disposed || event.generation !== generation) return;
            try { await bridge.invokeMethodAsync('ChangeAsync',event.values,event.sequence,event.version); }
            catch { if (!disposed && event.generation === generation) root.dataset.neoStateUncertain = 'true'; }
        });
    };
    const set = next => { if (next === open) return; paint(next); notify(); };
    const click = event => {
        if (!mobile.matches || event.button > 0 || event.ctrlKey || event.metaKey || event.altKey || event.shiftKey) return;
        const trigger = event.target.closest?.('[data-sidebar="trigger"],[data-neo-pill-expand]');
        if (triggers().includes(trigger) && !trigger.matches(':disabled,[aria-disabled="true"]')) {
            event.preventDefault(); set(!open); return;
        }
        if (!open) return;
        if (event.target === overlay || panel.contains(event.target.closest?.('[data-neo-sheet-close]'))) { event.preventDefault(); set(false); return; }
        const link = event.target.closest?.('a[href]');
        if (sheet.dataset.neoSheetNavigate === 'true' && panel.contains(link) && !link.hasAttribute('download') && (!link.target || link.target === '_self')) {
            const url = new URL(link.href,doc.baseURI);
            if (['http:','https:'].includes(url.protocol)) set(false);
        }
    };
    const keydown = event => {
        if (event.defaultPrevented || event.isComposing || !mobile.matches) return;
        if ((event.ctrlKey || event.metaKey) && event.key.toLowerCase() === 'b') {
            const targetProvider = event.target.closest?.('[data-neo-sidebar-provider]');
            if (targetProvider && targetProvider.dataset.neoSidebarProvider !== owner) return;
            const targetSheet = event.target.closest?.('[data-neo-sheet-owner]');
            if (targetSheet && targetSheet.dataset.neoSheetOwner !== owner) return;
            if (!targetProvider && !targetSheet && doc.querySelector('[data-neo-sidebar-provider]')?.dataset.neoSidebarProvider !== owner) return;
            // A provider's existing keyboard bridge must not toggle this again.
            event.preventDefault(); event.stopImmediatePropagation(); set(!open); return;
        }
        if (open && panel.contains(event.target) && event.key === 'Escape') { event.preventDefault(); event.stopImmediatePropagation(); set(false); }
        if (open && event.key === 'Tab' && event.target === panel) {
            event.preventDefault(); const items = [...panel.querySelectorAll('button:not(:disabled),a[href],input:not(:disabled),[tabindex="0"]')].filter(el=>el.getClientRects().length && !el.closest('[inert]'));
            (event.shiftKey ? items.at(-1) : items[0])?.focus();
        }
    };
    const shortcut = event => { if ((event.ctrlKey || event.metaKey) && event.key.toLowerCase() === 'b') keydown(event); };
    const resized = () => { if (!mobile.matches) set(false); };
    const navigated = () => { if (sheet.dataset.neoSheetNavigate === 'true') set(false); };
    const observer = new MutationObserver(()=>{
        const next = JSON.parse(root.dataset.neoStateHost);
        if (next.version !== version) { version=next.version; generation++; delete root.dataset.neoStateUncertain; paint(next.values.includes('open')); }
        config=next; paintTriggers();
    });
    observer.observe(root,{attributes:true,attributeFilter:['data-neo-state-host']});
    // Queue readiness before installing listeners, in the same JS turn. Later
    // Blazor click callbacks are ordered after this notification and stand down.
    let readyAcknowledged = false;
    const reportReady = () => { if (!disposed) void bridge.invokeMethodAsync('ReadyAsync').then(()=>readyAcknowledged=true).catch(()=>{}); };
    const reconnect = () => { if (!readyAcknowledged && doc.documentElement.dataset.neoConnectionAvailable !== 'false') reportReady(); };
    reportReady();
    doc.addEventListener('neoui:input-connection-state',reconnect);
    // Mobile Safari synthesizes clicks on noninteractive elements only when
    // they have a direct click listener. Delegation alone misses backdrop taps.
    const backdropTarget = () => {};
    overlay.addEventListener('click',backdropTarget);
    doc.addEventListener('click',click,true);
    // Bubble Escape so nested floating controllers consume it first.
    doc.addEventListener('keydown',keydown); doc.addEventListener('keydown',shortcut,true);
    doc.defaultView.addEventListener('popstate',navigated); doc.addEventListener('enhancedload',navigated); mobile.addEventListener('change',resized);
    const controller = { dispose() {
        if (disposed) return; disposed=true; generation++; presence++;
        focusTrap?.apply(); scrollLock?.apply(); observer.disconnect();
        if (open && panel.contains(doc.activeElement) && previousFocus?.isConnected) previousFocus.focus({preventScroll:true});
        for (const element of [overlay,panel]) { element.hidden=true; element.inert=true; element.style.display='none'; }
        doc.removeEventListener('neoui:input-connection-state',reconnect);
        overlay.removeEventListener('click',backdropTarget);
        doc.removeEventListener('click',click,true); doc.removeEventListener('keydown',keydown); doc.removeEventListener('keydown',shortcut,true);
        doc.defaultView.removeEventListener('popstate',navigated); doc.removeEventListener('enhancedload',navigated); mobile.removeEventListener('change',resized);
        for (const trigger of triggers()) { trigger.removeAttribute('aria-controls'); trigger.removeAttribute('aria-expanded'); trigger.removeAttribute('aria-haspopup'); }
        instances.delete(root);
        if (!instances.size) { removals?.disconnect(); removals=null; }
    }};
    instances.set(root,controller);
    if (!removals) { removals=new MutationObserver(()=>{for(const [element,instance] of instances)if(!element.isConnected)instance.dispose();});removals.observe(doc.body,{childList:true,subtree:true}); }
    try { paint(config.values.includes('open') && mobile.matches); paintTriggers(); root.dataset.neoStateReady='true'; return controller; }
    catch (error) { controller.dispose(); throw error; }
}
