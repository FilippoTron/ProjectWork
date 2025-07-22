document.getElementById("loanForm")?.addEventListener("submit", async e => {
    e.preventDefault();
    const importo = parseFloat(document.getElementById("importo").value);
    const tasso = parseFloat(document.getElementById("tasso").value);
    const mesi = parseInt(document.getElementById("mesi").value);
    const res = await fetch(`${apiUrl}/api/LoanSimulation/simulate`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Authorization": "Bearer " + localStorage.getItem("token")
        },
        body: JSON.stringify({ importo, tassoInteresse: tasso, durata: mesi })
    });
    const data = await res.json();
    document.getElementById("loanResult").textContent = res.ok
        ? `Rata Mensile: € ${data.rataMensile.toFixed(2)}`
        : "Errore nella simulazione.";
});
