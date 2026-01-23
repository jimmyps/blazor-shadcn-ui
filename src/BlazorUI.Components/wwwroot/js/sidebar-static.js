/**
 * Sidebar Static Rendering Support
 * Provides immediate sidebar toggle functionality during SSR
 * Works entirely via DOM manipulation - no Blazor interop needed
 */

(function() {
    'use strict';

    function toggleSidebar() {
        // Find the sidebar element
        const sidebar = document.querySelector('aside[data-sidebar="sidebar"]');
        if (!sidebar) return;

        // Get current state and collapsible setting
        const currentState = sidebar.getAttribute('data-state');
        const collapsible = sidebar.getAttribute('data-collapsible');
        
        // Determine new state based on collapsible setting
        let newState;
        if (currentState === 'expanded') {
            // If collapsible is enabled (not 'none'), collapse to icon view
            // Otherwise, fully close
            // newState = (collapsible && collapsible !== 'none') ? 'collapsed' : 'closed';
            newState = 'collapsed';
        } else {
            // Expand from either collapsed or closed state
            newState = 'expanded';
        }
        
        sidebar.setAttribute('data-state', newState);

        // Update collapsible attribute when collapsed
        if (collapsible && collapsible !== 'none') {
            sidebar.setAttribute('data-collapsible', newState === 'collapsed' ? 'icon' : 'none');
        }
    }

    function setupTriggers() {
        const triggers = document.querySelectorAll('[data-sidebar="trigger"]');
        
        triggers.forEach((trigger) => {
            trigger.addEventListener('click', function(e) {
                e.preventDefault();
                e.stopPropagation();
                toggleSidebar();
            });
        });
    }

    // Initialize when DOM is ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', setupTriggers);
    } else {
        setupTriggers();
    }
})();

