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
            !c.startsWith('font-') &&
            !c.startsWith('menu-accent-') &&
            !c.startsWith('menu-color-')
        );

        html.className = filtered.join(' ');

        // Add new theme classes
        html.classList.add(config.base);

        if (config.primary) html.classList.add(config.primary);
        if (config.style)   html.classList.add(config.style);
        if (config.radius)  html.classList.add(config.radius);
        if (config.font)    html.classList.add(config.font);
        if (config.menuAccent) html.classList.add(config.menuAccent);
        if (config.menuColor)  html.classList.add(config.menuColor);

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
        let savedMenuAccent   = '';
        let savedMenuColor    = '';

        try {
            if (typeof window !== 'undefined' && window.localStorage) {
                savedTheme        = window.localStorage.getItem('theme')        || null;
                savedBaseColor    = window.localStorage.getItem('baseColor')    || 'Zinc';
                savedPrimaryColor = window.localStorage.getItem('primaryColor') || 'Default';
                savedStyle        = window.localStorage.getItem('styleVariant') || '';
                savedRadius       = window.localStorage.getItem('radiusPreset') || '';
                savedFont         = window.localStorage.getItem('fontPreset')   || '';
                savedMenuAccent   = window.localStorage.getItem('menuAccent')   || '';
                savedMenuColor    = window.localStorage.getItem('menuColor')    || '';
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
            'Mist', 'Mauve', 'Taupe', 'Olive'
        ];
        const allowedPrimaryColors = [
            'Default', 'Red', 'Rose', 'Orange', 'Amber', 'Yellow', 'Lime', 'Green', 'Emerald',
            'Teal', 'Cyan', 'Sky', 'Blue', 'Indigo', 'Violet', 'Purple', 'Fuchsia', 'Pink'
        ];
        const allowedStyles  = ['Default', 'Vega', 'Nova', 'Maia', 'Lyra', 'Mira', 'Luma'];
        const allowedRadius  = ['Default', 'None', 'Small', 'Medium', 'Large'];
        const allowedFonts   = ['System', 'Inter', 'Geist', 'CalSans', 'DmSans', 'PlusJakarta'];
        const allowedMenuAccents = ['Subtle', 'Bold'];
        const allowedMenuColors  = ['Default', 'Inverted', 'DefaultTranslucent', 'InvertedTranslucent'];

        /** @param {string[]} list @param {string} val @param {string} fallback */
        function validated(list, val, fallback) {
            return val ? (list.find(c => c.toLowerCase() === val.toLowerCase()) || fallback) : fallback;
        }

        const validBase    = validated(allowedBaseColors,    savedBaseColor,    'Zinc');
        const validPrimary = validated(allowedPrimaryColors, savedPrimaryColor, 'Default');
        const validStyle   = validated(allowedStyles,        savedStyle,        '');
        const validRadius  = validated(allowedRadius,        savedRadius,       '');
        const validFont    = validated(allowedFonts,         savedFont,         '');
        const validMenuAccent = validated(allowedMenuAccents, savedMenuAccent,  '');
        const validMenuColor  = validated(allowedMenuColors,  savedMenuColor,   '');

        html.classList.add('base-' + validBase.toLowerCase());

        if (validPrimary !== 'Default') html.classList.add('primary-' + validPrimary.toLowerCase());
        if (validStyle) html.classList.add('style-' + validStyle.toLowerCase());
        if (validRadius && validRadius !== 'Default') html.classList.add('radius-' + validRadius.toLowerCase());
        if (validFont && validFont !== 'System') html.classList.add('font-' + validFont.toLowerCase());

        const menuColorClassMap = {
            'Default': '',
            'Inverted': 'menu-color-inverted',
            'DefaultTranslucent': 'menu-color-default-translucent',
            'InvertedTranslucent': 'menu-color-inverted-translucent'
        };
        if (validMenuAccent && validMenuAccent !== 'Subtle') html.classList.add('menu-accent-' + validMenuAccent.toLowerCase());
        const menuColorClass = menuColorClassMap[validMenuColor] || '';
        if (menuColorClass) html.classList.add(menuColorClass);

        if (savedTheme === 'dark' || (!savedTheme && prefersDark)) {
            html.classList.add('dark');
        }
    }
};

// Auto-initialize on script load to prevent FOUC.
// No manual call needed in the host page.
window.theme.initialize();

