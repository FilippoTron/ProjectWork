const form = document.getElementById("loanRequestForm");
let prestitoData = {};

// Recupera dati da localStorage se disponibili
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
}

// Submit form → mostra modale
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

// Funzione per caricare documenti con gestione errori per file
const documentsInput = document.getElementById("documents");
const fileListDiv = document.getElementById("fileList");

// Array temporaneo per gestire i file selezionati
let selectedFiles = [];

documentsInput.addEventListener("change", () => {
    const newFiles = Array.from(documentsInput.files);

    // Aggiungi solo i file non ancora presenti (evita duplicati)
    newFiles.forEach(file => {
        if (!selectedFiles.some(f => f.name === file.name && f.size === file.size)) {
            selectedFiles.push(file);
        }
    });

    renderFileList();

    // Reset input per permettere di selezionare di nuovo gli stessi file se vuoi
    documentsInput.value = "";
});
function renderFileList() {
    fileListDiv.innerHTML = ""; // resetta lista

    if (selectedFiles.length === 0) {
        fileListDiv.textContent = "Nessun file selezionato.";
        return;
    }

    const ul = document.createElement("ul");
    ul.classList.add("list-group");

    selectedFiles.forEach((file, index) => {
        const li = document.createElement("li");
        li.className = "list-group-item fade-out d-flex justify-content-between align-items-center";

        const fileName = document.createElement("span");
        fileName.textContent = `${file.name} (${(file.size / 1024).toFixed(2)} KB)`;

        const removeBtn = document.createElement("button");
        removeBtn.classList.add("btn", "btn-sm", "btn-danger");
        removeBtn.textContent = "Rimuovi";
        removeBtn.addEventListener("click", () => {
            li.classList.add("removing");
            setTimeout(() => {
                selectedFiles.splice(index, 1);
                renderFileList();
            }, 300);
        });

        li.appendChild(fileName);
        li.appendChild(removeBtn);
        ul.appendChild(li);
    });

    fileListDiv.appendChild(ul);
}

// Quando invii i file, usa selectedFiles invece di documentsInput.files
async function uploadDocuments(loanRequestId, files) {
    if (!files || files.length === 0) throw new Error("Seleziona almeno un documento.");

    const formData = new FormData();
    for (let file of files) {
        formData.append("files", file); // deve corrispondere al parametro List<IFormFile> lato server
    }

    const res = await fetch(`${apiUrl}/api/LoanRequest/uploadDocuments/${loanRequestId}`, {
        method: "POST",
        headers: {
            "Authorization": "Bearer " + localStorage.getItem("token")
        },
        body: formData
    });

    if (!res.ok) {
        let errData = {};
        try { errData = await res.json(); } catch (e) { }
        throw new Error(errData?.message || "Errore durante il caricamento dei documenti.");
    }

    try {
        return await res.json();
    } catch (e) {
        return { message: "Documenti caricati correttamente." };
    }
}


// Conferma invio
document.getElementById("confirmBtn")?.addEventListener("click", async () => {
    const modalElement = document.getElementById("confirmModal");
    const requestMsg = document.getElementById("requestMsg");

    try {

        if (!selectedFiles || selectedFiles.length === 0) {
            throw new Error("Devi caricare almeno un documento.");
        }

        const formData = new FormData();
        formData.append("Importo", prestitoData.importo);
        formData.append("Durata", prestitoData.durata);
        formData.append("TipoPrestito", prestitoData.tipoPrestito);

        selectedFiles.forEach(file => {
            formData.append("Documents", file); // deve corrispondere a SubmitRequestDto.Documents
        });

        // 🔹 Invio direttamente al submit
        const submitRes = await fetch(`${apiUrl}/api/LoanRequest/submit`, {
            method: "POST",
            headers: {
                "Authorization": "Bearer " + localStorage.getItem("token")
                // ❌ NON mettere "Content-Type", FormData lo gestisce da solo
            },
            body: formData
        });

        if (!submitRes.ok) {
            const errData = await submitRes.text();
            throw new Error(errData || "Errore durante l'invio della richiesta.");
        }

        requestMsg.textContent = "✅ Richiesta inviata con successo!";

        // 🔹 Reset se ok
        form.reset();
        selectedFiles = [];
        renderFileList();
        localStorage.removeItem("lastSimulation");

    } catch (err) {
        console.error(err);
        requestMsg.textContent = "❌ " + err.message;
    } finally {
        bootstrap.Modal.getInstance(modalElement)?.hide();
    }
});
