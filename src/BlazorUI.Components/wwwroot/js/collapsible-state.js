/**
 * Collapsible state management module
 * Handles state persistence in both localStorage and cookies for SSR support
 */

const STORAGE_PREFIX = "blazorui:collapsible:";
const COOKIE_EXPIRATION_DAYS = 365;

/**
 * Get collapsible state from localStorage (with cookie fallback)
 * @param {string} key - The collapsible key (without prefix)
 * @returns {boolean|null} The saved state or null if not found
 */
export function getCollapsibleState(key) {
    const storageKey = STORAGE_PREFIX + key;
    
    // Try localStorage first
    const localValue = localStorage.getItem(storageKey);
    if (localValue !== null) {
        return localValue === 'true';
    }
    
    // Fallback to cookie
    const cookieValue = getCookie(storageKey);
    if (cookieValue === 'true') return true;
    if (cookieValue === 'false') return false;
    
    return null;
}

/**
 * Save collapsible state to both localStorage and cookie
 * @param {string} key - The collapsible key (without prefix)
 * @param {boolean} isOpen - Whether the collapsible is open
 */
export function setCollapsibleState(key, isOpen) {
    const storageKey = STORAGE_PREFIX + key;
    const value = isOpen.toString();
    
    // Save to localStorage
    localStorage.setItem(storageKey, value);
    
    // Save to cookie for SSR
    setCookie(storageKey, value, COOKIE_EXPIRATION_DAYS);
}

/**
 * Clear collapsible state from both localStorage and cookie
 * @param {string} key - The collapsible key (without prefix)
 */
export function clearCollapsibleState(key) {
    const storageKey = STORAGE_PREFIX + key;
    
    // Remove from localStorage
    localStorage.removeItem(storageKey);
    
    // Remove from cookie
    deleteCookie(storageKey);
}

/**
 * Get all localStorage keys matching the prefix
 * @returns {string[]} Array of matching keys
 */
export function getAllCollapsibleKeys() {
    const keys = [];
    for (let i = 0; i < localStorage.length; i++) {
        const key = localStorage.key(i);
        if (key && key.startsWith(STORAGE_PREFIX)) {
            keys.push(key);
        }
    }
    return keys;
}

/**
 * Clear all collapsible states
 */
export function clearAllCollapsibleStates() {
    const keys = getAllCollapsibleKeys();
    keys.forEach(key => {
        localStorage.removeItem(key);
        deleteCookie(key);
    });
}

/**
 * Get cookie value
 * @param {string} name - Cookie name
 * @returns {string|null} Cookie value or null
 */
function getCookie(name) {
    // URL-encode the name to match how it was set
    const encodedName = encodeURIComponent(name);
    const nameEQ = encodedName + "=";
    const ca = document.cookie.split(';');
    for (let i = 0; i < ca.length; i++) {
        let c = ca[i];
        while (c.charAt(0) === ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) === 0) {
            // URL-decode the value when returning
            return decodeURIComponent(c.substring(nameEQ.length, c.length));
        }
    }
    return null;
}

/**
 * Set cookie value
 * @param {string} name - Cookie name
 * @param {string} value - Cookie value
 * @param {number} days - Expiration in days
 */
function setCookie(name, value, days) {
    let expires = "";
    if (days) {
        const date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toUTCString();
    }
    
    // Add Secure flag for HTTPS to ensure cookie is sent to server during pre-rendering
    const secure = window.location.protocol === 'https:' ? '; Secure' : '';
    
    // URL-encode the name and value to handle special characters like colons
    const encodedName = encodeURIComponent(name);
    const encodedValue = encodeURIComponent(value || "");
    
    document.cookie = encodedName + "=" + encodedValue + expires + "; path=/; SameSite=Lax" + secure;
}

/**
 * Delete cookie
 * @param {string} name - Cookie name
 */
function deleteCookie(name) {
    setCookie(name, "", -1);
}
