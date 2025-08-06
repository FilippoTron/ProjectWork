document.getElementById("loginForm").addEventListener("submit", async e => {
    e.preventDefault();

    const username = document.getElementById("loginUsername").value;
    const password = document.getElementById("loginPassword").value;

    const btn = document.getElementById("loginBtn");
    const spinner = document.getElementById("loginSpinner");
    const btnText = document.getElementById("loginBtnText");
    const msg = document.getElementById("loginMsg");

    // UI feedback
    btn.disabled = true;
    spinner.classList.remove("d-none");
    btnText.textContent = "Accesso...";

    try {
        const res = await fetch(`${apiUrl}/api/auth/login`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ username, password })
        });

        if (res.ok) {
            const data = await res.json();
            localStorage.setItem("token", data.token);
            const payload = parseJwt(data.token);
            if (payload.role === "Admin") {
                window.location.href = "admin.html";
            } else {
                window.location.href = "loan.html";
            }
        } else {
            msg.textContent = "Credenziali non valide.";
        }
    } catch (err) {
        msg.textContent = "Errore durante il login.";
        console.error(err);
    }

    btn.disabled = false;
    spinner.classList.add("d-none");
    btnText.textContent = "Login";
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
