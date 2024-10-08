window.themeManager = {
    setTheme: function (theme) {
        document.body.setAttribute('data-theme', theme);
        localStorage.setItem('theme', theme);
    },
    getPreferredTheme: function () {
        const storedTheme = localStorage.getItem('theme');
        if (storedTheme) {
            return storedTheme;
        }
        const prefersDarkScheme = window.matchMedia("(prefers-color-scheme: dark)").matches;
        return prefersDarkScheme ? 'dark' : 'light';
    },
    applyPreferredTheme: function () {
        const theme = window.themeManager.getPreferredTheme();
        document.body.setAttribute('data-theme', theme);
    }
};

document.addEventListener('DOMContentLoaded', (event) => {
    window.themeManager.applyPreferredTheme();
});
