namespace ProjectWork.DTO;

public class LoanRequestDto
{
    public int UserId { get; set; } // ID dell'utente che richiede il prestito
    public double Importo { get; set; } // Importo del prestito
    public double TassoInteresse { get; set; } // Tasso di interesse annuale in percentuale
    public int Durata { get; set; } // Durata del prestito in mesi
    public DateTime DataRichiesta { get; set; } // Data della richiesta di prestito
}
