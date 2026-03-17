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
export function initColumnResize(containerEl, dotNetRef, minWidth = 80) {
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

                // Compute effective minimum: max(specified minWidth, header content's
                // natural width including the sort icon and th padding).
                // We probe off-screen so the measurement is unaffected by the current
                // column width or any overflow:hidden on the <th>.
                let effectiveMin = minWidth;
                const contentDiv = th.firstElementChild;
                if (contentDiv) {
                    const probe = contentDiv.cloneNode(true);
                    Object.assign(probe.style, {
                        position: 'fixed', top: '-9999px', left: '-9999px',
                        visibility: 'hidden', whiteSpace: 'nowrap', width: 'auto',
                    });
                    document.body.appendChild(probe);
                    const probeW = probe.getBoundingClientRect().width;
                    document.body.removeChild(probe);
                    const cs = getComputedStyle(th);
                    const padding = parseFloat(cs.paddingLeft) + parseFloat(cs.paddingRight);
                    effectiveMin = Math.max(minWidth, Math.ceil(probeW + padding));
                }

                function onPointerMove(e) {
                    const newWidth = Math.max(effectiveMin, startWidth + (e.clientX - startX));
                    th.style.width = `${newWidth}px`;
                    if (col) col.style.width = `${newWidth}px`;
                }

                async function onPointerUp(e) {
                    const finalWidth = Math.max(effectiveMin, startWidth + (e.clientX - startX));
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
 * Pointer-based column reordering — mirrors dnd-kit / TanStack Table behaviour:
 *
 *   - The real column follows the cursor via translateX (no HTML5 ghost image).
 *   - Slot detection uses the closest-centre algorithm (same as dnd-kit
 *     closestCenter + horizontalListSortingStrategy): the "over" target is
 *     whichever column's ORIGINAL centre is nearest to the dragged column's
 *     current visual centre.  No threshold tricks — the dragged column is
 *     physically at the cursor so the midpoint crossing feels completely natural.
 *   - Both <th> and every <td> in a column animate together (full-column shift).
 *   - A 5 px movement threshold prevents accidental drags on sort-header clicks.
 *
 * @param {HTMLElement} containerEl
 * @param {object}      dotNetRef
 * @param {string[]}    reorderableIds  Column IDs eligible for drag.
 * @returns {{ dispose: () => void }}
 */
export function initColumnReorder(containerEl, dotNetRef, reorderableIds) {
    const listeners = [];
    let drag = null;

    // ── Helpers ───────────────────────────────────────────────────────────────

    function getHeaders() {
        return Array.from(containerEl.querySelectorAll('th[data-col-id]'));
    }

    function getColumnCells(colId) {
        return [
            containerEl.querySelector(`th[data-col-id="${colId}"]`),
            ...containerEl.querySelectorAll(`td[data-col-id="${colId}"]`),
        ].filter(Boolean);
    }

    function setCells(colId, styles) {
        getColumnCells(colId).forEach(c => Object.assign(c.style, styles));
    }

    function clearAll() {
        getHeaders().forEach(th => {
            setCells(th.dataset.colId, {
                transform: '', transition: '', zIndex: '', opacity: '', boxShadow: '',
            });
        });
        document.body.style.cursor = '';
        document.body.style.userSelect = '';
    }

    // Reset any lingering styles from a previous drag (e.g. opacity:0 left on
    // the dragged column while Blazor was re-rendering the committed order).
    clearAll();

    // ── DOM commit ───────────────────────────────────────────────────────────
    //
    // Physically relocates the dragged column's <th>, <td>, and <col> nodes to
    // their committed position.  No Blazor re-render is needed — state is synced
    // via the JSInvokable callback which updates _columns without StateHasChanged.
    //
    // Reference cell formula: after removing the dragged column from its original
    // slot, the target element to insert-before is:
    //   headers[targetSlot]            if targetSlot < dragIdx  (moving left)
    //   headers[targetSlot + 1]        if targetSlot > dragIdx  (moving right)
    // which simplifies to: headers[ targetSlot + (targetSlot >= dragIdx ? 1 : 0) ]
    function commitDomReorder(dragIdx, targetSlot, headers) {
        const movedColId = headers[dragIdx].dataset.colId;
        const refIdx     = targetSlot + (targetSlot >= dragIdx ? 1 : 0);
        const refColId   = refIdx < headers.length ? headers[refIdx].dataset.colId : null;

        // Move every row's cell for this column (thead + tbody)
        containerEl.querySelectorAll('tr').forEach(row => {
            const dragCell = row.querySelector(`[data-col-id="${movedColId}"]`);
            if (!dragCell) return;
            const refCell = refColId ? row.querySelector(`[data-col-id="${refColId}"]`) : null;
            if (refCell) row.insertBefore(dragCell, refCell);
            else         row.appendChild(dragCell);
        });

        // Move the matching <col> element so column widths stay correct
        const colGroup = containerEl.querySelector('colgroup');
        if (colGroup) {
            const dragCol = colGroup.querySelector(`col[data-col-id="${movedColId}"]`);
            if (dragCol) {
                const refCol = refColId ? colGroup.querySelector(`col[data-col-id="${refColId}"]`) : null;
                if (refCol) colGroup.insertBefore(dragCol, refCol);
                else        colGroup.appendChild(dragCol);
            }
        }
    }


    //
    // Returns the index of the column (in the original headers array) whose
    // original centre is closest to dragVisualCenterX.  The dragged column
    // itself is included in the search, so the slot only changes when another
    // column becomes *strictly* closer — i.e. the threshold fires at the
    // midpoint between the dragged column and each neighbour.
    //
    // This matches TanStack Table's closestCenter collision detection and maps
    // directly to the C# array operation:
    //   _columns.Remove(col); _columns.Insert(targetSlot, col);
    function findTargetSlot(dragVisualCenterX) {
        const { dragIdx, originalCenters } = drag;
        let closestIdx = dragIdx;
        let closestDist = Math.abs(originalCenters[dragIdx] - dragVisualCenterX);
        for (let i = 0; i < originalCenters.length; i++) {
            if (i === dragIdx) continue;
            const dist = Math.abs(originalCenters[i] - dragVisualCenterX);
            if (dist < closestDist) {
                closestDist = dist;
                closestIdx = i;
            }
        }
        return closestIdx;
    }

    // ── Shift transforms ──────────────────────────────────────────────────────
    //
    // Columns strictly between dragIdx and targetSlot shift by ±dragWidth.
    // Columns outside that range snap back to translateX(0).
    function applyShifts(targetSlot) {
        const { dragIdx, headers, dragWidth } = drag;
        headers.forEach((th, i) => {
            if (i === dragIdx) return;
            let shift = 0;
            if (dragIdx < targetSlot && i > dragIdx && i <= targetSlot) shift = -dragWidth;
            else if (dragIdx > targetSlot && i >= targetSlot && i < dragIdx) shift = dragWidth;
            setCells(th.dataset.colId, {
                transform:  shift ? `translateX(${shift}px)` : '',
                transition: 'transform 200ms ease',
            });
        });
    }

    // ── Per-column setup ──────────────────────────────────────────────────────

    reorderableIds.forEach(colId => {
        const th = containerEl.querySelector(`th[data-col-id="${colId}"]`);
        if (!th) return;

        // Suppress the HTML5 ghost image that fires when draggable="true" is set
        const onDragStart = (e) => e.preventDefault();
        th.addEventListener('dragstart', onDragStart);
        listeners.push({ el: th, type: 'dragstart', fn: onDragStart });

        // Grab cursor on reorderable headers
        th.style.cursor     = 'grab';
        th.style.touchAction = 'none';   // prevent scroll-vs-drag conflict on touch

        const onPointerDown = (e) => {
            if (e.button !== 0 || drag) return;

            // Always read colId from the DOM — Blazor may reuse this th node
            // for a different column after a previous reorder without re-init.
            const currentColId = th.dataset.colId;
            const headers   = getHeaders();
            const dragIdx   = headers.findIndex(h => h.dataset.colId === currentColId);
            if (dragIdx === -1) return;

            const thRect = th.getBoundingClientRect();
            // Snapshot original centres before any transforms are applied
            const originalCenters = headers.map(h => {
                const r = h.getBoundingClientRect();
                return r.left + r.width * 0.5;
            });

            // Initial targetSlot = dragIdx (count of columns to the left at rest)
            drag = {
                colId: currentColId, dragIdx, targetSlot: dragIdx,
                startX: e.clientX,
                dragWidth: thRect.width,
                headers, originalCenters,
                started: false,
            };

            // ── Document-level listeners track the pointer across the page ──

            const onMove = (e) => {
                if (!drag) return;
                const dx = e.clientX - drag.startX;

                // 5 px threshold: ignore tiny movements so sort-clicks still work
                if (!drag.started) {
                    if (Math.abs(dx) < 5) return;
                    drag.started = true;
                    document.body.style.cursor     = 'grabbing';
                    document.body.style.userSelect = 'none';
                    // Lift the dragged column
                    setCells(drag.colId, {
                        zIndex:     '20',
                        opacity:    '0.75',
                        boxShadow:  '0 4px 20px rgba(0,0,0,0.18)',
                        transition: 'opacity 120ms ease, box-shadow 120ms ease',
                    });
                }

                // Dragged column follows cursor exactly — no easing
                getColumnCells(drag.colId).forEach(c => c.style.transform = `translateX(${dx}px)`);

                // Count-to-left slot detection: slot only changes when dragged
                // column's visual centre crosses another column's original centre
                const visualCenterX = drag.originalCenters[drag.dragIdx] + dx;
                const newSlot = findTargetSlot(visualCenterX);
                if (newSlot !== drag.targetSlot) {
                    drag.targetSlot = newSlot;
                    applyShifts(newSlot);
                }
            };

            const onUp = async () => {
                if (!drag) return;
                const { started, colId: movedId, dragIdx, targetSlot, headers } = drag;
                cleanup();
                drag = null;
                document.body.style.cursor     = '';
                document.body.style.userSelect = '';

                if (!started) return;   // plain click — do nothing

                // After a real drag the browser still synthesises a click event on the
                // released element (e.g. the sort icon).  Intercept it in the capture
                // phase — before any element handler runs — and discard it, then
                // immediately remove the listener so only this one click is eaten.
                const eatClick = (ev) => {
                    ev.stopPropagation();
                    document.removeEventListener('click', eatClick, true);
                };
                document.addEventListener('click', eatClick, true);

                if (dragIdx !== targetSlot) {
                    // Snap non-dragged columns back instantly (clear shift transforms)
                    headers.forEach((h, i) => {
                        if (i !== dragIdx)
                            setCells(h.dataset.colId, { transform: '', transition: 'none', zIndex: '', opacity: '', boxShadow: '' });
                    });

                    // Physically insert the dragged column at its committed slot — the
                    // column is still at its dragged transform offset, but now in the
                    // correct DOM position.  Clearing transform below lands it perfectly.
                    commitDomReorder(dragIdx, targetSlot, headers);

                    // Reset dragged column: clear transform now that DOM is correct
                    setCells(movedId, { transform: '', transition: 'none', zIndex: '', opacity: '', boxShadow: '' });

                    // Sync C# state — no StateHasChanged, DOM is already committed
                    await dotNetRef.invokeMethodAsync('OnColumnReordered', movedId, targetSlot);
                } else {
                    // No movement — just clear drag styles
                    headers.forEach(h =>
                        setCells(h.dataset.colId, { transform: '', transition: 'none', zIndex: '', opacity: '', boxShadow: '' })
                    );
                }
            };

            function cleanup() {
                document.removeEventListener('pointermove',   onMove);
                document.removeEventListener('pointerup',     onUp);
                document.removeEventListener('pointercancel', onUp);
            }

            document.addEventListener('pointermove',   onMove);
            document.addEventListener('pointerup',     onUp);
            document.addEventListener('pointercancel', onUp);
        };

        th.addEventListener('pointerdown', onPointerDown);
        listeners.push({ el: th, type: 'pointerdown', fn: onPointerDown });
    });

    return {
        dispose() {
            listeners.forEach(({ el, type, fn }) => el.removeEventListener(type, fn));
            reorderableIds.forEach(colId => {
                const th = containerEl.querySelector(`th[data-col-id="${colId}"]`);
                if (th) { th.style.cursor = ''; th.style.touchAction = ''; }
            });
            listeners.length = 0;
            clearAll();
            drag = null;
        },
    };
}
