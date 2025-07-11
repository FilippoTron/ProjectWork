namespace ProjectWork.Models;

public class LoanRequest
{
    public int Id { get; set; }
    public double Importo { get; set; }
    public double TassoInteresse { get; set; }
    public int Durata { get; set; } // Durata in mesi
    public DateTime DataRichiesta { get; set; }
}
