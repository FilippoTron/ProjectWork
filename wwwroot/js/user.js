const token = localStorage.getItem("token");
if (!token) {
    window.location.href = "login.html";
}

const payload = parseJwt(token);

async function fetchLoans() {
    try {
        const res = await fetch(`${apiUrl}/api/LoanRequest/user/`, {
            headers: {
                Authorization: `Bearer ${token}`
            }
        });

        if (!res.ok) throw new Error("Errore durante il recupero delle richieste");

        const loans = await res.json();
        const tbody = document.getElementById("loanTableBody");
        tbody.innerHTML = "";

        if (loans.length === 0) {
            tbody.innerHTML = `<tr><td colspan="5" class="text-center text-muted">Nessuna richiesta trovata.</td></tr>`;
            return;
        }

        loans.forEach(loan => {
            const row = document.createElement("tr");
            row.innerHTML = `
                <td>${loan.id}</td>
                <td>€ ${loan.importo.toFixed(2)}</td>
                <td>${loan.durata} mesi</td>
                <td>${loan.status}</td>
                <td>${new Date(loan.dataRichiesta).toLocaleDateString()}</td>
            `;
            tbody.appendChild(row);
        });
    } catch (err) {
        document.getElementById("errorMsg").textContent = err.message;
    }
}

function parseJwt(token) {
    try {
        return JSON.parse(atob(token.split('.')[1]));
    } catch (e) {
        return null;
    }
}

document.getElementById("simulateBtn").addEventListener("click", () => {
    window.location.href = "loan.html";
});

document.getElementById("requestBtn").addEventListener("click", () => {
    window.location.href = "request.html";
});

document.getElementById("logoutBtn").addEventListener("click", () => {
    localStorage.removeItem("token");
    window.location.href = "login.html";
});

// Caricamento al primo accesso
fetchLoans();
