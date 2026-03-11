/**
 * Tests for positioning.js – FloatingPortal race-condition fixes.
 *
 * These tests simulate the two intermittent issues that only manifest in
 * Interactive Server (Blazor Server) mode because SignalR delivers JS interop
 * calls asynchronously, allowing multiple requestAnimationFrame callbacks to
 * be queued before any of them fires.
 *
 * Issue 1 – Flash on quick open/close:
 *   showFloating() and hideFloating() are called in rapid succession. Without
 *   the generation counter both RFAs would run, briefly flashing the element
 *   open before hiding it. The fix: the second call increments
 *   _floatingGeneration; the first RFA sees a stale generation and bails out.
 *
 * Issue 2 – First-open animation skipped:
 *   applyPosition(makeVisible=true) must set data-state="open" synchronously
 *   (before the RFA fires) so that CSS animation classes are already bound
 *   when a Blazor re-render makes the element visible. The fix: data-state is
 *   set before the RFA; only the visibility/opacity toggle remains in the RFA.
 */

import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest';
import { showFloating, hideFloating, applyPosition } from '../wwwroot/js/primitives/positioning.js';

// ---------------------------------------------------------------------------
// Helpers
// ---------------------------------------------------------------------------

/**
 * Creates a minimal floating element structure that matches what FloatingPortal
 * renders: an outer container (floating) with an inner element carrying
 * data-state (content).
 */
function createFloating(initialDataState = 'closed') {
  const floating = document.createElement('div');
  floating.style.visibility = 'hidden';
  floating.style.opacity = '0';

  const content = document.createElement('div');
  content.setAttribute('data-state', initialDataState);
  floating.appendChild(content);

  document.body.appendChild(floating);
  return { floating, content };
}

// ---------------------------------------------------------------------------
// Shared teardown
// ---------------------------------------------------------------------------

afterEach(() => {
  vi.useRealTimers();
  document.body.innerHTML = '';
});

// ---------------------------------------------------------------------------
// Issue 1 – Generation counter prevents flash on rapid open/close
// ---------------------------------------------------------------------------

describe('Issue 1 – generation counter prevents flash on rapid open/close', () => {
  beforeEach(() => {
    vi.useFakeTimers();
  });

  it('show then immediate hide: only the hide RFA applies (simulates ~2 ms Server race)', async () => {
    const { floating, content } = createFloating('closed');

    // Simulate what Interactive Server does: showFloating arrives over SignalR,
    // then hideFloating arrives ~2 ms later – both before any RFA fires.
    const showPromise = showFloating(floating);       // gen → 1, schedules show RFA
    await Promise.resolve();                          // let the async function settle (no real IO)
    hideFloating(floating);                           // gen → 2, schedules hide RFA

    await showPromise;

    // Flush all pending requestAnimationFrame callbacks
    vi.runAllTimers();

    // The show RFA should have bailed (generation mismatch); only hide RFA ran.
    expect(content.getAttribute('data-state')).toBe('closed');
    expect(floating.style.getPropertyValue('visibility')).toBe('hidden');
    expect(floating.style.getPropertyValue('opacity')).toBe('0');
  });

  it('hide then immediate show: only the show RFA applies', async () => {
    const { floating, content } = createFloating('open');
    floating.style.setProperty('visibility', 'visible', 'important');
    floating.style.setProperty('opacity', '1', 'important');

    hideFloating(floating);                           // gen → 1, schedules hide RFA
    const showPromise = showFloating(floating);       // gen → 2, schedules show RFA
    await showPromise;

    vi.runAllTimers();

    // The hide RFA should have bailed; only show RFA ran.
    expect(content.getAttribute('data-state')).toBe('open');
    expect(floating.style.getPropertyValue('visibility')).toBe('visible');
    expect(floating.style.getPropertyValue('opacity')).toBe('1');
  });

  it('generation increments monotonically with each call', async () => {
    const { floating } = createFloating();

    expect(floating._floatingGeneration).toBeUndefined();

    await showFloating(floating);      // gen → 1
    expect(floating._floatingGeneration).toBe(1);

    hideFloating(floating);            // gen → 2
    expect(floating._floatingGeneration).toBe(2);

    await showFloating(floating);      // gen → 3
    expect(floating._floatingGeneration).toBe(3);
  });

  it('standalone show with no subsequent call: show RFA applies normally', async () => {
    const { floating, content } = createFloating('closed');

    await showFloating(floating);
    vi.runAllTimers();

    expect(content.getAttribute('data-state')).toBe('open');
    expect(floating.style.getPropertyValue('visibility')).toBe('visible');
    expect(floating.style.getPropertyValue('opacity')).toBe('1');
    expect(floating.style.getPropertyValue('pointer-events')).toBe('auto');
  });

  it('standalone hide with no subsequent call: hide RFA applies normally', () => {
    const { floating, content } = createFloating('open');
    floating.style.setProperty('visibility', 'visible', 'important');

    hideFloating(floating);
    vi.runAllTimers();

    expect(content.getAttribute('data-state')).toBe('closed');
    expect(floating.style.getPropertyValue('visibility')).toBe('hidden');
    expect(floating.style.getPropertyValue('opacity')).toBe('0');
    expect(floating.style.getPropertyValue('pointer-events')).toBe('none');
  });

  it('neoui:visible event fires when show RFA is not cancelled', async () => {
    const { floating } = createFloating('closed');
    const visibleSpy = vi.fn();
    floating.addEventListener('neoui:visible', visibleSpy);

    await showFloating(floating);
    vi.runAllTimers();

    expect(visibleSpy).toHaveBeenCalledTimes(1);
  });

  it('neoui:visible event does NOT fire when show RFA is cancelled by hide', async () => {
    const { floating } = createFloating('closed');
    const visibleSpy = vi.fn();
    floating.addEventListener('neoui:visible', visibleSpy);

    const showPromise = showFloating(floating);
    hideFloating(floating);
    await showPromise;
    vi.runAllTimers();

    expect(visibleSpy).not.toHaveBeenCalled();
  });

  it('neoui:hidden event fires when hide RFA is not cancelled', () => {
    const { floating } = createFloating('open');
    const hiddenSpy = vi.fn();
    floating.addEventListener('neoui:hidden', hiddenSpy);

    hideFloating(floating);
    vi.runAllTimers();

    expect(hiddenSpy).toHaveBeenCalledTimes(1);
  });

  it('neoui:hidden event does NOT fire when hide RFA is cancelled by show', async () => {
    const { floating } = createFloating('open');
    const hiddenSpy = vi.fn();
    floating.addEventListener('neoui:hidden', hiddenSpy);

    hideFloating(floating);
    const showPromise = showFloating(floating);
    await showPromise;
    vi.runAllTimers();

    expect(hiddenSpy).not.toHaveBeenCalled();
  });
});

