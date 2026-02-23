// File upload drag-and-drop handler
// Provides enhanced drag-and-drop functionality for file uploads

export function initializeDragDrop(dropZoneId, inputFileId) {
    const dropZone = document.getElementById(dropZoneId);
    const inputFile = document.getElementById(inputFileId);
    
    if (!dropZone || !inputFile) return;

    // Prevent default drag behaviors
    ['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => {
        dropZone.addEventListener(eventName, preventDefaults, false);
        document.body.addEventListener(eventName, preventDefaults, false);
    });

    // Highlight drop zone when item is dragged over it
    ['dragenter', 'dragover'].forEach(eventName => {
        dropZone.addEventListener(eventName, highlight, false);
    });

    ['dragleave', 'drop'].forEach(eventName => {
        dropZone.addEventListener(eventName, unhighlight, false);
    });

    // Handle dropped files
    dropZone.addEventListener('drop', handleDrop, false);

    function preventDefaults(e) {
        e.preventDefault();
        e.stopPropagation();
    }

    function highlight(e) {
        dropZone.classList.add('drag-over');
    }

    function unhighlight(e) {
        dropZone.classList.remove('drag-over');
    }

    function handleDrop(e) {
        const dt = e.dataTransfer;
        const files = dt.files;

        if (files.length > 0) {
            // Trigger the file input change event with the dropped files
            inputFile.files = files;
            const event = new Event('change', { bubbles: true });
            inputFile.dispatchEvent(event);
        }
    }
}

export function cleanup(dropZoneId) {
    const dropZone = document.getElementById(dropZoneId);
    if (!dropZone) return;

    // Remove event listeners
    dropZone.classList.remove('drag-over');
}
