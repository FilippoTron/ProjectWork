const isLocalhost = window.location.hostname === "localhost";
const apiUrl = isLocalhost
    ? "https://localhost:7194"
    : window.location.origin;