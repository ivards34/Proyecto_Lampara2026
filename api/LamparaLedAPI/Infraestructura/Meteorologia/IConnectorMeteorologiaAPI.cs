using System.Threading.Tasks;

namespace Infraestructura.Meteorologia
{
    public interface IConnectorMeteorologiaAPI
    {
        Task<MeteorologiaDTO> ObtenerDatosMeteorologia(double latitud, double longitud);
    }
}
