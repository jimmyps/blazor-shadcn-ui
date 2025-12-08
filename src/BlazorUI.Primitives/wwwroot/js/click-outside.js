// Click outside handler for NavigationMenu
let clickHandler = null;
let element = null;

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
    }, 100);
}

export function cleanup() {
    if (clickHandler) {
        document.removeEventListener('click', clickHandler);
        clickHandler = null;
    }
    element = null;
}
