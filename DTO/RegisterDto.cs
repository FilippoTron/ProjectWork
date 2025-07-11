using System.ComponentModel.DataAnnotations;

namespace ProjectWork.DTO;

public class RegisterDto
{
    [Required]
    public string Username { get; set; } // Nome utente per la registrazione
    [Required]
    public string Email { get; set; } // Email per la registrazione
    [Required]
    public string Password { get; set; } // Password per la registrazione
}
