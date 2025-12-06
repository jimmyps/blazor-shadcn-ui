// Global keyboard shortcuts handler
let dotNetReference = null;

export function initialize(dotNetRef) {
    dotNetReference = dotNetRef;
    document.addEventListener('keydown', handleKeyDown);
}

export function cleanup() {
    document.removeEventListener('keydown', handleKeyDown);
    dotNetReference = null;
}

function handleKeyDown(event) {
    // Check for Cmd+K (Mac) or Ctrl+K (Windows/Linux)
    if ((event.metaKey || event.ctrlKey) && event.key === 'k') {
        event.preventDefault();
        if (dotNetReference) {
            dotNetReference.invokeMethodAsync('OnShortcutPressed', 'k');
        }
    }
}
