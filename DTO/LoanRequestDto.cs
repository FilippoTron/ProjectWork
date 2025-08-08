using ProjectWork.Models;

namespace ProjectWork.DTO;

public class LoanRequestDto
{
    public int Id { get; set; } // ID della richiesta di prestito
    public int UserId { get; set; } // ID dell'utente che richiede il prestito
    public UserDto User { get; set; } = null!; // Informazioni sull'utente che richiede il prestito
    public double Importo { get; set; } // Importo del prestito
    public double TassoInteresse { get; set; } // Tasso di interesse applicato al prestito
    public int Durata { get; set; } // Durata del prestito in mesi
    public Status Status { get; set; }
    public DateTime DataRichiesta { get; set; } // Data della richiesta di prestito
}

public class UserDto
{
    public string Username { get; set; } = string.Empty; // Nome utente
    public string Email { get; set; } = string.Empty; // Email dell'utente
}
