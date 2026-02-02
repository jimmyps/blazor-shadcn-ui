<<<<<<< HEAD
// Resizable panel drag functionality

const cleanupRegistry = new Map();
let cleanupIdCounter = 0;

/**
 * Initializes drag handling for a resizable handle.
 * @param {HTMLElement} handleElement - The handle element to make draggable
 * @param {HTMLElement} groupElement - The parent panel group element
 * @param {Object} dotNetRef - DotNet object reference for callback
 * @param {number} handleIndex - The index of the handle (between panels)
 * @param {string} direction - 'horizontal' or 'vertical'
 * @returns {Object} Disposable object with dispose() method
 */
export function initResizeHandle(handleElement, groupElement, dotNetRef, handleIndex, direction) {
    if (!handleElement || !groupElement || !dotNetRef) {
        console.warn('resizable: handleElement, groupElement, or dotNetRef is null');
        return {
            _cleanupId: -1,
            dispose: function() {}
        };
    }

    let isDragging = false;
    let startPos = 0;
    let startSizes = [];

    const isHorizontal = direction === 'horizontal';

    const handleMouseDown = (e) => {
        if (e.button !== 0) return; // Only left mouse button
        
        e.preventDefault();
        isDragging = true;
        startPos = isHorizontal ? e.clientX : e.clientY;
        
        // Get current panel sizes from the group
        const panels = groupElement.querySelectorAll('[data-panel]');
        startSizes = Array.from(panels).map(panel => {
            const style = window.getComputedStyle(panel);
            return isHorizontal ? panel.offsetWidth : panel.offsetHeight;
        });

        document.body.style.cursor = isHorizontal ? 'col-resize' : 'row-resize';
        document.body.style.userSelect = 'none';

        document.addEventListener('mousemove', handleMouseMove);
        document.addEventListener('mouseup', handleMouseUp);
    };

    const handleMouseMove = (e) => {
        if (!isDragging) return;

        e.preventDefault();
        
        const currentPos = isHorizontal ? e.clientX : e.clientY;
        const delta = currentPos - startPos;

        // Calculate total size of the group
        const groupRect = groupElement.getBoundingClientRect();
        const totalSize = isHorizontal ? groupRect.width : groupRect.height;

        // Convert delta to percentage
        const deltaPercent = (delta / totalSize) * 100;

        // Update startPos to avoid accumulation
        startPos = currentPos;

        try {
            if (dotNetRef && !dotNetRef._disposed) {
                dotNetRef.invokeMethodAsync('HandleDrag', handleIndex, deltaPercent);
            }
        } catch (error) {
            console.error('resizable drag callback error:', error);
        }
    };

    const handleMouseUp = (e) => {
        if (!isDragging) return;
        
        isDragging = false;
        document.body.style.cursor = '';
        document.body.style.userSelect = '';

        document.removeEventListener('mousemove', handleMouseMove);
        document.removeEventListener('mouseup', handleMouseUp);

        try {
            if (dotNetRef && !dotNetRef._disposed) {
                dotNetRef.invokeMethodAsync('HandleDragEnd', handleIndex);
            }
        } catch (error) {
            console.error('resizable drag end callback error:', error);
        }
    };

    // Touch support
    const handleTouchStart = (e) => {
        if (e.touches.length !== 1) return;
        
        const touch = e.touches[0];
        handleMouseDown({ button: 0, clientX: touch.clientX, clientY: touch.clientY, preventDefault: () => {} });
    };

    const handleTouchMove = (e) => {
        if (!isDragging || e.touches.length !== 1) return;
        e.preventDefault();
        
        const touch = e.touches[0];
        handleMouseMove({ clientX: touch.clientX, clientY: touch.clientY });
    };

    const handleTouchEnd = (e) => {
        handleMouseUp({});
    };

    // Keyboard support for accessibility
    const handleKeyDown = (e) => {
        const step = e.shiftKey ? 10 : 1; // Larger step with Shift key
        let delta = 0;

        if (isHorizontal) {
            if (e.key === 'ArrowLeft') delta = -step;
            else if (e.key === 'ArrowRight') delta = step;
        } else {
            if (e.key === 'ArrowUp') delta = -step;
            else if (e.key === 'ArrowDown') delta = step;
        }

        if (delta !== 0) {
            e.preventDefault();
            try {
                if (dotNetRef && !dotNetRef._disposed) {
                    // Chain the calls to avoid race conditions
                    dotNetRef.invokeMethodAsync('HandleDrag', handleIndex, delta)
                        .then(() => dotNetRef.invokeMethodAsync('HandleDragEnd', handleIndex));
                }
            } catch (error) {
                console.error('resizable keyboard callback error:', error);
            }
        }

        // Home/End to collapse/expand
        if (e.key === 'Home') {
            e.preventDefault();
            try {
                if (dotNetRef && !dotNetRef._disposed) {
                    dotNetRef.invokeMethodAsync('HandleCollapse', handleIndex, 'previous');
                }
            } catch (error) {
                console.error('resizable collapse callback error:', error);
            }
        } else if (e.key === 'End') {
            e.preventDefault();
            try {
                if (dotNetRef && !dotNetRef._disposed) {
                    dotNetRef.invokeMethodAsync('HandleCollapse', handleIndex, 'next');
                }
            } catch (error) {
                console.error('resizable collapse callback error:', error);
            }
        }
    };

    handleElement.addEventListener('mousedown', handleMouseDown);
    handleElement.addEventListener('touchstart', handleTouchStart, { passive: true });
    handleElement.addEventListener('touchmove', handleTouchMove, { passive: false });
    handleElement.addEventListener('touchend', handleTouchEnd);
    handleElement.addEventListener('keydown', handleKeyDown);

    const cleanupFunc = () => {
        handleElement.removeEventListener('mousedown', handleMouseDown);
        handleElement.removeEventListener('touchstart', handleTouchStart);
        handleElement.removeEventListener('touchmove', handleTouchMove);
        handleElement.removeEventListener('touchend', handleTouchEnd);
        handleElement.removeEventListener('keydown', handleKeyDown);
        document.removeEventListener('mousemove', handleMouseMove);
        document.removeEventListener('mouseup', handleMouseUp);
    };

    const id = cleanupIdCounter++;
    cleanupRegistry.set(id, cleanupFunc);

    return {
        _cleanupId: id,
        dispose: function() {
            const cleanup = cleanupRegistry.get(this._cleanupId);
            if (cleanup) {
                cleanup();
                cleanupRegistry.delete(this._cleanupId);
            }
        }
    };
}

