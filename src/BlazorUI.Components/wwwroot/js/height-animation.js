/**
 * Height Animation JavaScript Module
 * Provides smooth height transitions for dynamic content using ResizeObserver and MutationObserver
 * Can be used with any component that needs height animation (Command, Combobox, etc.)
 */

/**
 * Configuration for height animation
 * @typedef {Object} HeightAnimationConfig
 * @property {string} contentSelector - CSS selector for the content element to observe (e.g., '[role="listbox"]')
 * @property {string} [inputSelector] - Optional CSS selector for a fixed-height header/input element
 * @property {number} [maxHeight] - Optional maximum height in pixels (null = no limit)
 * @property {boolean} [includeInputHeight] - Whether to include input/header height in total (default: true)
 */

class HeightAnimationInstance {
    /**
     * @param {HTMLElement} container - The container element to animate
     * @param {HeightAnimationConfig} config - Animation configuration
     */
    constructor(container, config) {
        this.container = container;
        this.config = config;
        this.resizeObserver = null;
        this.mutationObserver = null;
        this.contentElement = null;
        this.inputElement = null;
        this.maxHeight = config.maxHeight || null;
        this.includeInputHeight = config.includeInputHeight !== false;
    }

    /**
     * Initialize the height animation
     */
    setup() {
        if (!this.container) return false;

        // Clean up any existing observers first
        this.cleanup();

        // Find the content element to observe
        this.contentElement = this.container.querySelector(this.config.contentSelector);
        if (!this.contentElement) {
            console.warn(`HeightAnimation: Content element not found with selector "${this.config.contentSelector}"`);
            return false;
        }

        // Find the optional input/header element
        if (this.config.inputSelector) {
            this.inputElement = this.container.querySelector(this.config.inputSelector);
        }

        // Set initial height
        this.updateHeight();

        // Observe size changes to the content
        this.resizeObserver = new ResizeObserver(() => {
            this.updateHeight();
        });

        // Observe DOM mutations (items added/removed/changed)
        this.mutationObserver = new MutationObserver(() => {
            this.updateHeight();
        });

        // Start observing the content element
        this.resizeObserver.observe(this.contentElement);
        this.mutationObserver.observe(this.contentElement, {
            childList: true,
            subtree: true,
            attributes: true,
            attributeFilter: ['style', 'class', 'hidden']
        });

        return true;
    }

    /**
     * Updates the container height based on content
     */
    updateHeight() {
        if (!this.container || !this.contentElement) return;

        // Get the actual content height
        const contentHeight = this.contentElement.scrollHeight;

        // Get the input element height if applicable
        let inputHeight = 0;
        if (this.includeInputHeight && this.inputElement) {
            inputHeight = this.inputElement.offsetHeight;
        }

        // Calculate max content height if specified
        if (this.maxHeight === null) {
            // Cache the initial max height from the content element's actual height
            this.maxHeight = this.contentElement.offsetHeight;
        }

        // Calculate total height (input + content, capped at max-height)
        const actualContentHeight = Math.min(contentHeight, this.maxHeight);
        const totalHeight = inputHeight + actualContentHeight;

        // Apply the height - CSS transition will handle the animation
        this.container.style.height = `${totalHeight}px`;
    }

    /**
     * Clean up observers and reset state
     */
    cleanup() {
        if (this.resizeObserver) {
            this.resizeObserver.disconnect();
            this.resizeObserver = null;
        }

        if (this.mutationObserver) {
            this.mutationObserver.disconnect();
            this.mutationObserver = null;
        }

        this.contentElement = null;
        this.inputElement = null;
    }
}

// Registry to store active animation instances
const instanceRegistry = new Map();
let instanceIdCounter = 0;

/**
 * Sets up height animation for a container element
 * @param {HTMLElement} container - The container element to animate
 * @param {HeightAnimationConfig} config - Animation configuration
 * @returns {number} Instance ID for cleanup
 */
export function setupHeightAnimation(container, config) {
    if (!container || !config || !config.contentSelector) {
        console.error('HeightAnimation: Invalid parameters', { container, config });
        return -1;
    }

    const instance = new HeightAnimationInstance(container, config);
    const success = instance.setup();

    if (!success) {
        return -1;
    }

    const id = instanceIdCounter++;
    instanceRegistry.set(id, instance);
    return id;
}

/**
 * Cleans up a specific height animation instance
 * @param {number} instanceId - The instance ID returned from setupHeightAnimation
 */
export function cleanupHeightAnimation(instanceId) {
    const instance = instanceRegistry.get(instanceId);
    if (instance) {
        instance.cleanup();
        instanceRegistry.delete(instanceId);
    }
}

/**
 * Cleans up all height animation instances
 */
export function cleanupAll() {
    instanceRegistry.forEach(instance => instance.cleanup());
    instanceRegistry.clear();
}
