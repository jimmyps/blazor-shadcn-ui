/**
 * datatable.js — NeoUI DataTable JS module
 *
 * Single entry point for all DataTable JavaScript features:
 *   - Column resizing  (initColumnResize)
 *   - Column reordering with smooth shift animation (initColumnReorder)
 *
 * Each feature returns a { dispose } handle that Blazor calls on DisposeAsync.
 */

// ── Column Resizing ──────────────────────────────────────────────────────────

/**
 * Attaches pointer-capture resize listeners to all [data-resize-handle] elements
 * inside containerEl. Column widths are updated live in the DOM during drag;
 * only on pointerup is dotNetRef.OnResizeCompleted called (single Blazor callback).
 *
 * @param {HTMLElement} containerEl - The table scroll container div.
 * @param {object} dotNetRef - DotNetObjectReference for JSInvokable callbacks.
 * @param {number} minWidth - Minimum column width in pixels.
 * @returns {{ dispose: () => void }}
 */
export function initColumnResize(containerEl, dotNetRef, minWidth = 50) {
    const listeners = [];

    function attachHandles() {
        const handles = containerEl.querySelectorAll('[data-resize-handle]');
        handles.forEach(handle => {
            const onPointerDown = (e) => {
                // Only primary button
                if (e.button !== 0) return;
                e.preventDefault();
                e.stopPropagation();

                const colId = handle.dataset.resizeHandle;
                const th = containerEl.querySelector(`th[data-col-id="${colId}"]`);
                const col = containerEl.querySelector(`col[data-col-id="${colId}"]`);
                if (!th) return;

                const startX = e.clientX;
                const startWidth = th.getBoundingClientRect().width;
                handle.setPointerCapture(e.pointerId);

                function onPointerMove(e) {
                    const newWidth = Math.max(minWidth, startWidth + (e.clientX - startX));
                    th.style.width = `${newWidth}px`;
                    if (col) col.style.width = `${newWidth}px`;
                }

                async function onPointerUp(e) {
                    const finalWidth = Math.max(minWidth, startWidth + (e.clientX - startX));
                    th.style.width = `${finalWidth}px`;
                    if (col) col.style.width = `${finalWidth}px`;

                    handle.releasePointerCapture(e.pointerId);
                    handle.removeEventListener('pointermove', onPointerMove);
                    handle.removeEventListener('pointerup', onPointerUp);

                    await dotNetRef.invokeMethodAsync('OnResizeCompleted', colId, finalWidth);
                }

                handle.addEventListener('pointermove', onPointerMove);
                handle.addEventListener('pointerup', onPointerUp);
            };

            handle.addEventListener('pointerdown', onPointerDown);
            listeners.push({ el: handle, type: 'pointerdown', fn: onPointerDown });
        });
    }

    attachHandles();

    return {
        dispose() {
            listeners.forEach(({ el, type, fn }) => el.removeEventListener(type, fn));
            listeners.length = 0;
        }
    };
}

// ── Column Reordering ────────────────────────────────────────────────────────

/**
 * Attaches HTML5 drag-and-drop reorder to header cells listed in reorderableIds.
 *
 * TanStack-style UX:
 *   - On dragover: columns between the dragged column and the target slot smoothly
 *     shift via CSS translateX(), giving live visual feedback.
 *   - A vertical drop-indicator line shows the exact insertion point.
 *   - On drop: columns snap back, dotNetRef.OnColumnReordered is called once.
 *
 * @param {HTMLElement} containerEl
 * @param {object} dotNetRef
 * @param {string[]} reorderableIds - Column IDs eligible for drag.
 * @returns {{ dispose: () => void }}
 */
