using System.Threading.Tasks;

namespace Negocio.Lampara
{
    public interface IClimaService
    {
        Task<ClimaDTO> ObtenerClimaActual(string ubicacion);
    }
}
