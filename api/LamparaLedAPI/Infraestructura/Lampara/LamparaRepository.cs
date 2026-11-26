using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infraestructura.Lampara
{
    public class LamparaRepository : ILamparaRepository
    {
        private readonly LamparaDbContext _context;

        public LamparaRepository(LamparaDbContext context)
        {
            _context = context;
        }

        public async Task GuardarLectura(LamparaEntity lectura)
        {
            _context.LamparaLecturas.Add(lectura);
            await _context.SaveChangesAsync();
        }

        public async Task<List<LamparaEntity>> ObtenerTodasLecturas()
        {
            return await Task.FromResult(
                _context.LamparaLecturas
                    .OrderByDescending(l => l.FechaHora)
                    .ToList()
            );
        }

        public async Task<LamparaEntity?> ObtenerUltimaLectura()
        {
            return await Task.FromResult(
                _context.LamparaLecturas
                    .OrderByDescending(l => l.FechaHora)
                    .FirstOrDefault()
            );
        }
    }
}
