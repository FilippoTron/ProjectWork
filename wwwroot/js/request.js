document.getElementById("loanRequestForm")?.addEventListener("submit", async e => {
    e.preventDefault();
    const importo = parseFloat(document.getElementById("importo").value);
    const durata = parseInt(document.getElementById("durata").value);
    const res = await fetch(`${apiUrl}/api/LoanRequest/submit`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Authorization": "Bearer " + localStorage.getItem("token")
        },
        body: JSON.stringify({ amount: importo, durationMonths: durata })
    });
    document.getElementById("requestMsg").textContent = res.ok
        ? "Richiesta inviata!"
        : "Errore invio richiesta.";
});

// in admin.html
async function fetchRequests() {
    const res = await fetch(`${apiUrl}/api/LoanRequest/all`, {
        headers: { "Authorization": "Bearer " + localStorage.getItem("token") }
    });
    const table = document.getElementById("requestTable");
    table.innerHTML = "";
    if (!res.ok) return;
    const list = await res.json();
    list.forEach(r => {
        const tr = document.createElement("tr");
        tr.innerHTML = `
      <td>${r.id}</td>
      <td>${r.username}</td>
      <td>€ ${r.amount}</td>
      <td>${r.durationMonths}</td>
      <td>€ ${r.monthlyRate}</td>
      <td>${r.status}</td>
      <td>
        <button class="btn btn-success btn-sm" onclick="updateStatus(${r.id}, 'Approvata')">✔</button>
        <button class="btn btn-danger btn-sm" onclick="updateStatus(${r.id}, 'Rifiutata')">✖</button>
      </td>`;
        table.appendChild(tr);
    });
}

async function updateStatus(id, status) {
    await fetch(`${apiUrl}/api/LoanRequest/${id}/status`, {
        method: "PUT",
        headers: {
            "Content-Type": "application/json",
            "Authorization": "Bearer " + localStorage.getItem("token")
        },
        body: JSON.stringify({ status })
    });
    fetchRequests();
}
