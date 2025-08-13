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

function renderTable(data) {
    const table = document.getElementById("requestTable");
    table.innerHTML = "";

    if (data.length === 0) {
        table.innerHTML = `<tr><td colspan="7" class="text-center">Nessuna richiesta trovata.</td></tr>`;
        return;
    }

    data.forEach(r => {
        const username = r.user?.username || "Utente sconosciuto";
        const rataMensile = (r.importo / r.durata).toFixed(2);
        const tr = document.createElement("tr");
        tr.innerHTML = `
      <td>${r.id}</td>
      <td>${username}</td>
      <td>€ ${r.importo}</td>
      <td>${r.tipoPrestito}</td>
      <td>${r.durata}</td>
      <td>€ ${rataMensile}</td>
      <td><span class="badge bg-${statusColors[r.status] || 'secondary'}">
        ${r.status}
    </span></td>
      <td>
        <button class="btn btn-success btn-sm me-1" onclick="updateStatus(${r.id}, 'Approvata')">✔</button>
        <button class="btn btn-danger btn-sm" onclick="updateStatus(${r.id}, 'Rifiutata')">✖</button>
      </td>`;
        table.appendChild(tr);
    });
}

function filterRequests() {
    const status = document.getElementById("statusFilter").value;
    const filtered = status ? allRequests.filter(r => r.status === status) : allRequests;
    renderTable(filtered);
}

async function updateStatus(id, status) {
    console.log("Inviando:", { status });
    try {
        const res = await fetch(`${apiUrl}/api/LoanRequest/${id}/status`, {
            method: "PUT",
            headers: {
                "Content-Type": "application/json",
                "Authorization": "Bearer " + localStorage.getItem("token")
            },
            body: JSON.stringify({ status })
        });

        if (!res.ok) return alert("Errore nell'aggiornamento");

        await fetchRequests(); // ricarica
    } catch (err) {
        alert("Errore connessione.");
    }
}

function logout() {
    localStorage.removeItem("token");
    window.location.href = "login.html";
}

// ==================== GRAFICO ====================

let chartInstance;

function updateChart(data) {
    const count = {
        Approvata: 0,
        Rifiutata: 0,
        "Pendente": 0
    };

    data.forEach(r => {
        count[r.status] = (count[r.status] || 0) + 1;
    });

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
            plugins: {
                legend: { position: "bottom" }
            }
        }
    });
}
