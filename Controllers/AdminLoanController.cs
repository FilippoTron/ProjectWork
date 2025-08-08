using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectWork.Data;
using ProjectWork.Models;

namespace ProjectWork.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class AdminLoanController : ControllerBase
{
    private readonly AppDbContext _context;
    public AdminLoanController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("requests")]
    public IActionResult GetLoanRequests()
    {
        try
        {
            var requests = _context.LoanRequests
                .Select(r => new
                {
                    r.Id,
                    UserName = r.User.Username,
                    r.Importo,
                    r.TassoInteresse,
                    r.Durata,
                    r.Status,
                    r.DataRichiesta
                }).ToList();
            return Ok(requests);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Errore durante il recupero delle richieste di prestito: {ex.Message}");
        }
    }
    [HttpPut("approve/{id}")]
    public IActionResult ApproveLoanRequest(int id)
    {
        var request = _context.LoanRequests.Find(id);
        if (request == null)
        {
            return NotFound("Richiesta di prestito non trovata.");
        }
        request.Status = Status.Approvata;
        _context.SaveChanges();
        return Ok("Richiesta di prestito approvata.");
    }
    [HttpPut("reject/{id}")]
    public IActionResult RejectLoanRequest(int id)
    {
        var request = _context.LoanRequests.Find(id);
        if (request == null)
        {
            return NotFound("Richiesta di prestito non trovata.");
        }
        request.Status = Status.Rifiutata;
        _context.SaveChanges();
        return Ok("Richiesta di prestito rifiutata.");
    }
}
