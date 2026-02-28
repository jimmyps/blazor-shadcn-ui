// Chart theme management utilities

export function isDarkMode() {
    return document.documentElement.classList.contains('dark');
}

export function getThemeColors() {
    const colors = [];
    for (let i = 1; i <= 5; i++) {
        const varName = `--chart-${i}`;
        const value = getComputedStyle(document.documentElement)
            .getPropertyValue(varName)
            .trim();
        colors.push(value ? `hsl(${value})` : `hsl(var(${varName}))`);
    }
    return colors;
}

export function watchThemeChanges(dotnetHelper, methodName) {
    const observer = new MutationObserver((mutations) => {
        for (const mutation of mutations) {
            if (mutation.type === 'attributes' && mutation.attributeName === 'class') {
                const isDark = isDarkMode();
                dotnetHelper.invokeMethodAsync(methodName, isDark);
                break;
            }
        }
    });
    
    observer.observe(document.documentElement, {
        attributes: true,
        attributeFilter: ['class']
    });
    
    return {
        dispose: () => observer.disconnect()
    };
}
