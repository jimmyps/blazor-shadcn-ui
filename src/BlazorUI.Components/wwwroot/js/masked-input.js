// Masked input cursor position management
// Helps maintain proper cursor position during masking operations

export function getCursorPosition(elementId) {
    const element = document.getElementById(elementId);
    if (!element) return 0;
    
    return element.selectionStart || 0;
}

export function setCursorPosition(elementId, position) {
    const element = document.getElementById(elementId);
    if (!element) return;
    
    // Ensure position is within bounds
    const maxPos = element.value.length;
    const safePos = Math.min(Math.max(0, position), maxPos);
    
    element.setSelectionRange(safePos, safePos);
}
