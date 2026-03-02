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
        
        // Only add primary color class if it's not empty
        if (config.primary && config.primary !== '') {
            html.classList.add(config.primary);
        }
        
        // Apply or remove dark mode
        if (config.dark) {
            html.classList.add('dark');
        } else {
            html.classList.remove('dark');
        }
    },

    /**
     * Checks if dark mode is currently active
     * @returns {boolean} True if the 'dark' class is present on the html element
     */
    isDark: function() {
        return document.documentElement.classList.contains('dark');
    },

    /**
     * Initializes theme before Blazor loads to prevent FOUC (Flash of Unstyled Content)
     */
    initialize: function() {
        // Defaults
        let savedTheme = null;
        let savedBaseColor = 'Zinc';
        let savedPrimaryColor = 'Default';

        // Safely read from localStorage (may throw in some environments)
        try {
            if (typeof window !== 'undefined' && window.localStorage) {
                const themeFromStorage = window.localStorage.getItem('theme');
                const baseFromStorage = window.localStorage.getItem('baseColor');
                const primaryFromStorage = window.localStorage.getItem('primaryColor');

                if (themeFromStorage) {
                    savedTheme = themeFromStorage;
                }
                if (baseFromStorage) {
                    savedBaseColor = baseFromStorage;
                }
                if (primaryFromStorage) {
                    savedPrimaryColor = primaryFromStorage;
                }
            }
        } catch (e) {
            // If access to localStorage is blocked, fall back to defaults
        }

        const prefersDark = window.matchMedia &&
            window.matchMedia('(prefers-color-scheme: dark)').matches;

        const html = document.documentElement;

        // Whitelists of allowed theme names
        const allowedBaseColors = ['Zinc', 'Slate', 'Gray', 'Neutral', 'Stone'];
        const allowedPrimaryColors = [
            'Default', 'Red', 'Rose', 'Orange', 'Amber', 'Yellow', 'Lime', 'Green', 'Emerald',
            'Teal', 'Cyan', 'Sky', 'Blue', 'Indigo', 'Violet', 'Purple', 'Fuchsia', 'Pink'
        ];

        // Normalize and validate base color
        const normalizedBase = savedBaseColor && savedBaseColor.toString();
        const validBaseColor = allowedBaseColors.find(c => c.toLowerCase() === normalizedBase.toLowerCase()) || 'Zinc';

        // Normalize and validate primary color
        const normalizedPrimary = savedPrimaryColor && savedPrimaryColor.toString();
        const validPrimaryColor = allowedPrimaryColors.find(c => c.toLowerCase() === normalizedPrimary.toLowerCase()) || 'Blue';

        // Apply base color class
        html.classList.add('base-' + validBaseColor.toLowerCase());

        // Apply primary color class (only if not Default)
        if (validPrimaryColor !== 'Default') {
            html.classList.add('primary-' + validPrimaryColor.toLowerCase());
        }
        
        // Apply dark mode if set
        if (savedTheme === 'dark' || (!savedTheme && prefersDark)) {
            html.classList.add('dark');
        }
    }
};
