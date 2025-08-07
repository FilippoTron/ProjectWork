using ProjectWork.DTO;

namespace ProjectWork.Services;

public interface ILoanRequestService
{
    Task<bool> SubmitLoanRequestAsync(SubmitRequestDto requestDto, int userId);
    Task<IEnumerable<LoanRequestDto>> GetLoanRequestByIdAsync(int id);
    Task<IEnumerable<LoanRequestDto>> GetAllLoanRequestsAsync();
    Task<bool> UpdateLoanRequestStatusAsync(int id, string status);
    double CalcoloTassoInteresse(double importo, int durata);
}
