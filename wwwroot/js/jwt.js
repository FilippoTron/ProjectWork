function parseJwt(token) {
    try {
        const [, payload] = token.split('.');
        const decoded = atob(payload.replace(/-/g, '+').replace(/_/g, '/'));
        return JSON.parse(decodeURIComponent(decoded.split('').map(c =>
            '%' + ('00' + c.charCodeAt(0).toString(16))).join('')));
    } catch {
        return null;
    }
}
