using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infraestructura.Lampara
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<LamparaDbContext>
    {
        public LamparaDbContext CreateDbContext(string[] args)
        {
            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                ?? Environment.GetEnvironmentVariable("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "No se encontró la cadena de conexión. Defina la variable de entorno " +
                    "'ConnectionStrings__DefaultConnection' antes de ejecutar 'dotnet ef'.");

            var optionsBuilder = new DbContextOptionsBuilder<LamparaDbContext>();
            optionsBuilder.UseNpgsql(connectionString);
            return new LamparaDbContext(optionsBuilder.Options);
        }
    }
}
