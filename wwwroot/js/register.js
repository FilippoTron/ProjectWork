const registerForm = document.getElementById("registerForm");
if (registerForm) {

    registerForm.addEventListener("submit", async e => {
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
}