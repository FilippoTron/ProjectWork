using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectWork.Data;
using ProjectWork.DTO;
using ProjectWork.Models;

namespace ProjectWork.Controllers;

[Authorize(Roles = "User")]
[ApiController]
[Route("api/[controller]")]
public class LoanRequestController : ControllerBase
{
    private readonly AppDbContext _context;
    public LoanRequestController(AppDbContext context)
    {
        _context = context;
    }
    [HttpPost("submit")]
    public async Task<IActionResult> SubmitLoanRequest([FromBody] LoanRequestDto requestDto)
    {
        if (requestDto == null || requestDto.Importo <= 0 || requestDto.TassoInteresse < 0 || requestDto.Durata <= 0)
        {
            return BadRequest("Dati di richiesta non validi.");
        }
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var loanRequest = new LoanRequest
        {
            UserId = userId,
            Importo = requestDto.Importo,
            TassoInteresse = requestDto.TassoInteresse,
            Durata = requestDto.Durata,
            Status = "In attesa",
            DataRichiesta = DateTime.UtcNow
        };
        _context.LoanRequests.Add(loanRequest);
        await _context.SaveChangesAsync();
        return Ok("Richiesta di prestito inviata con successo.");
    }
}
