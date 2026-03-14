/**
 * Tag Input JavaScript interop module for NeoUI.
 * Handles trigger key preventDefault, paste event reading, and container click-to-focus.
 */

const instances = new Map();

/**
 * Initializes event handling for a tag input component.
 * @param {HTMLElement} containerEl
 * @param {HTMLInputElement} inputEl
 * @param {DotNetObject} dotNetRef
 * @param {string} instanceId
 * @param {{ triggers: number }} config  Bitmask of TagInputTrigger flags.
 */
export function initialize(containerEl, inputEl, dotNetRef, instanceId, config) {
  if (!containerEl || !inputEl || !dotNetRef) return;

  const handleKeyDown = (e) => {
    const key = e.key;
    const isTrigger =
      (key === 'Enter'     && (config.triggers & 1))  ||
      (key === ','         && (config.triggers & 2))  ||
      (key === ' '         && (config.triggers & 4))  ||
      (key === 'Tab'       && (config.triggers & 8))  ||
      (key === ';'         && (config.triggers & 16));

    if (isTrigger) {
      e.preventDefault();
      dotNetRef.invokeMethodAsync('JsTriggerAdd').catch(() => {});
      return;
    }
    if (key === 'Backspace' && inputEl.value === '') {
      dotNetRef.invokeMethodAsync('JsBackspace').catch(() => {});
      return;
    }
    if (key === 'ArrowDown') { e.preventDefault(); dotNetRef.invokeMethodAsync('JsSuggestionNext').catch(() => {}); return; }
    if (key === 'ArrowUp')   { e.preventDefault(); dotNetRef.invokeMethodAsync('JsSuggestionPrev').catch(() => {}); return; }
    if (key === 'Escape')    { dotNetRef.invokeMethodAsync('JsSuggestionClose').catch(() => {}); }
  };

  const handlePaste = (e) => {
    const text = e.clipboardData ? e.clipboardData.getData('text') : '';
    if (!text) return;
    const hasDelimiter =
      ((config.triggers & 2)  && text.includes(',')) ||
      ((config.triggers & 16) && text.includes(';')) ||
      ((config.triggers & 4)  && text.includes(' '));
    if (hasDelimiter) {
      e.preventDefault();
      dotNetRef.invokeMethodAsync('JsPasteText', text).catch(() => {});
    }
  };

  const handleContainerClick = (e) => {
    if (e.target.closest('button')) return;
    inputEl.focus();
  };

  inputEl.addEventListener('keydown', handleKeyDown, true);
  inputEl.addEventListener('paste', handlePaste);
  containerEl.addEventListener('click', handleContainerClick);

  instances.set(instanceId, { handleKeyDown, handlePaste, handleContainerClick, inputEl, containerEl });
}

/** Focuses the input element programmatically. */
export function focusInput(instanceId) {
  const stored = instances.get(instanceId);
  if (stored) stored.inputEl.focus();
}

/** Removes event handlers and cleans up state. */
export function dispose(instanceId) {
  const stored = instances.get(instanceId);
  if (!stored) return;
  stored.inputEl.removeEventListener('keydown', stored.handleKeyDown, true);
  stored.inputEl.removeEventListener('paste', stored.handlePaste);
  stored.containerEl.removeEventListener('click', stored.handleContainerClick);
  instances.delete(instanceId);
}
