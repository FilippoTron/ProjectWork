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
            Durata = lr.Durata,
            Status = lr.Status,
            DataRichiesta = lr.DataRichiesta

        });
    }

    public async Task<bool> SubmitLoanRequestAsync(SubmitRequestDto requestDto, int userId)
    {
        if (requestDto.Importo <= 0 || requestDto.Durata <= 0)
            throw new ArgumentException("Importo e durata devono essere maggiori di zero.");

        var tassoInteresse = CalcoloTassoInteresse(requestDto.Importo, requestDto.Durata);
        var loanRequest = new LoanRequest
        {
            UserId = userId,
            Importo = requestDto.Importo,
            TassoInteresse = tassoInteresse,
            Durata = requestDto.Durata,
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

    public double CalcoloTassoInteresse(double importo, int durata)
    {
        if (importo <= 0 || durata <= 0)
            throw new ArgumentException("Importo e durata devono essere maggiori di zero.");
        else if (importo <= 5000 && durata <= 12)
            return 3.5; // 5% interest for small loans
        else if (importo <= 10000 && durata <= 24)
            return 5.0;
        else
            return 6.5;
    }
}
