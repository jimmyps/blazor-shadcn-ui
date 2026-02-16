// Theme management JavaScript helper
window.theme = {
    /**
     * Applies theme classes to the document root element
     * @param {Object} config - Theme configuration
     * @param {string} config.base - Base color class (e.g., 'base-zinc')
     * @param {string} config.primary - Primary color class (e.g., 'primary-blue')
     * @param {boolean} config.dark - Whether dark mode is enabled
     */
    apply: function(config) {
        const html = document.documentElement;
        const classes = html.className.split(' ');
        
        // Remove existing base and primary color classes
        const filteredClasses = classes.filter(c => 
            !c.startsWith('base-') && !c.startsWith('primary-')
        );
        
        html.className = filteredClasses.join(' ');
        
        // Add new theme classes
        html.classList.add(config.base);
        html.classList.add(config.primary);
        
        // Apply or remove dark mode
        if (config.dark) {
            html.classList.add('dark');
        } else {
            html.classList.remove('dark');
        }
    }
};
