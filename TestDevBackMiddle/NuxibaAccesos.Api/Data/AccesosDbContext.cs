using Microsoft.EntityFrameworkCore;
using NuxibaAccesos.Api.Models;

namespace NuxibaAccesos.Api.Data;

public class AccesosDbContext : DbContext
{
    public AccesosDbContext(DbContextOptions<AccesosDbContext> options) : base(options)
    {
    }

    public DbSet<Login> Logins => Set<Login>();

    public DbSet<Usuario> Usuarios => Set<Usuario>();

    public DbSet<Area> Areas => Set<Area>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Login>(e =>
        {
            e.Property(l => l.fecha).HasColumnType("datetime");
            e.HasIndex(l => new { l.User_id, l.fecha });
        });

        modelBuilder.Entity<Usuario>(e =>
        {
            e.ToTable("ccUsers");
            e.HasKey(u => u.User_id);
            e.Property(u => u.User_id).ValueGeneratedNever();
            e.Property(u => u.Login).HasMaxLength(100);
            e.Property(u => u.Nombres).HasMaxLength(100);
            e.Property(u => u.ApellidoPaterno).HasMaxLength(100);
            e.Property(u => u.ApellidoMaterno).HasMaxLength(100);
            e.Property(u => u.Password).HasMaxLength(100);
            e.Property(u => u.fCreate).HasColumnType("datetime");
            e.Property(u => u.LastLoginAttempt).HasColumnType("datetime");
        });

        modelBuilder.Entity<Area>(e =>
        {
            e.ToTable("ccRIACat_Areas");
            // En los datos de origen el IDArea 2 aparece dos veces (BBVA y Banamex),
            // por eso la llave incluye el nombre y no solo el identificador
            e.HasKey(a => new { a.IDArea, a.AreaName });
            e.Property(a => a.AreaName).HasMaxLength(100);
            e.Property(a => a.CreateDate).HasColumnType("datetime");
        });
    }
}
