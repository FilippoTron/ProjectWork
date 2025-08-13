namespace ProjectWork.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Indirizzo { get; set; }
    public string Citta { get; set; }
    public string Provincia { get; set; }
    public string Cap { get; set; }
    public string Telefono { get; set; }
    public DateTime DataNascita { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Role { get; set; } = "User"; // Esempio: "User", "Admin"
}
