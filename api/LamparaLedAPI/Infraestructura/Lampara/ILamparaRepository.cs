using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infraestructura.Lampara
{
    public interface ILamparaRepository
    {
        Task GuardarLectura(LamparaEntity lectura);
        Task<List<LamparaEntity>> ObtenerTodasLecturas();
        Task<LamparaEntity?> ObtenerUltimaLectura();
    }
}
