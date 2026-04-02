// Theme management JavaScript helper
window.theme = {
    /**
     * Applies theme classes to the document root element.
     * @param {Object} config - Theme configuration
     * @param {string} config.base    - Base color class (e.g., 'base-zinc')
     * @param {string} config.primary - Primary color class (e.g., 'primary-blue'), or empty
     * @param {string} config.style   - Style variant class (e.g., 'style-nova'), or empty
     * @param {string} config.radius  - Radius preset class (e.g., 'radius-small'), or empty
     * @param {string} config.font    - Font preset class (e.g., 'font-inter'), or empty
     * @param {boolean} config.dark   - Whether dark mode is enabled
     */
    apply: function(config) {
        const html = document.documentElement;
        const classes = html.className.split(' ');

        // Remove existing theme-controlled classes
        const filtered = classes.filter(c =>
            !c.startsWith('base-') &&
            !c.startsWith('primary-') &&
            !c.startsWith('style-') &&
            !c.startsWith('radius-') &&
            !c.startsWith('font-')
        );

        html.className = filtered.join(' ');

        // Add new theme classes
        html.classList.add(config.base);

        if (config.primary) html.classList.add(config.primary);
        if (config.style)   html.classList.add(config.style);
        if (config.radius)  html.classList.add(config.radius);
        if (config.font)    html.classList.add(config.font);

        if (config.dark) {
            html.classList.add('dark');
        } else {
            html.classList.remove('dark');
        }
    },

    /**
     * Checks if dark mode is currently active.
     * @returns {boolean}
     */
    isDark: function() {
        return document.documentElement.classList.contains('dark');
    },

    /**
     * Initializes theme before Blazor loads to prevent FOUC (Flash of Unstyled Content).
     * Reads all 6 persisted localStorage keys and applies them immediately.
     */
    initialize: function() {
        let savedTheme        = null;
        let savedBaseColor    = 'Zinc';
        let savedPrimaryColor = 'Default';
        let savedStyle        = '';
        let savedRadius       = '';
        let savedFont         = '';

        try {
            if (typeof window !== 'undefined' && window.localStorage) {
                savedTheme        = window.localStorage.getItem('theme')        || null;
                savedBaseColor    = window.localStorage.getItem('baseColor')    || 'Zinc';
                savedPrimaryColor = window.localStorage.getItem('primaryColor') || 'Default';
                savedStyle        = window.localStorage.getItem('styleVariant') || '';
                savedRadius       = window.localStorage.getItem('radiusPreset') || '';
                savedFont         = window.localStorage.getItem('fontPreset')   || '';
            }
        } catch (e) {
            // localStorage blocked — fall back to defaults
        }

        const prefersDark = window.matchMedia &&
            window.matchMedia('(prefers-color-scheme: dark)').matches;

        const html = document.documentElement;

        // Whitelists
        const allowedBaseColors = [
            'Zinc', 'Slate', 'Gray', 'Neutral', 'Stone',
            'Luma', 'Mist', 'Mauve', 'Taupe', 'Olive'
        ];
        const allowedPrimaryColors = [
            'Default', 'Red', 'Rose', 'Orange', 'Amber', 'Yellow', 'Lime', 'Green', 'Emerald',
            'Teal', 'Cyan', 'Sky', 'Blue', 'Indigo', 'Violet', 'Purple', 'Fuchsia', 'Pink'
        ];
        const allowedStyles  = ['Default', 'Vega', 'Nova', 'Maia', 'Lyra', 'Mira'];
        const allowedRadius  = ['None', 'Small', 'Medium', 'Large', 'Full'];
        const allowedFonts   = ['System', 'Inter', 'Geist', 'CalSans', 'DmSans', 'PlusJakarta'];

        const validBase    = allowedBaseColors.find(c => c.toLowerCase() === savedBaseColor.toLowerCase())    || 'Zinc';
        const validPrimary = allowedPrimaryColors.find(c => c.toLowerCase() === savedPrimaryColor.toLowerCase()) || 'Default';
        const validStyle   = savedStyle   ? allowedStyles.find(c => c.toLowerCase()  === savedStyle.toLowerCase())   || '' : '';
        const validRadius  = savedRadius  ? allowedRadius.find(c => c.toLowerCase()  === savedRadius.toLowerCase())  || '' : '';
        const validFont    = savedFont    ? allowedFonts.find(c => c.toLowerCase()   === savedFont.toLowerCase())    || '' : '';

        html.classList.add('base-' + validBase.toLowerCase());

        if (validPrimary !== 'Default') html.classList.add('primary-' + validPrimary.toLowerCase());
        if (validStyle && validStyle !== 'Default') html.classList.add('style-'  + validStyle.toLowerCase());
        if (validRadius && validRadius !== 'Medium') html.classList.add('radius-' + validRadius.toLowerCase());
        if (validFont && validFont !== 'System') html.classList.add('font-' + validFont.toLowerCase());

        if (savedTheme === 'dark' || (!savedTheme && prefersDark)) {
            html.classList.add('dark');
        }
    }
};

// Auto-initialize on script load to prevent FOUC.
// No manual call needed in the host page.
window.theme.initialize();

