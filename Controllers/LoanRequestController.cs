using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectWork.DTO;
using ProjectWork.Services;

namespace ProjectWork.Controllers;


[ApiController]
[Route("api/[controller]")]
public class LoanRequestController : ControllerBase
{
    private readonly ILoanRequestService loanRequestService;
    public LoanRequestController(ILoanRequestService loanRequestService)
    {
        this.loanRequestService = loanRequestService;
    }
    [Authorize(Roles = "User")]
    [HttpPost("submit")]
    public async Task<IActionResult> SubmitLoanRequest([FromBody] LoanRequestDto requestDto)
    {
        if (requestDto == null || requestDto.Importo <= 0 || requestDto.Durata <= 0)
        {
            return BadRequest("Dati di richiesta non validi.");
        }
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await loanRequestService.SubmitLoanRequestAsync(requestDto, userId);
            return Ok("Richiesta di prestito inviata con successo.");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Errore durante l'invio della richiesta di prestito: {ex.Message}");
        }


    }

    [Authorize(Roles = "Admin")]
    [HttpGet("all")]
    public async Task<IActionResult> GetAllLoanRequests()
    {
        try
        {
            var loanRequests = await loanRequestService.GetAllLoanRequestsAsync();
            return Ok(loanRequests);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Errore durante il recupero delle richieste di prestito: {ex.Message}");
        }
    }
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusDTO statusDto)
    {
        await loanRequestService.UpdateLoanRequestStatusAsync(id, statusDto.Status);
        return Ok("Stato della richiesta di prestito aggiornato con successo.");
    }

}
