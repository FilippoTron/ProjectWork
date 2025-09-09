using Microsoft.EntityFrameworkCore;
using ProjectWork.Models;

namespace ProjectWork.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configurazioni specifiche del modello, se necessarie
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<LoanRequest>()
            .Property(lr => lr.Status)
            .HasConversion<string>();
        modelBuilder.Entity<LoanRequest>()
            .Property(lr => lr.TipoPrestito)
            .HasConversion<string>();
    }
    // Definizione dei DbSet per le entità del database
    public DbSet<User> Users { get; set; }
    public DbSet<LoanRequest> LoanRequests { get; set; }
    public DbSet<Document> Documents { get; set; }
}
