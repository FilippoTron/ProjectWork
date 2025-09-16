let allRequests = [];

const statusColors = {
    "Pendente": "warning",
    "Approvata": "success",
    "Rifiutata": "danger"
};

document.addEventListener("DOMContentLoaded", () => {
    const token = localStorage.getItem("token");
    if (!token) return window.location.href = "login.html";

    const payload = JSON.parse(atob(token.split('.')[1]));
    if (payload.role !== "Admin") {
        document.getElementById("adminMsg").textContent = "Accesso non autorizzato.";
        return;
    }

    fetchRequests();
});

async function fetchRequests() {
    try {
        const res = await fetch(`${apiUrl}/api/LoanRequest/all`, {
            headers: { "Authorization": "Bearer " + localStorage.getItem("token") }
        });

        if (!res.ok) throw new Error("Errore durante il recupero");

        allRequests = await res.json();
        renderTable(allRequests);
        updateChart(allRequests);
    } catch (err) {
        document.getElementById("adminMsg").textContent = "Errore nel caricamento delle richieste.";
    }
}
function calcolaRataMensile(importo, durataMesi, tassoAnnuale) {
    const i = (tassoAnnuale / 100) / 12; // tasso mensile
    if (i === 0) return (importo / durataMesi).toFixed(2); // caso interesse 0%

    const rata = importo * (i / (1 - Math.pow(1 + i, -durataMesi)));
    return rata.toFixed(2);
}

function renderTable(data) {
    const table = document.getElementById("requestTable");
    table.innerHTML = "";

    if (data.length === 0) {
        table.innerHTML = `<tr><td colspan="8" class="text-center">Nessuna richiesta trovata.</td></tr>`;
        return;
    }

    data.forEach(r => {
        const name = r.user?.name || "Utente sconosciuto";
        const surname = r.user?.surname || "";
        const rataMensile = calcolaRataMensile(r.importo, r.durata, r.tassoInteresse) || "N/D";
        const documentsLinks = r.documents && r.documents.length
            ? r.documents.map(d => `<a href="${d.filePath}" target="_blank">${d.fileName}</a>`).join("<br>")
            : "Nessun documento";
        const tr = document.createElement("tr");
        tr.innerHTML = `
      <td>${r.id}</td>
      <td>${name} ${surname}</td>
      <td>€ ${r.importo}</td>
      <td>${r.tipoPrestito}</td>
      <td>${r.durata}</td>
      <td>€ ${rataMensile}</td>
      <td><span class="badge bg-${statusColors[r.status] || 'secondary'}">${r.status}</span></td>
      <td>${documentsLinks}</td>
      <td>
        <button class="btn btn-success btn-sm me-1" onclick="askMotivation(${r.id}, 'Approvata')">✔</button>
        <button class="btn btn-danger btn-sm" onclick="askMotivation(${r.id}, 'Rifiutata')">✖</button>
      </td>`;
        table.appendChild(tr);
    });
}

function filterRequests() {
    const status = document.getElementById("statusFilter").value;
    const filtered = status ? allRequests.filter(r => r.status === status) : allRequests;
    renderTable(filtered);
}

// ====== MODAL E MOTIVAZIONE ======
let selectedRequestId = null;
let selectedStatus = null;
const motivationModal = new bootstrap.Modal(document.getElementById("motivationModal"));

function askMotivation(id, status) {
    selectedRequestId = id;
    selectedStatus = status;
    document.getElementById("motivationText").value = "";
    motivationModal.show();
}

document.getElementById("confirmMotivationBtn").addEventListener("click", async () => {
    const motivation = document.getElementById("motivationText").value.trim();
    if (!motivation) {
        alert("Inserisci una motivazione.");
        return;
    }

    try {
        const res = await fetch(`${apiUrl}/api/LoanRequest/${selectedRequestId}/status`, {
            method: "PUT",
            headers: {
                "Content-Type": "application/json",
                "Authorization": "Bearer " + localStorage.getItem("token")
            },
            body: JSON.stringify({ status: selectedStatus, Motivazione : motivation })
        });

        if (!res.ok) {
            alert("Errore nell'aggiornamento");
            return;
        }

        motivationModal.hide();
        await fetchRequests();
    } catch (err) {
        alert("Errore connessione.");
    }
});

// ====== LOGOUT ======
function logout() {
    localStorage.removeItem("token");
    window.location.href = "login.html";
}

// ====== GRAFICO ======
let chartInstance;
function updateChart(data) {
    const count = { Approvata: 0, Rifiutata: 0, "Pendente": 0 };
    data.forEach(r => { count[r.status] = (count[r.status] || 0) + 1; });

    const ctx = document.getElementById("statusChart").getContext("2d");
    if (chartInstance) chartInstance.destroy();

    chartInstance = new Chart(ctx, {
        type: "pie",
        data: {
            labels: ["Approvate", "Rifiutate", "Pendente"],
            datasets: [{
                data: [count.Approvata, count.Rifiutata, count["Pendente"]],
                backgroundColor: ["#198754", "#dc3545", "#ffc107"]
            }]
        },
        options: {
            plugins: { legend: { position: "bottom" } }
        }
    });
}
