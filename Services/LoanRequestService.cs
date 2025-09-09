using Microsoft.EntityFrameworkCore;
using ProjectWork.Data;
using ProjectWork.DTO;
using ProjectWork.Models;

namespace ProjectWork.Services;

public class LoanRequestService : ILoanRequestService
{
    private readonly AppDbContext _context;

    public LoanRequestService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<LoanRequestDto>> GetAllLoanRequestsAsync()
    {
        var loanRequests = await _context.LoanRequests.
            Include(r => r.User)
            .Include(r => r.Documents)
            .Select(r => new LoanRequestDto
            {
                Id = r.Id,
                UserId = r.UserId,
                User = new UserDto
                {
                    Name = r.User.Name,
                    Surname = r.User.Surname,
                    Email = r.User.Email
                },
                Importo = r.Importo,
                TassoInteresse = r.TassoInteresse,
                TipoPrestito = r.TipoPrestito,
                Durata = r.Durata,
                Status = r.Status,
                Motivazione = r.Motivazione,
                DataRichiesta = r.DataRichiesta,
                Documents = r.Documents.Select(d => new DocumentDTO
                {
                    FilePath = d.FilePath,
                    FileName = d.FileName,
                }).ToList()
            }).ToListAsync();
        return loanRequests;
    }

    public async Task<IEnumerable<LoanRequestDto>> GetLoanRequestByIdAsync(int id)
    {
        var loanRequest = await _context.LoanRequests
            .Include(r => r.User)
            .Include(r => r.Documents)
            .Where(r => r.UserId == id).ToListAsync();
        if (loanRequest == null || !loanRequest.Any())
            throw new KeyNotFoundException($"Loan request with ID {id} not found.");

        return loanRequest.Select(lr => new LoanRequestDto
        {
            Id = lr.Id,
            UserId = lr.UserId,
            User = new UserDto
            {
                Name = lr.User.Name,
                Surname = lr.User.Surname,
                Email = lr.User.Email
            },
            Importo = lr.Importo,
            TassoInteresse = lr.TassoInteresse,
            TipoPrestito = lr.TipoPrestito,
            Durata = lr.Durata,
            Status = lr.Status,
            Motivazione = lr.Motivazione,
            DataRichiesta = lr.DataRichiesta,
            Documents = lr.Documents.Select(d => new DocumentDTO
            {
                FilePath = d.FilePath,
                FileName = d.FileName,
            }).ToList()

        });
    }

    public async Task<bool> SubmitLoanRequestAsync(SubmitRequestDto requestDto, int userId)
    {
        if (requestDto.Importo <= 0 || requestDto.Durata <= 0)
            throw new ArgumentException("Importo e durata devono essere maggiori di zero.");

        if (requestDto.Documents == null || !requestDto.Documents.Any())
            throw new ArgumentException("Devi caricare almeno un documento prima di inviare la richiesta.");

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var loanRequest = new LoanRequest
            {
                UserId = userId,
                Importo = requestDto.Importo,
                Durata = requestDto.Durata,
                TipoPrestito = Enum.TryParse<TipoPrestito>(requestDto.TipoPrestito, true, out var tipoPrestito)
                    ? tipoPrestito
                    : throw new ArgumentException("Tipo di prestito non valido"),
                TassoInteresse = CalcoloTassoInteresse(requestDto.TipoPrestito),
                Status = Status.Pendente,
                DataRichiesta = DateTime.UtcNow,
                Documents = new List<Document>()
            };

            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            // Salvo i documenti allegati
            foreach (var file in requestDto.Documents)
            {
                var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
                var filePath = Path.Combine(uploadPath, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                loanRequest.Documents.Add(new Document
                {
                    FileName = file.FileName,
                    FilePath = $"/uploads/{uniqueFileName}", // percorso relativo per frontend
                    UpdatedAt = DateTime.UtcNow
                });
            }

            _context.LoanRequests.Add(loanRequest);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> UpdateLoanRequestStatusAsync(int id, Status status, string motivazione)
    {
        var loanRequest = await _context.LoanRequests.FirstOrDefaultAsync(lr => lr.Id == id);
        if (loanRequest == null)
        {
            throw new KeyNotFoundException($"Loan request with ID {id} not found.");
        }

        loanRequest.Status = status;
        loanRequest.Motivazione = motivazione;
        _context.LoanRequests.Update(loanRequest);
        await _context.SaveChangesAsync();
        return true;
    }

    public double CalcoloTassoInteresse(string tipoPrestito)
    {
        if (Enum.TryParse<TipoPrestito>(tipoPrestito, true, out var tipo))
        {
            return tipo switch
            {
                TipoPrestito.Personale => 6.5,
                TipoPrestito.Veicolo => 4.2,
                TipoPrestito.Abitazione => 3.5,
                TipoPrestito.Altro => 5.0,
                _ => throw new ArgumentException("Tipo di prestito non valido")
            };
        }
        else
        {
            throw new ArgumentException("Tipo di prestito non valido");
        }
    }

    public async Task UploadDocumentAsync(int loanRequestId, List<IFormFile> files, int userId)
    {
        var loanRequest = await _context.LoanRequests.Include(lr => lr.Documents).FirstOrDefaultAsync(lr => lr.Id == loanRequestId);
        if (loanRequest == null)
        {
            throw new KeyNotFoundException($"Richiesta con id {loanRequest.Id} non trovata");
        }
        if (loanRequest.UserId != userId)
        {
            throw new UnauthorizedAccessException("Non sei autorizzato a caricare documenti per questa richiesta.");
        }
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }
        foreach (var file in files)
        {
            var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var document = new Document
            {
                LoanRequestId = loanRequestId,
                FilePath = $"/uploads/{uniqueFileName}",
                FileName = file.FileName,
                UpdatedAt = DateTime.UtcNow
            };
            _context.Documents.Add(document);
        }
        await _context.SaveChangesAsync();
    }

}
