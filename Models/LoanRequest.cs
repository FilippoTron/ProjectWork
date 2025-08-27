namespace ProjectWork.Models;

public class LoanRequest
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public double Importo { get; set; }
    public double TassoInteresse { get; set; }
    public int Durata { get; set; }
    public TipoPrestito TipoPrestito { get; set; }
    public Status Status { get; set; }
    public string? Motivazione { get; set; }
    public DateTime DataRichiesta { get; set; } = DateTime.UtcNow;
}

public enum TipoPrestito
{
    Personale,
    Veicolo,
    Abitazione,
    Altro
}

public enum Status
{
    Pendente,
    Approvata,
    Rifiutata
}