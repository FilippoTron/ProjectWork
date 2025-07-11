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
    [HttpPost("simulate")]
    public IActionResult SimulateLoan([FromBody] SimulationRequestDto request)
    {
        if (request == null || request.Importo <= 0 || request.TassoInteresse < 0 || request.Durata <= 0)
        {
            return BadRequest("Dati di richiesta non validi.");
        }
        try
        {
            var loanCalculator = new LoanCalculator();
            var rataMensile = loanCalculator.CalcoloRata(request.Importo, request.TassoInteresse, request.Durata);
            return Ok(new { RataMensile = rataMensile });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Errore durante il calcolo della rata: {ex.Message}");
        }
    }
}
