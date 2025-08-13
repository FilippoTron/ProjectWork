namespace ProjectWork.DTO;

public class SimulationRequestDto
{
    public double Importo { get; set; } // Importo del prestito
    public int Durata { get; set; } // Durata del prestito in mesi
    public string TipoPrestito { get; set; } // Tipo di prestito (Personale, Veicolo, Abitazione, Altro)
    public DateTime DataRichiesta { get; set; } // Data della richiesta di prestito
}
