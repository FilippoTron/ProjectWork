const registerForm = document.getElementById("registerForm");
if (registerForm) {

    registerForm.addEventListener("submit", async e => {
        e.preventDefault();

        const userData = {
            name: document.getElementById("name").value,
            surname: document.getElementById("surname").value,
            indirizzo: document.getElementById("indirizzo").value,
            citta: document.getElementById("citta").value,
            provincia: document.getElementById("provincia").value,
            cap: document.getElementById("cap").value,
            telefono: document.getElementById("telefono").value,
            dataNascita: document.getElementById("dataNascita").value,
            username: document.getElementById("registerUsername").value,
            email: document.getElementById("registerEmail").value,
            password: document.getElementById("registerPassword").value
        };

        const res = await fetch(`${apiUrl}/api/Auth/register`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(userData)
        });

        const msg = document.getElementById("registerMsg");
        if (res.ok) {
            msg.textContent = "Registrato! Vai a login...";
            msg.classList.remove("text-danger");
            msg.classList.add("text-success");
            setTimeout(() => window.location.href = "login.html", 1000);
        } else {
            const error = await res.text();
            msg.textContent = error;
            msg.classList.remove("text-success");
            msg.classList.add("text-danger");
        }
    });
}