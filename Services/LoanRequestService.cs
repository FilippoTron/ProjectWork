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
            .Select(r => new LoanRequestDto
            {
                Id = r.Id,
                UserId = r.UserId,
                User = new UserDto
                {
                    Username = r.User.Username,
                    Email = r.User.Email
                },
                Importo = r.Importo,
                TassoInteresse = r.TassoInteresse,
                TipoPrestito = r.TipoPrestito,
                Durata = r.Durata,
                Status = r.Status,
                DataRichiesta = r.DataRichiesta
            }).ToListAsync();
        return loanRequests;
    }

    public async Task<IEnumerable<LoanRequestDto>> GetLoanRequestByIdAsync(int id)
    {
        var loanRequest = await _context.LoanRequests
            .Include(r => r.User)
            .Where(r => r.UserId == id).ToListAsync();
        if (loanRequest == null || !loanRequest.Any())
            throw new KeyNotFoundException($"Loan request with ID {id} not found.");

        return loanRequest.Select(lr => new LoanRequestDto
        {
            Id = lr.Id,
            UserId = lr.UserId,
            User = new UserDto
            {
                Username = lr.User.Username,
                Email = lr.User.Email
            },
            Importo = lr.Importo,
            TassoInteresse = lr.TassoInteresse,
            TipoPrestito = lr.TipoPrestito,
            Durata = lr.Durata,
            Status = lr.Status,
            DataRichiesta = lr.DataRichiesta

        });
    }

    public async Task<bool> SubmitLoanRequestAsync(SubmitRequestDto requestDto, int userId)
    {
        if (requestDto.Importo <= 0 || requestDto.Durata <= 0)
            throw new ArgumentException("Importo e durata devono essere maggiori di zero.");

        var tassoInteresse = CalcoloTassoInteresse(requestDto.TipoPrestito);
        var loanRequest = new LoanRequest
        {
            UserId = userId,
            Importo = requestDto.Importo,
            TassoInteresse = tassoInteresse,
            Durata = requestDto.Durata,
            TipoPrestito = Enum.TryParse<TipoPrestito>(requestDto.TipoPrestito, true, out var tipoPrestito) ? tipoPrestito : throw new ArgumentException("Tipo di prestito non valido"),
            Status = Status.Pendente,
            DataRichiesta = DateTime.UtcNow
        };
        _context.LoanRequests.Add(loanRequest);
        await _context.SaveChangesAsync();
        return true;

    }

    public async Task<bool> UpdateLoanRequestStatusAsync(int id, Status status)
    {
        var loanRequest = await _context.LoanRequests.FirstOrDefaultAsync(lr => lr.Id == id);
        if (loanRequest == null)
        {
            throw new KeyNotFoundException($"Loan request with ID {id} not found.");
        }

        loanRequest.Status = status;
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
}
