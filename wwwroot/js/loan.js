const form = document.getElementById("loanForm");
let prestitoData = {};

form?.addEventListener("submit", async e => {
    e.preventDefault();

    const importo = parseFloat(document.getElementById("importo").value);
    const mesi = parseInt(document.getElementById("mesi").value);
    const tipoPrestito = document.getElementById("tipoPrestito").value;

    prestitoData = { importo, durata: mesi, tipoPrestito };

    const res = await fetch(`${apiUrl}/api/LoanSimulation/simulate`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Authorization": "Bearer " + localStorage.getItem("token")
        },
        body: JSON.stringify(prestitoData)
    });

    if (res.ok) {
        const data = await res.json();

        // Popola la modale
        document.getElementById("modalTipo").textContent = tipoPrestito;
        document.getElementById("modalImporto").textContent = importo.toFixed(2);
        document.getElementById("modalDurata").textContent = mesi;
        document.getElementById("modalRata").textContent = data.rataMensile.toFixed(2);

        // Mostra modale
        const modal = new bootstrap.Modal(document.getElementById("resultModal"));
        modal.show();
    } else {
        alert("Errore nella simulazione, riprova.");
    }
});

// 🔹 Pulsante "Invia la richiesta"
document.getElementById("goRequest")?.addEventListener("click", () => {
    // Puoi passare i dati con localStorage per precompilare la pagina request.html
    localStorage.setItem("lastSimulation", JSON.stringify(prestitoData));
    window.location.href = "request.html";
});
