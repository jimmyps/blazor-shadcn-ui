// Click outside handler for NavigationMenu
let clickHandler = null;
let element = null;

// Delay before activating click detection to prevent immediate closure
// when opening the menu with a click
const ACTIVATION_DELAY_MS = 100;

export function setupClickOutside(el, dotNetRef) {
    element = el;
    
    clickHandler = (event) => {
        // Check if click is outside the navigation element
        if (element && !element.contains(event.target)) {
            dotNetRef.invokeMethodAsync('OnClickOutside');
        }
    };
    
    // Add listener after a small delay to avoid immediate closure
    setTimeout(() => {
        document.addEventListener('click', clickHandler);
    }, ACTIVATION_DELAY_MS);
}

export function cleanup() {
    if (clickHandler) {
        document.removeEventListener('click', clickHandler);
        clickHandler = null;
    }
    element = null;
}
