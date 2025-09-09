using ProjectWork.DTO;
using ProjectWork.Models;

namespace ProjectWork.Services;

public interface ILoanRequestService
{
    Task<bool> SubmitLoanRequestAsync(SubmitRequestDto requestDto, int userId);
    Task<IEnumerable<LoanRequestDto>> GetLoanRequestByIdAsync(int id);
    Task<IEnumerable<LoanRequestDto>> GetAllLoanRequestsAsync();
    Task<bool> UpdateLoanRequestStatusAsync(int id, Status status, string motivazione);
    double CalcoloTassoInteresse(string tipoPrestito);
    Task UploadDocumentAsync(int loanRequestId, List<IFormFile> file, int userId);
}