// ---------------------------------------------------------------------------
// Issue 2 – applyPosition sets data-state synchronously for first-open animation
// ---------------------------------------------------------------------------

describe('Issue 2 – applyPosition sets data-state synchronously before RFA', () => {
  const position = {
    x: 100,
    y: 200,
    strategy: 'absolute',
    transformOrigin: 'top center',
    middlewareData: {},
  };

  beforeEach(() => {
    vi.useFakeTimers();
  });

  it('data-state="open" is set BEFORE the RFA fires (synchronous)', () => {
    const { floating, content } = createFloating('closed');

    applyPosition(floating, position, true);

    // At this point the RFA has been scheduled but NOT yet executed.
    // data-state must already be "open" so CSS animation classes bind immediately,
    // even if a Blazor re-render makes the element visible before the RFA fires.
    expect(content.getAttribute('data-state')).toBe('open');
  });

  it('visibility is NOT yet "visible" before the RFA fires', () => {
    const { floating } = createFloating('closed');

    applyPosition(floating, position, true);

    // visibility/opacity stay hidden until the RFA runs
    expect(floating.style.getPropertyValue('visibility')).not.toBe('visible');
  });

  it('visibility becomes "visible" after the RFA fires', () => {
    const { floating } = createFloating('closed');

    applyPosition(floating, position, true);
    vi.runAllTimers();

    expect(floating.style.getPropertyValue('visibility')).toBe('visible');
    expect(floating.style.getPropertyValue('opacity')).toBe('1');
    expect(floating.style.getPropertyValue('pointer-events')).toBe('auto');
  });

  it('neoui:visible event fires after the RFA', () => {
    const { floating } = createFloating('closed');
    const visibleSpy = vi.fn();
    floating.addEventListener('neoui:visible', visibleSpy);

    applyPosition(floating, position, true);
    expect(visibleSpy).not.toHaveBeenCalled();

    vi.runAllTimers();
    expect(visibleSpy).toHaveBeenCalledTimes(1);
  });

  it('applyPosition RFA is cancelled by a subsequent hideFloating call', () => {
    // Simulates: applyPosition(makeVisible=true) is called for first open,
    // then hideFloating is called before the RFA fires (e.g., user clicks
    // outside almost instantly). The visibility RFA should bail.
    const { floating, content } = createFloating('closed');
    const visibleSpy = vi.fn();
    floating.addEventListener('neoui:visible', visibleSpy);

    applyPosition(floating, position, true);   // gen → 1, data-state already "open"
    hideFloating(floating);                    // gen → 2, schedules hide RFA

    vi.runAllTimers();

    // applyPosition RFA bailed (gen 1 !== 2); hideFloating RFA ran
    expect(floating.style.getPropertyValue('visibility')).toBe('hidden');
    expect(content.getAttribute('data-state')).toBe('closed');
    expect(visibleSpy).not.toHaveBeenCalled();
  });

  it('applyPosition with makeVisible=false does NOT set data-state or schedule RFA', () => {
    const { floating, content } = createFloating('closed');

    applyPosition(floating, position, false);

    // No data-state change
    expect(content.getAttribute('data-state')).toBe('closed');

    vi.runAllTimers();

    // Still no visibility change
    expect(floating.style.getPropertyValue('visibility')).not.toBe('visible');
  });

  it('position styles are applied synchronously regardless of makeVisible', () => {
    const { floating } = createFloating('closed');

    applyPosition(floating, position, false);

    expect(floating.style.left).toBe('100px');
    expect(floating.style.top).toBe('200px');
    expect(floating.style.position).toBe('absolute');
  });
});
