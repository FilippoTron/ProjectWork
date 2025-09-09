namespace ProjectWork.Models;

public class Document
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public int LoanRequestId { get; set; }
    public LoanRequest? LoanRequest { get; set; }
}
