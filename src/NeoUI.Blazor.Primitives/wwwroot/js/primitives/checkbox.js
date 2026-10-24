// One guard per document, including when asset aliases import this module twice.
// No component references or per-checkbox initialization/disposal are retained.
const key = Symbol.for('NeoUI.CheckboxKeyboard');
if (!document[key]) {
    function preventCheckboxEnter(event) {
        if (event.key === 'Enter' && event.target.closest?.('button[role="checkbox"]')) event.preventDefault();
    }
    document.addEventListener('keydown', preventCheckboxEnter, true);
    document[key] = preventCheckboxEnter;
    document.documentElement.dataset.neoCheckboxKeyboardReady = 'true';
}