/**
 * Gets the bounding rect of an element.
 * @param {HTMLElement} element - The element to measure
 * @returns {Object} The bounding rect with width and height
 */
export function getElementRect(element) {
    if (!element) return { width: 0, height: 0 };
    const rect = element.getBoundingClientRect();
    return { width: rect.width, height: rect.height };
=======
// Resizable panel drag handler
// Handles mouse drag events at document level for smooth resize interaction

let resizableStates = new Map();

/**
 * Initializes resize handling for a panel group
 * @param {HTMLElement} groupElement - The panel group container element
 * @param {DotNetObject} dotNetRef - Reference to the Blazor component
 * @param {string} groupId - Unique identifier for the group
 * @param {boolean} isHorizontal - Whether the layout is horizontal
 */
export function initializeResizable(groupElement, dotNetRef, groupId, isHorizontal) {
    if (!groupElement || !dotNetRef) {
        console.error('initializeResizable: missing required parameters');
        return;
    }

    const state = {
        groupElement,
        dotNetRef,
        isHorizontal,
        isDragging: false,
        activeHandleIndex: -1,
        startPosition: 0,
        startSizes: []
    };

    resizableStates.set(groupId, state);
}

/**
 * Starts a resize operation
 * @param {string} groupId - The group identifier
 * @param {number} handleIndex - Index of the handle being dragged
 * @param {number} clientX - Mouse X position
 * @param {number} clientY - Mouse Y position
 * @param {number[]} currentSizes - Current panel sizes as percentages
 */
export function startResize(groupId, handleIndex, clientX, clientY, currentSizes) {
    const state = resizableStates.get(groupId);
    if (!state) return;

    state.isDragging = true;
    state.activeHandleIndex = handleIndex;
    state.startPosition = state.isHorizontal ? clientX : clientY;
    state.startSizes = [...currentSizes];

    // Prevent text selection while dragging
    document.body.style.userSelect = 'none';
    document.body.style.cursor = state.isHorizontal ? 'col-resize' : 'row-resize';

    // Add document-level listeners
    const handleMouseMove = (e) => onMouseMove(groupId, e);
    const handleMouseUp = () => onMouseUp(groupId);

    state.handleMouseMove = handleMouseMove;
    state.handleMouseUp = handleMouseUp;

    document.addEventListener('mousemove', handleMouseMove);
    document.addEventListener('mouseup', handleMouseUp);
}

function onMouseMove(groupId, e) {
    const state = resizableStates.get(groupId);
    if (!state || !state.isDragging) return;

    e.preventDefault();

    const rect = state.groupElement.getBoundingClientRect();
    const totalSize = state.isHorizontal ? rect.width : rect.height;
    const currentPosition = state.isHorizontal ? e.clientX : e.clientY;

    // Calculate delta as percentage of total size
    const deltaPixels = currentPosition - state.startPosition;
    const deltaPercent = (deltaPixels / totalSize) * 100;

    // Calculate new sizes for the two panels adjacent to the handle
    const panelIndex = state.activeHandleIndex;
    const newSizes = [...state.startSizes];

    // Adjust the panel before and after the handle
    const newSize1 = state.startSizes[panelIndex] + deltaPercent;
    const newSize2 = state.startSizes[panelIndex + 1] - deltaPercent;

    // Apply minimum size constraints (10% minimum)
    const minSize = 10;
    if (newSize1 >= minSize && newSize2 >= minSize) {
        newSizes[panelIndex] = newSize1;
        newSizes[panelIndex + 1] = newSize2;

        // Notify Blazor of the new sizes
        state.dotNetRef.invokeMethodAsync('UpdatePanelSizes', newSizes).catch(err => {
            console.error('Error updating panel sizes:', err);
        });
    }
}

function onMouseUp(groupId) {
    const state = resizableStates.get(groupId);
    if (!state) return;

    state.isDragging = false;
    state.activeHandleIndex = -1;

    document.body.style.userSelect = '';
    document.body.style.cursor = '';

    if (state.handleMouseMove) {
        document.removeEventListener('mousemove', state.handleMouseMove);
    }
    if (state.handleMouseUp) {
        document.removeEventListener('mouseup', state.handleMouseUp);
    }
}

/**
 * Disposes resize handling for a panel group
 * @param {string} groupId - The group identifier
 */
export function disposeResizable(groupId) {
    const state = resizableStates.get(groupId);
    if (state) {
        if (state.handleMouseMove) {
            document.removeEventListener('mousemove', state.handleMouseMove);
        }
        if (state.handleMouseUp) {
            document.removeEventListener('mouseup', state.handleMouseUp);
        }
        resizableStates.delete(groupId);
    }
>>>>>>> 8835bfed9859e4bf8349954ac05f732fe9ffddcf
}
