using ProjectWork.DTO;
using ProjectWork.Models;

namespace ProjectWork.Services;

public interface ILoanRequestService
{
    Task<bool> SubmitLoanRequestAsync(LoanRequestDto requestDto, int userId);
    Task<LoanRequest> GetLoanRequestByIdAsync(int id);
    Task<IEnumerable<LoanRequest>> GetAllLoanRequestsAsync();
    Task<bool> UpdateLoanRequestStatusAsync(int id, string status);
    double CalcoloTassoInteresse(double importo, int durata);
}