export function initColumnReorder(containerEl, dotNetRef, reorderableIds) {
    const listeners = [];
    let dragColId = null;
    let indicator = null;

    // ── Indicator line ───────────────────────────────────────────────────────
    function getIndicator() {
        if (!indicator) {
            indicator = document.createElement('div');
            indicator.style.cssText = [
                'position:absolute',
                'top:0',
                'width:2px',
                'height:100%',
                'background:hsl(var(--primary))',
                'pointer-events:none',
                'z-index:50',
                'transition:left 80ms ease',
            ].join(';');
            indicator.id = 'neo-col-drop-indicator';
        }
        return indicator;
    }

    function showIndicator(th, insertBefore) {
        const ind = getIndicator();
        const containerRect = containerEl.getBoundingClientRect();
        const thRect = th.getBoundingClientRect();
        const left = (insertBefore ? thRect.left : thRect.right) - containerRect.left + containerEl.scrollLeft;
        ind.style.left = `${left}px`;
        if (!ind.parentElement) {
            // Position relative to the table container
            containerEl.style.position = 'relative';
            containerEl.appendChild(ind);
        }
    }

    function hideIndicator() {
        indicator?.remove();
        indicator = null;
    }

    // ── Shift animation ──────────────────────────────────────────────────────
    function applyShifts(headers, dragIdx, targetIdx) {
        headers.forEach((th, i) => {
            if (i === dragIdx) {
                th.style.opacity = '0.4';
                th.style.transform = '';
            } else if (dragIdx < targetIdx && i > dragIdx && i <= targetIdx) {
                // Dragging right: shift left
                const dragTh = headers[dragIdx];
                th.style.transform = `translateX(-${dragTh.offsetWidth}px)`;
                th.style.transition = 'transform 150ms ease';
            } else if (dragIdx > targetIdx && i >= targetIdx && i < dragIdx) {
                // Dragging left: shift right
                const dragTh = headers[dragIdx];
                th.style.transform = `translateX(${dragTh.offsetWidth}px)`;
                th.style.transition = 'transform 150ms ease';
            } else {
                th.style.transform = '';
                th.style.transition = 'transform 150ms ease';
            }
        });
    }

    function clearShifts(headers) {
        headers.forEach(th => {
            th.style.transform = '';
            th.style.transition = '';
            th.style.opacity = '';
        });
    }

    // ── Compute target index from cursor X ───────────────────────────────────
    function getTargetInfo(headers, clientX) {
        let targetIdx = headers.length;
        let insertBefore = false;

        for (let i = 0; i < headers.length; i++) {
            const rect = headers[i].getBoundingClientRect();
            const mid = rect.left + rect.width / 2;
            if (clientX < mid) {
                targetIdx = i;
                insertBefore = true;
                break;
            }
        }

        return { targetIdx, insertBefore, th: headers[Math.min(targetIdx, headers.length - 1)] };
    }

    // ── Setup ────────────────────────────────────────────────────────────────
    function getHeaders() {
        return Array.from(containerEl.querySelectorAll('th[data-col-id]'));
    }

    function getReorderableHeaders() {
        return getHeaders().filter(th => reorderableIds.includes(th.dataset.colId));
    }

    reorderableIds.forEach(colId => {
        const th = containerEl.querySelector(`th[data-col-id="${colId}"]`);
        if (!th) return;

        const onDragStart = (e) => {
            dragColId = colId;
            e.dataTransfer.effectAllowed = 'move';
            // Minimal ghost — browser default is fine, just set opacity via applyShifts
            e.dataTransfer.setData('text/plain', colId);
        };

        const onDragOver = (e) => {
            e.preventDefault();
            e.dataTransfer.dropEffect = 'move';
            if (!dragColId) return;

            const headers = getHeaders();
            const dragIdx = headers.findIndex(h => h.dataset.colId === dragColId);
            const { targetIdx, insertBefore, th: targetTh } = getTargetInfo(headers, e.clientX);

            applyShifts(headers, dragIdx, insertBefore ? targetIdx - 1 : targetIdx);
            showIndicator(targetTh, insertBefore);
        };

        const onDragEnd = () => {
            clearShifts(getHeaders());
            hideIndicator();
            dragColId = null;
        };

        const onDrop = async (e) => {
            e.preventDefault();
            if (!dragColId) return;

            const headers = getHeaders();
            const { targetIdx, insertBefore } = getTargetInfo(headers, e.clientX);

            // Compute new index in the full column list (not just visible headers)
            const allHeaders = getHeaders();
            const newIndex = insertBefore ? targetIdx : targetIdx + 1;

            clearShifts(allHeaders);
            hideIndicator();

            const colToMove = dragColId;
            dragColId = null;

            await dotNetRef.invokeMethodAsync('OnColumnReordered', colToMove, newIndex);
        };

        th.addEventListener('dragstart', onDragStart);
        th.addEventListener('dragover', onDragOver);
        th.addEventListener('dragend', onDragEnd);
        th.addEventListener('drop', onDrop);

        listeners.push(
            { el: th, type: 'dragstart', fn: onDragStart },
            { el: th, type: 'dragover', fn: onDragOver },
            { el: th, type: 'dragend', fn: onDragEnd },
            { el: th, type: 'drop', fn: onDrop },
        );
    });

    // dragover on container itself to handle gaps between headers
    const onContainerDragOver = (e) => {
        e.preventDefault();
        e.dataTransfer.dropEffect = 'move';
    };
    containerEl.addEventListener('dragover', onContainerDragOver);
    listeners.push({ el: containerEl, type: 'dragover', fn: onContainerDragOver });

    return {
        dispose() {
            listeners.forEach(({ el, type, fn }) => el.removeEventListener(type, fn));
            listeners.length = 0;
            hideIndicator();
            dragColId = null;
        }
    };
}
