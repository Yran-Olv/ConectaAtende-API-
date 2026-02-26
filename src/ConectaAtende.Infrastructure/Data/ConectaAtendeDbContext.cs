using ConectaAtende.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConectaAtende.Infrastructure.Data;

public class ConectaAtendeDbContext : DbContext
{
    public ConectaAtendeDbContext(DbContextOptions<ConectaAtendeDbContext> options)
        : base(options)
    {
    }

    public DbSet<Contact> Contacts { get; set; }
    public DbSet<Ticket> Tickets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuração de Contact
        modelBuilder.Entity<Contact>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Phones)
                .HasConversion(
                    v => string.Join(";", v),
                    v => v.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList());
        });

        // Configuração de Ticket
        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.Status)
                .HasConversion<string>();
            entity.Property(e => e.Priority)
                .HasConversion<string>();
        });
    }
}
