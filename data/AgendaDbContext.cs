using agendaClaude.Models;
using Microsoft.EntityFrameworkCore;

namespace agendaClaude.Data;

public sealed class AgendaDbContext(DbContextOptions<AgendaDbContext> options) : DbContext(options)
{
    public DbSet<AgendaItem> AgendaItems => Set<AgendaItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AgendaItem>(entity =>
        {
            entity.ToTable("AgendaItems");
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Contato).HasMaxLength(120).IsRequired();
            entity.Property(item => item.Data).HasColumnType("date").IsRequired();
            entity.Property(item => item.Horario).HasColumnType("time").IsRequired();
            entity.Property(item => item.Telefone).HasMaxLength(20).IsRequired();
            entity.Property(item => item.Valor).HasPrecision(12, 2).IsRequired();
        });
    }
}
