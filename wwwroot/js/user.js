const token = localStorage.getItem("token");
if (!token) {
    window.location.href = "login.html";
}

const payload = parseJwt(token);

const statusColors = {
    "Approvato": "success",
    "Rifiutato": "danger",
    "Pendente": "warning"
};

async function fetchLoans() {
    const spinner = document.getElementById("loadingSpinner");
    const errorMsg = document.getElementById("errorMsg");
    const tbody = document.getElementById("loanTableBody");

    spinner.classList.remove("d-none");
    errorMsg.innerHTML = "";
    tbody.innerHTML = "";

    try {
        const res = await fetch(`${apiUrl}/api/LoanRequest/user/`, {
            headers: { Authorization: `Bearer ${token}` }
        });

        if (!res.ok) throw new Error("Errore durante il recupero delle richieste.");

        const loans = await res.json();

        loans.forEach(loan => {
            const row = document.createElement("tr");
            const statusClass = statusColors[loan.status] || "secondary";
            const docCell = document.createElement("td");
            if (loan.documents && loan.documents.length > 0) {
                loan.documents.forEach(doc => {
                    const link = document.createElement("a");
                    link.href = doc.filePath;
                    link.target = "_blank";
                    link.textContent = doc.fileName;
                    link.className = "d-block";
                    docCell.appendChild(link);
                });
            } else {
                docCell.textContent = "Nessun documento";
            }

            row.innerHTML = `
                <td>${loan.id}</td>
                <td><strong>€ ${loan.importo.toFixed(2)}</strong></td>
                <td>${loan.durata} mesi</td>
                <td>${loan.tipoPrestito}</td>
                <td>
                    <a href="#" class="badge bg-${statusClass} text-decoration-none stato-link"
                       data-motivazione="${loan.motivazione}">
                       ${loan.status}
                    </a>
                </td>
                <td>
                    <span data-bs-toggle="tooltip" title="${new Date(loan.dataRichiesta).toLocaleString()}">
                        ${new Date(loan.dataRichiesta).toLocaleDateString()}
                    </span>
                </td>
            `;
            row.appendChild(docCell);
            tbody.appendChild(row);
        });

        // Attiva tooltip
        document.querySelectorAll('[data-bs-toggle="tooltip"]').forEach(el => {
            new bootstrap.Tooltip(el);
        });

    } catch (err) {
        errorMsg.innerHTML = `<div class="alert alert-danger">${err.message}</div>`;
    } finally {
        spinner.classList.add("d-none");
    }
}

function parseJwt(token) {
    try {
        return JSON.parse(atob(token.split('.')[1]));
    } catch {
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
    if (confirm("Sei sicuro di voler uscire?")) {
        localStorage.removeItem("token");
        window.location.href = "login.html";
    }
});

document.getElementById("searchInput").addEventListener("input", function () {
    const search = this.value.toLowerCase();
    document.querySelectorAll("#loanTableBody tr").forEach(row => {
        const text = row.textContent.toLowerCase();
        row.style.display = text.includes(search) ? "" : "none";
    });
});

// Mostra motivazione in modale al click sullo stato
document.addEventListener("click", function (e) {
    if (e.target.classList.contains("stato-link")) {
        e.preventDefault();
        const motivazione = e.target.getAttribute("data-motivazione") || "Nessuna motivazione disponibile.";
        document.getElementById("motivazioneContent").innerText = motivazione;
        const motivazioneModal = new bootstrap.Modal(document.getElementById("motivazioneModal"));
        motivazioneModal.show();
    }
});

fetchLoans();
