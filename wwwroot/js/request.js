
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
        body: JSON.stringify({ importo: importo, durata: durata, dataRichiesta: new Date().toISOString() })
    });
    document.getElementById("requestMsg").textContent = res.ok
        ? "Richiesta inviata!"
        : "Errore invio richiesta.";
});

