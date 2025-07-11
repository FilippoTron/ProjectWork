namespace ProjectWork.DTO;

public class SimulationRequestDto
{
    public double Importo { get; set; } // Importo del prestito
    public double TassoInteresse { get; set; } // Tasso di interesse annuale in percentuale
    public int Durata { get; set; } // Durata del prestito in mesi
    public DateTime DataRichiesta { get; set; } // Data della richiesta di prestito
}
