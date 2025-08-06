const loginForm = document.getElementById("loginForm");
if (loginForm) {

    loginForm.addEventListener("submit", async e => {
        e.preventDefault();

        const username = document.getElementById("loginUsername").value;
        const password = document.getElementById("loginPassword").value;

        const btn = document.getElementById("loginBtn");
        const spinner = document.getElementById("loginSpinner");
        const btnText = document.getElementById("loginBtnText");
        const msg = document.getElementById("loginMsg");

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
                window.location.href = payload.role === "Admin" ? "admin.html" : "loan.html";
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
}
