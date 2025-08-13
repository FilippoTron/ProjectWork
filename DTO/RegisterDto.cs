using System.ComponentModel.DataAnnotations;

namespace ProjectWork.DTO;

public class RegisterDto
{
    [Required]
    public string Name { get; set; } // Nome per la registrazione
    [Required]
    public string Surname { get; set; } // Cognome per la registrazione
    [Required]
    public string Indirizzo { get; set; } // Indirizzo per la registrazione
    [Required]
    public string Citta { get; set; } // Città per la registrazione
    [Required]
    public string Provincia { get; set; } // Provincia per la registrazione
    [Required]
    public string Cap { get; set; } // CAP per la registrazione
    [Required]
    public string Telefono { get; set; } // Numero di telefono per la registrazione
    [Required]
    public DateTime DataNascita { get; set; } // Data di nascita per la registrazione
    [Required]
    public string Username { get; set; } // Nome utente per la registrazione
    [Required]
    public string Email { get; set; } // Email per la registrazione
    [Required]
    public string Password { get; set; } // Password per la registrazione
}
