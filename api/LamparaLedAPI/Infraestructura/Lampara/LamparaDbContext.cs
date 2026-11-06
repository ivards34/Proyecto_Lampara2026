using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Infraestructura.Lampara
{
    public class LamparaDbContext : DbContext
    {
        public LamparaDbContext(DbContextOptions<LamparaDbContext> options)
            : base(options)
        {
        }

        public DbSet<LamparaEntity> LamparaLecturas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<LamparaEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FechaHora).IsRequired().HasColumnType("timestamp without time zone");
                entity.Property(e => e.DispositivoId).IsRequired().HasMaxLength(100);
                entity.Property(e => e.TemperaturaSensor).IsRequired();
                entity.Property(e => e.HumedadSensor).IsRequired();
                entity.Property(e => e.TemperaturaApi).IsRequired();
                entity.Property(e => e.HumedadApi).IsRequired();
                entity.Property(e => e.EstadoLampara).IsRequired().HasMaxLength(10);
            });
        }
    }
}
