using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProjectWork.Data;
using ProjectWork.DTO;
using ProjectWork.Models;

namespace ProjectWork.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly string _jwtKey = "Q9zXp7!pRt*DkL#3vU1&nE$zXyGmT@64";
    public AuthController(AppDbContext context)
    {
        _context = context;
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
            Password = user.Password // In un'applicazione reale, la password dovrebbe essere crittografata
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
        var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Username)
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);
        return Ok(new { Token = tokenString });
    }
}
