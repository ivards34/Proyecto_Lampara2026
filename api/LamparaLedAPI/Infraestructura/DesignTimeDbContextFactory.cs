using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infraestructura.Lampara
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<LamparaDbContext>
    {
        public LamparaDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<LamparaDbContext>();
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=lamparadb;Username=postgres;Password=sebcheco1105");
            return new LamparaDbContext(optionsBuilder.Options);
        }
    }
}
