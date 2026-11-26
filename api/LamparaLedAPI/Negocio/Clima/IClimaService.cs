using System.Threading.Tasks;

namespace Negocio.Clima
{
    public interface IClimaService
    {
        Task<ClimaDTO> ObtenerClimaActual(string ubicacion);
    }
}
