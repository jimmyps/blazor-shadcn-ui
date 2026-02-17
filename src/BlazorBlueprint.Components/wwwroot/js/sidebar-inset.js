/**
 * SidebarInset scroll utilities.
 * Scrolls both the element and the window to cover both layout patterns:
 * - Element-level scroll in h-screen layouts (SidebarInset is the scroll container)
 * - Document-level scroll in min-h-screen layouts (window is the scroll container)
 */

/**
 * Scrolls to the top of the given element and the window.
 * @param {HTMLElement} element - The element to scroll to top
 */
export function scrollToTop(element) {
  if (element) {
    element.scrollTo(0, 0);
  }
  window.scrollTo(0, 0);
}
