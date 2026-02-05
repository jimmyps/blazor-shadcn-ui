// Input validation tooltip handler
// Displays validation errors in native browser tooltips and manages focus

export function setValidationError(elementId, message) {
    const element = document.getElementById(elementId);
    if (!element) return;

    // Set custom validity for native browser tooltip
    element.setCustomValidity(message);
    
    // Report validity to show the tooltip
    element.reportValidity();
    
    // Focus the element to ensure visibility
    element.focus();
}

export function setValidationErrorSilent(elementId, message) {
    const element = document.getElementById(elementId);
    if (!element) return;

    // Set custom validity without showing tooltip or focusing
    element.setCustomValidity(message);
}

export function clearValidationError(elementId) {
    const element = document.getElementById(elementId);
    if (!element) return;

    // Clear custom validity
    element.setCustomValidity('');
}

// Legacy function name for backwards compatibility
export function showValidationError(elementId, message) {
    setValidationError(elementId, message);
}
