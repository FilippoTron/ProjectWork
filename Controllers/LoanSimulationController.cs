using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectWork.DTO;
using ProjectWork.Services;

namespace ProjectWork.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class LoanSimulationController : ControllerBase
{
    private readonly ILoanRequestService _loanRequestService;
    private readonly LoanCalculator loanCalculator;
    public LoanSimulationController(ILoanRequestService loanRequestService, LoanCalculator loanCalculator)
    {
        _loanRequestService = loanRequestService;
        this.loanCalculator = loanCalculator;
    }
    [HttpPost("simulate")]
    public IActionResult SimulateLoan([FromBody] SimulationRequestDto request)
    {
        if (request == null || request.Importo <= 0 || request.Durata <= 0)
        {
            return BadRequest(new { message = "Dati di richiesta non validi." });
        }
        try
        {
            var calcoloTasso = _loanRequestService.CalcoloTassoInteresse(request.TipoPrestito);
            var rataMensile = loanCalculator.CalcoloRata(request.Importo, calcoloTasso, request.Durata);
            return Ok(new { RataMensile = rataMensile });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Errore durante il calcolo della rata: {ex.Message}" });
        }
    }
}
