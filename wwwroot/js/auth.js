const apiUrl = "https://localhost:7194";

document.getElementById("loginForm")?.addEventListener("submit", async e => {
    e.preventDefault();
    const u = document.getElementById("loginUsername").value;
    const p = document.getElementById("loginPassword").value;
    const res = await fetch(`${apiUrl}/api/Auth/login`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ username: u, password: p })
    });
    const msg = document.getElementById("loginMsg");
    if (res.ok) {
        const { token } = await res.json();
        localStorage.setItem("token", token);
        const payload = parseJwt(token);
        if (payload?.role === "Admin") window.location.href = "admin.html";
        else window.location.href = "loan.html";
    } else {
        msg.textContent = "Credenziali non valide.";
    }
});

document.getElementById("registerForm")?.addEventListener("submit", async e => {
    e.preventDefault();
    const u = document.getElementById("registerUsername").value;
    const em = document.getElementById("registerEmail").value;
    const p = document.getElementById("registerPassword").value;
    const res = await fetch(`${apiUrl}/api/Auth/register`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ username: u, email: em, password: p })
    });
    const msg = document.getElementById("registerMsg");
    if (res.ok) {
        msg.textContent = "Registrato! Vai a login...";
        setTimeout(() => window.location.href = "login.html", 1000);
    } else {
        msg.textContent = "Registrazione fallita.";
    }
});
