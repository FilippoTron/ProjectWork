const form = document.getElementById("loanRequestForm");
let prestitoData = {};

// 🔹 Recupera dati da localStorage se disponibili
const savedSimulation = localStorage.getItem("lastSimulation");
if (savedSimulation) {
    prestitoData = JSON.parse(savedSimulation);

    // Precompila il form
    document.getElementById("tipoPrestito").value = prestitoData.tipoPrestito;
    document.getElementById("importo").value = prestitoData.importo;
    document.getElementById("durata").value = prestitoData.durata;

    // Popola la modale con i dati
    document.getElementById("modalTipo").textContent = prestitoData.tipoPrestito;
    document.getElementById("modalImporto").textContent = prestitoData.importo.toFixed(2);
    document.getElementById("modalDurata").textContent = prestitoData.durata;

    // Mostra direttamente la modale se vuoi
    // const modal = new bootstrap.Modal(document.getElementById("confirmModal"));
    // modal.show();
}

// 🔹 Submit form
form?.addEventListener("submit", e => {
    e.preventDefault();

    prestitoData = {
        importo: parseFloat(document.getElementById("importo").value),
        durata: parseInt(document.getElementById("durata").value),
        tipoPrestito: document.getElementById("tipoPrestito").value,
        dataRichiesta: new Date().toISOString()
    };

    // Aggiorna la modale con i nuovi dati
    document.getElementById("modalTipo").textContent = prestitoData.tipoPrestito;
    document.getElementById("modalImporto").textContent = prestitoData.importo.toFixed(2);
    document.getElementById("modalDurata").textContent = prestitoData.durata;

    const modal = new bootstrap.Modal(document.getElementById("confirmModal"));
    modal.show();
});

// 🔹 Conferma invio
document.getElementById("confirmBtn")?.addEventListener("click", async () => {
    const res = await fetch(`${apiUrl}/api/LoanRequest/submit`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Authorization": "Bearer " + localStorage.getItem("token")
        },
        body: JSON.stringify(prestitoData)
    });

    document.getElementById("requestMsg").textContent = res.ok
        ? "✅ Richiesta inviata con successo!"
        : "❌ Errore durante l'invio della richiesta.";

    // Chiudi la modale
    bootstrap.Modal.getInstance(document.getElementById("confirmModal")).hide();

    if (res.ok) {
        form.reset();
        localStorage.removeItem("lastSimulation"); // pulisce i dati salvati
    }
});
