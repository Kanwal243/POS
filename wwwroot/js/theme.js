function setTheme(theme) {
    document.documentElement.setAttribute('data-theme', theme);
    localStorage.setItem('theme', theme);
}

function getTheme() {
    return localStorage.getItem('theme') || 'light';
}

// Helper to save session state from Blazor to Server via Browser Fetch (Cookies)
async function saveSession(url, data) {
    try {
        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        });
        return response.ok;
    } catch (error) {
        console.error("Session save failed", error);
        return false;
    }
}

// Initialize theme on load
(function() {
    const theme = getTheme();
    document.documentElement.setAttribute('data-theme', theme);
})();
