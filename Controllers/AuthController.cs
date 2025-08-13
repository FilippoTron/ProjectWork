using Microsoft.AspNetCore.Mvc;
using ProjectWork.Data;
using ProjectWork.DTO;
using ProjectWork.Models;
using ProjectWork.Services;

namespace ProjectWork.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IJwtService _jwtService;
    public AuthController(AppDbContext context, IJwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto user)
    {
        if (user == null || string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
        {
            return BadRequest("Dati di registrazione non validi.");
        }
        // Verifica se l'utente esiste già
        var existingUser = _context.Users.FirstOrDefault(u => u.Username == user.Username);
        if (existingUser != null)
        {
            return Conflict("Utente già esistente.");
        }

        var newUser = new User
        {
            Username = user.Username,
            Email = user.Email,
            Password = user.Password,
            Name = user.Name,
            Surname = user.Surname,
            Indirizzo = user.Indirizzo,
            Citta = user.Citta,
            Provincia = user.Provincia,
            Cap = user.Cap,
            Telefono = user.Telefono,
            DataNascita = user.DataNascita,
            Role = "User",
        };
        // Aggiungi il nuovo utente al database
        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();
        return Ok("Registrazione completata con successo.");
    }
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto loginDto)
    {
        if (loginDto == null || string.IsNullOrEmpty(loginDto.Username) || string.IsNullOrEmpty(loginDto.Password))
        {
            return BadRequest("Dati di login non validi.");
        }
        // Verifica le credenziali dell'utente
        var user = _context.Users.FirstOrDefault(u => u.Username == loginDto.Username && u.Password == loginDto.Password);
        if (user == null)
        {
            return Unauthorized("Credenziali non valide.");
        }
        // Genera un token JWT (semplificato, senza libreria JWT)
        var token = _jwtService.GenerateToken(user);
        return Ok(new { token = token });
    }
}
