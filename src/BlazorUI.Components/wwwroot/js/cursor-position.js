// cursor-position.js - Helper for maintaining cursor position in numeric inputs

export function getCursorPosition(element) {
    if (!element) return null;
    return element.selectionStart;
}

export function setCursorPosition(element, position) {
    if (!element) return;
    
    // Use requestAnimationFrame to ensure DOM has updated
    requestAnimationFrame(() => {
        try {
            element.setSelectionRange(position, position);
        } catch (e) {
            // Silently fail if element doesn't support selection
        }
    });
}
