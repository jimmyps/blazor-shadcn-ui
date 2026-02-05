// Color Picker Canvas - Interactive 2D saturation/lightness selector
// Provides click and drag functionality for color selection

export function initializeColorCanvas(canvasId, hue, saturation, lightness, dotNetHelper) {
const canvas = document.getElementById(canvasId);
if (!canvas) return;

const ctx = canvas.getContext('2d');
if (!ctx) return;

    // Set canvas size - use parent dimensions if canvas has no size
    const rect = canvas.getBoundingClientRect();
    if (rect.width === 0 || rect.height === 0) {
        const parent = canvas.parentElement;
        if (parent) {
            const parentRect = parent.getBoundingClientRect();
            canvas.width = parentRect.width || 288; // Default width for w-72
            canvas.height = parentRect.height || 128; // Default height for h-32 (compact)
        }
    } else {
        canvas.width = rect.width;
        canvas.height = rect.height;
    }

    if (canvas.width === 0 || canvas.height === 0) return;

let isDragging = false;

    // Draw the saturation/lightness gradient
    function drawGradient() {
        // Draw hue background
        ctx.fillStyle = `hsl(${hue}, 100%, 50%)`;
        ctx.fillRect(0, 0, canvas.width, canvas.height);

        // Create white to transparent horizontal gradient (saturation)
        const whiteGradient = ctx.createLinearGradient(0, 0, canvas.width, 0);
        whiteGradient.addColorStop(0, 'rgba(255, 255, 255, 1)');
        whiteGradient.addColorStop(1, 'rgba(255, 255, 255, 0)');
        ctx.fillStyle = whiteGradient;
        ctx.fillRect(0, 0, canvas.width, canvas.height);

        // Create transparent to black vertical gradient (lightness)
        const blackGradient = ctx.createLinearGradient(0, 0, 0, canvas.height);
        blackGradient.addColorStop(0, 'rgba(0, 0, 0, 0)');
        blackGradient.addColorStop(1, 'rgba(0, 0, 0, 1)');
        ctx.fillStyle = blackGradient;
        ctx.fillRect(0, 0, canvas.width, canvas.height);

        // Draw cursor
        const x = (saturation / 100) * canvas.width;
        const y = ((100 - lightness) / 100) * canvas.height;
        
        ctx.strokeStyle = 'white';
        ctx.fillStyle = 'transparent';
        ctx.lineWidth = 2;
        ctx.beginPath();
        ctx.arc(x, y, 8, 0, 2 * Math.PI);
        ctx.stroke();
        
        ctx.strokeStyle = 'rgba(0, 0, 0, 0.5)';
        ctx.lineWidth = 1;
        ctx.beginPath();
        ctx.arc(x, y, 9, 0, 2 * Math.PI);
        ctx.stroke();
    }

    // Get color at position
    function getColorAtPosition(clientX, clientY) {
        const rect = canvas.getBoundingClientRect();
        const x = Math.max(0, Math.min(clientX - rect.left, canvas.width));
        const y = Math.max(0, Math.min(clientY - rect.top, canvas.height));

        const s = (x / canvas.width) * 100;
        const l = 100 - (y / canvas.height) * 100;

        return { saturation: s, lightness: l };
    }

    // Handle color selection
    function handleColorSelect(e) {
        e.preventDefault();
        const { saturation: s, lightness: l } = getColorAtPosition(e.clientX, e.clientY);
        
        // Update local variables immediately for visual feedback
        saturation = s;
        lightness = l;
        
        // Redraw gradient with new cursor position
        drawGradient();
        
        // Notify Blazor of the change
        dotNetHelper.invokeMethodAsync('OnCanvasColorChanged', hue, s, l);
    }

    // Mouse events
    canvas.addEventListener('mousedown', (e) => {
        isDragging = true;
        handleColorSelect(e);
    });

    canvas.addEventListener('mousemove', (e) => {
        if (isDragging) {
            handleColorSelect(e);
        }
    });

    canvas.addEventListener('mouseup', () => {
        isDragging = false;
    });

    canvas.addEventListener('mouseleave', () => {
        isDragging = false;
    });

    // Touch events for mobile
    canvas.addEventListener('touchstart', (e) => {
        e.preventDefault();
        isDragging = true;
        const touch = e.touches[0];
        handleColorSelect({ clientX: touch.clientX, clientY: touch.clientY, preventDefault: () => {} });
    });

    canvas.addEventListener('touchmove', (e) => {
        e.preventDefault();
        if (isDragging) {
            const touch = e.touches[0];
            handleColorSelect({ clientX: touch.clientX, clientY: touch.clientY, preventDefault: () => {} });
        }
    });

    canvas.addEventListener('touchend', () => {
        isDragging = false;
    });

    // Initial draw
    drawGradient();

    // Store update function
    canvas._updateGradient = (newHue, newSat, newLight) => {
        hue = newHue;
        saturation = newSat;
        lightness = newLight;
        drawGradient();
    };
}

export function updateColorCanvas(canvasId, hue, saturation, lightness) {
    const canvas = document.getElementById(canvasId);
    if (canvas && canvas._updateGradient) {
        canvas._updateGradient(hue, saturation, lightness);
    }
}

export function disposeColorCanvas(canvasId) {
    const canvas = document.getElementById(canvasId);
    if (canvas) {
        delete canvas._updateGradient;
    }
}
