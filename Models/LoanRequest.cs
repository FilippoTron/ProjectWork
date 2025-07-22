namespace ProjectWork.Models;

public class LoanRequest
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public double Importo { get; set; }
    public double TassoInteresse { get; set; }
    public int Durata { get; set; } // Durata in mesi
    public string Status { get; set; } = "In attesa"; // Esempio: "In attesa", "Approvato", "Rifiutato"
    public DateTime DataRichiesta { get; set; } = DateTime.UtcNow;
}
