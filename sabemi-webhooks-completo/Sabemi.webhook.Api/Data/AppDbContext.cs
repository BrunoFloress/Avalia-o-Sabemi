using Microsoft.EntityFrameworkCore;
using Sabemi.webhook.Api.Models;

namespace Sabemi.webhook.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<EventoLog> EventosLog => Set<EventoLog>();
    public DbSet<StatusContrato> StatusContratos => Set<StatusContrato>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EventoLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.IdTransacao).IsUnique(); // idempotência garantida no banco
        });

        modelBuilder.Entity<StatusContrato>(entity =>
        {
            entity.HasKey(e => e.IdContrato);
        });
    }
}
