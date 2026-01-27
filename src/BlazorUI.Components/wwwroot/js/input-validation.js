/**
 * Input validation helper module for native browser validation API
 */

/**
 * Sets a custom validation error on an input and shows the tooltip
 * @param {string} elementId - The ID of the input element
 * @param {string} errorMessage - The error message to display
 */
export function setValidationError(elementId, errorMessage) {
    const input = document.getElementById(elementId);
    if (!input) {
        console.warn(`Input element with ID "${elementId}" not found`);
        return;
    }

    // Set custom validation message
    input.setCustomValidity(errorMessage);

    // Show the validation tooltip and focus the input
    const isValid = input.reportValidity();

    // If reportValidity doesn't auto-focus (some browsers), focus manually
    if (!isValid && document.activeElement !== input) {
        input.focus();
    }
}

/**
 * Sets a custom validation error on an input WITHOUT showing the tooltip or focusing
 * Used for subsequent invalid inputs after the first one
 * @param {string} elementId - The ID of the input element
 * @param {string} errorMessage - The error message to display
 */
export function setValidationErrorSilent(elementId, errorMessage) {
    const input = document.getElementById(elementId);
    if (!input) {
        return;
    }

    // Only set custom validation message, don't show tooltip or focus
    input.setCustomValidity(errorMessage);
}

/**
 * Clears the validation error on an input
 * @param {string} elementId - The ID of the input element
 */
export function clearValidationError(elementId) {
    const input = document.getElementById(elementId);
    if (!input) {
        return;
    }

    // Clear custom validation message
    input.setCustomValidity('');
}

/**
 * Validates an input and shows the tooltip if invalid
 * @param {string} elementId - The ID of the input element
 * @returns {boolean} True if valid, false if invalid
 */
export function validateInput(elementId) {
    const input = document.getElementById(elementId);
    if (!input) {
        return true;
    }

    // Check validity and show tooltip if invalid
    const isValid = input.reportValidity();

    // Focus the input if invalid
    if (!isValid && document.activeElement !== input) {
        input.focus();
    }

    return isValid;
}

/**
 * Checks if an input is valid without showing the tooltip
 * @param {string} elementId - The ID of the input element
 * @returns {boolean} True if valid, false if invalid
 */
export function checkValidity(elementId) {
    const input = document.getElementById(elementId);
    if (!input) {
        return true;
    }

    return input.checkValidity();
}

/**
 * Gets the validation message for an input
 * @param {string} elementId - The ID of the input element
 * @returns {string} The validation message, or empty string if valid
 */
export function getValidationMessage(elementId) {
    const input = document.getElementById(elementId);
    if (!input) {
        return '';
    }

    return input.validationMessage;
}
