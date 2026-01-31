/**
 * Storage helper functions for BlazorUI Components
 * Provides safe utilities for working with localStorage
 */

/**
 * Gets all localStorage keys that start with the specified prefix
 * @param {string} prefix - The prefix to filter keys by
 * @returns {string[]} Array of matching keys
 */
export function getLocalStorageKeysByPrefix(prefix) {
    const keys = [];
    for (let i = 0; i < localStorage.length; i++) {
        const key = localStorage.key(i);
        if (key && key.startsWith(prefix)) {
            keys.push(key);
        }
    }
    return keys;
}
